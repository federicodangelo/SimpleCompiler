using System;

namespace SimpleCompiler.Shared
{
	public class VariableDefinition
	{
		public DataType Type;
		public string Name;

		public VariableDefinition()
		{
		}
		
		public void Print(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			string tipo = "";

			switch(Type.Type)
			{
				case DataType.DataTypeEnum.TYPE_INT:
					tipo = "int";
					break;
				case DataType.DataTypeEnum.TYPE_FLOAT:
					tipo = "float";
					break;
				case DataType.DataTypeEnum.TYPE_STRING:
					tipo = "string";
					break;
			}

			Console.Write("Name: {0}, Data Type: {1}\n", Name, tipo);
		}
	}
}
