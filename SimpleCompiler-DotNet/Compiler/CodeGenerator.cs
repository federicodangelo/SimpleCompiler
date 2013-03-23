using System;
using System.Collections.Generic;
using SimpleCompiler.Shared;

namespace SimpleCompiler.Compiler
{
	public class CodeGeneratorError : Exception
	{
		public CodeGeneratorError(string description) : base(description)
		{
		}
	}

	public class CodeGenerator
	{
		private VariablesTable variablesTable;
        private FunctionsDefinitions functionsDefinitions;
        private InstructionsList instructionList;
        private Function function;

		public CodeGenerator()
		{
		}

		public bool GenerateCode(SyntTree tree, ref List<CodeGeneratorError> errors)
		{
			bool ok;

			functionsDefinitions = new FunctionsDefinitions();
			variablesTable = new VariablesTable();
			instructionList = new InstructionsList(8192);

			try
			{
				PrvAgregarInstruccion(tree);

				ok = true;
			}
			catch(CodeGeneratorError error)
			{
				errors.Add(error);

				functionsDefinitions.Limpiar();

				ok = false;
			}

			return ok;
		}

		public FunctionsDefinitions GetFunctionsDefinitions() 
		{ 
			return functionsDefinitions;
		}

		private int PrvAddConstant(int valor)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolInt(valor);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT;
				simbolo.Integer = valor;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAddConstant(float valor)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolFloat(valor);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_FLOAT;
				simbolo.Float = valor;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAddConstant(string valor)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolString(valor);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING;
				simbolo.String = valor;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		int PrvAgregarSimbolo(Symbol simbolo)
		{
			int n = -1;

			return(n);
		}

		void PrvAgregarInstruccion(SyntTree nodo)
		{
			int i;

			switch(nodo.Type)
			{
				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTION_NULL:
					instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_NULL);
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTION_DECLARATION:
				{
					Symbol definicionFuncion = nodo.FunctionDefinitionSymbol;

					if (!definicionFuncion.NativeFunction)
					{
						function = functionsDefinitions.FindFunction(definicionFuncion.FullFunctionName);

						SymbolTable tablaSimbolos = nodo.GetChild(0).SymbolsTable;

						int nCantVars = 0;

						for (i = 0; i < tablaSimbolos.GetSymbols(); i++)
						{
							Symbol simbolo = tablaSimbolos.GetSymbol(i);
                            
							variablesTable.AddLocalVariable(simbolo.Name);

							nCantVars++;
						}
				
						instructionList = function.InstructionsList;

						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));

						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_RETURN);
				
						while(nCantVars-- != 0)
							variablesTable.RemoveLastVariable();

