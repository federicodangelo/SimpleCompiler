#pragma once
#include "CSyntTree.h"
#include "CListaDefinicionesObjetos.h"
#include "CTablaVariables.h"

class CGeneradorCodigoError
{
private:
	CCadena	m_Description;

public:
	CGeneradorCodigoError(const char* pcDescription) { m_Description.SetearCadena(pcDescription); }

	const char* GetDescription(void) { return(m_Description.ObtenerCadena()); }
};

#define ThrowGeneradorCodigoError(Description) throw CGeneradorCodigoError(Description)
#define ThrowGeneradorCodigoErrorIf(Condition, Description) if (Condition) ThrowGeneradorCodigoError(Description)

typedef FuncionNativa* BuscarFuncionNativaFunc(const char* pcNombreFuncion);

class CGeneradorCodigo
{
private:
	CTablaVariables			m_TablaVariables;
	CListaDefinicionesObjetos m_ListaDefiniciones;
	CDefinicionObjeto* m_pDefinicionObjeto;
	CListaInstrucciones* m_pListaInstrucciones;
	CFuncion*			m_pFuncion;
	BuscarFuncionNativaFunc* m_pBuscarFuncionNativa;
	
	int m_PosInstComienzoTry;
	int	m_PosInstFinTry;

private:
	void PrvAgregarInstruccion(CSyntTree* pNodo);

	int PrvAgregarConstante(long valor);
	int PrvAgregarConstante(float valor);
	int PrvAgregarConstante(const char* valor);

	int PrvAgregarSimbolo(CSimbolo* pSimbolo);
	
	CTipoDato PrvTraducirSimboloATipoDato(CSimbolo* pSimbolo);

public:

	CGeneradorCodigo();
	~CGeneradorCodigo();

	bool GenerarCodigo(CSyntTree* pTree, BuscarFuncionNativaFunc *pBuscarFuncionNativa);

	CListaDefinicionesObjetos* GetListaDefiniciones(void) { return(&m_ListaDefiniciones); }
};
