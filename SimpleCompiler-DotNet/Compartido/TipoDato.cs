using System;

namespace CompiladorReducido.Compartido
{
	public class TipoDato
	{
		public enum TipoDatoEnum
		{
			TIPO_NINGUNO,
			TIPO_VOID,
			TIPO_INT,
			TIPO_FLOAT,
			TIPO_STRING
		};

		public const string NOMBRE_INT	= "int";
		public const string NOMBRE_FLOAT = "float";
		public const string NOMBRE_STRING = "string";
		public const string NOMBRE_VOID = "void";
		
		public TipoDatoEnum Tipo;

		public TipoDato()
		{
			Tipo = TipoDatoEnum.TIPO_NINGUNO;
		}

		public TipoDato(TipoDatoEnum Tipo)
		{
            this.Tipo = Tipo;
		}

		public TipoDato(string nombre)
		{
			if (nombre == NOMBRE_INT)
				Tipo = TipoDatoEnum.TIPO_INT;
			else if (nombre == NOMBRE_FLOAT)
				Tipo = TipoDatoEnum.TIPO_FLOAT;
			else if (nombre == NOMBRE_STRING)
				Tipo = TipoDatoEnum.TIPO_STRING;
			else if (nombre == NOMBRE_VOID)
				Tipo = TipoDatoEnum.TIPO_VOID;
			else
	            throw new Exception("Tipo de dato " + nombre + " no soportado");
		}

		public override string ToString()
		{
			string s;

			switch(Tipo)
			{
				case TipoDatoEnum.TIPO_VOID:
					s =  NOMBRE_VOID;
					break;
				case TipoDatoEnum.TIPO_INT:
					s =  NOMBRE_INT;
					break;
				case TipoDatoEnum.TIPO_FLOAT:
					s =  NOMBRE_FLOAT;
					break;
				case TipoDatoEnum.TIPO_STRING:
					s =  NOMBRE_STRING;
					break;
				default:
					s = "???";
					break;
			}

			return s;
		}

	}
}
