#include "CParser.h"
#include "FuncionesUnix.h"

CParser::CParser()
{
	m_pTablaSimbolos = NULL;
	m_StackLexemas = 0;
	m_pFuncionesExternas = NULL;
	m_CantidadFuncionesExternas = 0;
}

CParser::~CParser()
{
	Clear();
}

void CParser::Clear(void)
{
	m_Lexer.Clear();
	m_StackLexemas = 0;
	m_pFuncionesExternas = NULL;
	m_CantidadFuncionesExternas = 0;
}

void CParser::SetProgram(char* pcText)
{
	m_Lexer.Clear();
	m_Lexer.SetTextToParse(pcText);
}

void CParser::SetFuncionesExternas(CFuncion* pFuncionesExternas, int CantidadFuncionesExternas)
{
	m_pFuncionesExternas = pFuncionesExternas;
	m_CantidadFuncionesExternas = CantidadFuncionesExternas;
}


bool CParser::GenSyntTree(CSyntTree** ppRoot)
{
	bool bOk = true;

	PrvCrearTablaSimbolos();

	try
	{
		PrvGetLexema();

		*ppRoot = PrvFunciones();
	}
	catch(CParserError Error)
	{
		printf("Error: linea %ld, columna %ld, %s\n", Error.GetLine() + 1, Error.GetColumn() + 1, Error.GetDescription());
		bOk = false;
	}

	return(bOk);
}

