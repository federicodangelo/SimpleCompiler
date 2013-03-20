#pragma once
#include "CCadena.h"

class CTablaVariables
{
public:
	enum UbicacionVariableEnum
	{
		UBICACION_GLOBAL,
		UBICACION_LOCAL			
	};

private:
	class CDatoVariable
	{
	public:
		CCadena					Nombre;
		UbicacionVariableEnum	Ubicacion;
		int						Posicion;
	};

private:
	CDatoVariable** m_ppDatoVariable;
	int	m_DatosVariable;
	int m_PosLocal;
	int m_PosGlobal;

private:
	void PrvAgregarVariable(CDatoVariable* pDatoVariable);

public:
	CTablaVariables();
	~CTablaVariables();

	CTablaVariables(CTablaVariables& TablaVariables);
	CTablaVariables& operator=(CTablaVariables& TablaVariables);

	void Limpiar(void);

	void AgregarVariableGlobal(const char* pcNombre);
	void AgregarVariableLocal(const char* pcNombre);
	void EliminarVariable(void);

	int BuscarVariable(const char* pcNombre);

	UbicacionVariableEnum UbicacionVariable(int n) { return(m_ppDatoVariable[n]->Ubicacion); }
	int PosicionVariable(int n) { return(m_ppDatoVariable[n]->Posicion); }
	const char* NombreVariable(int n) { return(m_ppDatoVariable[n]->Nombre.ObtenerCadena()); }

	int CantidadVariablesLocales(void) { return(m_PosLocal); }
	int CantidadVariablesGlobales(void) { return(m_PosGlobal); }
};
