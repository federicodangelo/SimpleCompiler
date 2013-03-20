#pragma once
#include "CCadena.h"

class CErrorEjecucion
{
private:
	CCadena m_Descripcion;

public:
	CErrorEjecucion(const char* pcDescription) { m_Descripcion.SetearCadena(pcDescription); }

	const char* GetDescription(void) { return(m_Descripcion.ObtenerCadena()); }
};

#define ThrowErrorEjecucion(Description) throw CErrorEjecucion(Description)
#define ThrowErrorEjecucionSi(Condition, Description) if (Condition) ThrowErrorEjecucion(Description)

/*#define ThrowErrorEjecucion(a)
#define ThrowErrorEjecucionSi(a, b)*/