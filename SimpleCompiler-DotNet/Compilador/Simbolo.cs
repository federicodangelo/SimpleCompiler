using System;
using System.Collections;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.Compilador
{
	public class Simbolo
	{
		public enum SimboloTypeEnum
		{
			SIMBOLO_TIPO_DATO,
			SIMBOLO_TIPO_VARIABLE,
			SIMBOLO_TIPO_FUNCION
		}

		public static Simbolo SimboloInt = new Simbolo(SimboloTypeEnum.SIMBOLO_TIPO_DATO, TipoDato.NOMBRE_INT);
		public static Simbolo SimboloFloat = new Simbolo(SimboloTypeEnum.SIMBOLO_TIPO_DATO, TipoDato.NOMBRE_FLOAT);
		public static Simbolo SimboloString = new Simbolo(SimboloTypeEnum.SIMBOLO_TIPO_DATO, TipoDato.NOMBRE_STRING);
		public static Simbolo SimboloVoid = new Simbolo(SimboloTypeEnum.SIMBOLO_TIPO_DATO, TipoDato.NOMBRE_VOID);

		public const char SIMBOLO_SEPARADOR_FUNCION = '#';
		public const char SIMBOLO_SEPARADOR_PARAMETRO = ',';

		public string Nombre;
		public string NombreFuncionCompleto;
		public SimboloTypeEnum Type;
		public Simbolo ReturnType;
		public TablaSimbolos TablaSimbolos;
		public int CantVariables;
		public bool FuncionNativa;
		ArrayList parametros;

		public Simbolo(SimboloTypeEnum Type, string Nombre) : this()
		{
			this.Type = Type;
			this.Nombre = Nombre;
		}

		public Simbolo()
		{
			Nombre = "";
			NombreFuncionCompleto = "";
			Type = SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE;
			ReturnType = null;
			TablaSimbolos = null;
			FuncionNativa = false;
			CantVariables = 0;
			parametros = null;
		}

		public void Clear()
		{
			Nombre = "";
			NombreFuncionCompleto = "";
			Type = SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE;
			ReturnType = null;
			TablaSimbolos = null;
			FuncionNativa = false;
			CantVariables = 0;
			parametros = null;
		}

		public void AddParametro(Simbolo parametro)
		{
			if (parametros == null)
				parametros = new ArrayList();

            parametros.Add(parametro);
		}

		public int	GetParametros() 
		{ 
			if (parametros == null)
				return 0;

			return parametros.Count; 
		}

		public Simbolo GetParametro(int n) 
		{ 
			return (Simbolo) parametros[n];
		}

		public void ActualizarNombreCompletoFuncion()
		{
			string nombre = "";
			string separador = new string(SIMBOLO_SEPARADOR_PARAMETRO, 1);
			string separadorFuncion = new string(SIMBOLO_SEPARADOR_FUNCION, 1);

			if (ReturnType == null)
				nombre += TipoDato.NOMBRE_VOID;
			else
				nombre += ReturnType.Nombre;

			nombre += separadorFuncion;
			nombre += Nombre;
			nombre += "(";

			for (int i = 0; i < GetParametros(); i++)
			{
				if (i != 0)
					nombre += separador;

				nombre += GetParametro(i).ReturnType.Nombre;
			}

			nombre += ")";

			NombreFuncionCompleto = nombre;
		}

		public void Imprimir(int n)
		{
			for (int i = 0; i < n; i++)
				Console.Write("    ");

			string tipo;

			switch(Type)
			{
				case SimboloTypeEnum.SIMBOLO_TIPO_DATO:
					tipo = "Tipo de Dato";
					break;

				case SimboloTypeEnum.SIMBOLO_TIPO_VARIABLE:
					tipo = "Variable";
					break;

				case SimboloTypeEnum.SIMBOLO_TIPO_FUNCION:
					tipo = "Funcion";
					break;

				default:
					tipo = "";
					break;
			}

			Console.Write("Simbolo: {0}, Tipo: {1}", Nombre, tipo);

			if (ReturnType != null)
				Console.Write(", Devuelve: {0}\n", ReturnType.Nombre);
			else
				Console.Write("\n");

			if (parametros != null)
			{
				int i;

				for (i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Parametros: \n");

				foreach(Simbolo parametro in parametros)
					parametro.Imprimir(n + 2);
			}

			if (Type == SimboloTypeEnum.SIMBOLO_TIPO_FUNCION)
			{
				for (int i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Cantidad de variables locales usadas: {0}\n", CantVariables);
			}

			if (TablaSimbolos != null)
			{
				for (int i = 0; i < n + 1; i++)
					Console.Write("    ");

				Console.Write("Simbolos adicionales: \n");

				TablaSimbolos.Imprimir(n + 2);
			}
		}
	}
}
