using System;
using System.Collections;

namespace CompiladorReducido.Compilador
{
	public class TablaSimbolos
	{
		public TablaSimbolos TablaPadre;
		private ArrayList simbolos;
		
		public TablaSimbolos()
		{
			simbolos = new ArrayList();
		}

		public void Clear()
		{
			simbolos.Clear();
		}

		public void AddSimbolo(Simbolo simbolo)
		{
			simbolos.Add(simbolo);
		}
		
		public int GetSimbolos() 
		{ 
			return(simbolos.Count); 
		}

		public Simbolo GetSimbolo(int n) 
		{ 
			return (Simbolo) simbolos[n]; 
		}

		public Simbolo FindSimboloGlobal(string nombre)
		{
			Simbolo simbolo = FindSimboloLocal(nombre);

			if (simbolo == null && TablaPadre != null)
				simbolo = TablaPadre.FindSimboloGlobal(nombre);

			return simbolo;
		}

		public Simbolo FindProximoSimboloGlobal(Simbolo simbolo)
		{
			Simbolo proximoSimbolo = FindProximoSimboloLocal(simbolo);

			if (proximoSimbolo == null && TablaPadre != null)
				proximoSimbolo = TablaPadre.FindProximoSimboloGlobal(simbolo);

			return proximoSimbolo;
		}

		public Simbolo FindSimboloLocal(string nombre)
		{
			Simbolo simbolo = null;

			foreach (Simbolo s in simbolos)
				if (s.Nombre == nombre)
				{
					simbolo = s;
					break;
				}

			return simbolo;
		}

		public Simbolo FindProximoSimboloLocal(Simbolo simbolo)
		{
			bool compare = false;
			Simbolo proximoSimbolo = null;

			foreach (Simbolo s in simbolos)
			{
				if (compare)
				{
					if (s.Nombre == simbolo.Nombre)
					{
						proximoSimbolo = s;
						break;
					}
				}
				else
				{
                    if (s == simbolo)
						compare = true;
				}
			}

			return proximoSimbolo;
		}

		public void Imprimir(int n)
		{
			foreach(Simbolo s in simbolos)
				s.Imprimir(n);
		}
	}
}
