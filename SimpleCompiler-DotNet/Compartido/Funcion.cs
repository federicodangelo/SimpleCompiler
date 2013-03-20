using System;
using System.Collections;
using CompiladorReducido.MaquinaVirtual;

namespace CompiladorReducido.Compartido
{
	public class Funcion
	{
		public enum TipoFuncionEnum 
		{ 
			FUNCION_NORMAL, 
			FUNCION_NATIVA 
		}

		public delegate bool FuncionNativaDelegate(Proceso proceso, CompiladorReducido.MaquinaVirtual.Stack stack, out string descipcionError);

		public string Nombre;
		public string NombreCorto;
		public ListaInstrucciones ListaInstrucciones;
		public TipoDato	TipoDevuelto;
		private ArrayList parametros;
		public int CantVariables;
		public TipoFuncionEnum TipoFuncion;
		public FuncionNativaDelegate FuncionNativa;
		
		public Funcion()
		{
			parametros = new ArrayList();
			Nombre = "";
			TipoFuncion = TipoFuncionEnum.FUNCION_NORMAL;
			ListaInstrucciones = new ListaInstrucciones(8192);
		}

		public Funcion(string definicion, FuncionNativaDelegate funcionNativa)
		{
			parametros = new ArrayList();
			this.FuncionNativa = funcionNativa;
			TipoFuncion = TipoFuncionEnum.FUNCION_NATIVA;
			ConfigurarDesdeDefinicion(definicion);
		}

		public void Limpiar()
		{
			parametros.Clear();
			Nombre = "";
            TipoFuncion = TipoFuncionEnum.FUNCION_NORMAL;
			ListaInstrucciones.Limpiar();
		}

		public void ConfigurarDesdeDefinicion(string definicion)
		{
			bool ok = false;

			definicion = definicion.Trim();

			while(definicion.IndexOf("  ") >= 0)
				definicion = definicion.Replace("  ", " ");

			while(definicion.IndexOf(", ") >= 0)
				definicion = definicion.Replace(", ", ",");

			while(definicion.IndexOf(" ,") >= 0)
				definicion = definicion.Replace(" ,", ",");

			try
			{
				string[] tipoDevuelto = definicion.Split( new char[] { ' ' }, 2);
				//tipoDevuelto[0] == tipo devuelto
				//tipoDevuelto[1] == resto de la declaración de funcion
	            
				if (tipoDevuelto.Length == 2)
				{
					this.TipoDevuelto = new TipoDato(tipoDevuelto[0]);

					string[] nombre = tipoDevuelto[1].Split(new char[] { '(' }, 3);
					//nombre[0] == nombre de la funcion
					//nombre[1] == resto parametros menos el parentesis de comienzo

					if (nombre.Length == 2)
					{
						this.NombreCorto = nombre[0];

						string[] pars = nombre[1].Split(new char[] { ',' });
						//pars[0] == primer parametro o ")"
						//pars[n] == siguiente parametro
						//pars[ultimo] = parametro + ")"
	                    
						if (pars.Length >= 1)
						{
							if (pars[0] == ")")
							{
								ok = true;
							}
							else
							{
								ok = true;

								for (int i = 0; ok == true && i < pars.Length; i++)
								{
									int posPar = pars[i].IndexOf(')');

									if (posPar >= 0)
									{
										if (i != pars.Length - 1 ||
											posPar != pars[i].Length - 1)
										{
											ok = false;
										}
										else
										{
											pars[i] = pars[i].Substring(0, pars[i].Length - 1);
										}
									}

									if (ok)
									{
										string[] defs = pars[i].Split(new char[] { ' ' } , 3);
										//defs[0] == tipo de dato
										//defs[1] == nombre de parametro

										if (defs.Length == 2)
										{
											DefinicionVariable defParam = new DefinicionVariable();

											defParam.Tipo = new TipoDato(defs[0]);
											defParam.Nombre = defs[1];
											
											AddParametro(defParam);
										}
										else
											ok = false;										
									}
								}
							}
						}
					}
				}

				if (ok)
					ActualizarNombreCompleto();
				else
					throw new Exception("La definición pasada es inválida [" + definicion + "]");
			}
			catch(Exception)
			{
                throw new Exception("La definición pasada es inválida [" + definicion + "]");
			}
		}

		public void ActualizarNombreCompleto()
		{
			string nombre = "";
			string separador = ",";
			string separadorFuncion = "#";

			if (TipoDevuelto == null)
				nombre += TipoDato.NOMBRE_VOID;
			else
				nombre += TipoDevuelto.ToString();

			nombre += separadorFuncion;
			nombre += NombreCorto;
			nombre += "(";

			for (int i = 0; i < GetParametros(); i++)
			{
				if (i != 0)
					nombre += separador;

				nombre += GetParametro(i).Tipo.ToString();
			}

			nombre += ")";

			this.Nombre = nombre;
		}

		public void AddParametro(DefinicionVariable parametro)
		{
			parametros.Add(parametro);
		}
		
		public int	GetParametros() 
		{ 
			return parametros.Count; 
		}

		public DefinicionVariable GetParametro(int n) 
		{ 
			return (DefinicionVariable) parametros[n];
		}

		public void Imprimir(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Funcion: {0}\n", Nombre);

			for (i = 0; i < n + 1; i++)
				Console.Write("    ");

			Console.Write("Parametros:\n");
			foreach(DefinicionVariable parametro in parametros)
				parametro.Imprimir(n + 2);
	
			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
	
			Console.Write("Cantidad de variables locales: {0}\n", CantVariables);
	
			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
	
			Console.Write("Instrucciones:\n");
			ListaInstrucciones.Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
		}
	}
}
