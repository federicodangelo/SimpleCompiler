#pragma once
#include "FuncionesUnix.h"

class CLexema
{
public:
	enum LexTypeEnum
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
	};

private:
	LexTypeEnum m_Type;
	long		m_Line;
	long		m_Column;
	
	long	m_Integer;
	float	m_Float;
	char	*m_pcString;

public:
	CLexema();
	~CLexema();

	CLexema(CLexema&);
	CLexema& operator=(CLexema& Lexema);

	void SetLine(long Line) { m_Line = Line; }
	long GetLine(void) { return(m_Line); }
	
	void SetColumn(long Column) { m_Column = Column; }
	long GetColumn(void) { return(m_Column); }

	void SetType(LexTypeEnum e) { m_Type = e; } 
	LexTypeEnum GetType(void) { return(m_Type); }

	char* GetString(void) { return(m_pcString); }
	void SetString(char* pcString) { if (m_pcString) free(m_pcString); m_pcString = (char*) malloc(strlen(pcString) + 1); strcpy(m_pcString, pcString); }

	long GetInteger(void) { return(m_Integer); }
	void SetInteger(long i) { m_Integer = i; }

	float GetFloat(void) { return(m_Float); }
	void SetFloat(float f) { m_Float = f; }
};
