#pragma once

#include "CSimbolo.h"

class CTablaSimbolos
{
private:
	CTablaSimbolos* m_pTablaPadre;
	CSimbolo**		m_ppListaSimbolos;
	int				m_Simbolos;

public:
	CTablaSimbolos();
	~CTablaSimbolos();

	void Clear(void);

	void SetTablaPadre(CTablaSimbolos* pTablaPadre) { m_pTablaPadre = pTablaPadre; }
	CTablaSimbolos* GetTablaPadre(void) { return(m_pTablaPadre); }

	void AddSimbolo(CSimbolo* pSimbolo);
	int GetSimbolos(void) { return(m_Simbolos); }
	CSimbolo* GetSimbolo(int n) { return(m_ppListaSimbolos[n]); }

	CSimbolo* FindSimboloGlobal(const char* pcNombre);
	CSimbolo* FindProximoSimboloGlobal(CSimbolo* pSimbolo);

	CSimbolo* FindSimboloLocal(const char* pcNombre);
	CSimbolo* FindProximoSimboloLocal(CSimbolo* pSimbolo);

	void Imprimir(int n = 0);
};
