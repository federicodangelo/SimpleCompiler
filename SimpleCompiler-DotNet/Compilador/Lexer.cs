using System;

namespace CompiladorReducido.Compilador
{
	public class Lexer
	{
		#region Atributos

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

		void PrvGetChar()
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

		public bool GetLexema(ref Lexema lexema)
		{
			bool ok = true, repeat = true;

            lexema.Line = line;
			lexema.Column = column;

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
						lexema.Line = line;
						lexema.Column = column;
						PrvGetChar();
						break;

					case ';':
						lexema.Type = Lexema.LexTypeEnum.LEX_END;
						PrvGetChar();
						break;

					case '.':
						lexema.Type = Lexema.LexTypeEnum.LEX_OP_MIEMBRO;
						PrvGetChar();
						break;

					case '=':
						PrvGetChar();

						if (letra == '=')
						{
							PrvGetChar();
							lexema.Type = Lexema.LexTypeEnum.LEX_OP_IGUAL;
						}
						else
							lexema.Type = Lexema.LexTypeEnum.LEX_OP_ASIGNACION;
						break;

					case '<':
						PrvGetChar();

						if (letra == '>')
						{
							PrvGetChar();
							lexema.Type = Lexema.LexTypeEnum.LEX_OP_DISTINTO;
						}
						else
							lexema.Type = Lexema.LexTypeEnum.LEX_OP_MENOR;
						break;

					case '>':
						PrvGetChar();
						lexema.Type = Lexema.LexTypeEnum.LEX_OP_MAYOR;
						break;

					case '(':
						lexema.Type = Lexema.LexTypeEnum.LEX_PAR_ABRE;
						PrvGetChar();
						break;

					case ')':
						lexema.Type = Lexema.LexTypeEnum.LEX_PAR_CIERRA;
						PrvGetChar();
						break;

					case '{':
						lexema.Type = Lexema.LexTypeEnum.LEX_LLAVE_ABRE;
						PrvGetChar();
						break;

					case '}':
						lexema.Type = Lexema.LexTypeEnum.LEX_LLAVE_CIERRA;
						PrvGetChar();
						break;

					case '+':
						lexema.Type = Lexema.LexTypeEnum.LEX_OP_MAS;
						PrvGetChar();
						break;

					case '-':
						lexema.Type = Lexema.LexTypeEnum.LEX_OP_MENOS;
						PrvGetChar();
						break;

					case '*':
						lexema.Type = Lexema.LexTypeEnum.LEX_OP_POR;
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
								lexema.Line = line;
								lexema.Column = column;
								break;

							case '/':
								while (letra != '\n' && letra != '\0')
									PrvGetChar();
							
								repeat = true;
								lexema.Line = line;
								lexema.Column = column;
								break;

							default:
								lexema.Type = Lexema.LexTypeEnum.LEX_OP_DIV;
								break;
						}
						break;

					case ',':
						lexema.Type = Lexema.LexTypeEnum.LEX_COMA;
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

						lexema.Type = Lexema.LexTypeEnum.LEX_STRING;
						lexema.String = new string(str, 0, i);
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
					
							lexema.Float = f;
							lexema.Type = Lexema.LexTypeEnum.LEX_FLOAT;
						}
						else
						{
							lexema.Integer = IntegerPart;
							lexema.Type = Lexema.LexTypeEnum.LEX_INTEGER;
						}
						break;	
					}
					
						
					case '\0':
						lexema.Type = Lexema.LexTypeEnum.LEX_EOF;
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

							string strVerdadero = new string(str, 0, i);

							string[] palabrasReservadas = 
							{
								"if",		((int) Lexema.LexTypeEnum.LEX_IF).ToString(), 
								"else",		((int) Lexema.LexTypeEnum.LEX_ELSE).ToString(), 
								"class",	((int) Lexema.LexTypeEnum.LEX_CLASS).ToString(), 
								"return",	((int) Lexema.LexTypeEnum.LEX_RETURN).ToString(), 
								"new",		((int) Lexema.LexTypeEnum.LEX_NEW).ToString(), 
								"for",		((int) Lexema.LexTypeEnum.LEX_FOR).ToString(), 
								"while",	((int) Lexema.LexTypeEnum.LEX_WHILE).ToString(),
								"try",		((int) Lexema.LexTypeEnum.LEX_TRY).ToString(),
								"catch",	((int) Lexema.LexTypeEnum.LEX_CATCH).ToString(),
								"finally",	((int) Lexema.LexTypeEnum.LEX_FINALLY).ToString(),
								"throw",	((int) Lexema.LexTypeEnum.LEX_THROW).ToString(),
								null, null 
							};

							lexema.String = strVerdadero;

							int n = 0;
							bool bReservado = false;

							while (palabrasReservadas[n] != null)
							{
								if (strVerdadero == palabrasReservadas[n])
								{
									lexema.Type = (Lexema.LexTypeEnum) int.Parse(palabrasReservadas[n + 1]);
									bReservado = true;
									break;
								}
								n += 2;
							}

							if (bReservado == false)
							{
								lexema.Type = Lexema.LexTypeEnum.LEX_IDENTIFICADOR;
								lexema.String = strVerdadero;
							}
					
						}
						break;
				}
			}
			return(ok);
		}
	}
}
