#include "CCadena.h"

CCadena::CCadena()
{
	m_pcCadena = NULL;
}

CCadena::~CCadena()
{
	if (m_pcCadena)
		free(m_pcCadena);
}

CCadena::CCadena(CCadena& Cadena)
{
	m_pcCadena = NULL;
	SetearCadena(Cadena.ObtenerCadena());
}

CCadena& CCadena::operator=(CCadena& Cadena)
{
	if (this != &Cadena)
		SetearCadena(Cadena.ObtenerCadena());

	return(*this);
}

CCadenaConHash::CCadenaConHash()
{
	m_pcCadena = NULL;
	m_Hash = 0;
}

CCadenaConHash::CCadenaConHash(const char* pcCadena)
{
	m_pcCadena = NULL;
	SetearCadena(pcCadena);
}

CCadenaConHash::~CCadenaConHash()
{
	if (m_pcCadena)
		free(m_pcCadena);
}

CCadenaConHash::CCadenaConHash(CCadenaConHash& Cadena)
{
	m_pcCadena = NULL;
	SetearCadena(Cadena.ObtenerCadena());
}

CCadenaConHash& CCadenaConHash::operator=(CCadenaConHash& Cadena)
{
	if (this != &Cadena)
		SetearCadena(Cadena.ObtenerCadena());

	return(*this);
}
