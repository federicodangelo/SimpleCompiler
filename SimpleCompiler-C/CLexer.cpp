#include "CLexer.h"
#include "FuncionesUnix.h"

CLexer::CLexer()
{
	m_Line = 0;
	m_Column = 0;
	m_pcText = m_pcOriginalPosition = NULL;
	m_bEnd = false;
	m_bFirstLexema = true;

	/*m_pcLastText = m_pcText;
	m_LastLine = m_Line;
	m_bLastEnd = m_bEnd;*/
}

CLexer::~CLexer()
{
	Clear();
}

void CLexer::Clear(void)
{
	m_Line = 0;
	m_Column = 0;
	m_bEnd = false;
	m_bFirstLexema = true;
	
	if (m_pcOriginalPosition)
	{
		free(m_pcOriginalPosition);
		m_pcOriginalPosition = NULL;
		m_pcText = NULL;
	}
}

void CLexer::SetTextToParse(char* pcText)
{
	Clear();

	m_pcOriginalPosition = (char*) malloc(strlen(pcText) + 1);

	strcpy(m_pcOriginalPosition, pcText);

	m_pcText = m_pcOriginalPosition;

	m_bFirstLexema = true;
}

bool CLexer::GetLexema(CLexema* pLexema)
{
	bool bOk = true, bRepeat = true;

	pLexema->SetLine(m_Line);
	pLexema->SetColumn(m_Column);

	if (m_bFirstLexema)
	{
		PrvGetChar();
		m_bFirstLexema = false;
	}

	while(bRepeat == true)
	{
		bRepeat = false;

		switch(m_Char)
		{
			case '\t':
				m_Column += 3;
			case '\r':
			case '\n':
			case ' ':
				bRepeat = true;
				pLexema->SetLine(m_Line);
				pLexema->SetColumn(m_Column);
				PrvGetChar();
				break;

			case ';':
				pLexema->SetType(CLexema::LEX_END);
				PrvGetChar();
				break;

			case '.':
				pLexema->SetType(CLexema::LEX_OP_MIEMBRO);
				PrvGetChar();
				break;

			case '=':
				PrvGetChar();

				if (m_Char == '=')
				{
					PrvGetChar();
					pLexema->SetType(CLexema::LEX_OP_IGUAL);
				}
				else
					pLexema->SetType(CLexema::LEX_OP_ASIGNACION);
				break;

			case '<':
				PrvGetChar();

				if (m_Char == '>')
				{
					PrvGetChar();
					pLexema->SetType(CLexema::LEX_OP_DISTINTO);
				}
				else
					pLexema->SetType(CLexema::LEX_OP_MENOR);
				break;

			case '>':
				PrvGetChar();
				pLexema->SetType(CLexema::LEX_OP_MAYOR);
				break;

			case '(':
				pLexema->SetType(CLexema::LEX_PAR_ABRE);
				PrvGetChar();
				break;

			case ')':
				pLexema->SetType(CLexema::LEX_PAR_CIERRA);
				PrvGetChar();
				break;

			case '{':
				pLexema->SetType(CLexema::LEX_LLAVE_ABRE);
				PrvGetChar();
				break;

			case '}':
				pLexema->SetType(CLexema::LEX_LLAVE_CIERRA);
				PrvGetChar();
				break;

			case '+':
				pLexema->SetType(CLexema::LEX_OP_MAS);
				PrvGetChar();
				break;

			case '-':
				pLexema->SetType(CLexema::LEX_OP_MENOS);
				PrvGetChar();
				break;

			case '*':
				pLexema->SetType(CLexema::LEX_OP_POR);
				PrvGetChar();
				break;

			case '/':
				PrvGetChar();
				switch(m_Char)
				{
					case '*':
						while (m_Char != '\0')
						{
							PrvGetChar();
							if (m_Char == '*')
							{
								PrvGetChar();
								if (m_Char == '/')
								{
									PrvGetChar();
									break;
								}
							}
						}
						
						bRepeat = true;
						pLexema->SetLine(m_Line);
						pLexema->SetColumn(m_Column);
						break;

					case '/':
						while (m_Char != '\n' && m_Char != '\0')
							PrvGetChar();
						
						bRepeat = true;
						pLexema->SetLine(m_Line);
						pLexema->SetColumn(m_Column);
						break;

					default:
						pLexema->SetType(CLexema::LEX_OP_DIV);
						break;
				}
				break;

			case ',':
				pLexema->SetType(CLexema::LEX_COMA);
				PrvGetChar();
				break;

			case '"':
			{
				char str[MAX_STRING_LEN];
				int i = 0;

				PrvGetChar();

				while (m_Char != '\0' && m_Char != '"')
				{
					if (m_Char == '\\')
					{
						PrvGetChar();

						switch(m_Char)
						{
							case 't':
								m_Char = '\t';
								break;
							case 'n':
								m_Char = '\n';
								break;
							case '\\':
								m_Char = '\\';
								break;
							default:
								break;
						}
					}

					if (i < MAX_STRING_LEN - 1)
						str[i++] = m_Char;

					PrvGetChar();
				}

				if (m_Char == '"')
					PrvGetChar();

				str[i] = '\0';

				pLexema->SetType(CLexema::LEX_STRING);
				pLexema->SetString(str);

				break;
			}

			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				bool bIsFloat = false;
				long IntegerPart = 0;
				long FloatPart = 0;
				int FloatDigits = 0;
				float f;

				IntegerPart = m_Char - '0';

				PrvGetChar();

				while (m_Char >= '0' && m_Char <= '9')
				{
					IntegerPart = IntegerPart * 10 + (m_Char - '0');
					PrvGetChar();
				}
				
				if (m_Char == '.')
				{
					bIsFloat = true;

					PrvGetChar();
					
					while (m_Char >= '0' && m_Char <= '9')
					{
						FloatPart = FloatPart * 10 + (m_Char - '0');
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
					
					pLexema->SetFloat(f);
					pLexema->SetType(CLexema::LEX_FLOAT);
				}
				else
				{
					pLexema->SetInteger(IntegerPart);
					pLexema->SetType(CLexema::LEX_INTEGER);
				}
				
				break;
			}

			default:
				if (m_Char >= 'a' && m_Char <= 'z' ||
					m_Char >= 'A' && m_Char <= 'Z' ||
					m_Char == '_')
				{
					char str[MAX_IDENTIFIER_LEN];
					int i = 1;

					str[0] = m_Char;

					PrvGetChar();

					while(	m_Char >= 'a' && m_Char <= 'z' ||
							m_Char >= 'A' && m_Char <= 'Z' ||
							m_Char >= '0' && m_Char <= '9' ||
							m_Char == '_')
					{
						if (i < MAX_IDENTIFIER_LEN - 1)
							str[i++] = m_Char;
						PrvGetChar();
					}
							
					str[i] = '\0';

					char *pcPalabrasReservadas[] = {	"if",		(char*) CLexema::LEX_IF, 
														"else",		(char*) CLexema::LEX_ELSE, 
														"class",	(char*) CLexema::LEX_CLASS, 
														"return",	(char*) CLexema::LEX_RETURN, 
														"new",		(char*) CLexema::LEX_NEW, 
														"for",		(char*) CLexema::LEX_FOR, 
														"while",	(char*) CLexema::LEX_WHILE,
														"try",		(char*) CLexema::LEX_TRY,
														"catch",	(char*) CLexema::LEX_CATCH,
														"finally",	(char*) CLexema::LEX_FINALLY,
														"throw",	(char*) CLexema::LEX_THROW,
														NULL, NULL };

					int n = 0;
					bool bReservado = false;

					while (pcPalabrasReservadas[n] != NULL)
					{
						if (strcmp(str, pcPalabrasReservadas[n]) == 0)
						{
							pLexema->SetType((CLexema::LexTypeEnum) (int) pcPalabrasReservadas[n + 1]);
							bReservado = true;
							break;
						}
						n += 2;
					}

					if (bReservado == false)
					{
						pLexema->SetType(CLexema::LEX_IDENTIFICADOR);
						pLexema->SetString(str);
					}
					
				}
				break;

			case '\0':
				pLexema->SetType(CLexema::LEX_EOF);
				break;
		}
	}

	return(bOk);
}

bool CLexer::IsEof(void)
{
	return(m_Char == '\0');
}

void CLexer::PrvGetChar(void)
{
	m_Char = *m_pcText;

	if (m_Char != '\0')
		m_pcText++;
	else
		m_bEnd = true;

	if (m_Char == '\n')
	{
		m_Line++;
		m_Column = 0;
	}
	else
	{
		m_Column++;
	}
}
