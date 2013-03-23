using System;
using SimpleCompiler.Shared;

namespace SimpleCompiler.VirtualMachine
{
	public class Runtime
	{
		private Variables variables;
		private FunctionsDefinitions definicionFunciones;
	
		public Runtime(FunctionsDefinitions definicionFunciones)
		{
            this.definicionFunciones = definicionFunciones;
			
			PrvConstruirVariables();
		}

		void PrvConstruirVariables()
		{
            variables = new Variables();

			variables.Create(definicionFunciones.GetVariables());

			for (int i = 0; i < definicionFunciones.GetVariables(); i++)
				variables.Variable(i).Type = definicionFunciones.GetVariable(i).Type;
		}
		
		public Variables ObtenerVariables() 
		{ 
			return(variables); 
		}

		public void SetearVariables(Variables variables) 
		{ 
			this.variables = variables; 
		}

		public FunctionsDefinitions ObtenerDefinicionFunciones() 
		{ 
			return definicionFunciones; 
		}
	}
}
