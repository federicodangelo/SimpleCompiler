using System;

namespace SimpleCompiler.VirtualMachine
{
	public class Variables
	{
		private Variable[] variables;

		public void Clear()
		{
			variables = null;
		}

		public void Create(int nVariables)
		{
			variables = new Variable[nVariables];

			for (int i = 0; i < nVariables; i++)
				variables[i] = new Variable();
		}

		public Variable Variable(int n)
		{
			if (n >= variables.Length)
				throw new RuntimeError("Invalid variable number");
			
			return variables[n];
		}
	}
}
