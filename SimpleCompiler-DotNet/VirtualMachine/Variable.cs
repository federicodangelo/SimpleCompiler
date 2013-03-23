using System;
using SimpleCompiler.Shared;

namespace SimpleCompiler.VirtualMachine
{
	public class Variable
	{
		public DataType	Type;
		
        private object val;

		public Variable()
		{
		}

		private void ValidateOrSetDataType(DataType.DataTypeEnum type)
		{
			if (Type.Type != DataType.DataTypeEnum.TYPE_NONE)
			{
                if (Type.Type != type)
                    throw new RuntimeError("Tipo de variable incorrecto");
			}
			else
				Type.Type = type;
		}

		private void ValidateDataType(DataType.DataTypeEnum tipo)
		{
			if (Type.Type != tipo)
				throw new RuntimeError("Tipo de variable incorrecto");
		}

		public void SetInt(int valor)
		{
			ValidateOrSetDataType(DataType.DataTypeEnum.TYPE_INT);

			this.val = valor;
		}

		public void SetFloat(float valor)
		{
			ValidateOrSetDataType(DataType.DataTypeEnum.TYPE_FLOAT);

			this.val = valor;
		}

		public void SetString(string valor)
		{
			ValidateOrSetDataType(DataType.DataTypeEnum.TYPE_STRING);

			this.val = valor;
		}

		public int GetInt()
		{
			ValidateDataType(DataType.DataTypeEnum.TYPE_INT);

			return (int) val;
		}

		public float GetFloat()
		{
			ValidateDataType(DataType.DataTypeEnum.TYPE_FLOAT);

			return (float) val;
		}

		public string GetString()
		{
			ValidateDataType(DataType.DataTypeEnum.TYPE_STRING);

			return (string) val;
		}
	}
}
