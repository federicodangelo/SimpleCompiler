using System;

namespace SimpleCompiler.Shared
{
	public class SymbolConstant
	{
		public enum SymbolTypeEnum
		{
			SYMBOL_TYPE_CONSTANT_INT,
			SYMBOL_TYPE_CONSTANT_FLOAT,
			SYMBOL_TYPE_CONSTANT_STRING
		}

		public string Nombre;
		public SymbolTypeEnum Type;

		public float Float;
		public int Integer;
		public string String;

		public SymbolConstant()
		{
			Type = SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT;
		}

		public void Print(int n)
		{
			for (int i = 0; i < n; i++)
				Console.Write("    ");

			switch(Type)
			{
				case SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT:
					Console.Write("Constante int: {0}\n", Integer);
					break;

				case SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_FLOAT:
					Console.Write("Constante float: {0}\n", Float);
					break;

				case SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING:
					Console.Write("Constante string: {0}\n", String);
					break;
			}
		}
	}
}
