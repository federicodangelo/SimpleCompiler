#include "CTablaVariables.h"

CTablaVariables::CTablaVariables()
{
	m_ppDatoVariable = NULL;
	m_DatosVariable = 0;
	m_PosLocal = 0;
	m_PosGlobal = 0;
}

CTablaVariables::~CTablaVariables()
{
	Limpiar();
}

void CTablaVariables::Limpiar(void)
{
	if (m_ppDatoVariable)
	{
		while(m_DatosVariable != 0)
			delete m_ppDatoVariable[--m_DatosVariable];

		free(m_ppDatoVariable);
	}

	m_PosLocal = 0;
	m_PosGlobal = 0;
}

void CTablaVariables::PrvAgregarVariable(CTablaVariables::CDatoVariable* pDatoVariable)
{
	m_ppDatoVariable = (CDatoVariable**) realloc(m_ppDatoVariable, sizeof(CDatoVariable*) * (m_DatosVariable + 1));
	
	m_ppDatoVariable[m_DatosVariable] = pDatoVariable;
	
	m_DatosVariable++;
}

void CTablaVariables::AgregarVariableGlobal(const char* pcNombre)
{
	CDatoVariable *pDatoVariable = new CDatoVariable();

	pDatoVariable->Nombre.SetearCadena(pcNombre);
	pDatoVariable->Posicion = m_PosGlobal++;
	pDatoVariable->Ubicacion = UBICACION_GLOBAL;

	PrvAgregarVariable(pDatoVariable);
}

void CTablaVariables::AgregarVariableLocal(const char* pcNombre)
{
	CDatoVariable *pDatoVariable = new CDatoVariable();

	pDatoVariable->Nombre.SetearCadena(pcNombre);
	pDatoVariable->Posicion = m_PosLocal++;
	pDatoVariable->Ubicacion = UBICACION_LOCAL;

	PrvAgregarVariable(pDatoVariable);
}

void CTablaVariables::EliminarVariable(void)
{
	m_DatosVariable--;

	CDatoVariable *pDatoVariable = m_ppDatoVariable[m_DatosVariable];

	if (pDatoVariable->Ubicacion == UBICACION_LOCAL)
		m_PosLocal--;
	else
		m_PosGlobal--;

	delete pDatoVariable;

	m_ppDatoVariable = (CDatoVariable**) realloc(m_ppDatoVariable, sizeof(CDatoVariable*) * (m_DatosVariable));
}

int CTablaVariables::BuscarVariable(const char* pcNombre)
{
	for (int i = m_DatosVariable - 1; i >= 0; i--)
		if (strcmp(m_ppDatoVariable[i]->Nombre.ObtenerCadena(), pcNombre) == 0)
			return(i);

	return(-1);
}

