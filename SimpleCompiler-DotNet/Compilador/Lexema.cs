using System;

namespace CompiladorReducido.Compilador
{
	public struct Lexema
	{
		public enum LexTypeEnum
		{
			LEX_CLASS,
			LEX_NEW,
			LEX_RETURN,
			LEX_IF,
			LEX_WHILE,
			LEX_FOR,
			LEX_ELSE,
			LEX_TRY,
			LEX_CATCH,
			LEX_FINALLY,
			LEX_THROW,
			LEX_IDENTIFICADOR,
			LEX_INTEGER,
			LEX_FLOAT,
			LEX_STRING,
			LEX_OP_IGUAL,
			LEX_OP_DISTINTO,
			LEX_OP_MAYOR,
			LEX_OP_MENOR,
			LEX_OP_ASIGNACION,
			LEX_OP_MAS,
			LEX_OP_MENOS,
			LEX_OP_POR,
			LEX_OP_DIV,
			LEX_OP_MIEMBRO,
			LEX_PAR_ABRE,
			LEX_PAR_CIERRA,
			LEX_LLAVE_ABRE,
			LEX_LLAVE_CIERRA,
			LEX_COMA,
			LEX_END,
			LEX_EOF,
			LEX_NONE
		}

		public LexTypeEnum Type;
		public long	Line;
		public long	Column;
	
		public long	Integer;
		public float Float;
		public string String;
	}
}
