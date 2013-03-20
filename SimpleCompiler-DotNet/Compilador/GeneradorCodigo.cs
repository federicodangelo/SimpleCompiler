using System;
using System.Collections;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.Compilador
{
	public class GeneradorCodigoError : Exception
	{
		public GeneradorCodigoError(string description) : base(description)
		{
		}
	}

	public class GeneradorCodigo
	{
		TablaVariables tablaVariables;
		DefinicionFunciones definicionFunciones;
		ListaInstrucciones listaInstrucciones;
		Funcion funcion;

		public GeneradorCodigo()
		{
		}

		public bool GenerarCodigo(SyntTree tree, out ArrayList errores)
		{
			bool ok;

			definicionFunciones = new DefinicionFunciones();
			tablaVariables = new TablaVariables();
			listaInstrucciones = new ListaInstrucciones(8192);

			errores = new ArrayList();

			try
			{
				PrvAgregarInstruccion(tree);

				ok = true;
			}
			catch(GeneradorCodigoError error)
			{
				errores.Add(error);

				definicionFunciones.Limpiar();

				ok = false;
			}

			return ok;
		}

		public DefinicionFunciones GetDefinicionFunciones() 
		{ 
			return definicionFunciones;
		}

		int PrvAgregarConstante(int valor)
		{
			int n;

			n = definicionFunciones.TablaConstantes.FindSimboloInt(valor);

			if (n == TablaSimbolosSimple.NO_ENCONTRADO)
			{
				SimboloSimple simbolo = new SimboloSimple();
				simbolo.Type = SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_INT;
				simbolo.Integer = valor;
				n = definicionFunciones.TablaConstantes.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAgregarConstante(float valor)
		{
			int n;

			n = definicionFunciones.TablaConstantes.FindSimboloFloat(valor);

			if (n == TablaSimbolosSimple.NO_ENCONTRADO)
			{
				SimboloSimple simbolo = new SimboloSimple();
				simbolo.Type = SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_FLOAT;
				simbolo.Float = valor;
				n = definicionFunciones.TablaConstantes.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAgregarConstante(string valor)
		{
			int n;

			n = definicionFunciones.TablaConstantes.FindSimboloString(valor);

			if (n == TablaSimbolosSimple.NO_ENCONTRADO)
			{
				SimboloSimple simbolo = new SimboloSimple();
				simbolo.Type = SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_STRING;
				simbolo.String = valor;
				n = definicionFunciones.TablaConstantes.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAgregarSimbolo(Simbolo simbolo)
		{
			int n = -1;

			return(n);
		}

		void PrvAgregarInstruccion(SyntTree nodo)
		{
			int i;

			switch(nodo.Type)
			{
				case SyntTree.SyntTypeEnum.SYNT_INSTRUCCION_NULA:
					listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_NADA);
					break;

				case SyntTree.SyntTypeEnum.SYNT_DECLARACION_FUNCION:
				{
					Simbolo definicionFuncion = nodo.SimboloDefinicionFuncion;

					if (!definicionFuncion.FuncionNativa)
					{
						funcion = definicionFunciones.BuscarFuncion(definicionFuncion.NombreFuncionCompleto);

						TablaSimbolos tablaSimbolos = nodo.GetChild(0).TablaSimbolos;

						int nCantVars = 0;

						for (i = 0; i < tablaSimbolos.GetSimbolos(); i++)
						{
							Simbolo simbolo = tablaSimbolos.GetSimbolo(i);
                            
							tablaVariables.AgregarVariableLocal(simbolo.Nombre);

							nCantVars++;
						}
				
						listaInstrucciones = funcion.ListaInstrucciones;

						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));

						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RETURN);
				
						while(nCantVars-- != 0)
							tablaVariables.EliminarVariable();

						listaInstrucciones = null;
						funcion = null;
					}
					else
					{
						Funcion funcion = definicionFunciones.BuscarFuncion(definicionFuncion.NombreFuncionCompleto);

						/*if (m_pBuscarFuncionNativa != null)
						{
							FuncionNativa* pFuncionNativa = m_pBuscarFuncionNativa(definicionFuncion.NombreFuncionCompleto);

							if (pFuncionNativa != null)
							{
								funcion.SetearTipoFuncion(Funcion.FUNCION_NATIVA);

								funcion.SetearFuncionNativa(pFuncionNativa);
							}
							else
							{
								CCadena cad;

								cad.ConcatenarCadena("No se encontro la función externa ");
								cad.ConcatenarCadena(definicionFuncion.NombreFuncionCompleto);

								ThrowGeneradorCodigoError(cad);
							}
						}
						else
						{*/
						throw new GeneradorCodigoError(string.Format("No se encontro la función externa {0}", definicionFuncion.NombreFuncionCompleto));
						//}
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_RETURN:
					PrvAgregarInstruccion(nodo.GetChild(0));
					listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RETURN);
					break;

				case SyntTree.SyntTypeEnum.SYNT_LISTA_FUNCIONES:
				{
					TablaSimbolos tablaSimbolos = nodo.GetChild(0).TablaSimbolos;

					int nCantVars = 0;

					for (i = 0; i < tablaSimbolos.GetSimbolos(); i++)
					{
						Simbolo simbolo = tablaSimbolos.GetSimbolo(i);

						switch(simbolo.Type)
						{
							case Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE:
							{
								DefinicionVariable variable = new DefinicionVariable();

								variable.Tipo = PrvTraducirSimboloATipoDato(simbolo.ReturnType);
								variable.Nombre = simbolo.Nombre;

								definicionFunciones.AgregarVariable(variable);

								tablaVariables.AgregarVariableGlobal(simbolo.Nombre);

								nCantVars++;

								break;
							}

							case Simbolo.SimboloTypeEnum.SIMBOLO_TIPO_FUNCION:
							{
								if (simbolo.FuncionNativa == false)
								{
									Funcion funcion = new Funcion();

									definicionFunciones.AgregarFuncion(funcion);

									Simbolo definicionFuncion = simbolo;

									funcion.Nombre = definicionFuncion.NombreFuncionCompleto;
									funcion.NombreCorto = definicionFuncion.Nombre;

									funcion.TipoDevuelto = PrvTraducirSimboloATipoDato(definicionFuncion.ReturnType);

									funcion.CantVariables = definicionFuncion.CantVariables;

									for (int k = 0; k < definicionFuncion.GetParametros(); k++)
									{
										DefinicionVariable parametro = new DefinicionVariable();

										parametro.Tipo = PrvTraducirSimboloATipoDato(definicionFuncion.GetParametro(k).ReturnType);
										parametro.Nombre = definicionFuncion.GetParametro(k).Nombre;

										funcion.AddParametro(parametro);
									}
								}
								break;
							}
						}
					}

					for (i = 0; i < nodo.GetChilds(); i++)
					{
						if (nodo.GetChild(i).Type == SyntTree.SyntTypeEnum.SYNT_DECLARACION_FUNCION)
							PrvAgregarInstruccion(nodo.GetChild(i));
					}

					while(nCantVars-- != 0)
						tablaVariables.EliminarVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_LISTA_INSTRUCCIONES:
				{
					int nCantVarsLocales = tablaVariables.CantidadVariablesLocales();

					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					while (tablaVariables.CantidadVariablesLocales() != nCantVarsLocales)
						tablaVariables.EliminarVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_INSTRUCCION:
					PrvAgregarInstruccion(nodo.GetChild(0));
					if (nodo.ReturnType != null)
						if (nodo.ReturnType.Nombre != TipoDato.NOMBRE_VOID)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP);
					break;

				case SyntTree.SyntTypeEnum.SYNT_DECLARACION_VARIABLE:
					if (listaInstrucciones != null)
					{
						for (i = 0; i < nodo.GetChilds(); i++)
						{
							tablaVariables.AgregarVariableLocal(nodo.GetChild(i).String);

							//Me fijo si tiene algun inicializador
							if (nodo.GetChild(i).GetChilds() != 0)
							{
								//Tiene un inicializador
								PrvAgregarInstruccion(nodo.GetChild(i).GetChild(0));
							}
							else
							{
								//No tiene inicializador
								//Genero el código que inicializa la variable a los tipos de datos básicos
								if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
									listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_FLOAT, (short) PrvAgregarConstante((float) 0.0f));

								else if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_INT)
									listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_INT, (short) PrvAgregarConstante((long) 0));

								else if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_STRING)
									listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_STRING, (short) PrvAgregarConstante(""));
							}
					
							//Genero el código que va a sacar del stack el valor con el cual se inicializa la variable
							//y se lo va a asignar a la variable
							if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
								listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_FLOAT, (short) (tablaVariables.CantidadVariablesLocales() - 1));
							
							else if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_INT)
								listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_INT, (short) (tablaVariables.CantidadVariablesLocales() - 1));
					
							else if (nodo.GetChild(i).ReturnType.Nombre == TipoDato.NOMBRE_STRING)
								listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_STRING, (short) (tablaVariables.CantidadVariablesLocales() - 1));
					
							else
								listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_OBJ, (short) (tablaVariables.CantidadVariablesLocales() - 1));
						}
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_LLAMADO_FUNCION:
				{
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					int posFuncion = definicionFunciones.BuscarPosicionFuncion(nodo.String);

					if (posFuncion != -1)
					{
                        listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_CALL_FUNCTION, (short) posFuncion);
					}
					else
					{
                        posFuncion = PrvAgregarConstante(nodo.String);

						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_CALL_GLOBAL_FUNCTION, (short) posFuncion);
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_TABLA_SIMBOLOS:
					break;

				case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
				{
					int nPos = tablaVariables.BuscarVariable(nodo.String);

					if (tablaVariables.UbicacionVariable(nPos) == TablaVariables.UbicacionVariableEnum.UBICACION_LOCAL)
					{
						nPos = tablaVariables.PosicionVariable(nPos);

						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_FLOAT, (short) nPos);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_INT, (short) nPos);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_STRING)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_STRING, (short) nPos);
					}
					else
					{
						nPos = PrvAgregarConstante(nodo.String);

						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT, (short) nPos);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_INT, (short) nPos);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_STRING)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING, (short) nPos);
					}

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_IDENTIFICADOR:
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONSTANTE:
					if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_STRING)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_STRING, (short) PrvAgregarConstante((string) nodo.String));
					else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_FLOAT, (short) PrvAgregarConstante((float) nodo.Float));
					else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_INT, (short) PrvAgregarConstante((int) nodo.Integer));
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONVERSION:
					PrvAgregarInstruccion(nodo.GetChild(0));

					if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_INT_A_FLOAT);
					else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_FLOAT_A_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_ASIGNACION:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a asignar
					PrvAgregarInstruccion(nodo.GetChild(1));
					listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_DUP);
				
					//Asgino el valor del stack a la variable segun corresponda..
					switch(nodo.GetChild(0).Type)
					{
						case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
						{
							int nPos = tablaVariables.BuscarVariable(nodo.GetChild(0).String);

							switch(tablaVariables.UbicacionVariable(nPos))
							{
								case TablaVariables.UbicacionVariableEnum.UBICACION_LOCAL:
									nPos = tablaVariables.PosicionVariable(nPos);

									switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Tipo)
									{
										case TipoDato.TipoDatoEnum.TIPO_INT:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_INT, (short) nPos);
											break;
										case TipoDato.TipoDatoEnum.TIPO_FLOAT:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_FLOAT, (short) nPos);
											break;
										case TipoDato.TipoDatoEnum.TIPO_STRING:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_STRING, (short) nPos);
											break;
									}
										
									break;

								case TablaVariables.UbicacionVariableEnum.UBICACION_GLOBAL:
									nPos = PrvAgregarConstante(nodo.GetChild(0).String);
										
									switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Tipo)
									{
										case TipoDato.TipoDatoEnum.TIPO_INT:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_INT, (short) nPos);
											break;
										case TipoDato.TipoDatoEnum.TIPO_FLOAT:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT, (short) nPos);
											break;
										case TipoDato.TipoDatoEnum.TIPO_STRING:
											listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_STRING, (short) nPos);
											break;
									}

									break;
							}
							break;
						}
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_COMPARACION_IGUAL:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(0));

					//Agrego la instrucción de comparación que corresponda
				switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Tipo)
				{
					case TipoDato.TipoDatoEnum.TIPO_INT:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_INT);
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_ISZERO_INT);
						break;
					case TipoDato.TipoDatoEnum.TIPO_FLOAT:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_FLOAT);
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_FLOAT_A_INT);
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_ISZERO_INT);
						break;
					case TipoDato.TipoDatoEnum.TIPO_STRING:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_COMPARE_STRING);
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_ISZERO_INT);
						break;
				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_COMPARACION_DISTINTO:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(0));

					//Agrego la instrucción de comparación que corresponda
				switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Tipo)
				{
					case TipoDato.TipoDatoEnum.TIPO_INT:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_INT);
						break;
					case TipoDato.TipoDatoEnum.TIPO_FLOAT:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_FLOAT);
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_FLOAT_A_INT);
						break;
					case TipoDato.TipoDatoEnum.TIPO_STRING:
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_COMPARE_STRING);
						break;
				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_MAS:
					if (nodo.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANTE && 
						nodo.GetChild(1).ReturnType.Nombre == TipoDato.NOMBRE_INT &&
						nodo.GetChild(1).Integer == 1)
					{
						//Uso el operador de incremento de int en 1
						PrvAgregarInstruccion(nodo.GetChild(0));
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_INC_INT);
					}
					else
					{
						//Uso la suma normal
						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));
					
						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_SUMAR_FLOAT);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_SUMAR_INT);
						else if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_STRING)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_CONCAT_STRING);
					}

					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_MENOS:
					if (nodo.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANTE && 
						nodo.GetChild(1).ReturnType.Nombre == TipoDato.NOMBRE_INT &&
						nodo.GetChild(1).Integer == 1)
					{
						//Uso el operador de decremento de int en 1
						PrvAgregarInstruccion(nodo.GetChild(0));
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_DEC_INT);
					}
					else
					{
						//Uso la resta normal
						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));

						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_FLOAT);
						else
							if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
							listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_RESTAR_INT);
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_POR:
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_MULT_FLOAT);
					else
						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_MULT_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_DIV:
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_DIV_FLOAT);
					else
						if (nodo.ReturnType.Nombre == TipoDato.NOMBRE_INT)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_DIV_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_IF:
				{
					//Agrego las instrucciones que ejecutan la condicion
					PrvAgregarInstruccion(nodo.GetChild(0));
					//Agrego el salto segun la condicion
					int nInstruccionIf = listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP_IF_ZERO, 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					int nInstruccionElse = 0;
				
					if (nodo.GetChilds() == 3)
						nInstruccionElse = listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP, 0);

					int nInstruccion = listaInstrucciones.ObtenerInstrucciones();

					//Actualizo la instruccion de salto cuando la condición no se cumple
					listaInstrucciones.SetearParametro(nInstruccionIf, (short) (nInstruccion - nInstruccionIf - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro));
				
					if (nodo.GetChilds() == 3)
					{
						//Agrego las instrucciones que se ejecutan cuando la condicion no se cumple
						PrvAgregarInstruccion(nodo.GetChild(2));

						nInstruccion = listaInstrucciones.ObtenerInstrucciones();

						listaInstrucciones.SetearParametro(nInstruccionElse, (short) (nInstruccion - nInstruccionElse - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro));
					}
										
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_WHILE:
				{
					int nInstCondicion = listaInstrucciones.ObtenerInstrucciones();

					//Agrego las instrucciones que ejecutan la condicion
					PrvAgregarInstruccion(nodo.GetChild(0));
					//Agrego el salto segun la condicion
					int nInstruccionSaltoNoCumple = listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP_IF_ZERO, (short) 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = listaInstrucciones.ObtenerInstrucciones();
					int Salto = nInstCondicion - nInstFinCiclo - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro;
					listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP, (short) Salto);

					int nInstruccion = listaInstrucciones.ObtenerInstrucciones();

					//Actualizo la instruccion de salto cuando la condición no se cumple
					listaInstrucciones.SetearParametro(nInstruccionSaltoNoCumple, (short) (nInstruccion - nInstruccionSaltoNoCumple - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro));
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_FOR:
				{
					//Agrego las instrucciones que inicializan la condición el ciclo
					PrvAgregarInstruccion(nodo.GetChild(0));
					if (PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Tipo != TipoDato.TipoDatoEnum.TIPO_VOID)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP);

					//Agrego las instrucciones que evaluan la condición del ciclo
					int nInstCondicion = listaInstrucciones.ObtenerInstrucciones();
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					//Agrego el salto segun el resultado de la condicion
					int nInstCondicionSalto = listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP_IF_ZERO, 0);

					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(3));

					//Agrego las instrucciones que incrementar en contador del ciclo
					PrvAgregarInstruccion(nodo.GetChild(2));
					if (PrvTraducirSimboloATipoDato(nodo.GetChild(2).ReturnType).Tipo != TipoDato.TipoDatoEnum.TIPO_VOID)
						listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_STACK_POP);

					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = listaInstrucciones.ObtenerInstrucciones();
					int Salto = nInstCondicion - nInstFinCiclo - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro;
					listaInstrucciones.AgregarInstruccion(ListaInstrucciones.InstruccionesEnum.INST_JUMP, (short) Salto);

					//Actualizo la instruccion de salto a fin de ciclo donde se evalua la condicion
					nInstFinCiclo = listaInstrucciones.ObtenerInstrucciones();
					Salto = nInstFinCiclo - nInstCondicionSalto - ListaInstrucciones.lenInstruccion - ListaInstrucciones.lenParametro;
					listaInstrucciones.SetearParametro(nInstCondicionSalto, (short) Salto);
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_NONE:
					break;
			}
		}

		TipoDato PrvTraducirSimboloATipoDato(Simbolo simbolo)
		{
			TipoDato Tipo = new TipoDato();

			if (simbolo != null)
			{
				string nombre = simbolo.Nombre;

				if (nombre == TipoDato.NOMBRE_INT)
					Tipo.Tipo = TipoDato.TipoDatoEnum.TIPO_INT;
				else if (nombre == TipoDato.NOMBRE_FLOAT)
					Tipo.Tipo = TipoDato.TipoDatoEnum.TIPO_FLOAT;
				else if (nombre == TipoDato.NOMBRE_VOID)
					Tipo.Tipo = TipoDato.TipoDatoEnum.TIPO_VOID;
				else if (nombre == TipoDato.NOMBRE_STRING)
					Tipo.Tipo = TipoDato.TipoDatoEnum.TIPO_STRING;
			}
			else
			{
				Tipo.Tipo = TipoDato.TipoDatoEnum.TIPO_VOID;
			}

			return(Tipo);
		}
	}
}