						instructionList = null;
						function = null;
					}
					else
					{
						Function funcion = functionsDefinitions.FindFunction(definicionFuncion.FullFunctionName);

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

								cad.ConcatenarCadena("No se encontro la funci�n externa ");
								cad.ConcatenarCadena(definicionFuncion.NombreFuncionCompleto);

								ThrowGeneradorCodigoError(cad);
							}
						}
						else
						{*/
						throw new CodeGeneratorError(string.Format("No se encontro la funci�n externa {0}", definicionFuncion.FullFunctionName));
						//}
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_RETURN:
					PrvAgregarInstruccion(nodo.GetChild(0));
					instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_RETURN);
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTIONS_LIST:
				{
					SymbolTable tablaSimbolos = nodo.GetChild(0).SymbolsTable;

					int nCantVars = 0;

					for (i = 0; i < tablaSimbolos.GetSymbols(); i++)
					{
						Symbol simbolo = tablaSimbolos.GetSymbol(i);

						switch(simbolo.Type)
						{
							case Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE:
							{
								VariableDefinition variable = new VariableDefinition();

								variable.Type = PrvTraducirSimboloATipoDato(simbolo.ReturnType);
								variable.Name = simbolo.Name;

								functionsDefinitions.AddVariable(variable);

								variablesTable.AddGlobalVariable(simbolo.Name);

								nCantVars++;

								break;
							}

							case Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION:
							{
								if (simbolo.NativeFunction == false)
								{
									Function funcion = new Function();

									functionsDefinitions.AddFunction(funcion);

									Symbol definicionFuncion = simbolo;

									funcion.Name = definicionFuncion.FullFunctionName;
									funcion.NameShort = definicionFuncion.Name;

									funcion.ReturnType = PrvTraducirSimboloATipoDato(definicionFuncion.ReturnType);

									funcion.NumberOfVariables = definicionFuncion.NumberOfVariables;

									for (int k = 0; k < definicionFuncion.GetParameters(); k++)
									{
										VariableDefinition parametro = new VariableDefinition();

										parametro.Type = PrvTraducirSimboloATipoDato(definicionFuncion.GetParameter(k).ReturnType);
										parametro.Name = definicionFuncion.GetParameter(k).Name;

										funcion.AddParameter(parametro);
									}
								}
								break;
							}
						}
					}

					for (i = 0; i < nodo.GetChilds(); i++)
					{
						if (nodo.GetChild(i).Type == SyntTree.SyntTypeEnum.SYNT_FUNCTION_DECLARATION)
							PrvAgregarInstruccion(nodo.GetChild(i));
					}

					while(nCantVars-- != 0)
						variablesTable.RemoveLastVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTIONS_LIST:
				{
					int nCantVarsLocales = variablesTable.GetNumberOfLocalVariable();

					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					while (variablesTable.GetNumberOfLocalVariable() != nCantVarsLocales)
						variablesTable.RemoveLastVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTION:
					PrvAgregarInstruccion(nodo.GetChild(0));
					if (nodo.ReturnType != null)
						if (nodo.ReturnType.Name != DataType.NAME_VOID)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);
					break;

				case SyntTree.SyntTypeEnum.SYNT_VARIABLE_DECLARATION:
					if (instructionList != null)
					{
						for (i = 0; i < nodo.GetChilds(); i++)
						{
							variablesTable.AddLocalVariable(nodo.GetChild(i).String);

							//Me fijo si tiene algun inicializador
							if (nodo.GetChild(i).GetChilds() != 0)
							{
								//Tiene un inicializador
								PrvAgregarInstruccion(nodo.GetChild(i).GetChild(0));
							}
							else
							{
								//No tiene inicializador
								//Genero el c�digo que inicializa la variable a los tipos de datos b�sicos
								if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_FLOAT)
									instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT, (short) PrvAddConstant((float) 0.0f));

								else if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_INT)
									instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT, (short) PrvAddConstant((long) 0));

								else if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_STRING)
									instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING, (short) PrvAddConstant(""));
							}
					
							//Genero el c�digo que va a sacar del stack el valor con el cual se inicializa la variable
							//y se lo va a asignar a la variable
							if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_FLOAT)
								instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT, (short) (variablesTable.GetNumberOfLocalVariable() - 1));
							
							else if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_INT)
								instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT, (short) (variablesTable.GetNumberOfLocalVariable() - 1));
					
							else if (nodo.GetChild(i).ReturnType.Name == DataType.NAME_STRING)
								instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING, (short) (variablesTable.GetNumberOfLocalVariable() - 1));
					
							else
								instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_OBJ, (short) (variablesTable.GetNumberOfLocalVariable() - 1));
						}
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTION_CALL:
				{
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					int posFuncion = functionsDefinitions.FunctionFunctionPosition(nodo.String);

					if (posFuncion != -1)
					{
                        instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_CALL_FUNCTION, (short) posFuncion);
					}
					else
					{
                        posFuncion = PrvAddConstant(nodo.String);

						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_CALL_GLOBAL_FUNCTION, (short) posFuncion);
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_SYMBOLS_TABLE:
					break;

				case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
				{
					int nPos = variablesTable.FindVariable(nodo.String);

					if (variablesTable.GetVariableLocation(nPos) == VariablesTable.VariableLocationEnum.LOCATION_LOCAL)
					{
						nPos = variablesTable.GetVariablePosition(nPos);

						if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_FLOAT, (short) nPos);
						else if (nodo.ReturnType.Name == DataType.NAME_INT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_INT, (short) nPos);
						else if (nodo.ReturnType.Name == DataType.NAME_STRING)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_STRING, (short) nPos);
					}
					else
					{
						nPos = PrvAddConstant(nodo.String);

						if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT, (short) nPos);
						else if (nodo.ReturnType.Name == DataType.NAME_INT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_INT, (short) nPos);
						else if (nodo.ReturnType.Name == DataType.NAME_STRING)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING, (short) nPos);
					}

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_IDENTIFIER:
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONSTANT:
					if (nodo.ReturnType.Name == DataType.NAME_STRING)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING, (short) PrvAddConstant((string) nodo.String));
					else if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT, (short) PrvAddConstant((float) nodo.Float));
					else if (nodo.ReturnType.Name == DataType.NAME_INT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT, (short) PrvAddConstant((int) nodo.Integer));
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONVERSION:
					PrvAgregarInstruccion(nodo.GetChild(0));

					if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_INT_A_FLOAT);
					else if (nodo.ReturnType.Name == DataType.NAME_INT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_A_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_ASSIGNATION:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a asignar
					PrvAgregarInstruccion(nodo.GetChild(1));
					instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_DUP);
				
					//Asgino el valor del stack a la variable segun corresponda..
					switch(nodo.GetChild(0).Type)
					{
						case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
						{
							int nPos = variablesTable.FindVariable(nodo.GetChild(0).String);

							switch(variablesTable.GetVariableLocation(nPos))
							{
								case VariablesTable.VariableLocationEnum.LOCATION_LOCAL:
									nPos = variablesTable.GetVariablePosition(nPos);

									switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Type)
									{
										case DataType.DataTypeEnum.TYPE_INT:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT, (short) nPos);
											break;
										case DataType.DataTypeEnum.TYPE_FLOAT:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT, (short) nPos);
											break;
										case DataType.DataTypeEnum.TYPE_STRING:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING, (short) nPos);
											break;
									}
										
									break;

								case VariablesTable.VariableLocationEnum.LOCATION_GLOBAL:
									nPos = PrvAddConstant(nodo.GetChild(0).String);
										
									switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Type)
									{
										case DataType.DataTypeEnum.TYPE_INT:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_INT, (short) nPos);
											break;
										case DataType.DataTypeEnum.TYPE_FLOAT:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT, (short) nPos);
											break;
										case DataType.DataTypeEnum.TYPE_STRING:
											instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_STRING, (short) nPos);
											break;
									}

									break;
							}
							break;
						}
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_COMPARISON_EQUAL:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(0));

					//Agrego la instrucci�n de comparaci�n que corresponda
				switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Type)
				{
					case DataType.DataTypeEnum.TYPE_INT:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
						break;
					case DataType.DataTypeEnum.TYPE_FLOAT:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_A_INT);
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
						break;
					case DataType.DataTypeEnum.TYPE_STRING:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_COMPARE_STRING);
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
						break;
				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_COMPARISON_NOT_EQUAL:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAgregarInstruccion(nodo.GetChild(0));

					//Agrego la instrucci�n de comparaci�n que corresponda
				switch(PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Type)
				{
					case DataType.DataTypeEnum.TYPE_INT:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
						break;
					case DataType.DataTypeEnum.TYPE_FLOAT:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_A_INT);
						break;
					case DataType.DataTypeEnum.TYPE_STRING:
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_COMPARE_STRING);
						break;
				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_ADDITION:
					if (nodo.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANT && 
						nodo.GetChild(1).ReturnType.Name == DataType.NAME_INT &&
						nodo.GetChild(1).Integer == 1)
					{
						//Uso el operador de incremento de int en 1
						PrvAgregarInstruccion(nodo.GetChild(0));
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_INC_INT);
					}
					else
					{
						//Uso la suma normal
						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));
					
						if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_ADD_FLOAT);
						else if (nodo.ReturnType.Name == DataType.NAME_INT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_ADD_INT);
						else if (nodo.ReturnType.Name == DataType.NAME_STRING)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_CONCAT_STRING);
					}

					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_SUBTRACT:
					if (nodo.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANT && 
						nodo.GetChild(1).ReturnType.Name == DataType.NAME_INT &&
						nodo.GetChild(1).Integer == 1)
					{
						//Uso el operador de decremento de int en 1
						PrvAgregarInstruccion(nodo.GetChild(0));
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_DEC_INT);
					}
					else
					{
						//Uso la resta normal
						for (i = 0; i < nodo.GetChilds(); i++)
							PrvAgregarInstruccion(nodo.GetChild(i));

						if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
						else
							if (nodo.ReturnType.Name == DataType.NAME_INT)
							instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_MULTIPLICATION:
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_MULT_FLOAT);
					else
						if (nodo.ReturnType.Name == DataType.NAME_INT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_MULT_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_DIVISION:
					for (i = 0; i < nodo.GetChilds(); i++)
						PrvAgregarInstruccion(nodo.GetChild(i));

					if (nodo.ReturnType.Name == DataType.NAME_FLOAT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_DIV_FLOAT);
					else
						if (nodo.ReturnType.Name == DataType.NAME_INT)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_DIV_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_IF:
				{
					//Agrego las instrucciones que ejecutan la condicion
					PrvAgregarInstruccion(nodo.GetChild(0));
					//Agrego el salto segun la condicion
					int nInstruccionIf = instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					int nInstruccionElse = 0;
				
					if (nodo.GetChilds() == 3)
						nInstruccionElse = instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, 0);

					int nInstruccion = instructionList.GetInstructions();

					//Actualizo la instruccion de salto cuando la condici�n no se cumple
					instructionList.SetParameter(nInstruccionIf, (short) (nInstruccion - nInstruccionIf - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
				
					if (nodo.GetChilds() == 3)
					{
						//Agrego las instrucciones que se ejecutan cuando la condicion no se cumple
						PrvAgregarInstruccion(nodo.GetChild(2));

						nInstruccion = instructionList.GetInstructions();

						instructionList.SetParameter(nInstruccionElse, (short) (nInstruccion - nInstruccionElse - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
					}
										
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_WHILE:
				{
					int nInstCondicion = instructionList.GetInstructions();

					//Agrego las instrucciones que ejecutan la condicion
					PrvAgregarInstruccion(nodo.GetChild(0));
					//Agrego el salto segun la condicion
					int nInstruccionSaltoNoCumple = instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, (short) 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = instructionList.GetInstructions();
					int Salto = nInstCondicion - nInstFinCiclo - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, (short) Salto);

					int nInstruccion = instructionList.GetInstructions();

					//Actualizo la instruccion de salto cuando la condici�n no se cumple
					instructionList.SetParameter(nInstruccionSaltoNoCumple, (short) (nInstruccion - nInstruccionSaltoNoCumple - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_FOR:
				{
					//Agrego las instrucciones que inicializan la condici�n el ciclo
					PrvAgregarInstruccion(nodo.GetChild(0));
					if (PrvTraducirSimboloATipoDato(nodo.GetChild(0).ReturnType).Type != DataType.DataTypeEnum.TYPE_VOID)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);

					//Agrego las instrucciones que evaluan la condici�n del ciclo
					int nInstCondicion = instructionList.GetInstructions();
					PrvAgregarInstruccion(nodo.GetChild(1));
				
					//Agrego el salto segun el resultado de la condicion
					int nInstCondicionSalto = instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, 0);

					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAgregarInstruccion(nodo.GetChild(3));

					//Agrego las instrucciones que incrementar en contador del ciclo
					PrvAgregarInstruccion(nodo.GetChild(2));
					if (PrvTraducirSimboloATipoDato(nodo.GetChild(2).ReturnType).Type != DataType.DataTypeEnum.TYPE_VOID)
						instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);

					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = instructionList.GetInstructions();
					int Salto = nInstCondicion - nInstFinCiclo - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, (short) Salto);

					//Actualizo la instruccion de salto a fin de ciclo donde se evalua la condicion
					nInstFinCiclo = instructionList.GetInstructions();
					Salto = nInstFinCiclo - nInstCondicionSalto - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionList.SetParameter(nInstCondicionSalto, (short) Salto);
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_NONE:
					break;
			}
		}

		DataType PrvTraducirSimboloATipoDato(Symbol simbolo)
		{
			DataType Tipo = new DataType();

			if (simbolo != null)
			{
				string nombre = simbolo.Name;

				if (nombre == DataType.NAME_INT)
					Tipo.Type = DataType.DataTypeEnum.TYPE_INT;
				else if (nombre == DataType.NAME_FLOAT)
					Tipo.Type = DataType.DataTypeEnum.TYPE_FLOAT;
				else if (nombre == DataType.NAME_VOID)
					Tipo.Type = DataType.DataTypeEnum.TYPE_VOID;
				else if (nombre == DataType.NAME_STRING)
					Tipo.Type = DataType.DataTypeEnum.TYPE_STRING;
			}
			else
			{
				Tipo.Type = DataType.DataTypeEnum.TYPE_VOID;
			}

			return(Tipo);
		}
	}
}
