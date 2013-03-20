#pragma once
#include "FuncionesUnix.h"
#include "CCadena.h"
#include "CTipoDato.h"

class CSimboloSimple
{
public:
	enum SimboloTypeEnum
	{
		SIMBOLO_TIPO_CONSTANTE_INT,
		SIMBOLO_TIPO_CONSTANTE_FLOAT,
		SIMBOLO_TIPO_CONSTANTE_STRING
	};

private:
	CCadena	m_Nombre;
	SimboloTypeEnum	m_Type;

	float	m_Float;
	long	m_Int;
	CCadenaConHash	m_String;

public:
	CSimboloSimple();

	const char* GetNombre(void) { return(m_Nombre.ObtenerCadena()); }
	void SetNombre(const char* pcNombre) { m_Nombre.SetearCadena(pcNombre); }

	void SetType(SimboloTypeEnum Type) { m_Type = Type; }
	SimboloTypeEnum GetType(void) { return(m_Type); }

	void SetInt(long value) { m_Int = value; }
	void SetFloat(float value) { m_Float = value; }
	void SetString(const char* pcValue) { m_String.SetearCadena(pcValue); }

	long GetInt(void) { return(m_Int); }
	float GetFloat(void) { return(m_Float); }
	CCadenaConHash& GetString(void) { return(m_String); }

	void Imprimir(int n);
};

