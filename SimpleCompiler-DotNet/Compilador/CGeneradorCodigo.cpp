#include "CGeneradorCodigo.h"

CGeneradorCodigo::CGeneradorCodigo()
{
	m_pListaInstrucciones = NULL;
	m_pDefinicionObjeto = NULL;
}

CGeneradorCodigo::~CGeneradorCodigo()
{
	
}

bool CGeneradorCodigo::GenerarCodigo(CSyntTree *pTree, BuscarFuncionNativaFunc *pBuscarFuncionNativa)
{
	bool bOk;

	try
	{
		m_ListaDefiniciones.Limpiar();

		m_pBuscarFuncionNativa = pBuscarFuncionNativa;

		PrvAgregarInstruccion(pTree);

		bOk = true;
	}
	catch(CGeneradorCodigoError Error)
	{
		printf("Error: %s\n", Error.GetDescription());

		m_ListaDefiniciones.Limpiar();

		bOk = false;
	}

	return(bOk);
}

CTipoDato CGeneradorCodigo::PrvTraducirSimboloATipoDato(CSimbolo* pSimbolo)
{
	CTipoDato TipoDato;

	if (pSimbolo != NULL)
	{
		const char* pcNombre = pSimbolo->GetNombre();

		if (strcmp(pcNombre, SIMBOLO_INT) == 0)
			TipoDato.SetearTipoDato(CTipoDato::TIPO_INT);
		else if (strcmp(pcNombre, SIMBOLO_FLOAT) == 0)
			TipoDato.SetearTipoDato(CTipoDato::TIPO_FLOAT);
		else if (strcmp(pcNombre, SIMBOLO_VOID) == 0)
			TipoDato.SetearTipoDato(CTipoDato::TIPO_VOID);
		else if (strcmp(pcNombre, SIMBOLO_STRING) == 0)
			TipoDato.SetearTipoDato(CTipoDato::TIPO_CADENA);
		else
		{
			TipoDato.SetearTipoDato(CTipoDato::TIPO_OBJETO);
			TipoDato.SetearDefinicionObjeto(m_ListaDefiniciones.BuscarDefinicion(pcNombre));
		}
	}
	else
	{
		TipoDato.SetearTipoDato(CTipoDato::TIPO_VOID);
	}

	return(TipoDato);
}

