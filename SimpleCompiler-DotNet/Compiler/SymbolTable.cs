using System;
using System.Collections.Generic;

namespace SimpleCompiler.Compiler
{
	public class SymbolTable
	{
		public SymbolTable ParentTable;
		private List<Symbol> symbols;
		
		public SymbolTable()
		{
            symbols = new List<Symbol>();
		}

		public void Clear()
		{
			symbols.Clear();
		}

		public void AddSymbol(Symbol symbol)
		{
			symbols.Add(symbol);
		}
		
		public int GetSymbols() 
		{ 
			return(symbols.Count); 
		}

		public Symbol GetSymbol(int n) 
		{ 
			return symbols[n]; 
		}

		public Symbol FindGlobalSymbol(string nombre)
		{
			Symbol simbolo = FindLocalSymbol(nombre);

			if (simbolo == null && ParentTable != null)
				simbolo = ParentTable.FindGlobalSymbol(nombre);

			return simbolo;
		}

		public Symbol FindNextGlobalSymbol(Symbol symbol)
		{
			Symbol nextSymbol = FindNextLocalSymbol(symbol);

			if (nextSymbol == null && ParentTable != null)
				nextSymbol = ParentTable.FindNextGlobalSymbol(symbol);

			return nextSymbol;
		}

		public Symbol FindLocalSymbol(string name)
		{
			Symbol symbol = null;

			foreach (Symbol s in symbols)
            {
				if (s.Name == name)
				{
					symbol = s;
					break;
				}
            }

			return symbol;
		}

		public Symbol FindNextLocalSymbol(Symbol symbol)
		{
			bool compare = false;
			Symbol nextSymbol = null;

			foreach (Symbol s in symbols)
			{
				if (compare)
				{
					if (s.Name == symbol.Name)
					{
						nextSymbol = s;
						break;
					}
				}
				else
				{
                    if (s == symbol)
						compare = true;
				}
			}

			return nextSymbol;
		}

		public void Print(int n)
		{
			foreach(Symbol s in symbols)
				s.Print(n);
		}
	}
}
