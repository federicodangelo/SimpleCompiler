using System;

namespace SimpleCompiler.Compiler
{
	public class Lexer
	{
		#region Attributes

		const int MAX_STRING_LEN = 128;
		const int MAX_IDENTIFIER_LEN = 128;

		string	text;
		int		offsetInText;
		long	line;
		long	column;
		bool	firstLexema;
		char	letra;
		        
		public long Line
		{
			get
			{
				return line;
			}
		}

		public long Column
		{
			get
			{
				return column;
			}
		}

		public bool IsEof
		{
			get
			{
				return (letra == 0);
			}
		}

		#endregion
        
		public Lexer()
		{
			line = 0;
			column = 0;
			text = "";
			offsetInText = 0;
			firstLexema = true;
		}

		public void Clear()
		{
			line = 0;
			column = 0;
			text = "";
			offsetInText = 0;
			firstLexema = true;
		}

		private void PrvGetChar()
		{
			if (offsetInText != text.Length)
			{
				letra = text[offsetInText];
				offsetInText++;
			}
			else
			{
				letra = '\0';
			}

			if (letra == '\n')
			{
				line++;
				column = 0;
			}
			else
			{
				column++;
			}
		}

		public void SetTextToParse(string text)
		{
			Clear();

			this.text = text;

			firstLexema = true;
		}

