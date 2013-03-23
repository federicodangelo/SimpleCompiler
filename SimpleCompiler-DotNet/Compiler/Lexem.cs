using System;

namespace SimpleCompiler.Compiler
{
	public struct Lexem
	{
		public enum LexTypeEnum
		{
			LEX_CLASS,          //class
			LEX_NEW,            //new
			LEX_RETURN,         //return
			LEX_IF,             //if
			LEX_WHILE,          //while
			LEX_FOR,            //for
			LEX_ELSE,           //else
			LEX_TRY,            //try
			LEX_CATCH,          //catch
			LEX_FINALLY,        //finally
			LEX_THROW,          //throw
			LEX_IDENTIFIER,     //ANY STRING that doesnt match an existing lexem
			LEX_INTEGER,        //Integer value (use Integer variable)
			LEX_FLOAT,          //Float value   (use Float variable)
			LEX_STRING,         //String value  (use String variable)
			LEX_OP_EQUAL,       // ==
			LEX_OP_NOT_EQUAL,   // !=
			LEX_OP_GREATER,     // >
			LEX_OP_LESS,        // <
			LEX_OP_ASSIGN,      // =
			LEX_OP_ADDITION,        // +
			LEX_OP_SUBTRACTION,     // -
			LEX_OP_MULTIPLICATION,  // *
			LEX_OP_DIVISION,        // /
			LEX_OP_MEMBER,      // .
			LEX_PAR_OPEN,       // (
			LEX_PAR_CLOSE,      // )
			LEX_BRACES_OPEN,    // {
            LEX_BRACES_CLOSE,   // }
			LEX_COMMA,          // ,
            LEX_END,            // ;
			LEX_EOF,            // End of file reached
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
