using System;
using System.Collections.Generic;
using SimpleCompiler.Shared;

namespace SimpleCompiler.Compiler
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
		#region Attributes

        private const int MAX_LEXEMS_STACK_SIZE = 12;

        private Lexer lexer;
        private Lexem lexem;

        private Lexem[] lexemsStack;
        private int		lexemsStackPosition;

        private SymbolTable symbolsTable;
        private Symbol	symbolInt;
        private Symbol	symbolFloat;
        private Symbol	symbolString;
        private Symbol	symbolVoid;

        private Symbol returnType;

        private int stackVariables;
        private int maxVariables;

		private Function[] externalFunctions;

		#endregion

		#region Public Methods

		public Parser()
		{
            lexemsStack = new Lexem[MAX_LEXEMS_STACK_SIZE];
			lexemsStackPosition = 0;
			lexer = new Lexer();
			lexem = new Lexem();
		}

		public void Clear()
		{
			lexemsStackPosition = 0;
			lexer.Clear();
			lexer = new Lexer();
			lexem = new Lexem();
		}

		public void SetProgram(String text)
		{
			lexer.Clear();
			lexer.SetTextToParse(text);
		}

		public void SetExternalsFunctions(Function[] funciones)
		{
            externalFunctions = funciones;            
		}

		public bool GenSyntTree(out SyntTree root, ref List<ParserError> errors)
		{
			bool ok = true;

			PrvCreateSymbolsTable();

			root = null;

			try
			{
				PrvGetLexem();

				root = PrvFunctions();
			}
			catch(ParserError error)
			{
				errors.Add(error);

				ok = false;
			}

			return(ok);
		}

		#endregion

		#region Private Methods

		#region Aux Functions

        private void PrvGetLexem()
		{
			if (lexemsStackPosition > 0)
				lexem = lexemsStack[--lexemsStackPosition];
			else
				lexer.GetLexem(ref lexem);
		}

        private void PrvReturnLexem(Lexem lexem)
		{
			if (lexemsStackPosition == MAX_LEXEMS_STACK_SIZE)
				throw CreateParserError("Lexems stack is full");

			lexemsStack[lexemsStackPosition++] = this.lexem;

			this.lexem = lexem;
		}
		
		private void PrvCreateSymbolsTable()
		{
			symbolsTable = new SymbolTable();

			symbolInt = Symbol.SymbolInt;
			symbolsTable.AddSymbol(symbolInt);

			symbolFloat = Symbol.SymbolFloat;
			symbolsTable.AddSymbol(symbolFloat);

			symbolString = Symbol.SymbolString;
			symbolsTable.AddSymbol(symbolString);

			symbolVoid = Symbol.SymbolVoid;
			symbolsTable.AddSymbol(symbolVoid);

			if (externalFunctions != null)
			{
				foreach(Function f in externalFunctions)
				{
					Symbol functionSymbol = new Symbol();

					functionSymbol.Type = Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION;
					functionSymbol.ReturnType = symbolsTable.FindGlobalSymbol(f.ReturnType.ToString());
					functionSymbol.Name = f.NameShort;

					for (int i = 0; i < f.GetParameters(); i++)
					{
						VariableDefinition defPar = f.GetParameter(i);

                        Symbol parameterDataType = symbolsTable.FindGlobalSymbol(defPar.Type.ToString());

						Symbol parameter = new Symbol();

						parameter.Type = Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE;
						parameter.Name = defPar.Name;
						parameter.ReturnType = parameterDataType;

						functionSymbol.AddParameter(parameter);
					}

					functionSymbol.UpdateFullFunctionName();

					functionSymbol.NativeFunction = true;

					symbolsTable.AddSymbol(functionSymbol);
				}
			}
		}

		private SyntTree PrvAddSymbolsTable(SymbolTable symbolsTable)
		{
			SyntTree node = new SyntTree();

			node.Type = SyntTree.SyntTypeEnum.SYNT_SYMBOLS_TABLE;
			node.SymbolsTable = symbolsTable;

			return node;
		}

		private SyntTree PrvConvertReturnType(SyntTree node, Symbol returnType)
		{
			if (node.ReturnType != returnType)
			{
				if (node.ReturnType == symbolInt)
				{
					if (returnType == symbolFloat)
					{
						SyntTree conversor = new SyntTree();
						conversor.Type = SyntTree.SyntTypeEnum.SYNT_CONVERSION;
						conversor.ReturnType = symbolFloat;
						conversor.AddChild(node);
						node = conversor;
					}
				}
				else
				{
					if (node.ReturnType == symbolFloat)
					{
						if (returnType == symbolInt)
						{
							SyntTree conversor = new SyntTree();
							conversor.Type = SyntTree.SyntTypeEnum.SYNT_CONVERSION;
							conversor.ReturnType = symbolInt;
							conversor.AddChild(node);
							node = conversor;
						}
					}
				}
			}

			return node;
		}

		private ParserError CreateParserError(string description)
		{
			return new ParserError(description, this.lexem.Line, this.lexem.Column);
		}

		#endregion

		#region Parse tree generation
		
		private SyntTree PrvFunctions()
		{
			SyntTree node = null;

			node = new SyntTree();

			node.Type = SyntTree.SyntTypeEnum.SYNT_FUNCTIONS_LIST;

			node.AddChild(PrvAddSymbolsTable(symbolsTable));

			while (this.lexem.Type != Lexem.LexTypeEnum.LEX_EOF)
			{
				SyntTree child = PrvFunction();
				
				if (child == null)
					child = PrvVariableDeclaration(false);

				if (child == null)
					throw CreateParserError("Function or variable declaration expected");

				node.AddChild(child);
			}

			return node;
		}

		private SyntTree PrvFunction()
		{
			SyntTree node = null;

			if (lexem.Type == Lexem.LexTypeEnum.LEX_IDENTIFIER)
			{
				Symbol symbolReturnType = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

				if (symbolReturnType == null)
					throw CreateParserError("Symbol not found");

				if (symbolReturnType.Type == Symbol.SymbolTypeEnum.SYMBOL_TYPE_DATA)
				{
					Lexem lexTipoDato = this.lexem;
					PrvGetLexem();

					if (this.lexem.Type == Lexem.LexTypeEnum.LEX_IDENTIFIER)
					{
						Symbol functionSymbol = this.symbolsTable.FindLocalSymbol(this.lexem.String);

						//Valido que el identificador este no utiliza o, si esta utilizado, que sea una funci�n y que coincida el return type
						//Despues de obtener los parametros, ahi genero el nombre completo
						//de la funci�n y en base a eso determino si la funci�n ya esta declarada o no
						if (functionSymbol != null && functionSymbol.Type != Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION)
							throw CreateParserError("Duplicated identifier, it already exists as a variable");

						if (functionSymbol != null && functionSymbol.ReturnType != symbolReturnType)
							throw CreateParserError("Cant have to functions with same name and different return type");

						Lexem lexNombreFuncion = this.lexem;
						PrvGetLexem();

						if (this.lexem.Type == Lexem.LexTypeEnum.LEX_PAR_OPEN)
						{
							Symbol oldTipoReturn = this.returnType;

							this.returnType = symbolReturnType;

							functionSymbol = new Symbol();

							functionSymbol.Type = Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION;
							functionSymbol.ReturnType = symbolReturnType;
							functionSymbol.Name = lexNombreFuncion.String;

							//Lo agrego antes de haber verificado si ya esta declarado o no, de
							//esta forma me evito los problemas de borrar el simbolo cuando
							//falle si ya existe la funci�n
							this.symbolsTable.AddSymbol(functionSymbol);

							node = new SyntTree();
							node.Type = SyntTree.SyntTypeEnum.SYNT_FUNCTION_DECLARATION;
							node.ReturnType = null;

							PrvGetLexem();

							SymbolTable table = new SymbolTable();
							table.ParentTable = this.symbolsTable;
							this.symbolsTable = table;

							node.AddChild(PrvAddSymbolsTable(table));

							try
							{
								bool firstParameter = true;

								while(this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
								{
									if (firstParameter == false)
									{
										if (this.lexem.Type != Lexem.LexTypeEnum.LEX_COMMA)
											throw CreateParserError("Expected ,");

										PrvGetLexem();
									}
									else
									{
										firstParameter = false;
									}

									if (this.lexem.Type != Lexem.LexTypeEnum.LEX_IDENTIFIER)
										throw CreateParserError("Data type expected");

									Symbol parameterDataType = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

									if (parameterDataType == null)
										throw CreateParserError("Data type expected");

									PrvGetLexem();

									if (this.lexem.Type != Lexem.LexTypeEnum.LEX_IDENTIFIER)
										throw CreateParserError("Identifier expected");

									Symbol parameter = this.symbolsTable.FindLocalSymbol(this.lexem.String);

									if (parameter != null)
										throw CreateParserError("Duplicated symbol");

									parameter = new Symbol();

									parameter.Type = Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE;
									parameter.Name = this.lexem.String;
									parameter.ReturnType = parameterDataType;

									functionSymbol.AddParameter(parameter);

									this.symbolsTable.AddSymbol(parameter);

									PrvGetLexem();
								}

								PrvGetLexem();

								//Ya obtuve el tipo de dato de todos los parametros, ahora genero el nombre completo de la funci�n
								functionSymbol.UpdateFullFunctionName();

								Symbol existingSymbol = this.symbolsTable.FindGlobalSymbol(functionSymbol.Name);

								while(existingSymbol != null)
								{
									if (existingSymbol != functionSymbol)
										if (functionSymbol.FullFunctionName == existingSymbol.FullFunctionName)
                                            throw CreateParserError("Duplicated function definition (there is an existing function with same name and parameters)");

									existingSymbol = this.symbolsTable.FindNextGlobalSymbol(existingSymbol);
								}

								node.String = functionSymbol.Name;
								node.FunctionDefinitionSymbol = functionSymbol;

								this.stackVariables = functionSymbol.GetParameters();

								if (functionSymbol.ReturnType.Name != DataType.NAME_VOID)
									this.stackVariables++;

								this.maxVariables = this.stackVariables;

								SyntTree instrucciones = PrvInstructions();

								if (instrucciones == null)
								{
									if (this.lexem.Type == Lexem.LexTypeEnum.LEX_END)
									{
										PrvGetLexem();

										functionSymbol.NativeFunction = true;
									}
									else
										throw CreateParserError("Expected {");
								}
								else
								{
									node.AddChild(instrucciones);
								}

								functionSymbol.NumberOfVariables = this.maxVariables;
							}
							catch(ParserError err)
							{
								this.symbolsTable = this.symbolsTable.ParentTable;
								throw err;
							}

							this.symbolsTable = this.symbolsTable.ParentTable;

							this.returnType = oldTipoReturn;
						}
						else
						{
							PrvReturnLexem(lexNombreFuncion);
							PrvReturnLexem(lexTipoDato);
						}
					}
					else
					{
						PrvReturnLexem(lexTipoDato);
					}
				}
			}

			return node;
		}


		private SyntTree PrvInstructions()
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_BRACES_OPEN)
			{
				PrvGetLexem();

				node = new SyntTree();

				node.Type = SyntTree.SyntTypeEnum.SYNT_INSTRUCTIONS_LIST;

				SymbolTable table;

				table = new SymbolTable();
				table.ParentTable = this.symbolsTable;
				this.symbolsTable = table;

				node.AddChild(PrvAddSymbolsTable(table));

				int oldStackVars = stackVariables;

				try
				{
					while (this.lexem.Type != Lexem.LexTypeEnum.LEX_BRACES_CLOSE)
					{
						SyntTree child = PrvInstruction();

						if (child == null)
							throw CreateParserError("Instruction expected");
						
						node.AddChild(child);
					}

					PrvGetLexem();
				}
				catch(ParserError err)
				{
					this.symbolsTable = this.symbolsTable.ParentTable;
					throw err;
				}

				if (this.stackVariables > this.maxVariables)
					this.maxVariables = this.stackVariables;

				this.stackVariables = oldStackVars;

				this.symbolsTable = this.symbolsTable.ParentTable;
			}

			return node;
		}

		private SyntTree PrvInstruction()
		{
			SyntTree node = null;
			SyntTree child = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_END)
			{
				child = new SyntTree();
				child.Type = SyntTree.SyntTypeEnum.SYNT_INSTRUCTION_NULL;

				PrvGetLexem();
			}
			else
			{
				if (this.lexem.Type == Lexem.LexTypeEnum.LEX_BRACES_OPEN)
				{
					child = PrvInstructions();
				}
				else
				{
					child = PrvExpresion();

					if (child != null)
					{
						if (this.lexem.Type != Lexem.LexTypeEnum.LEX_END)
							throw CreateParserError("Expected ;");
						PrvGetLexem();
					}
			
					if (child == null)
						child = PrvVariableDeclaration(true);

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
				node.Type = SyntTree.SyntTypeEnum.SYNT_INSTRUCTION;

				node.AddChild(child);

				node.ReturnType = child.ReturnType;
			}

			return node;
		}

		private SyntTree PrvVariableDeclaration(bool allowInitialization)
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_IDENTIFIER)
			{
				Symbol simboloTipoDato = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

				if (simboloTipoDato == null)
					throw CreateParserError("Symbol not found");

				if (simboloTipoDato.Type == Symbol.SymbolTypeEnum.SYMBOL_TYPE_DATA)
				{
					if (simboloTipoDato == symbolVoid)
						throw CreateParserError("Cant declare variables of type void");

					node = new SyntTree();
					node.Type = SyntTree.SyntTypeEnum.SYNT_VARIABLE_DECLARATION;
					node.String = this.lexem.String;
					node.ReturnType = null;

					PrvGetLexem();

					bool firstDeclaration = true;

					while (this.lexem.Type != Lexem.LexTypeEnum.LEX_END)
					{
						if (firstDeclaration == false)
						{
							if (this.lexem.Type != Lexem.LexTypeEnum.LEX_COMMA)
								throw CreateParserError("Expected ,");
							PrvGetLexem();
						}
						else
						{
							firstDeclaration = false;
						}

						if (this.lexem.Type != Lexem.LexTypeEnum.LEX_IDENTIFIER)
							throw CreateParserError("Expected identifier");

						if (this.symbolsTable.FindLocalSymbol(this.lexem.String) != null)
							throw CreateParserError("Duplicated variable definition");

						SyntTree variable = new SyntTree();

						variable.Type = SyntTree.SyntTypeEnum.SYNT_IDENTIFIER;
						variable.ReturnType = simboloTipoDato;
						variable.String = this.lexem.String;

						node.AddChild(variable);

						Symbol variableSymbol;

						variableSymbol = new Symbol();

						variableSymbol.Name = variable.String;
						variableSymbol.ReturnType = simboloTipoDato;
						variableSymbol.Type = Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE;

						this.symbolsTable.AddSymbol(variableSymbol);

						PrvGetLexem();

						if (allowInitialization)
						{
							if (this.lexem.Type == Lexem.LexTypeEnum.LEX_OP_ASSIGN)
							{
								PrvGetLexem();

								SyntTree inicializacion = null;
								//Es la inicializaci�n de la variable
								inicializacion = PrvExpresion();

								if (inicializacion == null)
									throw CreateParserError("Expected expression");

								inicializacion = PrvConvertReturnType(inicializacion, simboloTipoDato);

								if (inicializacion.ReturnType != simboloTipoDato)
									throw CreateParserError("The data type returned by the initialization doesnt match the variable data type");

								variable.AddChild(inicializacion);
							}
						}

						stackVariables++;

						if (stackVariables > maxVariables)
							maxVariables = stackVariables;
					}

					PrvGetLexem();
				}
			}

			return node;
		}

		private SyntTree PrvIf()
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_IF)
			{
				SyntTree condition = null;
                SyntTree instruction = null;
                SyntTree elseInstruction = null;

				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_OPEN)
					throw CreateParserError("Expected (");

				PrvGetLexem();

				condition = PrvExpresion();

				if (condition == null)
					throw CreateParserError("Expected expression");

				condition = PrvConvertReturnType(condition, symbolInt);

				if (condition.ReturnType != symbolInt)
					throw CreateParserError("The returned data type cant be evaluated");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
					throw CreateParserError("Expected )");
		
				PrvGetLexem();

				instruction = PrvInstruction();
		
				if (instruction == null)
					throw CreateParserError("Expected instruction");

				if (this.lexem.Type == Lexem.LexTypeEnum.LEX_ELSE)
				{
					PrvGetLexem();

                    elseInstruction = PrvInstruction();
			
					if (instruction == null)
						throw CreateParserError("Expected instruction");
				}

				node = new SyntTree();

				node.Type = SyntTree.SyntTypeEnum.SYNT_IF;
				node.AddChild(condition);
				node.AddChild(instruction);

                if (elseInstruction != null)
                    node.AddChild(elseInstruction);
			}

			return(node);
		}

		private SyntTree PrvFor()
		{
			SyntTree node = null;

			/* for (Initialization, Condition, Increment)
					Cycle; */

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_FOR)
			{
				SyntTree initialization = null;
				SyntTree condition = null;
				SyntTree increment = null;
				SyntTree cycle = null;

				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_OPEN)
					throw CreateParserError("Expected (");

				PrvGetLexem();

				initialization = PrvExpresion();

				if (initialization == null)
					throw CreateParserError("Expected expression");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_END)
					throw CreateParserError("Expected ;");

				PrvGetLexem();

				condition = PrvExpresion();

				if (condition == null)
					throw CreateParserError("Expected expression");

				condition = PrvConvertReturnType(condition, symbolInt);

				if (condition.ReturnType != symbolInt)
					throw CreateParserError("El tipo de dato devuelto por la expresion no es evaluable");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_END)
					throw CreateParserError("Expected ;");

				PrvGetLexem();

				increment = PrvExpresion();

				if (increment == null)
                    throw CreateParserError("Expected expression");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
					throw CreateParserError("Expected )");

				PrvGetLexem();

				cycle = PrvInstruction();
		
				if (cycle == null)
					throw CreateParserError("Expected instruction");

				node = new SyntTree();

				node.Type = SyntTree.SyntTypeEnum.SYNT_FOR;
				node.AddChild(initialization);
				node.AddChild(condition);
				node.AddChild(increment);
				node.AddChild(cycle);
			}

			return(node);
		}

		private SyntTree PrvWhile()
		{
			SyntTree node = null;

			/*while (condition)
				cycle;*/

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_WHILE)
			{
				SyntTree condition = null;
				SyntTree cycle = null;

				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_OPEN)
					throw CreateParserError("Se esperaba (");

				PrvGetLexem();

				condition = PrvExpresion();

				if (condition == null)
					throw CreateParserError("Se esperaba una expresion");

				condition = PrvConvertReturnType(condition, symbolInt);

				if (condition.ReturnType != symbolInt)
					throw CreateParserError("El tipo de dato devuelto no es evaluable");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
					throw CreateParserError("Se esperaba )");
		
				PrvGetLexem();

				cycle = PrvInstruction();
		
				if (cycle == null)
					throw CreateParserError("Se esperaba una instrucci�n");

				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_WHILE;
				node.AddChild(condition);
				node.AddChild(cycle);
			}

			return(node);
		}

		private SyntTree PrvReturn()
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_RETURN)
			{
				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_OPEN)
					throw CreateParserError("Se esperaba (");

				PrvGetLexem();

				SyntTree child = PrvExpresion();

				if (child == null)
					throw CreateParserError("Se esperaba una expresion");

				child = PrvConvertReturnType(child, this.returnType);

				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_RETURN;
				node.AddChild(child);

				if (child.ReturnType != this.returnType)
					throw CreateParserError("El tipo de dato devuelto no coincide con el tipo de dato que devuelve la funci�n.");

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
					throw CreateParserError("Se esperaba )");
		
				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_END)
					throw CreateParserError("Se esperaba ;");

				PrvGetLexem();
			}

			return node;
		}

		private SyntTree PrvExpresion()
		{
			SyntTree node = null;
			bool loop = true;

			node = PrvAdditionSubstraction();

			while(loop)
			{
				switch(this.lexem.Type)
				{
					case Lexem.LexTypeEnum.LEX_OP_ASSIGN:
					{
						if (node.Type != SyntTree.SyntTypeEnum.SYNT_VARIABLE)
							throw CreateParserError("El lado izquierdo de una asignaci�n debe ser una variable");

						PrvGetLexem();

						SyntTree nodo2 = PrvAdditionSubstraction();

						if (nodo2 == null)
							throw CreateParserError("Error de sintaxis");

						nodo2 = PrvConvertReturnType(nodo2, node.ReturnType);

						if (nodo2.ReturnType != node.ReturnType)
							throw CreateParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree asignacion = new SyntTree();

						asignacion.Type = SyntTree.SyntTypeEnum.SYNT_OP_ASSIGNATION;
						asignacion.ReturnType = node.ReturnType;
						asignacion.AddChild(node);
						asignacion.AddChild(nodo2);
						node = asignacion;
						break;
					}

					case Lexem.LexTypeEnum.LEX_OP_EQUAL:
					{
						node = PrvLogicOperator(node);
						node.Type = SyntTree.SyntTypeEnum.SYNT_OP_COMPARISON_EQUAL;
						break;
					}

					case Lexem.LexTypeEnum.LEX_OP_NOT_EQUAL:
					{
						node = PrvLogicOperator(node);
						node.Type = SyntTree.SyntTypeEnum.SYNT_OP_COMPARISON_NOT_EQUAL;
						break;
					}

					default:
						loop = false;
						break;
				}
			}

			return(node);
		}

		private SyntTree PrvAdditionSubstraction()
		{
			SyntTree termino = null;
			bool bLoop = true;

			termino = PrvMultiplicationDivision();

			while(bLoop)
			{
				switch(this.lexem.Type)
				{
					case Lexem.LexTypeEnum.LEX_OP_ADDITION:
					case Lexem.LexTypeEnum.LEX_OP_SUBTRACTION:
					{
						Lexem.LexTypeEnum LexOpType = this.lexem.Type;

						if (termino == null)
							throw CreateParserError("Error de sintaxis");

						if (termino.ReturnType != symbolInt && termino.ReturnType != symbolFloat && termino.ReturnType != symbolString)
							throw CreateParserError("El operador izquierdo devuelve un tipo de dato no utilizable");

						PrvGetLexem();

						SyntTree termino2 = PrvMultiplicationDivision();

						if (termino2 == null)
							throw CreateParserError("Error de sintaxis");

						termino2 = PrvConvertReturnType(termino2, termino.ReturnType);

						if (termino2.ReturnType != termino.ReturnType)
							throw CreateParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree suma = new SyntTree();

						if (LexOpType == Lexem.LexTypeEnum.LEX_OP_ADDITION)
							suma.Type = SyntTree.SyntTypeEnum.SYNT_OP_ADDITION;
						else
							suma.Type = SyntTree.SyntTypeEnum.SYNT_OP_SUBTRACT;

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

		private SyntTree PrvLogicOperator(SyntTree nodo)
		{
			PrvGetLexem();

			if (nodo.ReturnType != symbolInt &&
				nodo.ReturnType != symbolFloat &&
				nodo.ReturnType != symbolString)
				throw CreateParserError("El lado izquierdo de una comparaci�n debe ser un numero o una cadena");

			SyntTree nodo2 = PrvAdditionSubstraction();

			if (nodo2 == null)
				throw CreateParserError("Error de sintaxis");

			nodo2 = PrvConvertReturnType(nodo2, nodo.ReturnType);

			if (nodo2.ReturnType != nodo.ReturnType)
				throw CreateParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

			SyntTree comparacion = new SyntTree();

			//comparacion.SetType(SyntTree.SYNT_OP_COMPARACION_IGUAL);
			//El tipo se seteado por la funci�n que me llama
			comparacion.ReturnType = symbolInt;
			comparacion.AddChild(nodo);
			comparacion.AddChild(nodo2);
			nodo = comparacion;

			return(nodo);
		}

		private SyntTree PrvMultiplicationDivision()
		{
			SyntTree factor = null;

			bool bLoop = true;

			factor = PrvValue();

			while(bLoop)
			{
				switch(this.lexem.Type)
				{
					case Lexem.LexTypeEnum.LEX_OP_MULTIPLICATION:
					case Lexem.LexTypeEnum.LEX_OP_DIVISION:
					{
						Lexem.LexTypeEnum LexOpType = this.lexem.Type;

						if (factor == null)
							throw CreateParserError("Error de sintaxis");

						if (factor.ReturnType != symbolInt && factor.ReturnType != symbolFloat)
							throw CreateParserError("El operador izquierdo devuelve un tipo de dato no utilizable");

						PrvGetLexem();

						SyntTree factor2 = PrvValue();

						if (factor2 == null)
							throw CreateParserError("Error de sintaxis");

						factor2 = PrvConvertReturnType(factor2, factor.ReturnType);

						if (factor2.ReturnType != factor.ReturnType)
							throw CreateParserError("El tipo de datos devuelto por el operador derecho no es compatible con el operador izquierdo");

						SyntTree mult = new SyntTree();

						if (LexOpType == Lexem.LexTypeEnum.LEX_OP_MULTIPLICATION)
							mult.Type = SyntTree.SyntTypeEnum.SYNT_OP_MULTIPLICATION;
						else
							mult.Type = SyntTree.SyntTypeEnum.SYNT_OP_DIVISION;
				
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

		private SyntTree PrvValue()
		{
			SyntTree nodo = null;

			bool bLoop = true;

			nodo = PrvValue2();

			while(bLoop)
			{
				switch(this.lexem.Type)
				{
					default:
						bLoop = false;
						break;
				}
			}

			return(nodo);
		}

		private SyntTree PrvValue2()
		{
			SyntTree node = null;

			switch(this.lexem.Type)
			{
				case Lexem.LexTypeEnum.LEX_PAR_OPEN:
					PrvGetLexem();
					node = PrvExpresion();
					if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
						throw CreateParserError("Se esperaba )");
					PrvGetLexem();
					break;
	
				case Lexem.LexTypeEnum.LEX_IDENTIFIER:
				{
					Symbol simbolo = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

					if (simbolo == null)
						throw CreateParserError("El simbolo especificado no se encontro");

					switch(simbolo.Type)
					{
						case Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE:
							node = PrvVariable();
							break;

						case Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION:
							node = PrvFunctionCall();
							break;
					}
			
					break;
				}

				case Lexem.LexTypeEnum.LEX_FLOAT:
				case Lexem.LexTypeEnum.LEX_INTEGER:
				case Lexem.LexTypeEnum.LEX_STRING:
					node = PrvConstant();
					break;

				default:
					break;
			}

			return node;
		}

		private SyntTree PrvConstant()
		{
			SyntTree node = null;

			switch(this.lexem.Type)
			{
				case Lexem.LexTypeEnum.LEX_FLOAT:
				case Lexem.LexTypeEnum.LEX_INTEGER:
				case Lexem.LexTypeEnum.LEX_STRING:
					node = new SyntTree();
					node.Integer = this.lexem.Integer;
					node.Type = SyntTree.SyntTypeEnum.SYNT_CONSTANT;

					if (this.lexem.Type == Lexem.LexTypeEnum.LEX_INTEGER)
					{
						node.ReturnType = symbolInt;
						node.Integer = this.lexem.Integer;
					}
					else
					{
						if (this.lexem.Type == Lexem.LexTypeEnum.LEX_FLOAT)
						{
							node.ReturnType = symbolFloat;
							node.Float = this.lexem.Float;
						}
						else
						{
							node.ReturnType = symbolString;
							node.String = this.lexem.String;
						}
					}

					PrvGetLexem();
					break;
			}

			return node;
		}

		private SyntTree PrvVariable()
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_IDENTIFIER)
			{
				Symbol simbolo = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

				if (simbolo == null)
					throw CreateParserError("El simbolo especificado no se encontro");

				node = PrvVariable2(simbolo);
			}

			return node;
		}

		private SyntTree PrvVariable2(Symbol simbolo)
		{
			SyntTree node = null;

			if (simbolo.Type == Symbol.SymbolTypeEnum.SYMBOL_TYPE_VARIABLE)
			{
				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_VARIABLE;
				node.String = this.lexem.String;
				node.ReturnType = simbolo.ReturnType;
				PrvGetLexem();
			}

			return node;
		}

		private SyntTree PrvFunctionCall()
		{
			SyntTree node = null;

			if (this.lexem.Type == Lexem.LexTypeEnum.LEX_IDENTIFIER)
			{
				Symbol simbolo = this.symbolsTable.FindGlobalSymbol(this.lexem.String);

				if (simbolo == null)
					throw CreateParserError("El simbolo especificado no se encontro");

				if (simbolo.Type == Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION)
				{
					node = PrvFunctionCall2(simbolo.Name, this.symbolsTable);
				}
			}

			return node;
		}

		private SyntTree PrvFunctionCall2(string nombreFuncion, SymbolTable tablaSimbolos)
		{
			SyntTree node = null;
	
			Symbol simbolo = tablaSimbolos.FindGlobalSymbol(nombreFuncion);
	
			if (simbolo != null && simbolo.Type == Symbol.SymbolTypeEnum.SYMBOL_TYPE_FUNCTION)
			{
				node = new SyntTree();
				node.Type = SyntTree.SyntTypeEnum.SYNT_FUNCTION_CALL;
		
				PrvGetLexem();

				if (this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_OPEN)
					throw CreateParserError("Se esperaba un (");

				node.Type = SyntTree.SyntTypeEnum.SYNT_FUNCTION_CALL;
				PrvGetLexem();
		
				int CantParametros = 0;
		
				while(this.lexem.Type != Lexem.LexTypeEnum.LEX_PAR_CLOSE)
				{
					if (CantParametros > 0)
					{
						if (this.lexem.Type != Lexem.LexTypeEnum.LEX_COMMA)
							throw CreateParserError("Se esperaba ,");
						PrvGetLexem();
					}

					SyntTree pParameter;
					pParameter = PrvExpresion();

					if (pParameter == null)
						throw CreateParserError("Se esperaba un par�metro");

					if (pParameter.ReturnType == null || pParameter.ReturnType == symbolVoid)
						throw CreateParserError("El parametro es de un tipo de dato no compatible");
			
					node.AddChild(pParameter);

					CantParametros++;				
				}

				//Genero el nombre de la funci�n en base a los tipos de datos devueltos por
				//los parametros, pero omito el tipo de dato que devuelve la funci�n
				nombreFuncion = "";
				nombreFuncion += simbolo.Name;
				nombreFuncion += "(";

				string separador = new String(Symbol.CHAR_SEPARATOR_PARAMETER, 1);

				for (int i = 0; i < CantParametros; i++)
				{
					if (i != 0)
						nombreFuncion += separador;

					nombreFuncion += node.GetChild(i).ReturnType.Name;
				};

				nombreFuncion += ")";

				//Busco alguna funci�n cuyos parametros coincidan con los que tengo

				Symbol simboloFuncion = tablaSimbolos.FindGlobalSymbol(simbolo.Name);

				simbolo = null;

				while(simboloFuncion != null)
				{
					string[] nombre = simboloFuncion.FullFunctionName.Split( new char[] { Symbol.CHAR_SEPARATOR_FUNCTION }, 2);

					if (nombre[1] == nombreFuncion)
					{
						simbolo = simboloFuncion;
						break;
					}
											
					simboloFuncion = tablaSimbolos.FindNextGlobalSymbol(simboloFuncion);
				}

				if (simbolo == null)
					throw CreateParserError(String.Format("No se encontro ninguna funci�n de la forma '{0}'", nombreFuncion));

				node.FunctionDefinitionSymbol = simbolo;
				node.String = simbolo.FullFunctionName;
				node.ReturnType = simbolo.ReturnType;

				PrvGetLexem();

				if (CantParametros != simbolo.GetParameters())
					throw CreateParserError("Faltan parametros");
			}

			return node;
		}

		#endregion

		#endregion
	}
}
