#include "CLexema.h"

CLexema::CLexema()
{
	m_Integer = 0;
	m_Float = 0.0f;
	m_pcString = NULL;
	m_Type = LEX_NONE;
}

CLexema::~CLexema()
{
	if (m_pcString)
		free(m_pcString);
}

CLexema::CLexema(CLexema &Lexema)
{
	m_pcString = NULL;
	*this = Lexema;
}

CLexema& CLexema::operator=(CLexema &Lexema)
{
	if (&Lexema != this)
	{
		SetType(Lexema.GetType());
		SetString(Lexema.GetString());
		SetInteger(Lexema.GetInteger());
		SetLine(Lexema.GetLine());
		SetFloat(Lexema.GetFloat());
	}

	return(*this);
}
