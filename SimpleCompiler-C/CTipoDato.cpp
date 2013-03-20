#include "CTipoDato.h"
#include "CSimbolo.h"
#include "FuncionesUnix.h"
#include "CProceso.h"

CTipoDato::CTipoDato()
{
	m_Tipo = TIPO_NINGUNO;
}

CTipoDato::~CTipoDato()
{
}

CTipoDato::CTipoDato(TipoDatoEnum Tipo)
{
    m_Tipo = Tipo;
}

CTipoDato::CTipoDato(const char* pcTipo)
{
	m_Tipo = TraducirTipoDato(pcTipo);
}

const char* CTipoDato::ToString()
{
	const char* pcTemp;

	switch(m_Tipo)
	{
		case TIPO_VOID:
			pcTemp =  SIMBOLO_VOID;
			break;
		case TIPO_INT:
			pcTemp =  SIMBOLO_INT;
			break;
		case TIPO_FLOAT:
			pcTemp =  SIMBOLO_FLOAT;
			break;
		case TIPO_CADENA:
			pcTemp =  SIMBOLO_STRING;
			break;
		default:
			pcTemp = "???";
			break;
	}

	return pcTemp;
}

CTipoDato::TipoDatoEnum CTipoDato::TraducirTipoDato(const char* pcTipo)
{
	if (strcmp(pcTipo, SIMBOLO_INT) == 0)
		return(TIPO_INT);
	else if (strcmp(pcTipo, SIMBOLO_FLOAT) == 0)
		return(TIPO_FLOAT);
	else if (strcmp(pcTipo, SIMBOLO_STRING) == 0)
		return(TIPO_CADENA);
	else if (strcmp(pcTipo, SIMBOLO_VOID) == 0)
		return(TIPO_VOID);
	
	ThrowErrorEjecucion("Tipo de dato no soportado");
}