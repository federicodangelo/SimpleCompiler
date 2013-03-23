using System;
using System.Collections.Generic;
using SimpleCompiler.Shared;

namespace SimpleCompiler.Compiler
{
	public class Symbol
	{
		public enum SymbolTypeEnum
		{
			SYMBOL_TYPE_DATA,
			SYMBOL_TYPE_VARIABLE,
			SYMBOL_TYPE_FUNCTION
		}

		public static Symbol SymbolInt = new Symbol(SymbolTypeEnum.SYMBOL_TYPE_DATA, DataType.NAME_INT);
		public static Symbol SymbolFloat = new Symbol(SymbolTypeEnum.SYMBOL_TYPE_DATA, DataType.NAME_FLOAT);
		public static Symbol SymbolString = new Symbol(SymbolTypeEnum.SYMBOL_TYPE_DATA, DataType.NAME_STRING);
		public static Symbol SymbolVoid = new Symbol(SymbolTypeEnum.SYMBOL_TYPE_DATA, DataType.NAME_VOID);

		public const char CHAR_SEPARATOR_FUNCTION = '#';
		public const char CHAR_SEPARATOR_PARAMETER = ',';

		public string Name;
		public string FullFunctionName;
		public SymbolTypeEnum Type;
		public Symbol ReturnType;
		public SymbolTable SymbolsTable;
		public int NumberOfVariables;
		public bool NativeFunction;
		
        private List<Symbol> parameters;

		public Symbol(SymbolTypeEnum Type, string Nombre) : this()
		{
			this.Type = Type;
			this.Name = Nombre;
		}

		public Symbol()
		{
			Name = "";
			FullFunctionName = "";
			Type = SymbolTypeEnum.SYMBOL_TYPE_VARIABLE;
			ReturnType = null;
			SymbolsTable = null;
			NativeFunction = false;
			NumberOfVariables = 0;
			parameters = null;
		}

		public void Clear()
		{
			Name = "";
			FullFunctionName = "";
			Type = SymbolTypeEnum.SYMBOL_TYPE_VARIABLE;
			ReturnType = null;
			SymbolsTable = null;
			NativeFunction = false;
			NumberOfVariables = 0;
			parameters = null;
		}

		public void AddParameter(Symbol parametro)
		{
			if (parameters == null)
                parameters = new List<Symbol>();

            parameters.Add(parametro);
		}

		public int GetParameters() 
		{ 
			if (parameters == null)
				return 0;

			return parameters.Count; 
		}

		public Symbol GetParameter(int n) 
		{ 
			return (Symbol) parameters[n];
		}

		public void UpdateFullFunctionName()
		{
			string name = "";
			string separator = new string(CHAR_SEPARATOR_PARAMETER, 1);
			string separatorFunction = new string(CHAR_SEPARATOR_FUNCTION, 1);

			if (ReturnType == null)
				name += DataType.NAME_VOID;
			else
				name += ReturnType.Name;

			name += separatorFunction;
			name += Name;
			name += "(";

			for (int i = 0; i < GetParameters(); i++)
			{
				if (i != 0)
					name += separator;

				name += GetParameter(i).ReturnType.Name;
			}

			name += ")";

			FullFunctionName = name;
		}

		public void Print(int n)
		{
			for (int i = 0; i < n; i++)
				Console.Write("    ");

			string type;

			switch(Type)
			{
				case SymbolTypeEnum.SYMBOL_TYPE_DATA:
					type = "Data Type";
					break;

				case SymbolTypeEnum.SYMBOL_TYPE_VARIABLE:
					type = "Variable";
					break;

				case SymbolTypeEnum.SYMBOL_TYPE_FUNCTION:
					type = "Function";
					break;

				default:
					type = "";
					break;
			}

			Console.Write("Symbol: {0}, Type: {1}", Name, type);

			if (ReturnType != null)
				Console.Write(", Returns: {0}\n", ReturnType.Name);
			else
				Console.Write("\n");

			if (parameters != null)
			{
				int i;

				for (i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Parameters: \n");

				foreach(Symbol parameter in parameters)
					parameter.Print(n + 2);
			}

			if (Type == SymbolTypeEnum.SYMBOL_TYPE_FUNCTION)
			{
				for (int i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Number of local vars: {0}\n", NumberOfVariables);
			}

			if (SymbolsTable != null)
			{
				for (int i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Additional symbols: \n");

				SymbolsTable.Print(n + 2);
			}
		}
	}
}
