#include "CTablaSimbolosSimple.h"
#include "FuncionesUnix.h"

CTablaSimbolosSimple::CTablaSimbolosSimple()
{
	m_ppListaSimbolos = NULL;
	m_Simbolos = 0;
}

CTablaSimbolosSimple::~CTablaSimbolosSimple()
{
	Clear();
}

void CTablaSimbolosSimple::Clear(void)
{
	if (m_ppListaSimbolos)
	{
		while (m_Simbolos != 0)
			delete m_ppListaSimbolos[--m_Simbolos];

		free(m_ppListaSimbolos);

		m_ppListaSimbolos = NULL;
	}
}

int CTablaSimbolosSimple::AddSimbolo(CSimboloSimple* pSimbolo)
{
	m_ppListaSimbolos = (CSimboloSimple**) realloc(m_ppListaSimbolos, sizeof(CSimboloSimple*) * (m_Simbolos + 1));
	
	m_ppListaSimbolos[m_Simbolos] = pSimbolo;

	return(m_Simbolos++);
}

int CTablaSimbolosSimple::FindSimboloInt(long value)
{
	int n = NO_ENCONTRADO;

	for (int i = 0; i < m_Simbolos; i++)
	{
		if (m_ppListaSimbolos[i]->GetType() == CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_INT &&
			m_ppListaSimbolos[i]->GetInt() == value)
		{
			n = i;
			break;
		}
	}

	return(n);
}

int CTablaSimbolosSimple::FindSimboloFloat(float value)
{
	int n = NO_ENCONTRADO;

	for (int i = 0; i < m_Simbolos; i++)
	{
		if (m_ppListaSimbolos[i]->GetType() == CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_FLOAT &&
			m_ppListaSimbolos[i]->GetFloat() == value)
		{
			n = i;
			break;
		}
	}

	return(n);

}

int CTablaSimbolosSimple::FindSimboloString(CCadenaConHash& value)
{
	int n = NO_ENCONTRADO;

	for (int i = 0; i < m_Simbolos; i++)
	{
		if (m_ppListaSimbolos[i]->GetType() == CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_STRING &&
			m_ppListaSimbolos[i]->GetString() == value)
		{
			n = i;
			break;
		}
	}

	return(n);
}


void CTablaSimbolosSimple::Imprimir(int n)
{
	for (int i = 0; i < m_Simbolos; i++)
	{
		printf("%d: ", i);
		m_ppListaSimbolos[i]->Imprimir(n);
	}
}