void CGeneradorCodigo::PrvAgregarInstruccion(CSyntTree* pNodo)
{
	int i;

	switch(pNodo->GetType())
	{
		case CSyntTree::SYNT_INSTRUCCION_NULA:
			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_NADA);
			break;

		case CSyntTree::SYNT_DECLARACION_FUNCION:
		{
			CSimbolo* pDefinicionFuncion = pNodo->GetSimboloDefinicionFuncion();

			if (!pDefinicionFuncion->GetFuncionNativa())
			{
				m_pFuncion = m_pDefinicionObjeto->BuscarFuncion(pDefinicionFuncion->GetNombreFuncionCompleto());

				CTablaSimbolos* pTablaSimbolos = pNodo->GetChild(0)->GetTablaSimbolos();

				int nCantVars = 0;

				for (i = 0; i < pTablaSimbolos->GetSimbolos(); i++)
				{
					CSimbolo* pSimbolo = pTablaSimbolos->GetSimbolo(i);

					m_TablaVariables.AgregarVariableLocal(pSimbolo->GetNombre());

					nCantVars++;
				}
				
				m_pListaInstrucciones = m_pFuncion->ObtenerListaInstrucciones();

				for (i = 0; i < pNodo->GetChilds(); i++)
					PrvAgregarInstruccion(pNodo->GetChild(i));

				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RETURN);
				
				while(nCantVars-- != 0)
					m_TablaVariables.EliminarVariable();

				m_pListaInstrucciones = NULL;
				m_pFuncion = NULL;
			}
			else
			{
				CFuncion* pFuncion = m_pDefinicionObjeto->BuscarFuncion(pDefinicionFuncion->GetNombreFuncionCompleto());

				if (m_pBuscarFuncionNativa != NULL)
				{
					FuncionNativa* pFuncionNativa = m_pBuscarFuncionNativa(pDefinicionFuncion->GetNombreFuncionCompleto());

					if (pFuncionNativa != NULL)
					{
						pFuncion->SetearTipoFuncion(CFuncion::FUNCION_NATIVA);

						pFuncion->SetearFuncionNativa(pFuncionNativa);
					}
					else
					{
						CCadena cad;

						cad.ConcatenarCadena("No se encontro la función externa ");
						cad.ConcatenarCadena(pDefinicionFuncion->GetNombreFuncionCompleto());

						ThrowGeneradorCodigoError(cad);
					}
				}
				else
				{
					CCadena cad;

					cad.ConcatenarCadena("No se encontro la función externa ");
					cad.ConcatenarCadena(pDefinicionFuncion->GetNombreFuncionCompleto());

					ThrowGeneradorCodigoError(cad);
				}
			}
			break;
		}

		case CSyntTree::SYNT_RETURN:
			PrvAgregarInstruccion(pNodo->GetChild(0));
			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RETURN);
			break;

		case CSyntTree::SYNT_LISTA_FUNCIONES:
		{
			m_pDefinicionObjeto = new CDefinicionObjeto();

			m_ListaDefiniciones.AgregarDefinicionObjeto(m_pDefinicionObjeto);

			m_pDefinicionObjeto->SetearNombre(pNodo->GetString());

			CTablaSimbolos* pTablaSimbolos = pNodo->GetChild(0)->GetTablaSimbolos();

			int nCantVars = 0;

			for (i = 0; i < pTablaSimbolos->GetSimbolos(); i++)
			{
				CSimbolo* pSimbolo = pTablaSimbolos->GetSimbolo(i);

				switch(pSimbolo->GetType())
				{
					case CSimbolo::SIMBOLO_TIPO_VARIABLE:
					{
						CDefinicionVariable* pVariable = new CDefinicionVariable();

						*pVariable->TipoDato() = PrvTraducirSimboloATipoDato(pSimbolo->GetReturnType());
						pVariable->SetearNombre(pSimbolo->GetNombre());

						m_pDefinicionObjeto->AgregarVariable(pVariable);

						m_TablaVariables.AgregarVariableClase(pSimbolo->GetNombre());

						nCantVars++;

						break;
					}

					case CSimbolo::SIMBOLO_TIPO_FUNCION:
					{
						CFuncion* pFuncion = new CFuncion();

						m_pDefinicionObjeto->AgregarFuncion(pFuncion);

						CSimbolo* pDefinicionFuncion = pSimbolo;

						pFuncion->SetearNombre(pDefinicionFuncion->GetNombreFuncionCompleto());

						*pFuncion->TipoDevuelto() = PrvTraducirSimboloATipoDato(pDefinicionFuncion->GetReturnType());

						pFuncion->SetCantVariables(pDefinicionFuncion->GetCantVariables());

						for (int i = 0; i < pDefinicionFuncion->GetParametros(); i++)
						{
							CDefinicionVariable* pParametro = new CDefinicionVariable();

							*pParametro->TipoDato() = PrvTraducirSimboloATipoDato(pDefinicionFuncion->GetParametro(i)->GetReturnType());
							pParametro->SetearNombre(pDefinicionFuncion->GetParametro(i)->GetNombre());

							pFuncion->AddParametro(pParametro);
						}
						break;
					}
				}
			}

			for (i = 0; i < pNodo->GetChilds(); i++)
				PrvAgregarInstruccion(pNodo->GetChild(i));

			while(nCantVars-- != 0)
				m_TablaVariables.EliminarVariable();

			m_pDefinicionObjeto = NULL;		

			break;
		}

		case CSyntTree::SYNT_LISTA_INSTRUCCIONES:
		{
			int nCantVarsLocales = m_TablaVariables.CantidadVariablesLocales();

			for (i = 0; i < pNodo->GetChilds(); i++)
				PrvAgregarInstruccion(pNodo->GetChild(i));

			while (m_TablaVariables.CantidadVariablesLocales() != nCantVarsLocales)
				m_TablaVariables.EliminarVariable();

			break;
		}

		case CSyntTree::SYNT_INSTRUCCION:
			PrvAgregarInstruccion(pNodo->GetChild(0));
			if (pNodo->GetReturnType() != NULL)
				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_VOID) != 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP);
			break;

		case CSyntTree::SYNT_DECLARACION_VARIABLE:
			if (m_pListaInstrucciones != NULL)
			{
				for (i = 0; i < pNodo->GetChilds(); i++)
				{
					m_TablaVariables.AgregarVariableLocal(pNodo->GetChild(i)->GetString());

					//Me fijo si tiene algun inicializador
					if (pNodo->GetChild(i)->GetChilds() != 0)
					{
						//Tiene un inicializador
						PrvAgregarInstruccion(pNodo->GetChild(i)->GetChild(0));
					}
					else
					{
						//No tiene inicializador
						//Genero el código que inicializa la variable a los tipos de datos básicos
						if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_FLOAT, PrvAgregarConstante((float) 0.0f));

						else if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_INT, PrvAgregarConstante((long) 0));

						else if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_STRING, PrvAgregarConstante(""));

						else
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_NULL);
					}
					
					//Genero el código que va a sacar del stack el valor con el cual se inicializa la variable
					//y se lo va a asignar a la variable
					if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
						m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_FLOAT, m_TablaVariables.CantidadVariablesLocales() - 1);
					
					else if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
						m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_INT, m_TablaVariables.CantidadVariablesLocales() - 1);
					
					else if (strcmp(pNodo->GetChild(i)->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
						m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_STRING, m_TablaVariables.CantidadVariablesLocales() - 1);
					
					else
						m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_OBJ, m_TablaVariables.CantidadVariablesLocales() - 1);
				}
			}
			break;

		case CSyntTree::SYNT_LLAMADO_FUNCION:
			for (i = 0; i < pNodo->GetChilds(); i++)
				PrvAgregarInstruccion(pNodo->GetChild(i));

			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_CALL_THIS_CLASS_FUNCTION, m_pDefinicionObjeto->BuscarPosicionFuncion(pNodo->GetString()));
			break;

		case CSyntTree::SYNT_TABLA_SIMBOLOS:
			break;

		case CSyntTree::SYNT_VARIABLE:
		{
			int nPos = m_TablaVariables.BuscarVariable(pNodo->GetString());

			if (m_TablaVariables.UbicacionVariable(nPos) == CTablaVariables::UBICACION_LOCAL)
			{
				nPos = m_TablaVariables.PosicionVariable(nPos);

				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_VAR_FLOAT, nPos);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_VAR_INT, nPos);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_VAR_STRING, nPos);
				else 
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_VAR_OBJ, nPos);
			}
			else
			{
				nPos = m_TablaVariables.PosicionVariable(nPos);

				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_FLOAT, nPos);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_INT, nPos);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_STRING, nPos);
				else
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_OBJ, nPos);
			}

			break;
		}

		case CSyntTree::SYNT_IDENTIFICADOR:
			break;

		case CSyntTree::SYNT_CONSTANTE:
			if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_STRING, PrvAgregarConstante(pNodo->GetString()));
			else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_FLOAT, PrvAgregarConstante(pNodo->GetFloat()));
			else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_INT, PrvAgregarConstante(pNodo->GetInteger()));
			break;

		case CSyntTree::SYNT_CONVERSION:
			PrvAgregarInstruccion(pNodo->GetChild(0));

			if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_INT_A_FLOAT);
			else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_FLOAT_A_INT);
			break;

		case CSyntTree::SYNT_OP_MIEMBRO:
			
			switch(pNodo->GetChild(1)->GetType())
			{
				case CSyntTree::SYNT_VARIABLE:
				{
					//La parte izquierda del punto deja en el stack el objeto al cual tengo que acceder
					PrvAgregarInstruccion(pNodo->GetChild(0));

					int nPos = PrvAgregarConstante(pNodo->GetChild(1)->GetString());
					
					switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(1)->GetReturnType()).ObtenerTipoDato())
					{
						case CTipoDato::TIPO_INT:
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_INT, nPos);
							break;
						case CTipoDato::TIPO_FLOAT:
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_FLOAT, nPos);
							break;
						case CTipoDato::TIPO_OBJETO:
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_OBJ, nPos);
							break;
						case CTipoDato::TIPO_CADENA:
							m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_STRING, nPos);
							break;
					}
					break;
				}

				case CSyntTree::SYNT_LLAMADO_FUNCION:
				{
					//Parametros de la función que voy a llamar
					for (i = 0; i < pNodo->GetChild(1)->GetChilds(); i++)
						PrvAgregarInstruccion(pNodo->GetChild(1)->GetChild(i));

					//La parte izquierda del punto deja en el stack el objeto al cual tengo que acceder
					PrvAgregarInstruccion(pNodo->GetChild(0));

					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_CALL_CLASS_FUNCTION, PrvAgregarConstante(pNodo->GetChild(1)->GetString()));
					break;
				}
			}
			break;

		case CSyntTree::SYNT_OP_ASIGNACION:
			//Agrego las instrucciones del lado derecho que dejan en el stack el valor a asignar
			PrvAgregarInstruccion(pNodo->GetChild(1));
			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_DUP);
			
			//Asgino el valor del stack a la variable segun corresponda..
			switch(pNodo->GetChild(0)->GetType())
			{
				case CSyntTree::SYNT_VARIABLE:
				{
					int nPos = m_TablaVariables.BuscarVariable(pNodo->GetChild(0)->GetString());

					switch(m_TablaVariables.UbicacionVariable(nPos))
					{
						case CTablaVariables::UBICACION_LOCAL:
							nPos = m_TablaVariables.PosicionVariable(nPos);

							switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(0)->GetReturnType()).ObtenerTipoDato())
							{
								case CTipoDato::TIPO_INT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_INT, nPos);
									break;
								case CTipoDato::TIPO_FLOAT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_FLOAT, nPos);
									break;
								case CTipoDato::TIPO_OBJETO:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_OBJ, nPos);
									break;
								case CTipoDato::TIPO_CADENA:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_VAR_STRING, nPos);
									break;
							}
							
							break;

						case CTablaVariables::UBICACION_CLASE:
							nPos = m_TablaVariables.PosicionVariable(nPos);
							
							switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(0)->GetReturnType()).ObtenerTipoDato())
							{
								case CTipoDato::TIPO_INT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_INT, nPos);
									break;
								case CTipoDato::TIPO_FLOAT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_FLOAT, nPos);
									break;
								case CTipoDato::TIPO_OBJETO:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_OBJ, nPos);
									break;
								case CTipoDato::TIPO_CADENA:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_STRING, nPos);
									break;
							}

							break;
					}

					break;
				}

				case CSyntTree::SYNT_OP_MIEMBRO:
				{
					pNodo = pNodo->GetChild(0);
					//La parte izquierda del punto deja en el stack el objeto al cual tengo que acceder
					PrvAgregarInstruccion(pNodo->GetChild(0));
					
					switch(pNodo->GetChild(1)->GetType())
					{
						case CSyntTree::SYNT_VARIABLE:
						{
							int nPos = PrvAgregarConstante(pNodo->GetChild(1)->GetString());
							
							switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(1)->GetReturnType()).ObtenerTipoDato())
							{
								case CTipoDato::TIPO_INT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_CLASS_VAR_INT, nPos);
									break;
								case CTipoDato::TIPO_FLOAT:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_CLASS_VAR_FLOAT, nPos);
									break;
								case CTipoDato::TIPO_OBJETO:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_CLASS_VAR_OBJ, nPos);
									break;
								case CTipoDato::TIPO_CADENA:
									m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP_CLASS_VAR_STRING, nPos);
									break;
							}
							break;
						}
					}
					break;
				}
			}
			break;

		case CSyntTree::SYNT_OP_COMPARACION_IGUAL:
			//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
			PrvAgregarInstruccion(pNodo->GetChild(1));
			//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
			PrvAgregarInstruccion(pNodo->GetChild(0));

			//Agrego la instrucción de comparación que corresponda
			switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(0)->GetReturnType()).ObtenerTipoDato())
			{
				case CTipoDato::TIPO_INT:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_INT);
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_ISZERO_INT);
					break;
				case CTipoDato::TIPO_FLOAT:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_FLOAT);
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_FLOAT_A_INT);
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_ISZERO_INT);
					break;
				case CTipoDato::TIPO_CADENA:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_COMPARE_STRING);
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_ISZERO_INT);
					break;
			}
			break;

		case CSyntTree::SYNT_OP_COMPARACION_DISTINTO:
			//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
			PrvAgregarInstruccion(pNodo->GetChild(1));
			//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
			PrvAgregarInstruccion(pNodo->GetChild(0));

			//Agrego la instrucción de comparación que corresponda
			switch(PrvTraducirSimboloATipoDato(pNodo->GetChild(0)->GetReturnType()).ObtenerTipoDato())
			{
				case CTipoDato::TIPO_INT:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_INT);
					break;
				case CTipoDato::TIPO_FLOAT:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_FLOAT);
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_FLOAT_A_INT);
					break;
				case CTipoDato::TIPO_CADENA:
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_COMPARE_STRING);
					break;
			}
			break;

		case CSyntTree::SYNT_OP_MAS:
			if (pNodo->GetChild(1)->GetType() == CSyntTree::SYNT_CONSTANTE && 
				strcmp(pNodo->GetChild(1)->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0 &&
				pNodo->GetChild(1)->GetInteger() == 1)
			{
				//Uso el operador de incremento de int en 1
				PrvAgregarInstruccion(pNodo->GetChild(0));
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_INC_INT);
			}
			else
			{
				//Uso la suma normal
				for (i = 0; i < pNodo->GetChilds(); i++)
					PrvAgregarInstruccion(pNodo->GetChild(i));
				
				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_SUMAR_FLOAT);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_SUMAR_INT);
				else if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_CONCAT_STRING);
			}

			break;

		case CSyntTree::SYNT_OP_MENOS:
			if (pNodo->GetChild(1)->GetType() == CSyntTree::SYNT_CONSTANTE && 
				strcmp(pNodo->GetChild(1)->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0 &&
				pNodo->GetChild(1)->GetInteger() == 1)
			{
				//Uso el operador de decremento de int en 1
				PrvAgregarInstruccion(pNodo->GetChild(0));
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_DEC_INT);
			}
			else
			{
				//Uso la resta normal
				for (i = 0; i < pNodo->GetChilds(); i++)
					PrvAgregarInstruccion(pNodo->GetChild(i));

				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_FLOAT);
				else
					if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
						m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_RESTAR_INT);
			}
			break;

		case CSyntTree::SYNT_OP_POR:
			for (i = 0; i < pNodo->GetChilds(); i++)
				PrvAgregarInstruccion(pNodo->GetChild(i));

			if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_MULT_FLOAT);
			else
				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_MULT_INT);
			break;

		case CSyntTree::SYNT_OP_DIV:
			for (i = 0; i < pNodo->GetChilds(); i++)
				PrvAgregarInstruccion(pNodo->GetChild(i));

			if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_DIV_FLOAT);
			else
				if (strcmp(pNodo->GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
					m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_DIV_INT);
			break;

		case CSyntTree::SYNT_IF:
		{
			//Agrego las instrucciones que ejecutan la condicion
			PrvAgregarInstruccion(pNodo->GetChild(0));
			//Agrego el salto segun la condicion
			int nInstruccionIf = m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP_IF_ZERO, 0);
			//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
			PrvAgregarInstruccion(pNodo->GetChild(1));
			
			int nInstruccionElse;
			
			if (pNodo->GetChilds() == 3)
				nInstruccionElse = m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP, 0);

			int nInstruccion = m_pListaInstrucciones->ObtenerInstrucciones();

			//Actualizo la instruccion de salto cuando la condición no se cumple
			m_pListaInstrucciones->SetearParametro(nInstruccionIf, nInstruccion - nInstruccionIf - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType));
			
			if (pNodo->GetChilds() == 3)
			{
				//Agrego las instrucciones que se ejecutan cuando la condicion no se cumple
				PrvAgregarInstruccion(pNodo->GetChild(2));

				nInstruccion = m_pListaInstrucciones->ObtenerInstrucciones();

				m_pListaInstrucciones->SetearParametro(nInstruccionElse, nInstruccion - nInstruccionElse - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType));
			}
									
			break;
		}

		case CSyntTree::SYNT_WHILE:
		{
			int nInstCondicion = m_pListaInstrucciones->ObtenerInstrucciones();

			//Agrego las instrucciones que ejecutan la condicion
			PrvAgregarInstruccion(pNodo->GetChild(0));
			//Agrego el salto segun la condicion
			int nInstruccionSaltoNoCumple = m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP_IF_ZERO, 0);
			//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
			PrvAgregarInstruccion(pNodo->GetChild(1));
			
			//Agrego la instruccion que salta hacia donde se evalua el ciclo
			int nInstFinCiclo = m_pListaInstrucciones->ObtenerInstrucciones();
			int Salto = nInstCondicion - nInstFinCiclo - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType);
			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP, Salto);

			int nInstruccion = m_pListaInstrucciones->ObtenerInstrucciones();

			//Actualizo la instruccion de salto cuando la condición no se cumple
			m_pListaInstrucciones->SetearParametro(nInstruccionSaltoNoCumple, nInstruccion - nInstruccionSaltoNoCumple - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType));
			break;
		}

		case CSyntTree::SYNT_FOR:
		{
			//Agrego las instrucciones que inicializan la condición el ciclo
			PrvAgregarInstruccion(pNodo->GetChild(0));
			if (PrvTraducirSimboloATipoDato(pNodo->GetChild(0)->GetReturnType()).ObtenerTipoDato() != CTipoDato::TIPO_VOID)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP);

			//Agrego las instrucciones que evaluan la condición del ciclo
			int nInstCondicion = m_pListaInstrucciones->ObtenerInstrucciones();
			PrvAgregarInstruccion(pNodo->GetChild(1));
			
			//Agrego el salto segun el resultado de la condicion
			int nInstCondicionSalto = m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP_IF_ZERO, 0);

			//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
			PrvAgregarInstruccion(pNodo->GetChild(3));

			//Agrego las instrucciones que incrementar en contador del ciclo
			PrvAgregarInstruccion(pNodo->GetChild(2));
			if (PrvTraducirSimboloATipoDato(pNodo->GetChild(2)->GetReturnType()).ObtenerTipoDato() != CTipoDato::TIPO_VOID)
				m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_STACK_POP);

			//Agrego la instruccion que salta hacia donde se evalua el ciclo
			int nInstFinCiclo = m_pListaInstrucciones->ObtenerInstrucciones();
			int Salto = nInstCondicion - nInstFinCiclo - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType);
			m_pListaInstrucciones->AgregarInstruccion(CListaInstrucciones::INST_JUMP, Salto);

			//Actualizo la instruccion de salto a fin de ciclo donde se evalua la condicion
			nInstFinCiclo = m_pListaInstrucciones->ObtenerInstrucciones();
			Salto = nInstFinCiclo - nInstCondicionSalto - sizeof(CListaInstrucciones::InstruccionType) - sizeof(CListaInstrucciones::ParametroType);
			m_pListaInstrucciones->SetearParametro(nInstCondicionSalto, Salto);
			break;
		}

		case CSyntTree::SYNT_NONE:
			break;
	}
}

