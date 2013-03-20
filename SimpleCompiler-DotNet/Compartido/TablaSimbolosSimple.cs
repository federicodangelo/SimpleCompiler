using System;
using System.Collections;

namespace CompiladorReducido.Compartido
{
	public class TablaSimbolosSimple
	{
		public const int NO_ENCONTRADO = -1;

		private ArrayList simbolos;

		public TablaSimbolosSimple()
		{
			simbolos = new ArrayList();
		}
		
		public void Clear()
		{
			simbolos.Clear();
		}

		public int AddSimbolo(SimboloSimple simbolo)
		{
			return simbolos.Add(simbolo);
		}
		
		public int GetSimbolos() 
		{ 
			return simbolos.Count; 
		}

		public SimboloSimple GetSimbolo(int n) 
		{ 
			/*No hace falta ningun chequeo por la validación que hago en CDefinicionObjeto::PrvValidarFuncion*/ 
			return (SimboloSimple) simbolos[n]; 
		}

		public int FindSimboloInt(long valor)
		{
			int n = NO_ENCONTRADO;

            int i = 0;

			foreach(SimboloSimple simbolo in simbolos)
			{
				if (simbolo.Type == SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_INT &&
					simbolo.Integer == valor)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public int FindSimboloFloat(float valor)
		{
			int n = NO_ENCONTRADO;

			int i = 0;

			foreach(SimboloSimple simbolo in simbolos)
			{
				if (simbolo.Type == SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_FLOAT &&
					simbolo.Float == valor)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public int FindSimboloString(string valor)
		{
			int n = NO_ENCONTRADO;

			int i = 0;

			foreach(SimboloSimple simbolo in simbolos)
			{
				if (simbolo.Type == SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_STRING &&
					simbolo.String == valor)
				{
					n = i;
					break;
				}

				i++;
			}

			return(n);
		}

		public void Imprimir(int n)
		{
			int i = 0;
			foreach(SimboloSimple simbolo in simbolos)
			{
				Console.Write("{0}: ", i);
				simbolo.Imprimir(n);
				i++;
			}
		}
	}
}
