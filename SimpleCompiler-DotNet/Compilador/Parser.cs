using System;
using System.Collections;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.Compilador
{
	public class ParserError : Exception
	{
		public long line;
		public long column;

		public ParserError(string descripcion, long line, long column) : base(descripcion)
		{ 
			this.line = line;
			this.column = column;
		}
	}


	public class Parser
	{
		#region Atributos

		const int MAX_STACK_LEXEMAS = 12;
		Lexer lexer;
		Lexema lexema;

		Lexema[] stackLexemas;
		int		posStackLexemas;

		TablaSimbolos tablaSimbolos;
		Simbolo	simboloInt;
		Simbolo	simboloFloat;
		Simbolo	simboloString;
		Simbolo	simboloVoid;

		Simbolo tipoReturn;

		int stackVariables;
		int maxVariables;

		Funcion[] funcionesExternas;

		#endregion

		#region Metodos Publicos

		public Parser()
		{
            stackLexemas = new Lexema[MAX_STACK_LEXEMAS];
			posStackLexemas = 0;
			lexer = new Lexer();
			lexema = new Lexema();
		}

		public void Clear()
		{
			posStackLexemas = 0;
			lexer.Clear();
			lexer = new Lexer();
			lexema = new Lexema();
		}

		public void SetProgram(String text)
		{
			lexer.Clear();
			lexer.SetTextToParse(text);
		}

		public void SetFuncionesExternas(Funcion[] funciones)
		{
            funcionesExternas = funciones;            
		}

		public bool GenSyntTree(out SyntTree root, out ArrayList errores)
		{
			bool ok = true;

			PrvCrearTablaSimbolos();

			errores = new ArrayList();

			root = null;

			try
			{
				PrvGetLexema();

				root = PrvFunciones();
			}
			catch(ParserError error)
			{
				errores.Add(error);

				ok = false;
			}

			return(ok);
		}

		#endregion

		#region Metodos Privados

		#region Funciones Auxiliares

		void PrvGetLexema()
		{
			if (posStackLexemas > 0)
				lexema = stackLexemas[--posStackLexemas];
			else
				lexer.GetLexema(ref lexema);
		}

		void PrvReturnLexema(Lexema lexema)
		{
			if (posStackLexemas == MAX_STACK_LEXEMAS)
				throw CrearParserError("El stack de lexemas esta lleno");

			stackLexemas[posStackLexemas++] = this.lexema;

			this.lexema = lexema;
		}
		
		void PrvCrearTablaSimbolos()
		{
			tablaSimbolos = new TablaSimbolos();

			simboloInt = Simbolo.SimboloInt;
			tablaSimbolos.AddSimbolo(simboloInt);

			simboloFloat = Simbolo.SimboloFloat;
			tablaSimbolos.AddSimbolo(simboloFloat);

			simboloString = Simbolo.SimboloString;
			tablaSimbolos.AddSimbolo(simboloString);

			simboloVoid = Simbolo.SimboloVoid;
			tablaSimbolos.AddSimbolo(simboloVoid);

			if (funcionesExternas != null)
			{
				foreach(Funcion f in funcionesExternas)
				{
					Simbolo simboloFuncion = new Simbolo();

					simboloFuncion.Type = Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION;
					simboloFuncion.ReturnType = tablaSimbolos.FindSimboloGlobal(f.TipoDevuelto.ToString());
					simboloFuncion.Nombre = f.NombreCorto;

					for (int i = 0; i < f.GetParametros(); i++)
					{
						DefinicionVariable defPar = f.GetParametro(i);

                        Simbolo tipoDatoParametro = tablaSimbolos.FindSimboloGlobal(defPar.Tipo.ToString());

						Simbolo parametro = new Simbolo();

						parametro.Type = Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE;
						parametro.Nombre = defPar.Nombre;
						parametro.ReturnType = tipoDatoParametro;

						simboloFuncion.AddParametro(parametro);
					}

					simboloFuncion.ActualizarNombreCompletoFuncion();

					simboloFuncion.FuncionNativa = true;

					tablaSimbolos.AddSimbolo(simboloFuncion);
				}
			}
		}

		SyntTree PrvAddTablaSimbolos(TablaSimbolos tablaSimbolos)
		{
			SyntTree node = new SyntTree();

			node.Type = SyntTree.SyntTypeEnum.SYNT_TABLA_SIMBOLOS;
			node.TablaSimbolos = tablaSimbolos;

			return node;
		}

		SyntTree PrvConvertirReturnType(SyntTree nodo, Simbolo returnType)
		{
			if (nodo.ReturnType != returnType)
			{
				if (nodo.ReturnType == simboloInt)
				{
					if (returnType == simboloFloat)
					{
						SyntTree conversor = new SyntTree();
						conversor.Type = SyntTree.SyntTypeEnum.SYNT_CONVERSION;
						conversor.ReturnType = simboloFloat;
						conversor.AddChild(nodo);
						nodo = conversor;
					}
				}
				else
				{
					if (nodo.ReturnType == simboloFloat)
					{
						if (returnType == simboloInt)
						{
							SyntTree conversor = new SyntTree();
							conversor.Type = SyntTree.SyntTypeEnum.SYNT_CONVERSION;
							conversor.ReturnType = simboloInt;
							conversor.AddChild(nodo);
							nodo = conversor;
						}
					}
				}
			}

			return nodo;
		}

		private ParserError CrearParserError(string descripcion)
		{
			return new ParserError(descripcion, this.lexema.Line, this.lexema.Column);
		}

		#endregion

		#region Generación de arbol de parseo
		
		SyntTree PrvFunciones()
		{
			SyntTree node = null;

			node = new SyntTree();

			node.Type = SyntTree.SyntTypeEnum.SYNT_LISTA_FUNCIONES;

			node.AddChild(PrvAddTablaSimbolos(tablaSimbolos));

			while (this.lexema.Type != Lexema.LexTypeEnum.LEX_EOF)
			{
				SyntTree child = PrvFuncion();
				
				if (child == null)
					child = PrvDeclaracionVariable(false);

				if (child == null)
					throw CrearParserError("Se esperaba una función o declaración de variable");

				node.AddChild(child);
			}

			return node;
		}

		SyntTree PrvFuncion()
		{
			SyntTree node = null;

			if (lexema.Type == Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
			{
				Simbolo simboloTipoReturn = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

				if (simboloTipoReturn == null)
					throw CrearParserError("El simbolo especificado no se encontro");

				if (simboloTipoReturn.Type == Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_DATO)
				{
					Lexema lexTipoDato = this.lexema;
					PrvGetLexema();

					if (this.lexema.Type == Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
					{
						Simbolo simboloFuncion = this.tablaSimbolos.FindSimboloLocal(this.lexema.String);

						//Valido que el identificador este no utiliza o, si esta utilizado, que sea una función y que coincida el return type
						//Despues de obtener los parametros, ahi genero el nombre completo
						//de la función y en base a eso determino si la función ya esta declarada o no
						if (simboloFuncion != null && simboloFuncion.Type != Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION)
							throw CrearParserError("El identificador ya esta declarado como una variable");

						if (simboloFuncion != null && simboloFuncion.ReturnType != simboloTipoReturn)
							throw CrearParserError("No se pueden tener 2 funciones con el mismo nombre y que devuelvan tipos de datos distintos");

						Lexema lexNombreFuncion = this.lexema;
						PrvGetLexema();

						if (this.lexema.Type == Lexema.LexTypeEnum.LEX_PAR_ABRE)
						{
							Simbolo oldTipoReturn = this.tipoReturn;

							this.tipoReturn = simboloTipoReturn;

							simboloFuncion = new Simbolo();

							simboloFuncion.Type = Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION;
							simboloFuncion.ReturnType = simboloTipoReturn;
							simboloFuncion.Nombre = lexNombreFuncion.String;

							//Lo agrego antes de haber verificado si ya esta declarado o no, de
							//esta forma me evito los problemas de borrar el simbolo cuando
							//falle si ya existe la función
							this.tablaSimbolos.AddSimbolo(simboloFuncion);

							node = new SyntTree();
							node.Type = SyntTree.SyntTypeEnum.SYNT_DECLARACION_FUNCION;
							node.ReturnType = null;

							PrvGetLexema();

							TablaSimbolos tabla = new TablaSimbolos();
							tabla.TablaPadre = this.tablaSimbolos;
							this.tablaSimbolos = tabla;

							node.AddChild(PrvAddTablaSimbolos(tabla));

							try
							{
								bool bPrimerParametro = true;

								while(this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
								{
									if (bPrimerParametro == false)
									{
										if (this.lexema.Type != Lexema.LexTypeEnum.LEX_COMA)
											throw CrearParserError("Se esperaba ,");

										PrvGetLexema();
									}
									else
									{
										bPrimerParametro = false;
									}

									if (this.lexema.Type != Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
										throw CrearParserError("Se esperaba un tipo de dato");

									Simbolo tipoDatoParametro = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

									if (tipoDatoParametro == null)
										throw CrearParserError("Se esperaba un tipo de dato");

									PrvGetLexema();

									if (this.lexema.Type != Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
										throw CrearParserError("Se esperaba un identificador");

									Simbolo parametro = this.tablaSimbolos.FindSimboloLocal(this.lexema.String);

									if (parametro != null)
										throw CrearParserError("El simbolo ya esta definido");

									parametro = new Simbolo();

									parametro.Type = Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE;
									parametro.Nombre = this.lexema.String;
									parametro.ReturnType = tipoDatoParametro;

									simboloFuncion.AddParametro(parametro);

									this.tablaSimbolos.AddSimbolo(parametro);

									PrvGetLexema();
								}

								PrvGetLexema();

								//Ya obtuve el tipo de dato de todos los parametros, ahora genero el nombre completo de la función
								simboloFuncion.ActualizarNombreCompletoFuncion();

								Simbolo simboloComparacion = this.tablaSimbolos.FindSimboloGlobal(simboloFuncion.Nombre);

								while(simboloComparacion != null)
								{
									if (simboloComparacion != simboloFuncion)
										if (simboloFuncion.NombreFuncionCompleto == simboloComparacion.NombreFuncionCompleto)
                                            throw CrearParserError("Ya existe una función con el mismo nombre y los mismos parametros");

									simboloComparacion = this.tablaSimbolos.FindProximoSimboloGlobal(simboloComparacion);
								}

								node.String = simboloFuncion.Nombre;
								node.SimboloDefinicionFuncion = simboloFuncion;

								this.stackVariables = simboloFuncion.GetParametros();

								if (simboloFuncion.ReturnType.Nombre != TipoDato.NOMBRE_VOID)
									this.stackVariables++;

								this.maxVariables = this.stackVariables;

								SyntTree instrucciones = PrvInstrucciones();

								if (instrucciones == null)
								{
									if (this.lexema.Type == Lexema.LexTypeEnum.LEX_END)
									{
										PrvGetLexema();

										simboloFuncion.FuncionNativa = true;
									}
									else
										throw CrearParserError("Se esperaba {");
								}
								else
								{
									node.AddChild(instrucciones);
								}

								simboloFuncion.CantVariables = this.maxVariables;
							}
							catch(ParserError err)
							{
								this.tablaSimbolos = this.tablaSimbolos.TablaPadre;
								throw err;
							}

							this.tablaSimbolos = this.tablaSimbolos.TablaPadre;

							this.tipoReturn = oldTipoReturn;
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

			return node;
		}


		SyntTree PrvInstrucciones()
		{
			SyntTree node = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_LLAVE_ABRE)
			{
				PrvGetLexema();

				node = new SyntTree();

				node.Type = SyntTree.SyntTypeEnum.SYNT_LISTA_INSTRUCCIONES;

				TablaSimbolos tabla;

				tabla = new TablaSimbolos();
				tabla.TablaPadre = this.tablaSimbolos;
				this.tablaSimbolos = tabla;

				node.AddChild(PrvAddTablaSimbolos(tabla));

				int oldStackVars = stackVariables;

				try
				{
					while (this.lexema.Type != Lexema.LexTypeEnum.LEX_LLAVE_CIERRA)
					{
						SyntTree child = PrvInstruccion();

						if (child == null)
							throw CrearParserError("Se esperaba una instrucción");
						
						node.AddChild(child);
					}

					PrvGetLexema();
				}
				catch(ParserError err)
				{
					this.tablaSimbolos = this.tablaSimbolos.TablaPadre;
					throw err;
				}

				if (this.stackVariables > this.maxVariables)
					this.maxVariables = this.stackVariables;

				this.stackVariables = oldStackVars;

				this.tablaSimbolos = this.tablaSimbolos.TablaPadre;
			}

			return node;
		}

		SyntTree PrvInstruccion()
		{
			SyntTree node = null;
			SyntTree child = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_END)
			{
				child = new SyntTree();
				child.Type = SyntTree.SyntTypeEnum.SYNT_INSTRUCCION_NULA;

				PrvGetLexema();
			}
			else
			{
				if (this.lexema.Type == Lexema.LexTypeEnum.LEX_LLAVE_ABRE)
																	{
																		child = PrvInstrucciones();
																	}
				else
				{
					child = PrvExpresion();

					if (child != null)
					{
						if (this.lexema.Type != Lexema.LexTypeEnum.LEX_END)
							throw CrearParserError("Se esperaba ;");
						PrvGetLexema();
					}
			
					if (child == null)
						child = PrvDeclaracionVariable(true);

					if (child == null)
						child = PrvIf();

					if (child == null)
						child = PrvFor();

					if (child == null)
						child = PrvWhile();

					if (child == null)
						child = PrvReturn();
				}
			}

			if (child != null)
			{
				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_INSTRUCCION;

				node.AddChild(child);

				node.ReturnType = child.ReturnType;
			}

			return node;
		}

		SyntTree PrvDeclaracionVariable(bool permitirInicializacion)
		{
			SyntTree node = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
			{
				Simbolo simboloTipoDato = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

				if (simboloTipoDato == null)
					throw CrearParserError("El simbolo especificado no se encontro");

				if (simboloTipoDato.Type == Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_DATO)
				{
					if (simboloTipoDato == simboloVoid)
						throw CrearParserError("No se pueden declarar variables del tipo void");

					node = new SyntTree();
					node.Type = SyntTree.SyntTypeEnum.SYNT_DECLARACION_VARIABLE;
					node.String = this.lexema.String;
					node.ReturnType = null;

					PrvGetLexema();

					bool bPrimerDeclaracion = true;

					while (this.lexema.Type != Lexema.LexTypeEnum.LEX_END)
					{
						if (bPrimerDeclaracion == false)
						{
							if (this.lexema.Type != Lexema.LexTypeEnum.LEX_COMA)
								throw CrearParserError("Se esperaba ,");
							PrvGetLexema();
						}
						else
						{
							bPrimerDeclaracion = false;
						}

						if (this.lexema.Type != Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
							throw CrearParserError("Se esperaba un identificador");

						if (this.tablaSimbolos.FindSimboloLocal(this.lexema.String) != null)
							throw CrearParserError("La variable ya esta declarada");

						SyntTree variable = new SyntTree();

						variable.Type = SyntTree.SyntTypeEnum.SYNT_IDENTIFICADOR;
						variable.ReturnType = simboloTipoDato;
						variable.String = this.lexema.String;

						node.AddChild(variable);

						Simbolo simboloVariable;

						simboloVariable = new Simbolo();

						simboloVariable.Nombre = variable.String;
						simboloVariable.ReturnType = simboloTipoDato;
						simboloVariable.Type = Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE;

						this.tablaSimbolos.AddSimbolo(simboloVariable);

						PrvGetLexema();

						if (permitirInicializacion)
						{
							if (this.lexema.Type == Lexema.LexTypeEnum.LEX_OP_ASIGNACION)
							{
								PrvGetLexema();

								SyntTree inicializacion = null;
								//Es la inicialización de la variable
								inicializacion = PrvExpresion();

								if (inicializacion == null)
									throw CrearParserError("Se esperaba una expresion");

								inicializacion = PrvConvertirReturnType(inicializacion, simboloTipoDato);

								if (inicializacion.ReturnType != simboloTipoDato)
									throw CrearParserError("El tipo de dato devuelto por el inicializador no coincide con el tipo de dato de la variable");

								variable.AddChild(inicializacion);
							}
						}

						stackVariables++;

						if (stackVariables > maxVariables)
							maxVariables = stackVariables;
					}

					PrvGetLexema();
				}
			}

			return node;
		}

		SyntTree PrvIf()
		{
			SyntTree nodo = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_IF)
			{
				SyntTree condicion = null;
				SyntTree entonces = null;
				SyntTree sino = null;

				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_ABRE)
					throw CrearParserError("Se esperaba (");

				PrvGetLexema();

				condicion = PrvExpresion();

				if (condicion == null)
					throw CrearParserError("Se esperaba una expresion");

				condicion = PrvConvertirReturnType(condicion, simboloInt);

				if (condicion.ReturnType != simboloInt)
					throw CrearParserError("El tipo de dato devuelto no es evaluable");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
					throw CrearParserError("Se esperaba )");
		
				PrvGetLexema();

				entonces = PrvInstruccion();
		
				if (entonces == null)
					throw CrearParserError("Se esperaba una instrucción");

				if (this.lexema.Type == Lexema.LexTypeEnum.LEX_ELSE)
				{
					PrvGetLexema();

					sino = PrvInstruccion();
			
					if (sino == null)
						throw CrearParserError("Se esperaba una instrucción");
				}

				nodo = new SyntTree();

				nodo.Type = SyntTree.SyntTypeEnum.SYNT_IF;
				nodo.AddChild(condicion);
				nodo.AddChild(entonces);

				if (sino != null)
					nodo.AddChild(sino);
			}

			return(nodo);
		}

		SyntTree PrvFor()
		{
			SyntTree nodo = null;

			/* for (Inicializacion, Condicion, Incremento)
					Ciclo; */

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_FOR)
			{
				SyntTree inicializacion = null;
				SyntTree condicion = null;
				SyntTree incremento = null;
				SyntTree ciclo = null;

				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_ABRE)
					throw CrearParserError("Se esperaba (");

				PrvGetLexema();

				inicializacion = PrvExpresion();

				if (inicializacion == null)
					throw CrearParserError("Se esperaba una expresion");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_END)
					throw CrearParserError("Se esperaba ;");

				PrvGetLexema();

				condicion = PrvExpresion();

				if (condicion == null)
					throw CrearParserError("Se esperaba una expresion");

				condicion = PrvConvertirReturnType(condicion, simboloInt);

				if (condicion.ReturnType != simboloInt)
					throw CrearParserError("El tipo de dato devuelto por la expresion no es evaluable");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_END)
					throw CrearParserError("Se esperaba ;");

				PrvGetLexema();

				incremento = PrvExpresion();

				if (incremento == null)
					throw CrearParserError("Se esperaba una expresion");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
					throw CrearParserError("Se esperaba )");

				PrvGetLexema();

				ciclo = PrvInstruccion();
		
				if (ciclo == null)
					throw CrearParserError("Se esperaba una instrucción");

				nodo = new SyntTree();

				nodo.Type = SyntTree.SyntTypeEnum.SYNT_FOR;
				nodo.AddChild(inicializacion);
				nodo.AddChild(condicion);
				nodo.AddChild(incremento);
				nodo.AddChild(ciclo);
			}

			return(nodo);
		}

		SyntTree PrvWhile()
		{
			SyntTree nodo = null;

			/*while (condicion)
				ciclo;*/

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_WHILE)
			{
				SyntTree condicion = null;
				SyntTree ciclo = null;

				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_ABRE)
					throw CrearParserError("Se esperaba (");

				PrvGetLexema();

				condicion = PrvExpresion();

				if (condicion == null)
					throw CrearParserError("Se esperaba una expresion");

				condicion = PrvConvertirReturnType(condicion, simboloInt);

				if (condicion.ReturnType != simboloInt)
					throw CrearParserError("El tipo de dato devuelto no es evaluable");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
					throw CrearParserError("Se esperaba )");
		
				PrvGetLexema();

				ciclo = PrvInstruccion();
		
				if (ciclo == null)
					throw CrearParserError("Se esperaba una instrucción");

				nodo = new SyntTree();
				nodo.Type = SyntTree.SyntTypeEnum.SYNT_WHILE;
				nodo.AddChild(condicion);
				nodo.AddChild(ciclo);
			}

			return(nodo);
		}

		SyntTree PrvReturn()
		{
			SyntTree node = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_RETURN)
			{
				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_ABRE)
					throw CrearParserError("Se esperaba (");

				PrvGetLexema();

				SyntTree child = PrvExpresion();

				if (child == null)
					throw CrearParserError("Se esperaba una expresion");

				child = PrvConvertirReturnType(child, this.tipoReturn);

				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_RETURN;
				node.AddChild(child);

				if (child.ReturnType != this.tipoReturn)
					throw CrearParserError("El tipo de dato devuelto no coincide con el tipo de dato que devuelve la función.");

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
					throw CrearParserError("Se esperaba )");
		
				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_END)
					throw CrearParserError("Se esperaba ;");

				PrvGetLexema();
			}

			return node;
		}

		SyntTree PrvExpresion()
		{
			SyntTree nodo = null;
			bool bLoop = true;

			nodo = PrvSumaResta();

			while(bLoop)
			{
				switch(this.lexema.Type)
				{
					case Lexema.LexTypeEnum.LEX_OP_ASIGNACION:
					{
						if (nodo.Type != SyntTree.SyntTypeEnum.SYNT_VARIABLE)
							throw CrearParserError("El lado izquierdo de una asignación debe ser una variable");

						PrvGetLexema();

						SyntTree nodo2 = PrvSumaResta();

						if (nodo2 == null)
							throw CrearParserError("Error de sintaxis");

						nodo2 = PrvConvertirReturnType(nodo2, nodo.ReturnType);

						if (nodo2.ReturnType != nodo.ReturnType)
							throw CrearParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree asignacion = new SyntTree();

						asignacion.Type = SyntTree.SyntTypeEnum.SYNT_OP_ASIGNACION;
						asignacion.ReturnType = nodo.ReturnType;
						asignacion.AddChild(nodo);
						asignacion.AddChild(nodo2);
						nodo = asignacion;
						break;
					}

					case Lexema.LexTypeEnum.LEX_OP_IGUAL:
					{
						nodo = PrvOperadorLogico(nodo);
						nodo.Type = SyntTree.SyntTypeEnum.SYNT_OP_COMPARACION_IGUAL;
						break;
					}

					case Lexema.LexTypeEnum.LEX_OP_DISTINTO:
					{
						nodo = PrvOperadorLogico(nodo);
						nodo.Type = SyntTree.SyntTypeEnum.SYNT_OP_COMPARACION_DISTINTO;
						break;
					}

					default:
						bLoop = false;
						break;
				}
			}

			return(nodo);
		}

		SyntTree PrvSumaResta()
		{
			SyntTree termino = null;
			bool bLoop = true;

			termino = PrvMultiplicacionDivision();

			while(bLoop)
			{
				switch(this.lexema.Type)
				{
					case Lexema.LexTypeEnum.LEX_OP_MAS:
					case Lexema.LexTypeEnum.LEX_OP_MENOS:
					{
						Lexema.LexTypeEnum LexOpType = this.lexema.Type;

						if (termino == null)
							throw CrearParserError("Error de sintaxis");

						if (termino.ReturnType != simboloInt && termino.ReturnType != simboloFloat && termino.ReturnType != simboloString)
							throw CrearParserError("El operador izquierdo devuelve un tipo de dato no utilizable");

						PrvGetLexema();

						SyntTree termino2 = PrvMultiplicacionDivision();

						if (termino2 == null)
							throw CrearParserError("Error de sintaxis");

						termino2 = PrvConvertirReturnType(termino2, termino.ReturnType);

						if (termino2.ReturnType != termino.ReturnType)
							throw CrearParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree suma = new SyntTree();

						if (LexOpType == Lexema.LexTypeEnum.LEX_OP_MAS)
							suma.Type = SyntTree.SyntTypeEnum.SYNT_OP_MAS;
						else
							suma.Type = SyntTree.SyntTypeEnum.SYNT_OP_MENOS;

						suma.ReturnType = termino.ReturnType;
						suma.AddChild(termino);
						suma.AddChild(termino2);
						termino = suma;
						break;
					}

					default:
						bLoop = false;
						break;
				}
			}

			return(termino);
		}

		SyntTree PrvOperadorLogico(SyntTree nodo)
		{
			PrvGetLexema();

			if (nodo.ReturnType != simboloInt &&
				nodo.ReturnType != simboloFloat &&
				nodo.ReturnType != simboloString)
				throw CrearParserError("El lado izquierdo de una comparación debe ser un numero o una cadena");

			SyntTree nodo2 = PrvSumaResta();

			if (nodo2 == null)
				throw CrearParserError("Error de sintaxis");

			nodo2 = PrvConvertirReturnType(nodo2, nodo.ReturnType);

			if (nodo2.ReturnType != nodo.ReturnType)
				throw CrearParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

			SyntTree comparacion = new SyntTree();

			//comparacion.SetType(SyntTree.SYNT_OP_COMPARACION_IGUAL);
			//El tipo se seteado por la función que me llama
			comparacion.ReturnType = simboloInt;
			comparacion.AddChild(nodo);
			comparacion.AddChild(nodo2);
			nodo = comparacion;

			return(nodo);
		}

		SyntTree PrvMultiplicacionDivision()
		{
			SyntTree factor = null;

			bool bLoop = true;

			factor = PrvValor();

			while(bLoop)
			{
				switch(this.lexema.Type)
				{
					case Lexema.LexTypeEnum.LEX_OP_POR:
					case Lexema.LexTypeEnum.LEX_OP_DIV:
					{
						Lexema.LexTypeEnum LexOpType = this.lexema.Type;

						if (factor == null)
							throw CrearParserError("Error de sintaxis");

						if (factor.ReturnType != simboloInt && factor.ReturnType != simboloFloat)
							throw CrearParserError("El operador izquierdo devuelve un tipo de dato no utilizable");

						PrvGetLexema();

						SyntTree factor2 = PrvValor();

						if (factor2 == null)
							throw CrearParserError("Error de sintaxis");

						factor2 = PrvConvertirReturnType(factor2, factor.ReturnType);

						if (factor2.ReturnType != factor.ReturnType)
							throw CrearParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree mult = new SyntTree();

						if (LexOpType == Lexema.LexTypeEnum.LEX_OP_POR)
							mult.Type = SyntTree.SyntTypeEnum.SYNT_OP_POR;
						else
							mult.Type = SyntTree.SyntTypeEnum.SYNT_OP_DIV;
				
						mult.ReturnType = factor.ReturnType;
						mult.AddChild(factor);
						mult.AddChild(factor2);
						factor = mult;
						break;
					}

					default:
						bLoop = false;
						break;
				}
			}

			return(factor);
		}

		SyntTree PrvValor()
		{
			SyntTree nodo = null;

			bool bLoop = true;

			nodo = PrvValor2();

			while(bLoop)
			{
				switch(this.lexema.Type)
				{
					default:
						bLoop = false;
						break;
				}
			}

			return(nodo);
		}

		SyntTree PrvValor2()
		{
			SyntTree node = null;

			switch(this.lexema.Type)
			{
				case Lexema.LexTypeEnum.LEX_PAR_ABRE:
					PrvGetLexema();
					node = PrvExpresion();
					if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
						throw CrearParserError("Se esperaba )");
					PrvGetLexema();
					break;
	
				case Lexema.LexTypeEnum.LEX_IDENTIFICADOR:
				{
					Simbolo simbolo = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

					if (simbolo == null)
						throw CrearParserError("El simbolo especificado no se encontro");

					switch(simbolo.Type)
					{
						case Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE:
							node = PrvVariable();
							break;

						case Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION:
							node = PrvLlamadoFuncion();
							break;
					}
			
					break;
				}

				case Lexema.LexTypeEnum.LEX_FLOAT:
				case Lexema.LexTypeEnum.LEX_INTEGER:
				case Lexema.LexTypeEnum.LEX_STRING:
					node = PrvConstante();
					break;

				default:
					break;
			}

			return node;
		}

		SyntTree PrvConstante()
		{
			SyntTree node = null;

			switch(this.lexema.Type)
			{
				case Lexema.LexTypeEnum.LEX_FLOAT:
				case Lexema.LexTypeEnum.LEX_INTEGER:
				case Lexema.LexTypeEnum.LEX_STRING:
					node = new SyntTree();
					node.Integer = this.lexema.Integer;
					node.Type = SyntTree.SyntTypeEnum.SYNT_CONSTANTE;

					if (this.lexema.Type == Lexema.LexTypeEnum.LEX_INTEGER)
					{
						node.ReturnType = simboloInt;
						node.Integer = this.lexema.Integer;
					}
					else
					{
						if (this.lexema.Type == Lexema.LexTypeEnum.LEX_FLOAT)
						{
							node.ReturnType = simboloFloat;
							node.Float = this.lexema.Float;
						}
						else
						{
							node.ReturnType = simboloString;
							node.String = this.lexema.String;
						}
					}

					PrvGetLexema();
					break;
			}

			return node;
		}

		SyntTree PrvVariable()
		{
			SyntTree node = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
			{
				Simbolo simbolo = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

				if (simbolo == null)
					throw CrearParserError("El simbolo especificado no se encontro");

				node = PrvVariable2(simbolo);
			}

			return node;
		}

		SyntTree PrvVariable2(Simbolo simbolo)
		{
			SyntTree node = null;

			if (simbolo.Type == Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE)
			{
				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_VARIABLE;
				node.String = this.lexema.String;
				node.ReturnType = simbolo.ReturnType;
				PrvGetLexema();
			}

			return node;
		}

		SyntTree PrvLlamadoFuncion()
		{
			SyntTree node = null;

			if (this.lexema.Type == Lexema.LexTypeEnum.LEX_IDENTIFICADOR)
			{
				Simbolo simbolo = this.tablaSimbolos.FindSimboloGlobal(this.lexema.String);

				if (simbolo == null)
					throw CrearParserError("El simbolo especificado no se encontro");

				if (simbolo.Type == Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION)
				{
					node = PrvLlamadoFuncion2(simbolo.Nombre, this.tablaSimbolos);
				}
			}

			return node;
		}

		SyntTree PrvLlamadoFuncion2(string nombreFuncion, TablaSimbolos tablaSimbolos)
		{
			SyntTree node = null;
	
			Simbolo simbolo = tablaSimbolos.FindSimboloGlobal(nombreFuncion);
	
			if (simbolo != null && simbolo.Type == Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION)
			{
				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_LLAMADO_FUNCION;
		
				PrvGetLexema();

				if (this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_ABRE)
					throw CrearParserError("Se esperaba un (");

				node.Type = SyntTree.SyntTypeEnum.SYNT_LLAMADO_FUNCION;
				PrvGetLexema();
		
				int CantParametros = 0;
		
				while(this.lexema.Type != Lexema.LexTypeEnum.LEX_PAR_CIERRA)
				{
					if (CantParametros > 0)
					{
						if (this.lexema.Type != Lexema.LexTypeEnum.LEX_COMA)
							throw CrearParserError("Se esperaba ,");
						PrvGetLexema();
					}

					SyntTree pParameter;
					pParameter = PrvExpresion();

					if (pParameter == null)
						throw CrearParserError("Se esperaba un parámetro");

					if (pParameter.ReturnType == null || pParameter.ReturnType == simboloVoid)
						throw CrearParserError("El parametro es de un tipo de dato no compatible");
			
					node.AddChild(pParameter);

					CantParametros++;				
				}

				//Genero el nombre de la función en base a los tipos de datos devueltos por
				//los parametros, pero omito el tipo de dato que devuelve la función
				nombreFuncion = "";
				nombreFuncion += simbolo.Nombre;
				nombreFuncion += "(";

				string separador = new String(Simbolo.SIMBOLO_SEPARADOR_PARAMETRO, 1);

				for (int i = 0; i < CantParametros; i++)
				{
					if (i != 0)
						nombreFuncion += separador;

					nombreFuncion += node.GetChild(i).ReturnType.Nombre;
				};

				nombreFuncion += ")";

				//Busco alguna función cuyos parametros coincidan con los que tengo

				Simbolo simboloFuncion = tablaSimbolos.FindSimboloGlobal(simbolo.Nombre);

				simbolo = null;

				while(simboloFuncion != null)
				{
					string[] nombre = simboloFuncion.NombreFuncionCompleto.Split( new char[] { Simbolo.SIMBOLO_SEPARADOR_FUNCION }, 2);

					if (nombre[1] == nombreFuncion)
					{
						simbolo = simboloFuncion;
						break;
					}
											
					simboloFuncion = tablaSimbolos.FindProximoSimboloGlobal(simboloFuncion);
				}

				if (simbolo == null)
					throw CrearParserError(String.Format("No se encontro ninguna función de la forma '{0}'", nombreFuncion));

				node.SimboloDefinicionFuncion = simbolo;
				node.String = simbolo.NombreFuncionCompleto;
				node.ReturnType = simbolo.ReturnType;

				PrvGetLexema();

				if (CantParametros != simbolo.GetParametros())
					throw CrearParserError("Faltan parametros");
			}

			return node;
		}

		#endregion

		#endregion
	}
}
