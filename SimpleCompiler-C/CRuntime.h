#pragma once
#include "CDefinicionFunciones.h"
#include "CVariables.h"

class CRuntime
{
private:
	CDefinicionFunciones* m_pDefinicionFunciones;
	CVariables m_Variables;
	
private:
	void PrvConstruirVariables(void);

public:
	CRuntime();
	~CRuntime();

	void Limpiar();

	void Inicializar(CDefinicionFunciones* pDefinicionFunciones);

	CVariables* ObtenerVariables(void) { return(&m_Variables); }
	void SetearVariables(CVariables& Variables) { m_Variables = Variables; }

	CDefinicionFunciones* ObtenerDefinicionFunciones(void) { return(m_pDefinicionFunciones); }
};
