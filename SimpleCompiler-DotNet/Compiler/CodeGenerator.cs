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
        private InstructionsList instructionsList;
        private Function function;

		public CodeGenerator()
		{
		}

		public bool GenerateCode(SyntTree tree, ref List<CodeGeneratorError> errors)
		{
			bool ok;

			functionsDefinitions = new FunctionsDefinitions();
			variablesTable = new VariablesTable();
			instructionsList = new InstructionsList(8192);

			try
			{
				PrvAddInstruction(tree);

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

		private int PrvAddConstant(int val)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolInt(val);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT;
				simbolo.Integer = val;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		private int PrvAddConstant(float val)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolFloat(val);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_FLOAT;
				simbolo.Float = val;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		private int PrvAddConstant(string val)
		{
			int n;

			n = functionsDefinitions.ConstantsTable.FindSymbolString(val);

			if (n == SymbolConstantTable.NOT_FOUND)
			{
				SymbolConstant simbolo = new SymbolConstant();
				simbolo.Type = SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING;
				simbolo.String = val;
				n = functionsDefinitions.ConstantsTable.AddSimbolo(simbolo);
			}

			return(n);
		}

		private int PrvAddSymbol(Symbol simbolo)
		{
			int n = -1;

			return(n);
		}

		private void PrvAddInstruction(SyntTree node)
		{
			int i;

			switch(node.Type)
			{
				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTION_NULL:
					instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_NULL);
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTION_DECLARATION:
				{
					Symbol functionDefinition = node.FunctionDefinitionSymbol;

					if (!functionDefinition.NativeFunction)
					{
						function = functionsDefinitions.FindFunction(functionDefinition.FullFunctionName);

						SymbolTable symbolsTable = node.GetChild(0).SymbolsTable;

						int numberOfVars = 0;

						for (i = 0; i < symbolsTable.GetSymbols(); i++)
						{
							Symbol simbolo = symbolsTable.GetSymbol(i);
                            
							variablesTable.AddLocalVariable(simbolo.Name);

							numberOfVars++;
						}
				
						instructionsList = function.InstructionsList;

						for (i = 0; i < node.GetChilds(); i++)
							PrvAddInstruction(node.GetChild(i));

						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_RETURN);
				
						while(numberOfVars-- != 0)
							variablesTable.RemoveLastVariable();

						instructionsList = null;
						function = null;
					}
					else
					{
						throw new CodeGeneratorError(string.Format("Invalid external function definition: {0}", functionDefinition.FullFunctionName));
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_RETURN:
					PrvAddInstruction(node.GetChild(0));
					instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_RETURN);
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTIONS_LIST:
				{
					SymbolTable tablaSimbolos = node.GetChild(0).SymbolsTable;

					int numberOfVars = 0;

					for (i = 0; i < tablaSimbolos.GetSymbols(); i++)
					{
						Symbol symbol = tablaSimbolos.GetSymbol(i);

						switch(symbol.Type)
						{
							case Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE:
							{
								VariableDefinition variable = new VariableDefinition();

								variable.Type = PrvTranslateSymbolToDataType(symbol.ReturnType);
								variable.Name = symbol.Name;

								functionsDefinitions.AddVariable(variable);

								variablesTable.AddGlobalVariable(symbol.Name);

								numberOfVars++;

								break;
							}

							case Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION:
							{
								if (symbol.NativeFunction == false)
								{
									Function function = new Function();

									functionsDefinitions.AddFunction(function);

									Symbol functionDefinition = symbol;

									function.Name = functionDefinition.FullFunctionName;
									function.NameShort = functionDefinition.Name;

									function.ReturnType = PrvTranslateSymbolToDataType(functionDefinition.ReturnType);

									function.NumberOfVariables = functionDefinition.NumberOfVariables;

									for (int k = 0; k < functionDefinition.GetParameters(); k++)
									{
										VariableDefinition parameter = new VariableDefinition();

										parameter.Type = PrvTranslateSymbolToDataType(functionDefinition.GetParameter(k).ReturnType);
										parameter.Name = functionDefinition.GetParameter(k).Name;

										function.AddParameter(parameter);
									}
								}
								break;
							}
						}
					}

					for (i = 0; i < node.GetChilds(); i++)
					{
						if (node.GetChild(i).Type == SyntTree.SyntTypeEnum.SYNT_FUNCTION_DECLARATION)
							PrvAddInstruction(node.GetChild(i));
					}

					while(numberOfVars-- != 0)
						variablesTable.RemoveLastVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTIONS_LIST:
				{
					int numberOfLocalVars = variablesTable.GetNumberOfLocalVariables();

					for (i = 0; i < node.GetChilds(); i++)
						PrvAddInstruction(node.GetChild(i));

					while (variablesTable.GetNumberOfLocalVariables() != numberOfLocalVars)
						variablesTable.RemoveLastVariable();

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_INSTRUCTION:
					PrvAddInstruction(node.GetChild(0));
					if (node.ReturnType != null)
						if (node.ReturnType.Name != DataType.NAME_VOID)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);
					break;

				case SyntTree.SyntTypeEnum.SYNT_VARIABLE_DECLARATION:
					if (instructionsList != null)
					{
						for (i = 0; i < node.GetChilds(); i++)
						{
							variablesTable.AddLocalVariable(node.GetChild(i).String);

							//Me fijo si tiene algun inicializador
							if (node.GetChild(i).GetChilds() != 0)
							{
								//Tiene un inicializador
								PrvAddInstruction(node.GetChild(i).GetChild(0));
							}
							else
							{
								//No tiene inicializador
								//Genero el c�digo que inicializa la variable a los tipos de datos b�sicos
								if (node.GetChild(i).ReturnType.Name == DataType.NAME_FLOAT)
									instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT, (short) PrvAddConstant((float) 0.0f));

								else if (node.GetChild(i).ReturnType.Name == DataType.NAME_INT)
									instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT, (short) PrvAddConstant((long) 0));

								else if (node.GetChild(i).ReturnType.Name == DataType.NAME_STRING)
									instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING, (short) PrvAddConstant(""));
							}
					
							//Genero el c�digo que va a sacar del stack el valor con el cual se inicializa la variable
							//y se lo va a asignar a la variable
							if (node.GetChild(i).ReturnType.Name == DataType.NAME_FLOAT)
								instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT, (short) (variablesTable.GetNumberOfLocalVariables() - 1));
							
							else if (node.GetChild(i).ReturnType.Name == DataType.NAME_INT)
								instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT, (short) (variablesTable.GetNumberOfLocalVariables() - 1));
					
							else if (node.GetChild(i).ReturnType.Name == DataType.NAME_STRING)
								instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING, (short) (variablesTable.GetNumberOfLocalVariables() - 1));
					
							else
								instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_OBJ, (short) (variablesTable.GetNumberOfLocalVariables() - 1));
						}
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_FUNCTION_CALL:
				{
					for (i = 0; i < node.GetChilds(); i++)
						PrvAddInstruction(node.GetChild(i));

					int functionIndex = functionsDefinitions.FindFunctionIndex(node.String);

					if (functionIndex != -1)
					{
                        instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_CALL_FUNCTION, (short) functionIndex);
					}
					else
					{
                        functionIndex = PrvAddConstant(node.String);

						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_CALL_GLOBAL_FUNCTION, (short) functionIndex);
					}
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_SYMBOLS_TABLE:
					break;

				case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
				{
					int variableIndex = variablesTable.FindVariable(node.String);

					if (variablesTable.GetVariableLocation(variableIndex) == VariablesTable.VariableLocationEnum.LOCATION_LOCAL)
					{
						variableIndex = variablesTable.GetVariablePosition(variableIndex);

						if (node.ReturnType.Name == DataType.NAME_FLOAT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_FLOAT, (short) variableIndex);
						else if (node.ReturnType.Name == DataType.NAME_INT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_INT, (short) variableIndex);
						else if (node.ReturnType.Name == DataType.NAME_STRING)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_STRING, (short) variableIndex);
					}
					else
					{
						variableIndex = PrvAddConstant(node.String);

						if (node.ReturnType.Name == DataType.NAME_FLOAT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT, (short) variableIndex);
						else if (node.ReturnType.Name == DataType.NAME_INT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_INT, (short) variableIndex);
						else if (node.ReturnType.Name == DataType.NAME_STRING)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING, (short) variableIndex);
					}

					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_IDENTIFIER:
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONSTANT:
					if (node.ReturnType.Name == DataType.NAME_STRING)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING, (short) PrvAddConstant((string) node.String));
					else if (node.ReturnType.Name == DataType.NAME_FLOAT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT, (short) PrvAddConstant((float) node.Float));
					else if (node.ReturnType.Name == DataType.NAME_INT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT, (short) PrvAddConstant((int) node.Integer));
					break;

				case SyntTree.SyntTypeEnum.SYNT_CONVERSION:
					PrvAddInstruction(node.GetChild(0));

					if (node.ReturnType.Name == DataType.NAME_FLOAT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_INT_TO_FLOAT);
					else if (node.ReturnType.Name == DataType.NAME_INT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_TO_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_ASSIGNATION:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a asignar
					PrvAddInstruction(node.GetChild(1));
					instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_DUP);
				
					//Asgino el valor del stack a la variable segun corresponda..
					switch(node.GetChild(0).Type)
					{
						case SyntTree.SyntTypeEnum.SYNT_VARIABLE:
						{
							int variableIndex = variablesTable.FindVariable(node.GetChild(0).String);

							switch(variablesTable.GetVariableLocation(variableIndex))
							{
								case VariablesTable.VariableLocationEnum.LOCATION_LOCAL:
									variableIndex = variablesTable.GetVariablePosition(variableIndex);

									switch(PrvTranslateSymbolToDataType(node.GetChild(0).ReturnType).Type)
									{
										case DataType.DataTypeEnum.TYPE_INT:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT, (short) variableIndex);
											break;
										case DataType.DataTypeEnum.TYPE_FLOAT:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT, (short) variableIndex);
											break;
										case DataType.DataTypeEnum.TYPE_STRING:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING, (short) variableIndex);
											break;
									}
										
									break;

								case VariablesTable.VariableLocationEnum.LOCATION_GLOBAL:
									variableIndex = PrvAddConstant(node.GetChild(0).String);
										
									switch(PrvTranslateSymbolToDataType(node.GetChild(0).ReturnType).Type)
									{
										case DataType.DataTypeEnum.TYPE_INT:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_INT, (short) variableIndex);
											break;
										case DataType.DataTypeEnum.TYPE_FLOAT:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT, (short) variableIndex);
											break;
										case DataType.DataTypeEnum.TYPE_STRING:
											instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_STRING, (short) variableIndex);
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
					PrvAddInstruction(node.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAddInstruction(node.GetChild(0));

					//Agrego la instrucci�n de comparaci�n que corresponda
    				switch(PrvTranslateSymbolToDataType(node.GetChild(0).ReturnType).Type)
    				{
    					case DataType.DataTypeEnum.TYPE_INT:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
    						break;

    					case DataType.DataTypeEnum.TYPE_FLOAT:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_TO_INT);
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
    						break;

    					case DataType.DataTypeEnum.TYPE_STRING:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_COMPARE_STRING);
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_ISZERO_INT);
    						break;
    				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_COMPARISON_NOT_EQUAL:
					//Agrego las instrucciones del lado derecho que dejan en el stack el valor a comparar
					PrvAddInstruction(node.GetChild(1));
					//Agrego las instrucciones del lado izquierdo que dejan en el stack el valor a comparar
					PrvAddInstruction(node.GetChild(0));

					//Agrego la instrucci�n de comparaci�n que corresponda
    				switch(PrvTranslateSymbolToDataType(node.GetChild(0).ReturnType).Type)
    				{
    					case DataType.DataTypeEnum.TYPE_INT:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
    						break;

    					case DataType.DataTypeEnum.TYPE_FLOAT:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_FLOAT_TO_INT);
    						break;

    					case DataType.DataTypeEnum.TYPE_STRING:
    						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_COMPARE_STRING);
    						break;
    				}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_ADDITION:
					if (node.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANT && 
						node.GetChild(1).ReturnType.Name == DataType.NAME_INT &&
						node.GetChild(1).Integer == 1)
					{
						//Uso el operador de incremento de int en 1
						PrvAddInstruction(node.GetChild(0));
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_INC_INT);
					}
					else
					{
						//Uso la suma normal
						for (i = 0; i < node.GetChilds(); i++)
							PrvAddInstruction(node.GetChild(i));
					
						if (node.ReturnType.Name == DataType.NAME_FLOAT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_ADD_FLOAT);
						else if (node.ReturnType.Name == DataType.NAME_INT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_ADD_INT);
						else if (node.ReturnType.Name == DataType.NAME_STRING)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_CONCAT_STRING);
					}

					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_SUBTRACT:
					if (node.GetChild(1).Type == SyntTree.SyntTypeEnum.SYNT_CONSTANT && 
						node.GetChild(1).ReturnType.Name == DataType.NAME_INT &&
						node.GetChild(1).Integer == 1)
					{
						//Uso el operador de decremento de int en 1
						PrvAddInstruction(node.GetChild(0));
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_DEC_INT);
					}
					else
					{
						//Uso la resta normal
						for (i = 0; i < node.GetChilds(); i++)
							PrvAddInstruction(node.GetChild(i));

						if (node.ReturnType.Name == DataType.NAME_FLOAT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_FLOAT);
						else
							if (node.ReturnType.Name == DataType.NAME_INT)
							instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_SUB_INT);
					}
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_MULTIPLICATION:
					for (i = 0; i < node.GetChilds(); i++)
						PrvAddInstruction(node.GetChild(i));

					if (node.ReturnType.Name == DataType.NAME_FLOAT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_MULT_FLOAT);
					else
						if (node.ReturnType.Name == DataType.NAME_INT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_MULT_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_OP_DIVISION:
					for (i = 0; i < node.GetChilds(); i++)
						PrvAddInstruction(node.GetChild(i));

					if (node.ReturnType.Name == DataType.NAME_FLOAT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_DIV_FLOAT);
					else
						if (node.ReturnType.Name == DataType.NAME_INT)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_DIV_INT);
					break;

				case SyntTree.SyntTypeEnum.SYNT_IF:
				{
					//Agrego las instrucciones que ejecutan la condicion
					PrvAddInstruction(node.GetChild(0));
					//Agrego el salto segun la condicion
					int instrucionIfIndex = instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAddInstruction(node.GetChild(1));
				
					int instructionElseIndex = 0;
				
					if (node.GetChilds() == 3)
						instructionElseIndex = instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, 0);

					int instructionIndex = instructionsList.GetInstructions();

					//Actualizo la instruccion de salto cuando la condici�n no se cumple
					instructionsList.SetParameter(instrucionIfIndex, (short) (instructionIndex - instrucionIfIndex - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
				
					if (node.GetChilds() == 3)
					{
						//Agrego las instrucciones que se ejecutan cuando la condicion no se cumple
						PrvAddInstruction(node.GetChild(2));

						instructionIndex = instructionsList.GetInstructions();

						instructionsList.SetParameter(instructionElseIndex, (short) (instructionIndex - instructionElseIndex - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
					}
										
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_WHILE:
				{
					int nInstCondicion = instructionsList.GetInstructions();

					//Agrego las instrucciones que ejecutan la condicion
					PrvAddInstruction(node.GetChild(0));
					//Agrego el salto segun la condicion
					int nInstruccionSaltoNoCumple = instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, (short) 0);
					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAddInstruction(node.GetChild(1));
				
					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = instructionsList.GetInstructions();
					int Salto = nInstCondicion - nInstFinCiclo - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, (short) Salto);

					int nInstruccion = instructionsList.GetInstructions();

					//Actualizo la instruccion de salto cuando la condici�n no se cumple
					instructionsList.SetParameter(nInstruccionSaltoNoCumple, (short) (nInstruccion - nInstruccionSaltoNoCumple - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER));
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_FOR:
				{
					//Agrego las instrucciones que inicializan la condici�n el ciclo
					PrvAddInstruction(node.GetChild(0));
					if (PrvTranslateSymbolToDataType(node.GetChild(0).ReturnType).Type != DataType.DataTypeEnum.TYPE_VOID)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);

					//Agrego las instrucciones que evaluan la condici�n del ciclo
					int nInstCondicion = instructionsList.GetInstructions();
					PrvAddInstruction(node.GetChild(1));
				
					//Agrego el salto segun el resultado de la condicion
					int nInstCondicionSalto = instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO, 0);

					//Agrego las instrucciones que se ejecutan cuando la condicion se cumple
					PrvAddInstruction(node.GetChild(3));

					//Agrego las instrucciones que incrementar en contador del ciclo
					PrvAddInstruction(node.GetChild(2));
					if (PrvTranslateSymbolToDataType(node.GetChild(2).ReturnType).Type != DataType.DataTypeEnum.TYPE_VOID)
						instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_STACK_POP);

					//Agrego la instruccion que salta hacia donde se evalua el ciclo
					int nInstFinCiclo = instructionsList.GetInstructions();
					int Salto = nInstCondicion - nInstFinCiclo - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionsList.AddInstruction(InstructionsList.InstructionsEnum.INST_JUMP, (short) Salto);

					//Actualizo la instruccion de salto a fin de ciclo donde se evalua la condicion
					nInstFinCiclo = instructionsList.GetInstructions();
					Salto = nInstFinCiclo - nInstCondicionSalto - InstructionsList.LEN_INSTRUCTION - InstructionsList.LEN_PARAMETER;
					instructionsList.SetParameter(nInstCondicionSalto, (short) Salto);
					break;
				}

				case SyntTree.SyntTypeEnum.SYNT_NONE:
					break;
			}
		}

		DataType PrvTranslateSymbolToDataType(Symbol simbolo)
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
