#pragma once
#include "CVariable.h"
#include "CErrorEjecucion.h"

class CObjeto;

class CVariables
{
private:
	CVariable* m_pVariables;
	unsigned int	m_Variables;

public:
	CVariables();
	~CVariables();

	CVariables(CVariables& Variables);
	CVariables& operator=(CVariables& Variables);

	void Limpiar(void);

	void Crear(unsigned int nVariables);

	inline CVariable* Variable(unsigned int n)
	{
		ThrowErrorEjecucionSi(n >= m_Variables, "Numero de variable invalido");
		return(&m_pVariables[n]);
	}
};