CSyntTree* CParser::PrvFunciones(void)
{
	CSyntTree* pNode = NULL;

	pNode = new CSyntTree();

	pNode->SetType(CSyntTree::SYNT_LISTA_FUNCIONES);

	pNode->AddChild(PrvAddTablaSimbolos(m_pTablaSimbolos));

	try
	{
		while (m_Lexema.GetType() != CLexema::LEX_EOF)
		{
			CSyntTree* pChild = PrvFuncion();
			
			if (pChild == NULL)
				pChild = PrvDeclaracionVariable(false);

			ThrowParserErrorIf(pChild == NULL, "Se esperaba una función o declaración de variable");

			pNode->AddChild(pChild);
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvFuncion(void)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_IDENTIFICADOR)
		{
			CSimbolo* pSimboloTipoReturn = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

			ThrowParserErrorIf(pSimboloTipoReturn == NULL, "El simbolo especificado no se encontro");

			if (pSimboloTipoReturn->GetType() == CSimbolo::SIMBOLO_TIPO_DATO)
			{
				CLexema lexTipoDato = m_Lexema;
				PrvGetLexema();

				if (m_Lexema.GetType() == CLexema::LEX_IDENTIFICADOR)
				{
					CSimbolo* pSimboloFuncion = m_pTablaSimbolos->FindSimboloLocal(m_Lexema.GetString());

					//Valido que el identificador este no utiliza o, si esta utilizado, que sea una función y que coincida el return type
					//Despues de obtener los parametros, ahi genero el nombre completo
					//de la función y en base a eso determino si la función ya esta declarada o no
					ThrowParserErrorIf(pSimboloFuncion != NULL && pSimboloFuncion->GetType() != CSimbolo::SIMBOLO_TIPO_FUNCION, "El identificador ya esta declarado como una clase o variable");
					ThrowParserErrorIf(pSimboloFuncion != NULL && pSimboloFuncion->GetReturnType() != pSimboloTipoReturn, "No se pueden tener 2 funciones con el mismo nombre y que devuelvan tipos de datos distintos");

					CLexema lexNombreFuncion = m_Lexema;
					PrvGetLexema();

					if (m_Lexema.GetType() == CLexema::LEX_PAR_ABRE)
					{
						CSimbolo* pOldTipoReturn = m_pTipoReturn;

						m_pTipoReturn = pSimboloTipoReturn;

						pSimboloFuncion = new CSimbolo();

						pSimboloFuncion->SetType(CSimbolo::SIMBOLO_TIPO_FUNCION);
						pSimboloFuncion->SetReturnType(pSimboloTipoReturn);
						pSimboloFuncion->SetNombre(lexNombreFuncion.GetString());

						//Lo agrego antes de haber verificado si ya esta declarado o no, de
						//esta forma me evito los problemas de borrar el simbolo cuando
						//falle si ya existe la función
						m_pTablaSimbolos->AddSimbolo(pSimboloFuncion);

						pNode = new CSyntTree();
						pNode->SetType(CSyntTree::SYNT_DECLARACION_FUNCION);
						pNode->SetReturnType(NULL);

						PrvGetLexema();

						CTablaSimbolos* pTabla = new CTablaSimbolos();
						pTabla->SetTablaPadre(m_pTablaSimbolos);
						m_pTablaSimbolos = pTabla;

						pNode->AddChild(PrvAddTablaSimbolos(pTabla));

						try
						{
							bool bPrimerParametro = true;

							while(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA)
							{
								if (bPrimerParametro == false)
								{
									ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_COMA, "Se esperaba ,");
									PrvGetLexema();
								}
								else
								{
									bPrimerParametro = false;
								}

								ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_IDENTIFICADOR, "Se esperaba un tipo de dato");

								CSimbolo* pTipoDatoParametro = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

								ThrowParserErrorIf(pTipoDatoParametro == NULL, "Se esperaba un tipo de dato");

								PrvGetLexema();

								ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_IDENTIFICADOR, "Se esperaba un identificador");

								CSimbolo* pParametro = m_pTablaSimbolos->FindSimboloLocal(m_Lexema.GetString());

								ThrowParserErrorIf(pParametro != NULL, "El simbolo ya esta definido");

								pParametro = new CSimbolo();

								pParametro->SetType(CSimbolo::SIMBOLO_TIPO_VARIABLE);
								pParametro->SetNombre(m_Lexema.GetString());
								pParametro->SetReturnType(pTipoDatoParametro);

								pSimboloFuncion->AddParametro(pParametro);

								m_pTablaSimbolos->AddSimbolo(pParametro);

								PrvGetLexema();
							}

							PrvGetLexema();

							//Ya obtuve el tipo de dato de todos los parametros, ahora genero el nombre completo de la función
							pSimboloFuncion->ActualizarNombreCompletoFuncion();

							CSimbolo* pSimboloComparacion = m_pTablaSimbolos->FindSimboloGlobal(pSimboloFuncion->GetNombre());

							while(pSimboloComparacion != NULL)
							{
								if (pSimboloComparacion != pSimboloFuncion)
									ThrowParserErrorIf(strcmp(pSimboloFuncion->GetNombreFuncionCompleto(), pSimboloComparacion->GetNombreFuncionCompleto()) == 0, "Ya existe una función con el mismo nombre y los mismos parametros");

								pSimboloComparacion = m_pTablaSimbolos->FindProximoSimboloGlobal(pSimboloComparacion);
							}

							pNode->SetString(pSimboloFuncion->GetNombre());
							pNode->SetSimboloDefinicionFuncion(pSimboloFuncion);
							
							m_StackVariables = pSimboloFuncion->GetParametros();

							if (strcmp(pSimboloFuncion->GetReturnType()->GetNombre(), SIMBOLO_VOID) != 0)
								m_StackVariables++;

							m_MaxVariables = m_StackVariables;
							
							CSyntTree *pInstrucciones = PrvInstrucciones();

							if (pInstrucciones == NULL)
							{
								if (m_Lexema.GetType() == CLexema::LEX_END)
								{
									PrvGetLexema();

									pSimboloFuncion->SetFuncionNativa(true);
								}
								else
									ThrowParserError("Se esperaba {");
							}
							else
							{
								pNode->AddChild(pInstrucciones);
							}

							pSimboloFuncion->SetCantVariables(m_MaxVariables);
						}
						catch(CParserError err)
						{
							err.GetDescription();
							m_pTablaSimbolos = m_pTablaSimbolos->GetTablaPadre();
							throw;
						}

						m_pTablaSimbolos = m_pTablaSimbolos->GetTablaPadre();

						m_pTipoReturn = pOldTipoReturn;
					}
					else
					{
						PrvReturnLexema(lexNombreFuncion);
						PrvReturnLexema(lexTipoDato);
					}
				}
				else
				{
					PrvReturnLexema(lexTipoDato);
				}
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvInstrucciones(void)
{
	CSyntTree* pNode = NULL;

	if (m_Lexema.GetType() == CLexema::LEX_LLAVE_ABRE)
	{
		PrvGetLexema();

		try
		{
			pNode = new CSyntTree();

			pNode->SetType(CSyntTree::SYNT_LISTA_INSTRUCCIONES);

			CTablaSimbolos* pTabla;

			pTabla = new CTablaSimbolos();
			pTabla->SetTablaPadre(m_pTablaSimbolos);
			m_pTablaSimbolos = pTabla;

			pNode->AddChild(PrvAddTablaSimbolos(pTabla));

			int oldStackVars = m_StackVariables;

			try
			{
				while (m_Lexema.GetType() != CLexema::LEX_LLAVE_CIERRA)
				{
					CSyntTree* pChild = PrvInstruccion();

					ThrowParserErrorIf(pChild == NULL, "Se esperaba una instrucción");
					
					pNode->AddChild(pChild);
				}

				PrvGetLexema();
			}
			catch(CParserError err)
			{
				err.GetDescription();
				m_pTablaSimbolos = m_pTablaSimbolos->GetTablaPadre();
				throw;
			}

			if (m_StackVariables > m_MaxVariables)
				m_MaxVariables = m_StackVariables;

			m_StackVariables = oldStackVars;

			m_pTablaSimbolos = m_pTablaSimbolos->GetTablaPadre();
		}
		catch(CParserError err)
		{
			err.GetDescription();
			delete pNode;
			throw;
		}
	}

	return(pNode);
}

CSyntTree* CParser::PrvAddTablaSimbolos(CTablaSimbolos* pTablaSimbolos)
{
	CSyntTree* pNode = new CSyntTree();

	pNode->SetType(CSyntTree::SYNT_TABLA_SIMBOLOS);
	pNode->SetTablaSimbolos(pTablaSimbolos);

	return(pNode);
}

CSyntTree* CParser::PrvInstruccion(void)
{
	CSyntTree* pNode = NULL;
	CSyntTree* pChild = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_END)
		{
			pChild = new CSyntTree();
			pChild->SetType(CSyntTree::SYNT_INSTRUCCION_NULA);

			PrvGetLexema();
		}
		else
		{
			if (m_Lexema.GetType() == CLexema::LEX_LLAVE_ABRE)
			{
				pChild = PrvInstrucciones();
			}
			else
			{
				pChild = PrvExpresion();

				if (pChild != NULL)
				{
					ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_END, "Se esperaba ;");
					PrvGetLexema();
				}
				
				if (pChild == NULL)
					pChild = PrvDeclaracionVariable(true);

				if (pChild == NULL)
					pChild = PrvIf();

				if (pChild == NULL)
					pChild = PrvFor();

				if (pChild == NULL)
					pChild = PrvWhile();

				if (pChild == NULL)
					pChild = PrvReturn();
			}
		}

		if (pChild != NULL)
		{
			pNode = new CSyntTree();
			pNode->SetType(CSyntTree::SYNT_INSTRUCCION);

			pNode->AddChild(pChild);

			pNode->SetReturnType(pChild->GetReturnType());
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvFor(void)
{
	CSyntTree* pNodo = NULL;

	/* for (Inicializacion, Condicion, Incremento)
			Ciclo; */
	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_FOR)
		{
			CSyntTree* pInicializacion = NULL;
			CSyntTree* pCondicion = NULL;
			CSyntTree* pIncremento = NULL;
			CSyntTree* pCiclo = NULL;

			try
			{
				PrvGetLexema();

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_ABRE, "Se esperaba (");

				PrvGetLexema();

				pInicializacion = PrvExpresion();

				ThrowParserErrorIf(pInicializacion == NULL, "Se esperaba una expresion");

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_END, "Se esperaba ;");

				PrvGetLexema();

				pCondicion = PrvExpresion();

				ThrowParserErrorIf(pCondicion == NULL, "Se esperaba una expresion");

				pCondicion = PrvConvertirReturnType(pCondicion, m_pSimboloInt);

				ThrowParserErrorIf( pCondicion->GetReturnType() != m_pSimboloInt,
									"El tipo de dato devuelto por la expresion no es evaluable");

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_END, "Se esperaba ;");

				PrvGetLexema();

				pIncremento = PrvExpresion();

				ThrowParserErrorIf(pIncremento == NULL, "Se esperaba una expresion");

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA, "Se esperaba )");

				PrvGetLexema();

				pCiclo = PrvInstruccion();
				
				ThrowParserErrorIf(pCiclo == NULL, "Se esperaba una instrucción");

				pNodo = new CSyntTree();

				pNodo->SetType(CSyntTree::SYNT_FOR);
				pNodo->AddChild(pInicializacion);
				pNodo->AddChild(pCondicion);
				pNodo->AddChild(pIncremento);
				pNodo->AddChild(pCiclo);
			}
			catch(CParserError err)
			{
				err.GetDescription();
				delete pInicializacion;
				delete pCondicion;
				delete pIncremento;
				delete pCiclo;
				throw;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo;
		throw;
	}

	return(pNodo);
}

