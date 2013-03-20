#pragma once
#include "CSyntTree.h"
#include "CDefinicionFunciones.h"
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
	CDefinicionFunciones* m_pDefinicionFunciones;
	CListaInstrucciones* m_pListaInstrucciones;
	CFuncion*			m_pFuncion;
	
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

	bool GenerarCodigo(CSyntTree* pTree);

	CDefinicionFunciones* GetDefinicionFunciones(void) { return(m_pDefinicionFunciones); }
};
