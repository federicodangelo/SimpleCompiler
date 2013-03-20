#pragma once

#include "CSimboloSimple.h"

class CTablaSimbolosSimple
{
public:
	enum { NO_ENCONTRADO = -1 };

private:
	CSimboloSimple**	m_ppListaSimbolos;
	int					m_Simbolos;

public:
	CTablaSimbolosSimple();
	~CTablaSimbolosSimple();

	void Clear(void);

	int AddSimbolo(CSimboloSimple* pSimbolo);
	int GetSimbolos(void) { return(m_Simbolos); }
	CSimboloSimple* GetSimbolo(int n) { /*No hace falta ningun chequeo por la validación que hago en CDefinicionObjeto::PrvValidarFuncion*/ return(m_ppListaSimbolos[n]); }

	int FindSimboloInt(long value);
	int FindSimboloFloat(float value);
	int FindSimboloString(const char* value) { CCadenaConHash val(value); return(FindSimboloString(val)); }
	int FindSimboloString(CCadenaConHash& value);
	

	void Imprimir(int n = 0);
};