CSyntTree* CParser::PrvWhile(void)
{
	CSyntTree* pNodo = NULL;

	/*while (condicion)
		ciclo;*/

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_WHILE)
		{
			CSyntTree* pCondicion = NULL;
			CSyntTree* pCiclo = NULL;

			try
			{
				PrvGetLexema();

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_ABRE, "Se esperaba (");

				PrvGetLexema();

				pCondicion = PrvExpresion();

				ThrowParserErrorIf(pCondicion == NULL, "Se esperaba una expresion");

				pCondicion = PrvConvertirReturnType(pCondicion, m_pSimboloInt);

				ThrowParserErrorIf( pCondicion->GetReturnType() != m_pSimboloInt,
									"El tipo de dato devuelto no es evaluable");

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA, "Se esperaba )");
				
				PrvGetLexema();

				pCiclo = PrvInstruccion();
				
				ThrowParserErrorIf(pCiclo == NULL, "Se esperaba una instrucción");

				pNodo = new CSyntTree();
				pNodo->SetType(CSyntTree::SYNT_WHILE);
				pNodo->AddChild(pCondicion);
				pNodo->AddChild(pCiclo);
			}
			catch(CParserError err)
			{
				err.GetDescription();
				delete pCondicion;
				delete pCiclo;
				throw;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo;
		throw;
	}

	return(pNodo);
}

