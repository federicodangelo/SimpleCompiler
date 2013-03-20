using System;

namespace CompiladorReducido.MaquinaVirtual
{
	public class ErrorEjecucion : Exception
	{
		public ErrorEjecucion(string descripcion) : base(descripcion)
		{
		}
	}
}
