#pragma once
#include "FuncionesUnix.h"

class CCadena
{
private:
	char* m_pcCadena;

public:
	CCadena();
	~CCadena();

	operator const char*() { return(ObtenerCadena()); }

	CCadena(CCadena& Cadena);
	CCadena& operator=(CCadena& Cadena);

	void Limpiar(void) { if (m_pcCadena) { free(m_pcCadena); m_pcCadena = NULL; } }

	void SetearCadena(const char* pcCadena) { if (m_pcCadena) free(m_pcCadena); if (int len = strlen(pcCadena)) { m_pcCadena = (char*) malloc(len + 1); strcpy(m_pcCadena, pcCadena); } else m_pcCadena = NULL; }
	const char* ObtenerCadena(void) { if (m_pcCadena != NULL) return(m_pcCadena); return(""); }
	void ConcatenarCadena(const char* pcCadena) { if (m_pcCadena) { if (char* pcNew = (char*) realloc(m_pcCadena, strlen(m_pcCadena) + strlen(pcCadena) + 1)) { m_pcCadena = pcNew; strcat(m_pcCadena, pcCadena); } } else { SetearCadena(pcCadena); } }
};

class CCadenaConHash
{
private:
	char* m_pcCadena;
	short m_Hash;

private:
	void PrvCalcularHash(const char* pcCadena) { short h = 0; while(*pcCadena != '\0') { h += *pcCadena++; } m_Hash = h; }
	
	bool PrvCadenasIguales(const char* p1, const char* p2) 
	{ 
		while(*p1 && *p2 && *p1++ == *p2++) {} 
		return(*p1 - *p2 == 0); 
	}

public:
	CCadenaConHash();
	CCadenaConHash(const char* pcCadena);
	~CCadenaConHash();

	CCadenaConHash(CCadenaConHash& Cadena);
	CCadenaConHash& operator=(CCadenaConHash& Cadena);

	operator const char*() { return(ObtenerCadena()); }

	bool operator==(CCadenaConHash& cad) { return(m_Hash == cad.m_Hash && PrvCadenasIguales(ObtenerCadena(), cad.ObtenerCadena())); }

	void Limpiar(void) { if (m_pcCadena) { free(m_pcCadena); m_pcCadena = NULL; } }

	void SetearCadena(const char* pcCadena) { if (m_pcCadena) free(m_pcCadena); if (int len = strlen(pcCadena)) { m_pcCadena = (char*) malloc(len + 1); strcpy(m_pcCadena, pcCadena); } else m_pcCadena = NULL; PrvCalcularHash(pcCadena); }
	const char* ObtenerCadena(void) { if (m_pcCadena != NULL) return(m_pcCadena); return(""); }
	void ConcatenarCadena(const char* pcCadena) { if (m_pcCadena) { if (char* pcNew = (char*) realloc(m_pcCadena, strlen(m_pcCadena) + strlen(pcCadena) + 1)) { m_pcCadena = pcNew; strcat(m_pcCadena, pcCadena); } } else { SetearCadena(pcCadena); } PrvCalcularHash(m_pcCadena); }
};
