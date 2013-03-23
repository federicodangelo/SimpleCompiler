using System;
using System.Collections.Generic;
using SimpleCompiler.Shared;

namespace SimpleCompiler.Compiler
{
	public class SyntTree
	{
		public enum SyntTypeEnum
		{
			SYNT_INSTRUCTION_NULL,
			SYNT_INSTRUCTION,
			SYNT_RETURN,
			SYNT_FUNCTIONS_LIST,
			SYNT_INSTRUCTIONS_LIST,
			SYNT_FUNCTION_DECLARATION,
			SYNT_VARIABLE_DECLARATION,
			SYNT_FUNCTION_CALL,
			SYNT_SYMBOLS_TABLE,
			SYNT_VARIABLE,
			SYNT_IDENTIFIER,
			SYNT_CONSTANT,
			SYNT_CONVERSION,
			SYNT_OP_COMPARISON_EQUAL,
			SYNT_OP_COMPARISON_NOT_EQUAL,
			SYNT_OP_ASSIGNATION,
			SYNT_OP_ADDITION,
			SYNT_OP_SUBTRACT,
			SYNT_OP_MULTIPLICATION,
			SYNT_OP_DIVISION,
			SYNT_IF,
			SYNT_FOR,
			SYNT_WHILE,
			SYNT_NONE
		};

		public SyntTypeEnum	Type;
		public Symbol	    ReturnType;
		public Symbol	    FunctionDefinitionSymbol;

		private List<SyntTree> childs;

		public long Integer;
		public float Float;
		public string String;
		public SymbolTable SymbolsTable;

		public SyntTree()
		{
			String = "";
		}
		
		private void Clear()
		{
			SymbolsTable = null;
			ReturnType = null;
			FunctionDefinitionSymbol = null;
			childs = null;
			Integer = 0;
			Float = 0.0f;
			String = "";
		}

		public void AddChild(SyntTree node)
		{
			if (childs == null)
                childs = new List<SyntTree>();

			childs.Add(node);
        }

		public int GetChilds() 
		{ 
			if (childs == null)
				return 0;

			return(childs.Count); 
		}

		public SyntTree GetChild(int n) 
		{ 
			return childs[n]; 
		}

		public void Print(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			int childNumber = 0;

			switch(Type)
			{
				case SyntTypeEnum.SYNT_INSTRUCTION:
					Console.Write("Instruction: \n");
					break;

				case SyntTypeEnum.SYNT_RETURN:
					Console.Write("Return: \n");
					break;

				case SyntTypeEnum.SYNT_FOR:
					Console.Write("Instruction For:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Initialization:\n");
					GetChild(childNumber++).Print(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condition:\n");
					GetChild(childNumber++).Print(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Increment:\n");
					GetChild(childNumber++).Print(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Cycle:\n");
					GetChild(childNumber++).Print(n + 2);
					break;

				case SyntTypeEnum.SYNT_WHILE:
					Console.Write("Instruction While:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condition:\n");
					GetChild(childNumber++).Print(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Cycle:\n");
					GetChild(childNumber++).Print(n + 2);
					break;

				case SyntTypeEnum.SYNT_IF:
					Console.Write("Instruction If:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condition:\n");
					GetChild(childNumber++).Print(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("If true:\n");
					GetChild(childNumber++).Print(n + 2);

					if (GetChilds() == 3)
					{
						for (i = 0; i < n + 1; i++)
							Console.Write("    ");
						Console.Write("If false:\n");
						GetChild(childNumber++).Print(n + 2);
					}
					break;

				case SyntTypeEnum.SYNT_INSTRUCTION_NULL:
					Console.Write("Instruction null\n");
					break;

				case SyntTypeEnum.SYNT_FUNCTIONS_LIST:
					Console.Write("Functions list:\n");
					break;

				case SyntTypeEnum.SYNT_INSTRUCTIONS_LIST:
					Console.Write("Instructions list:\n");
					break;

				case SyntTypeEnum.SYNT_IDENTIFIER:
					Console.Write("Identifier: {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_VARIABLE:
					Console.Write("Variable: {0}\n", String);
					break;
		
				case SyntTypeEnum.SYNT_CONSTANT:
					Console.Write("Constant value ");

					if (ReturnType.Name == DataType.NAME_INT)
						Console.Write("(int) {0}\n", Integer);
					else
						if (ReturnType.Name == DataType.NAME_FLOAT)
							Console.Write("(float) {0}\n", Float);
					else
						if (ReturnType.Name == DataType.NAME_STRING)
							Console.Write("(string) {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_OP_ASSIGNATION:
					Console.Write ("Operator =\n");
					break;

				case SyntTypeEnum.SYNT_OP_ADDITION:
                    Console.Write ("Operator +\n");
					break;

				case SyntTypeEnum.SYNT_OP_SUBTRACT:
                    Console.Write ("Operator -\n");
					break;

				case SyntTypeEnum.SYNT_OP_MULTIPLICATION:
                    Console.Write ("Operator *\n");
					break;

				case SyntTypeEnum.SYNT_OP_DIVISION:
					Console.Write ("Operador /\n");
					break;

				case SyntTypeEnum.SYNT_FUNCTION_CALL:
					Console.Write("Function call: {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_VARIABLE_DECLARATION:
					Console.Write("Variable declaration of type {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_FUNCTION_DECLARATION:
					Console.Write("Function declaration: {0}\n", String);
					break;
		
				case SyntTypeEnum.SYNT_CONVERSION:
					Console.Write("Conversion from {0} to {1}\n", GetChild(0).ReturnType.Name, ReturnType.Name);
					break;

                case SyntTypeEnum.SYNT_OP_COMPARISON_EQUAL:
                    Console.Write("Are equal:\n");
                    break;
               
                case SyntTypeEnum.SYNT_OP_COMPARISON_NOT_EQUAL:
                    Console.Write("Are not equal:\n");
                    break;

                case SyntTypeEnum.SYNT_SYMBOLS_TABLE:
					Console.Write("Symbols table\n");
					SymbolsTable.Print(n + 1);
					break;
			}

			n++;

			for (; childNumber < GetChilds(); childNumber++)
				GetChild(childNumber).Print(n);
		}
	}
}
