#include "CRuntime.h"

CRuntime::CRuntime()
{
}

CRuntime::~CRuntime()
{
}

void CRuntime::Limpiar()
{
	m_Variables.Limpiar();
	m_pDefinicionFunciones = NULL;
}

void CRuntime::Inicializar(CDefinicionFunciones* pDefinicionFunciones)
{
	Limpiar();

	m_pDefinicionFunciones = pDefinicionFunciones;
	PrvConstruirVariables();
}

void CRuntime::PrvConstruirVariables(void)
{
	int n = m_pDefinicionFunciones->ObtenerVariables();
	
	m_Variables.Crear(n);

	for (int i = 0; i < n; i++)
		*m_Variables.Variable(i)->TipoDato() = *m_pDefinicionFunciones->ObtenerVariable(i)->TipoDato();
}
