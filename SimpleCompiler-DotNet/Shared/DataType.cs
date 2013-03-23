using System;

namespace SimpleCompiler.Shared
{
	public class DataType
	{
		public enum DataTypeEnum
		{
			TYPE_NONE,
			TYPE_VOID,
			TYPE_INT,
			TYPE_FLOAT,
			TYPE_STRING
		};

		public const string NAME_INT	= "int";
		public const string NAME_FLOAT = "float";
		public const string NAME_STRING = "string";
		public const string NAME_VOID = "void";
		
		public DataTypeEnum Type;

		public DataType()
		{
			Type = DataTypeEnum.TYPE_NONE;
		}

		public DataType(DataTypeEnum Type)
		{
            this.Type = Type;
		}

		public DataType(string name)
		{
			if (name == NAME_INT)
				Type = DataTypeEnum.TYPE_INT;
			else if (name == NAME_FLOAT)
				Type = DataTypeEnum.TYPE_FLOAT;
			else if (name == NAME_STRING)
				Type = DataTypeEnum.TYPE_STRING;
			else if (name == NAME_VOID)
				Type = DataTypeEnum.TYPE_VOID;
			else
	            throw new Exception("Data type " + name + " not supported");
		}

		public override string ToString()
		{
			string s;

			switch(Type)
			{
				case DataTypeEnum.TYPE_VOID:
					s =  NAME_VOID;
					break;
				case DataTypeEnum.TYPE_INT:
					s =  NAME_INT;
					break;
				case DataTypeEnum.TYPE_FLOAT:
					s =  NAME_FLOAT;
					break;
				case DataTypeEnum.TYPE_STRING:
					s =  NAME_STRING;
					break;
				default:
					s = "???";
					break;
			}

			return s;
		}

	}
}
