using System;
using System.Collections.Generic;

namespace SimpleCompiler.Shared
{
	public class SymbolConstantTable
	{
		public const int NOT_FOUND = -1;

		private List<SymbolConstant> symbols;

		public SymbolConstantTable()
		{
            symbols = new List<SymbolConstant>();
		}
		
		public void Clear()
		{
			symbols.Clear();
		}

		public int AddSimbolo(SymbolConstant simbolo)
		{
			symbols.Add(simbolo);

            return symbols.Count - 1;
		}
		
		public int GetSimbolos() 
		{ 
			return symbols.Count; 
		}

		public SymbolConstant GetSymbol(int n) 
		{ 
			return symbols[n]; 
		}

		public int FindSymbolInt(int val)
		{
			int n = NOT_FOUND;

            int i = 0;

			foreach(SymbolConstant symbol in symbols)
			{
				if (symbol.Type == SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT &&
					symbol.Integer == val)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public int FindSymbolFloat(float val)
		{
			int n = NOT_FOUND;

			int i = 0;

			foreach(SymbolConstant symbol in symbols)
			{
				if (symbol.Type == SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_FLOAT &&
					symbol.Float == val)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public int FindSymbolString(string val)
		{
			int n = NOT_FOUND;

			int i = 0;

			foreach(SymbolConstant simbolo in symbols)
			{
				if (simbolo.Type == SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING &&
					simbolo.String == val)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public void Print(int n)
		{
			int i = 0;
			foreach(SymbolConstant symbol in symbols)
			{
				Console.Write("{0}: ", i);
				symbol.Print(n);
				i++;
			}
		}
	}
}
