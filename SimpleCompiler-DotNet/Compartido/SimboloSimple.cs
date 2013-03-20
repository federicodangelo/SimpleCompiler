using System;

namespace CompiladorReducido.Compartido
{
	public class SimboloSimple
	{
		public enum SimboloTypeEnum
		{
			SIMBOLO_TIPO_CONSTANTE_INT,
			SIMBOLO_TIPO_CONSTANTE_FLOAT,
			SIMBOLO_TIPO_CONSTANTE_STRING
		}

		public string Nombre;
		public SimboloTypeEnum Type;

		public float Float;
		public int Integer;
		public string String;

		public SimboloSimple()
		{
			Type = SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_INT;
		}

		public void Imprimir(int n)
		{
			for (int i = 0; i < n; i++)
				Console.Write("    ");

			switch(Type)
			{
				case SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_INT:
					Console.Write("Constante int: {0}\n", Integer);
					break;

				case SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_FLOAT:
					Console.Write("Constante float: {0}\n", Float);
					break;

				case SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_STRING:
					Console.Write("Constante string: {0}\n", String);
					break;
			}
		}
	}
}