CSyntTree* CParser::PrvReturn(void)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_RETURN)
		{
			PrvGetLexema();

			ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_ABRE, "Se esperaba (");

			PrvGetLexema();

			CSyntTree* pChild = PrvExpresion();

			ThrowParserErrorIf(pChild == NULL, "Se esperaba una expresion");

			pChild = PrvConvertirReturnType(pChild, m_pTipoReturn);

			pNode = new CSyntTree();
			pNode->SetType(CSyntTree::SYNT_RETURN);
			pNode->AddChild(pChild);

			ThrowParserErrorIf(pChild->GetReturnType() != m_pTipoReturn, "El tipo de dato devuelto no coincide con el tipo de dato que devuelve la función.");

			ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA, "Se esperaba )");
			
			PrvGetLexema();

			ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_END, "Se esperaba ;");

			PrvGetLexema();
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvIf(void)
{
	CSyntTree* pNodo = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_IF)
		{
			CSyntTree* pCondicion = NULL;
			CSyntTree* pThen = NULL;
			CSyntTree* pElse = NULL;

			try
			{
				PrvGetLexema();

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_ABRE, "Se esperaba (");

				PrvGetLexema();

				pCondicion = PrvExpresion();

				ThrowParserErrorIf(pCondicion == NULL, "Se esperaba una expresion");

				pCondicion = PrvConvertirReturnType(pCondicion, m_pSimboloInt);

				ThrowParserErrorIf( pCondicion->GetReturnType() != m_pSimboloInt,
									"El tipo de dato devuelto no es evaluable");

				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA, "Se esperaba )");
				
				PrvGetLexema();

				pThen = PrvInstruccion();
				
				ThrowParserErrorIf(pThen == NULL, "Se esperaba una instrucción");

				if (m_Lexema.GetType() == CLexema::LEX_ELSE)
				{
					PrvGetLexema();

					pElse = PrvInstruccion();
					
					ThrowParserErrorIf(pElse == NULL, "Se esperaba una instrucción");
				}

				pNodo = new CSyntTree();

				pNodo->SetType(CSyntTree::SYNT_IF);
				pNodo->AddChild(pCondicion);
				pNodo->AddChild(pThen);

				if (pElse)
					pNodo->AddChild(pElse);
			}
			catch(CParserError err)
			{
				err.GetDescription();
				delete pCondicion;
				delete pThen;
				delete pElse;
				throw;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo;
		throw;
	}

	return(pNodo);
}

