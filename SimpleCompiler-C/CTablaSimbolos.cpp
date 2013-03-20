#include "CTablaSimbolos.h"
#include "FuncionesUnix.h"

CTablaSimbolos::CTablaSimbolos()
{
	m_ppListaSimbolos = NULL;
	m_pTablaPadre = NULL;
	m_Simbolos = 0;
}

CTablaSimbolos::~CTablaSimbolos()
{
	Clear();
}

void CTablaSimbolos::Clear(void)
{
	if (m_ppListaSimbolos)
	{
		while (m_Simbolos != 0)
			delete m_ppListaSimbolos[--m_Simbolos];

		free(m_ppListaSimbolos);

		m_ppListaSimbolos = NULL;
	}
}

void CTablaSimbolos::AddSimbolo(CSimbolo* pSimbolo)
{
	m_ppListaSimbolos = (CSimbolo**) realloc(m_ppListaSimbolos, sizeof(CSimbolo*) * (m_Simbolos + 1));
	
	m_ppListaSimbolos[m_Simbolos] = pSimbolo;

	m_Simbolos++;
}

CSimbolo* CTablaSimbolos::FindSimboloGlobal(const char* pcNombre)
{
	CSimbolo *pSimbolo = NULL;

	for (int i = 0; i < m_Simbolos; i++)
	{
		if (strcmp(m_ppListaSimbolos[i]->GetNombre(), pcNombre) == 0)
		{
			pSimbolo = m_ppListaSimbolos[i];
			break;
		}
	}

	if (pSimbolo == NULL && m_pTablaPadre != NULL)
		pSimbolo = m_pTablaPadre->FindSimboloGlobal(pcNombre);

	return(pSimbolo);
}

CSimbolo* CTablaSimbolos::FindProximoSimboloGlobal(CSimbolo* pSimbolo)
{
	bool bCompare = false;
	CSimbolo* pProximoSimbolo = NULL;

	for (int i = 0; i < m_Simbolos; i++)
	{		
		if (bCompare)
		{
			if (strcmp(m_ppListaSimbolos[i]->GetNombre(), pSimbolo->GetNombre()) == 0)
			{
				pProximoSimbolo = m_ppListaSimbolos[i];
				break;
			}
		}

		if (m_ppListaSimbolos[i] == pSimbolo)
			bCompare = true;
	}

	if (pProximoSimbolo == NULL && m_pTablaPadre != NULL)
		pProximoSimbolo = m_pTablaPadre->FindProximoSimboloGlobal(pSimbolo);

	return(pProximoSimbolo);
}

CSimbolo* CTablaSimbolos::FindSimboloLocal(const char* pcNombre)
{
	CSimbolo *pSimbolo = NULL;

	for (int i = 0; i < m_Simbolos; i++)
	{
		if (strcmp(m_ppListaSimbolos[i]->GetNombre(), pcNombre) == 0)
		{
			pSimbolo = m_ppListaSimbolos[i];
			break;
		}
	}

	return(pSimbolo);
}

CSimbolo* CTablaSimbolos::FindProximoSimboloLocal(CSimbolo* pSimbolo)
{
	bool bCompare = false;
	CSimbolo* pProximoSimbolo = NULL;

	for (int i = 0; i < m_Simbolos; i++)
	{		
		if (bCompare)
		{
			if (strcmp(m_ppListaSimbolos[i]->GetNombre(), pSimbolo->GetNombre()) == 0)
			{
				pProximoSimbolo = m_ppListaSimbolos[i];
				break;
			}
		}

		if (m_ppListaSimbolos[i] == pSimbolo)
			bCompare = true;
	}

	return(pProximoSimbolo);
}

void CTablaSimbolos::Imprimir(int n)
{
	for (int i = 0; i < m_Simbolos; i++)
		m_ppListaSimbolos[i]->Imprimir(n);
}
