using System;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.MaquinaVirtual
{
	public class Runtime
	{
		private Variables variables;
		private DefinicionFunciones definicionFunciones;
	
		public Runtime(DefinicionFunciones definicionFunciones)
		{
            this.definicionFunciones = definicionFunciones;
			
			PrvConstruirVariables();
		}

		void PrvConstruirVariables()
		{
            variables = new Variables();

			variables.Crear(definicionFunciones.ObtenerVariables());

			for (int i = 0; i < definicionFunciones.ObtenerVariables(); i++)
				variables.Variable(i).Tipo = definicionFunciones.ObtenerVariable(i).Tipo;
		}
		
		public Variables ObtenerVariables() 
		{ 
			return(variables); 
		}

		public void SetearVariables(Variables variables) 
		{ 
			this.variables = variables; 
		}

		public DefinicionFunciones ObtenerDefinicionFunciones() 
		{ 
			return definicionFunciones; 
		}
	}
}