CSyntTree* CParser::PrvExpresion(void)
{
	CSyntTree* pNodo = NULL;
	bool bLoop = true;

	try
	{
		pNodo = PrvSumaResta();

		while(bLoop)
		{
			switch(m_Lexema.GetType())
			{
				case CLexema::LEX_OP_ASIGNACION:
				{
					ThrowParserErrorIf(	pNodo->GetType() != CSyntTree::SYNT_VARIABLE
										, "El lado izquierdo de una asignación debe ser una variable");

					PrvGetLexema();

					CSyntTree* pNodo2 = PrvSumaResta();

					ThrowParserErrorIf(pNodo2 == NULL, "Error de sintaxis");

					try
					{
						pNodo2 = PrvConvertirReturnType(pNodo2, pNodo->GetReturnType());

						ThrowParserErrorIf(pNodo2->GetReturnType() != pNodo->GetReturnType(), "El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");
					}
					catch(CParserError err)
					{
						err.GetDescription();
						delete pNodo2;
						throw;
					}

					CSyntTree* pAsignacion = new CSyntTree();

					pAsignacion->SetType(CSyntTree::SYNT_OP_ASIGNACION);
					pAsignacion->SetReturnType(pNodo->GetReturnType());
					pAsignacion->AddChild(pNodo);
					pAsignacion->AddChild(pNodo2);
					pNodo = pAsignacion;
					break;
				}

				case CLexema::LEX_OP_IGUAL:
				{
					pNodo = PrvOperadorLogico(pNodo);
					pNodo->SetType(CSyntTree::SYNT_OP_COMPARACION_IGUAL);
					break;
				}

				case CLexema::LEX_OP_DISTINTO:
				{
					pNodo = PrvOperadorLogico(pNodo);
					pNodo->SetType(CSyntTree::SYNT_OP_COMPARACION_DISTINTO);
					break;
				}

				default:
					bLoop = false;
					break;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo;
		throw;
	}

	return(pNodo);
}

CSyntTree* CParser::PrvOperadorLogico(CSyntTree* pNodo = NULL)
{
	PrvGetLexema();

	ThrowParserErrorIf(	pNodo->GetReturnType() != m_pSimboloInt &&
						pNodo->GetReturnType() != m_pSimboloFloat &&
						pNodo->GetReturnType() != m_pSimboloString
						, "El lado izquierdo de una comparación debe ser un numero o una cadena");

	CSyntTree* pNodo2 = PrvSumaResta();

	ThrowParserErrorIf(pNodo2 == NULL, "Error de sintaxis");

	try
	{
		pNodo2 = PrvConvertirReturnType(pNodo2, pNodo->GetReturnType());

		ThrowParserErrorIf(pNodo2->GetReturnType() != pNodo->GetReturnType(), "El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo2;
		throw;
	}

	CSyntTree* pComparacion = new CSyntTree();

	//pComparacion->SetType(CSyntTree::SYNT_OP_COMPARACION_IGUAL);
	//El tipo se seteado por la función que me llama
	pComparacion->SetReturnType(m_pSimboloInt);
	pComparacion->AddChild(pNodo);
	pComparacion->AddChild(pNodo2);
	pNodo = pComparacion;

	return(pNodo);
}

CSyntTree* CParser::PrvSumaResta(void)
{
	CSyntTree* pTermino = NULL;
	bool bLoop = true;

	try
	{
		pTermino = PrvMultiplicacionDivision();

		while(bLoop)
		{
			switch(m_Lexema.GetType())
			{
				case CLexema::LEX_OP_MAS:
				case CLexema::LEX_OP_MENOS:
				{
					CLexema::LexTypeEnum LexOpType = m_Lexema.GetType();

					ThrowParserErrorIf(pTermino == NULL, "Error de sintaxis");

					ThrowParserErrorIf(pTermino->GetReturnType() != m_pSimboloInt && pTermino->GetReturnType() != m_pSimboloFloat && pTermino->GetReturnType() != m_pSimboloString, "El operador izquierdo devuelve un tipo de dato no utilizable");

					PrvGetLexema();

					CSyntTree* pTermino2 = PrvMultiplicacionDivision();

					ThrowParserErrorIf(pTermino2 == NULL, "Error de sintaxis");

					try
					{
						pTermino2 = PrvConvertirReturnType(pTermino2, pTermino->GetReturnType());

						ThrowParserErrorIf(pTermino2->GetReturnType() != pTermino->GetReturnType(), "El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");
					}
					catch(CParserError err)
					{
						err.GetDescription();
						delete pTermino2;
						throw;
					}

					CSyntTree *pSuma = new CSyntTree();

					if (LexOpType == CLexema::LEX_OP_MAS)
						pSuma->SetType(CSyntTree::SYNT_OP_MAS);
					else
						pSuma->SetType(CSyntTree::SYNT_OP_MENOS);

					pSuma->SetReturnType(pTermino->GetReturnType());
					pSuma->AddChild(pTermino);
					pSuma->AddChild(pTermino2);
					pTermino = pSuma;
					break;
				}

				default:
					bLoop = false;
					break;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pTermino;
		throw;
	}

	return(pTermino);
}

CSyntTree* CParser::PrvMultiplicacionDivision(void)
{
	CSyntTree* pFactor = NULL;

	bool bLoop = true;

	try
	{
		pFactor = PrvValor();

		while(bLoop)
		{
			switch(m_Lexema.GetType())
			{
				case CLexema::LEX_OP_POR:
				case CLexema::LEX_OP_DIV:
				{
					CLexema::LexTypeEnum LexOpType = m_Lexema.GetType();

					ThrowParserErrorIf(pFactor == NULL, "Error de sintaxis");

					ThrowParserErrorIf(pFactor->GetReturnType() != m_pSimboloInt && pFactor->GetReturnType() != m_pSimboloFloat, "El operador izquierdo devuelve un tipo de dato no utilizable");

					PrvGetLexema();

					CSyntTree* pFactor2 = PrvValor();

					ThrowParserErrorIf(pFactor2 == NULL, "Error de sintaxis");

					try
					{
						pFactor2 = PrvConvertirReturnType(pFactor2, pFactor->GetReturnType());

						ThrowParserErrorIf(pFactor2->GetReturnType() != pFactor->GetReturnType(), "El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");
					}
					catch(CParserError err)
					{
						err.GetDescription();
						delete pFactor2;
						throw;
					}

					CSyntTree *pMult = new CSyntTree();

					if (LexOpType == CLexema::LEX_OP_POR)
						pMult->SetType(CSyntTree::SYNT_OP_POR);
					else
						pMult->SetType(CSyntTree::SYNT_OP_DIV);
					
					pMult->SetReturnType(pFactor->GetReturnType());
					pMult->AddChild(pFactor);
					pMult->AddChild(pFactor2);
					pFactor = pMult;
					break;
				}

				default:
					bLoop = false;
					break;
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pFactor;
		throw;
	}

	return(pFactor);
}

CSyntTree* CParser::PrvValor(void)
{
	CSyntTree* pNodo = NULL;

	bool bLoop = true;

	try
	{
		pNodo = PrvValor2();

		while(bLoop)
		{
			/*switch(m_Lexema.GetType())
			{
				default:
					bLoop = false;
					break;
			}*/
			bLoop = false;
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNodo;
		throw;
	}

	return(pNodo);
}

CSyntTree* CParser::PrvValor2(void)
{
	CSyntTree* pNode = NULL;

	try
	{
		switch(m_Lexema.GetType())
		{
			case CLexema::LEX_PAR_ABRE:
				PrvGetLexema();
				pNode = PrvExpresion();
				ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA, "Se esperaba )");
				PrvGetLexema();
				break;
		
			case CLexema::LEX_IDENTIFICADOR:
			{
				CSimbolo* pSimbolo = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

				ThrowParserErrorIf(pSimbolo == NULL, "El simbolo especificado no se encontro");

				switch(pSimbolo->GetType())
				{
					case CSimbolo::SIMBOLO_TIPO_VARIABLE:
						pNode = PrvVariable();
						break;

					case CSimbolo::SIMBOLO_TIPO_FUNCION:
						pNode = PrvLlamadoFuncion();
						break;
				}
				
				break;
			}

			case CLexema::LEX_FLOAT:
			case CLexema::LEX_INTEGER:
			case CLexema::LEX_STRING:
				pNode = PrvConstante();
				break;

			default:
				break;
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvVariable(void)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_IDENTIFICADOR)
		{
			CSimbolo* pSimbolo = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

			ThrowParserErrorIf(pSimbolo == NULL, "El simbolo especificado no se encontro");

			pNode = PrvVariable2(pSimbolo);
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvVariable2(CSimbolo* pSimbolo)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (pSimbolo->GetType() == CSimbolo::SIMBOLO_TIPO_VARIABLE)
		{
			pNode = new CSyntTree();
			pNode->SetType(CSyntTree::SYNT_VARIABLE);
			pNode->SetString(m_Lexema.GetString());
			pNode->SetReturnType(pSimbolo->GetReturnType());
			PrvGetLexema();
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}


CSyntTree* CParser::PrvConstante(void)
{
	CSyntTree* pNode = NULL;

	switch(m_Lexema.GetType())
	{
		case CLexema::LEX_FLOAT:
		case CLexema::LEX_INTEGER:
		case CLexema::LEX_STRING:
			pNode = new CSyntTree();
			pNode->SetInteger(m_Lexema.GetInteger());
			pNode->SetType(CSyntTree::SYNT_CONSTANTE);

			if (m_Lexema.GetType() == CLexema::LEX_INTEGER)
			{
				pNode->SetReturnType(m_pSimboloInt);
				pNode->SetInteger(m_Lexema.GetInteger());
			}
			else
			{
				if (m_Lexema.GetType() == CLexema::LEX_FLOAT)
				{
					pNode->SetReturnType(m_pSimboloFloat);
					pNode->SetFloat(m_Lexema.GetFloat());
				}
				else
				{
					pNode->SetReturnType(m_pSimboloString);
					pNode->SetString(m_Lexema.GetString());
				}
			}

			PrvGetLexema();
			break;
	}

	return(pNode);
}

CSyntTree* CParser::PrvDeclaracionVariable(bool bPermitirInicializacion)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_IDENTIFICADOR)
		{
			CSimbolo* pSimboloTipoDato = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

			ThrowParserErrorIf(pSimboloTipoDato == NULL, "El simbolo especificado no se encontro");

			if (pSimboloTipoDato->GetType() == CSimbolo::SIMBOLO_TIPO_DATO)
			{
				ThrowParserErrorIf(pSimboloTipoDato == m_pSimboloVoid, "No se pueden declarar variables del tipo void");

				pNode = new CSyntTree();
				pNode->SetType(CSyntTree::SYNT_DECLARACION_VARIABLE);
				pNode->SetString(m_Lexema.GetString());
				pNode->SetReturnType(NULL);

				PrvGetLexema();

				bool bPrimerDeclaracion = true;

				while (m_Lexema.GetType() != CLexema::LEX_END)
				{
					if (bPrimerDeclaracion == false)
					{
						ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_COMA, "Se esperaba ,");
						PrvGetLexema();
					}
					else
					{
						bPrimerDeclaracion = false;
					}

					ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_IDENTIFICADOR, "Se esperaba un identificador");

					ThrowParserErrorIf(m_pTablaSimbolos->FindSimboloLocal(m_Lexema.GetString()) != NULL, "La variable ya esta declarada");

					CSyntTree* pVariable = new CSyntTree;

					pVariable->SetType(CSyntTree::SYNT_IDENTIFICADOR);
					pVariable->SetReturnType(pSimboloTipoDato);
					pVariable->SetString(m_Lexema.GetString());
					
					pNode->AddChild(pVariable);

					CSimbolo* pSimboloVariable;

					pSimboloVariable = new CSimbolo();

					pSimboloVariable->SetNombre(pVariable->GetString());
					pSimboloVariable->SetReturnType(pSimboloTipoDato);
					pSimboloVariable->SetType(CSimbolo::SIMBOLO_TIPO_VARIABLE);

					m_pTablaSimbolos->AddSimbolo(pSimboloVariable);
				
					PrvGetLexema();

					if (bPermitirInicializacion)
					{
						if (m_Lexema.GetType() == CLexema::LEX_OP_ASIGNACION)
						{
							PrvGetLexema();

							CSyntTree* pInicializacion = NULL;
							//Es la inicialización de la variable
							try
							{
								pInicializacion = PrvExpresion();

								ThrowParserErrorIf(pInicializacion == NULL, "Se esperaba una expresion");

								pInicializacion = PrvConvertirReturnType(pInicializacion, pSimboloTipoDato);

								ThrowParserErrorIf(pInicializacion->GetReturnType() != pSimboloTipoDato, "El tipo de dato devuelto por el inicializador no coincide con el tipo de dato de la variable");

								pVariable->AddChild(pInicializacion);
							}
							catch(CParserError err)
							{
								err.GetDescription();
								delete pInicializacion;
								throw;
							}

						}
					}

					m_StackVariables++;

					if (m_StackVariables > m_MaxVariables)
						m_MaxVariables = m_StackVariables;
				}

				PrvGetLexema();
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvLlamadoFuncion(void)
{
	CSyntTree* pNode = NULL;

	try
	{
		if (m_Lexema.GetType() == CLexema::LEX_IDENTIFICADOR)
		{
			CSimbolo* pSimbolo = m_pTablaSimbolos->FindSimboloGlobal(m_Lexema.GetString());

			ThrowParserErrorIf(pSimbolo == NULL, "El simbolo especificado no se encontro");

			if (pSimbolo->GetType() == CSimbolo::SIMBOLO_TIPO_FUNCION)
			{
				pNode = PrvLlamadoFuncion2(pSimbolo->GetNombre(), m_pTablaSimbolos);
			}
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}

CSyntTree* CParser::PrvLlamadoFuncion2(const char* pcNombreFuncion, CTablaSimbolos* pTablaSimbolos)
{
	CSyntTree* pNode = NULL;
	
	try
	{
		CSimbolo* pSimbolo = pTablaSimbolos->FindSimboloGlobal(pcNombreFuncion);
		
		if (pSimbolo != NULL && pSimbolo->GetType() == CSimbolo::SIMBOLO_TIPO_FUNCION)
		{
			pNode = new CSyntTree();
			pNode->SetType(CSyntTree::SYNT_LLAMADO_FUNCION);
			
			PrvGetLexema();

			ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_PAR_ABRE, "Se esperaba un (");

			pNode->SetType(CSyntTree::SYNT_LLAMADO_FUNCION);
			PrvGetLexema();
			
			int CantParametros = 0;
			
			while(m_Lexema.GetType() != CLexema::LEX_PAR_CIERRA)
			{
				if (CantParametros > 0)
				{
					ThrowParserErrorIf(m_Lexema.GetType() != CLexema::LEX_COMA, "Se esperaba ,");
					PrvGetLexema();
				}

				CSyntTree* pParameter;
				pParameter = PrvExpresion();

				ThrowParserErrorIf(pParameter == NULL, "Se esperaba un parámetro");

				ThrowParserErrorIf(pParameter->GetReturnType() == NULL || pParameter->GetReturnType() == m_pSimboloVoid, "El parametro es de un tipo de dato no compatible");
				
				pNode->AddChild(pParameter);

				CantParametros++;				
			}

			//Genero el nombre de la función en base a los tipos de datos devueltos por
			//los parametros, pero omito el tipo de dato que devuelve la función
			char pcNombreFuncion[1024];
			strcpy(pcNombreFuncion, pSimbolo->GetNombre());
			strcat(pcNombreFuncion, "(");

			char pcSeparador[2];

			pcSeparador[0] = CSimbolo::SIMBOLO_SEPARADOR_PARAMETRO;
			pcSeparador[1] = '\0';

			for (int i = 0; i < CantParametros; i++)
			{
				if (i != 0)
					strcat(pcNombreFuncion, pcSeparador);

				strcat(pcNombreFuncion, pNode->GetChild(i)->GetReturnType()->GetNombre());
			};
			strcat(pcNombreFuncion, ")");

			//Busco alguna función cuyos parametros coincidan con los que tengo

			CSimbolo* pSimboloFuncion = pTablaSimbolos->FindSimboloGlobal(pSimbolo->GetNombre());

			pSimbolo = NULL;

			while(pSimboloFuncion != NULL)
			{
				char* pcNombre = strchr(pSimboloFuncion->GetNombreFuncionCompleto(), CSimbolo::SIMBOLO_SEPARADOR_FUNCION) + 1;

				if (strcmp(pcNombre, pcNombreFuncion) == 0)
				{
					pSimbolo = pSimboloFuncion;
					break;
				}
												
				pSimboloFuncion = pTablaSimbolos->FindProximoSimboloGlobal(pSimboloFuncion);
			}

			if (pSimbolo == NULL)
			{
				char pcTemp[1024];

				sprintf(pcTemp, "No se encontro ninguna función de la forma '%s'", pcNombreFuncion);
				ThrowParserError(pcTemp);
			}

			pNode->SetSimboloDefinicionFuncion(pSimbolo);
			pNode->SetString(pSimbolo->GetNombreFuncionCompleto());
			pNode->SetReturnType(pSimbolo->GetReturnType());

			PrvGetLexema();

			ThrowParserErrorIf(CantParametros != pSimbolo->GetParametros(), "Faltan parametros");
		}
	}
	catch(CParserError err)
	{
		err.GetDescription();
		delete pNode;
		throw;
	}

	return(pNode);
}


void CParser::PrvCrearTablaSimbolos(void)
{
	m_pTablaSimbolos = new CTablaSimbolos();

	m_pSimboloInt = new CSimbolo();
	m_pSimboloInt->SetType(CSimbolo::SIMBOLO_TIPO_DATO);
	m_pSimboloInt->SetNombre(SIMBOLO_INT);

	m_pTablaSimbolos->AddSimbolo(m_pSimboloInt);

	m_pSimboloFloat = new CSimbolo();
	m_pSimboloFloat->SetType(CSimbolo::SIMBOLO_TIPO_DATO);
	m_pSimboloFloat->SetNombre(SIMBOLO_FLOAT);

	m_pTablaSimbolos->AddSimbolo(m_pSimboloFloat);

	m_pSimboloString = new CSimbolo();
	m_pSimboloString->SetType(CSimbolo::SIMBOLO_TIPO_DATO);
	m_pSimboloString->SetNombre(SIMBOLO_STRING);

	m_pTablaSimbolos->AddSimbolo(m_pSimboloString);

	m_pSimboloVoid = new CSimbolo();
	m_pSimboloVoid->SetType(CSimbolo::SIMBOLO_TIPO_DATO);
	m_pSimboloVoid->SetNombre(SIMBOLO_VOID);

	m_pTablaSimbolos->AddSimbolo(m_pSimboloVoid);

	for (int i = 0; i < m_CantidadFuncionesExternas; i++)
	{
		CFuncion* pFuncion = &m_pFuncionesExternas[i];
		CSimbolo* pSimboloFuncion = new CSimbolo();

		pSimboloFuncion->SetType(CSimbolo::SIMBOLO_TIPO_FUNCION);
		pSimboloFuncion->SetReturnType(m_pTablaSimbolos->FindSimboloGlobal(pFuncion->TipoDevuelto()->ToString()));
		pSimboloFuncion->SetNombre(pFuncion->ObtenerNombreCorto());

		for (int i = 0; i < pFuncion->GetParametros(); i++)
		{
			CDefinicionVariable *pDefPar = pFuncion->GetParametro(i);

			CSimbolo *pTipoDatoParametro = m_pTablaSimbolos->FindSimboloGlobal(pDefPar->TipoDato()->ToString());

			CSimbolo *pParametro = new CSimbolo();

			pParametro->SetType(CSimbolo::SIMBOLO_TIPO_VARIABLE);
			pParametro->SetNombre(pDefPar->ObtenerNombre());
			pParametro->SetReturnType(pTipoDatoParametro);

			pSimboloFuncion->AddParametro(pParametro);
		}

		pSimboloFuncion->ActualizarNombreCompletoFuncion();

		pSimboloFuncion->SetFuncionNativa(true);

		m_pTablaSimbolos->AddSimbolo(pSimboloFuncion);
	}
}

CSyntTree* CParser::PrvConvertirReturnType(CSyntTree* pNodo, CSimbolo* pReturnType)
{
	if (pNodo->GetReturnType() != pReturnType)
	{
		if (pNodo->GetReturnType() == m_pSimboloInt)
		{
			if (pReturnType == m_pSimboloFloat)
			{
				CSyntTree* pConversor = new CSyntTree();
				pConversor->SetType(CSyntTree::SYNT_CONVERSION);
				pConversor->SetReturnType(m_pSimboloFloat);
				pConversor->AddChild(pNodo);
				pNodo = pConversor;
			}
		}
		else
		{
			if (pNodo->GetReturnType() == m_pSimboloFloat)
			{
				if (pReturnType == m_pSimboloInt)
				{
					CSyntTree* pConversor = new CSyntTree();
					pConversor->SetType(CSyntTree::SYNT_CONVERSION);
					pConversor->SetReturnType(m_pSimboloInt);
					pConversor->AddChild(pNodo);
					pNodo = pConversor;
				}
			}
		}
	}

	return(pNodo);
}

void CParser::PrvGetLexema(void)
{
	if (m_StackLexemas)
		m_Lexema = m_pStackLexemas[--m_StackLexemas];
	else
		m_Lexer.GetLexema(&m_Lexema);
}

void CParser::PrvReturnLexema(CLexema& Lexema)
{
	ThrowParserErrorIf(m_StackLexemas == MAX_STACK_LEXEMAS, "El stack de lexemas esta lleno");

	m_pStackLexemas[m_StackLexemas++] = m_Lexema;

	m_Lexema = Lexema;
}
