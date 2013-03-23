using System;

namespace SimpleCompiler.VirtualMachine
{
	public class RuntimeError : Exception
	{
		public RuntimeError(string description) : base(description)
		{
		}
	}
}
