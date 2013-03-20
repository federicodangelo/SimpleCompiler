using System;

namespace CompiladorReducido.MaquinaVirtual
{
	public class Variables
	{
		private Variable[] variables;

		public void Limpiar()
		{
			variables = null;
		}

		public void Crear(int nVariables)
		{
			variables = new Variable[nVariables];

			for (int i = 0; i < nVariables; i++)
				variables[i] = new Variable();
		}

		public Variable Variable(int n)
		{
			if (n >= variables.Length)
				throw new ErrorEjecucion("Numero de variable invalido");
			
			return variables[n];
		}
	}
}
