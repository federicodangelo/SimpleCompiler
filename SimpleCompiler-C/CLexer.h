#pragma once
#include "CLexema.h"

class CLexer
{
private:
	enum { MAX_STRING_LEN = 128 };
	enum { MAX_IDENTIFIER_LEN = 128 };

private:
	char*	m_pcText;
	char*	m_pcOriginalPosition;
	long	m_Line;
	long	m_Column;
	bool	m_bEnd;
	bool	m_bFirstLexema;

	char	m_Char;

private:
	void PrvGetChar(void);

public:
	CLexer();
	~CLexer();

	void Clear(void);

	void SetTextToParse(char* pcText);

	bool GetLexema(CLexema* pLexema);

	//void ReturnLastLexema(void);

	long GetLine(void) { return(m_Line); }

	bool IsEof(void);
};