int CGeneradorCodigo::PrvAgregarConstante(long valor)
{
	int n;

	n = m_pDefinicionObjeto->GetTablaConstantes()->FindSimboloInt(valor);

	if (n == CTablaSimbolosSimple::NO_ENCONTRADO)
	{
		CSimboloSimple* pSimbolo = new CSimboloSimple();
		pSimbolo->SetType(CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_INT);
		pSimbolo->SetInt(valor);
		n = m_pDefinicionObjeto->GetTablaConstantes()->AddSimbolo(pSimbolo);
	}

	return(n);
}

int CGeneradorCodigo::PrvAgregarConstante(float valor)
{
	int n;

	n = m_pDefinicionObjeto->GetTablaConstantes()->FindSimboloFloat(valor);

	if (n == CTablaSimbolosSimple::NO_ENCONTRADO)
	{
		CSimboloSimple* pSimbolo = new CSimboloSimple();
		pSimbolo->SetType(CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_FLOAT);
		pSimbolo->SetFloat(valor);
		n = m_pDefinicionObjeto->GetTablaConstantes()->AddSimbolo(pSimbolo);
	}

	return(n);
}

int CGeneradorCodigo::PrvAgregarConstante(const char* valor)
{
	int n;

	n = m_pDefinicionObjeto->GetTablaConstantes()->FindSimboloString(valor);

	if (n == CTablaSimbolosSimple::NO_ENCONTRADO)
	{
		CSimboloSimple* pSimbolo = new CSimboloSimple();
		pSimbolo->SetType(CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_STRING);
		pSimbolo->SetString(valor);
		n = m_pDefinicionObjeto->GetTablaConstantes()->AddSimbolo(pSimbolo);
	}

	return(n);
}

int CGeneradorCodigo::PrvAgregarSimbolo(CSimbolo* pSimbolo)
{
	int n = -1;

	return(n);
}
