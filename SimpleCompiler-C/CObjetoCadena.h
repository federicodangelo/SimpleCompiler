#pragma once

#include "CCadena.h"

class CObjetoCadena
{
private:
	int	m_Uso; //Cantidad de referencias a este objeto vivas..
	CCadena m_Cadena;
	
public:
	CObjetoCadena(const char* pcValue) { m_Cadena.SetearCadena(pcValue); m_Uso = 0; }

	CCadena& ObtenerCadena(void) { return(m_Cadena); }

	void IncrementarUso(void) 
	{ 
		m_Uso++; 
	}

	void DecrementarUso(void) 
	{ 
		m_Uso--; 
		if (m_Uso == 0) 
			delete this; 
	}
};
