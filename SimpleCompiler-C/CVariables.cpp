#include "CVariables.h"
#include "CErrorEjecucion.h"
#include "FuncionesUnix.h"

CVariables::CVariables()
{
	m_pVariables = NULL;
	m_Variables = 0;
}

CVariables::~CVariables()
{
	delete[] m_pVariables;
}

void CVariables::Limpiar(void)
{
	if (m_pVariables)
	{
		delete[] m_pVariables;
		m_Variables = 0;
		m_pVariables = NULL;
	}
}

void CVariables::Crear(unsigned int n)
{
	Limpiar();

	m_pVariables = new CVariable[n];

	m_Variables = n;
}
