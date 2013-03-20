#pragma once
#include "CTipoDato.h"
#include "CCadena.h"

class CDefinicionVariable
{
private:
	CTipoDato m_Tipo;
	CCadenaConHash	m_Nombre;

public:
	CDefinicionVariable();
	~CDefinicionVariable();

	CTipoDato* TipoDato(void) { return(&m_Tipo); }

	void SetearNombre(const char* pcNombre) { m_Nombre.SetearCadena(pcNombre); }
	CCadenaConHash& ObtenerNombre(void) { return(m_Nombre); }

	void Imprimir(int n);
};
