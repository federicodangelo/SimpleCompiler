using System;

namespace CompiladorReducido.Compartido
{
	public class DefinicionVariable
	{
		public TipoDato Tipo;
		public string Nombre;

		public DefinicionVariable()
		{
		}
		
		public void Imprimir(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			string tipo = "";

			switch(Tipo.Tipo)
			{
				case TipoDato.TipoDatoEnum.TIPO_INT:
					tipo = "int";
					break;
				case TipoDato.TipoDatoEnum.TIPO_FLOAT:
					tipo = "float";
					break;
				case TipoDato.TipoDatoEnum.TIPO_STRING:
					tipo = "string";
					break;
			}

			Console.Write("Nombre: {0}, Tipo de Dato: {1}\n", Nombre, tipo);
		}
	}
}