		public bool GetLexem(ref Lexem lexem)
		{
			bool ok = true, repeat = true;

            lexem.Line = line;
			lexem.Column = column;

			if (firstLexema)
			{
				PrvGetChar();
				firstLexema = false;
			}

			while(repeat == true)
			{
				repeat = false;

				switch(letra)
				{
					case '\t':
						column += 3;
						goto case ' ';
					case '\r':
						goto case ' ';
					case '\n':
						goto case ' ';
					case ' ':
						repeat = true;
						lexem.Line = line;
						lexem.Column = column;
						PrvGetChar();
						break;

					case ';':
						lexem.Type = Lexem.LexTypeEnum.LEX_END;
						PrvGetChar();
						break;

					case '.':
						lexem.Type = Lexem.LexTypeEnum.LEX_OP_MEMBER;
						PrvGetChar();
						break;

					case '=':
						PrvGetChar();

						if (letra == '=')
						{
							PrvGetChar();
							lexem.Type = Lexem.LexTypeEnum.LEX_OP_EQUAL;
						}
						else
							lexem.Type = Lexem.LexTypeEnum.LEX_OP_ASSIGN;
						break;

					case '<':
						PrvGetChar();

						if (letra == '>')
						{
							PrvGetChar();
							lexem.Type = Lexem.LexTypeEnum.LEX_OP_NOT_EQUAL;
						}
						else
							lexem.Type = Lexem.LexTypeEnum.LEX_OP_LESS;
						break;

					case '>':
						PrvGetChar();
						lexem.Type = Lexem.LexTypeEnum.LEX_OP_GREATER;
						break;

					case '(':
						lexem.Type = Lexem.LexTypeEnum.LEX_PAR_OPEN;
						PrvGetChar();
						break;

					case ')':
						lexem.Type = Lexem.LexTypeEnum.LEX_PAR_CLOSE;
						PrvGetChar();
						break;

					case '{':
						lexem.Type = Lexem.LexTypeEnum.LEX_BRACES_OPEN;
						PrvGetChar();
						break;

					case '}':
						lexem.Type = Lexem.LexTypeEnum.LEX_BRACES_CLOSE;
						PrvGetChar();
						break;

					case '+':
						lexem.Type = Lexem.LexTypeEnum.LEX_OP_ADDITION;
						PrvGetChar();
						break;

					case '-':
						lexem.Type = Lexem.LexTypeEnum.LEX_OP_SUBTRACTION;
						PrvGetChar();
						break;

					case '*':
						lexem.Type = Lexem.LexTypeEnum.LEX_OP_MULTIPLICATION;
						PrvGetChar();
						break;

					case '/':
						PrvGetChar();

						switch(letra)
						{
							case '*':
								while (letra != '\0')
								{
									PrvGetChar();
									if (letra == '*')
									{
										PrvGetChar();
										if (letra == '/')
										{
											PrvGetChar();
											break;
										}
									}
								}
							
								repeat = true;
								lexem.Line = line;
								lexem.Column = column;
								break;

							case '/':
								while (letra != '\n' && letra != '\0')
									PrvGetChar();
							
								repeat = true;
								lexem.Line = line;
								lexem.Column = column;
								break;

							default:
								lexem.Type = Lexem.LexTypeEnum.LEX_OP_DIVISION;
								break;
						}
						break;

					case ',':
						lexem.Type = Lexem.LexTypeEnum.LEX_COMMA;
						PrvGetChar();
						break;

					case '"':
					{
						char[] str = new char[MAX_STRING_LEN];
						int i = 0;

						PrvGetChar();

						while (letra != '\0' && letra != '"')
						{
							if (letra == '\\')
							{
								PrvGetChar();

								switch(letra)
								{
									case 't':
										letra = '\t';
										break;
									case 'n':
										letra = '\n';
										break;
									case '\\':
										letra = '\\';
										break;
									default:
										break;
								}
							}

							if (i < MAX_STRING_LEN - 1)
								str[i++] = letra;

							PrvGetChar();
						}

						if (letra == '"')
							PrvGetChar();

						str[i] = '\0';

						lexem.Type = Lexem.LexTypeEnum.LEX_STRING;
						lexem.String = new string(str, 0, i);
						break;
					}

					case '0':
						goto case '9';
					case '1':
						goto case '9';
					case '2':
						goto case '9';
					case '3':
						goto case '9';
					case '4':
						goto case '9';
					case '5':
						goto case '9';
					case '6':
						goto case '9';
					case '7':
						goto case '9';
					case '8':
						goto case '9';
					case '9':
					{
						long IntegerPart = 0;
						long FloatPart = 0;
						int FloatDigits = 0;
						float f;

						IntegerPart = letra - '0';

						PrvGetChar();

						while (letra >= '0' && letra <= '9')
						{
							IntegerPart = IntegerPart * 10 + (letra - '0');
							PrvGetChar();
						}
				
						if (letra == '.')
						{
							PrvGetChar();
					
							while (letra >= '0' && letra <= '9')
							{
								FloatPart = FloatPart * 10 + (letra - '0');
								PrvGetChar();
								FloatDigits++;
							}

							f = (float) IntegerPart;
					
							float aux = (float) FloatPart;

							while (FloatDigits > 0)
							{
								aux /= 10;
								FloatDigits--;
							}

							f += aux;
					
							lexem.Float = f;
							lexem.Type = Lexem.LexTypeEnum.LEX_FLOAT;
						}
						else
						{
							lexem.Integer = IntegerPart;
							lexem.Type = Lexem.LexTypeEnum.LEX_INTEGER;
						}
						break;	
					}
					
						
					case '\0':
						lexem.Type = Lexem.LexTypeEnum.LEX_EOF;
						break;

					default:
						if (letra >= 'a' && letra <= 'z' ||
							letra >= 'A' && letra <= 'Z' ||
							letra == '_')
						{
							char[] str = new char[MAX_IDENTIFIER_LEN];
							int i = 1;

							str[0] = letra;

							PrvGetChar();

							while(	letra >= 'a' && letra <= 'z' ||
								letra >= 'A' && letra <= 'Z' ||
								letra >= '0' && letra <= '9' ||
								letra == '_')
							{
								if (i < MAX_IDENTIFIER_LEN - 1)
									str[i++] = letra;
								PrvGetChar();
							}
							
							str[i] = '\0';

							string realString = new string(str, 0, i);

							string[] reservedKeywords = 
							{
								"if",		((int) Lexem.LexTypeEnum.LEX_IF).ToString(), 
								"else",		((int) Lexem.LexTypeEnum.LEX_ELSE).ToString(), 
								"class",	((int) Lexem.LexTypeEnum.LEX_CLASS).ToString(), 
								"return",	((int) Lexem.LexTypeEnum.LEX_RETURN).ToString(), 
								"new",		((int) Lexem.LexTypeEnum.LEX_NEW).ToString(), 
								"for",		((int) Lexem.LexTypeEnum.LEX_FOR).ToString(), 
								"while",	((int) Lexem.LexTypeEnum.LEX_WHILE).ToString(),
								"try",		((int) Lexem.LexTypeEnum.LEX_TRY).ToString(),
								"catch",	((int) Lexem.LexTypeEnum.LEX_CATCH).ToString(),
								"finally",	((int) Lexem.LexTypeEnum.LEX_FINALLY).ToString(),
								"throw",	((int) Lexem.LexTypeEnum.LEX_THROW).ToString(),
								null, null 
							};

							lexem.String = realString;

							int n = 0;
							bool reserved = false;

							while (reservedKeywords[n] != null)
							{
								if (realString == reservedKeywords[n])
								{
									lexem.Type = (Lexem.LexTypeEnum) int.Parse(reservedKeywords[n + 1]);
									reserved = true;
									break;
								}
								n += 2;
							}

							if (reserved == false)
							{
								lexem.Type = Lexem.LexTypeEnum.LEX_IDENTIFIER;
								lexem.String = realString;
							}
					
						}
						break;
				}
			}
			return(ok);
		}
	}
}
