#pragma once
#include "CCadena.h"
#include "CFuncion.h"
#include "CDefinicionVariable.h"
#include "CTablaSimbolosSimple.h"

class CDefinicionFunciones
{
public:
	enum { NO_ENCONTRADO = -1 };

private:
	CTablaSimbolosSimple m_TablaConstantes;
	CDefinicionVariable** m_ppVariables;
	int					m_Variables;
	CFuncion**			m_ppFunciones;
	int					m_Funciones;
private:
	bool PrvValidarFuncion(CFuncion* pFuncion);

public:
	CDefinicionFunciones();
	~CDefinicionFunciones();

	CDefinicionFunciones(CDefinicionFunciones& DefinicionFunciones);
	CDefinicionFunciones& operator=(CDefinicionFunciones& DefinicionFunciones);

	void Limpiar(void);

	CTablaSimbolosSimple* GetTablaConstantes(void) { return(&m_TablaConstantes); }
	void SetTablaConstantes(CTablaSimbolosSimple& TablaConstantes) { m_TablaConstantes = TablaConstantes; }

	int ObtenerVariables(void) { return(m_Variables); }
	CDefinicionVariable* ObtenerVariable(int n) { return(m_ppVariables[n]); }
	void AgregarVariable(CDefinicionVariable* pVariable);

	int ObtenerFunciones(void) { return(m_Funciones); }
	CFuncion* ObtenerFuncion(int n) { return(m_ppFunciones[n]); }
	void AgregarFuncion(CFuncion* pFuncion);

	int BuscarPosicionFuncion(const char* pcNombre) { CCadenaConHash val(pcNombre); return(BuscarPosicionFuncion(val)); }
	int BuscarPosicionFuncion(CCadenaConHash& Nombre);
	int BuscarPosicionVariable(const char* pcNombre) { CCadenaConHash val(pcNombre); return(BuscarPosicionVariable(val)); }
	int BuscarPosicionVariable(CCadenaConHash& Nombre);

	CFuncion* BuscarFuncion(const char* pcNombre) { CCadenaConHash val(pcNombre); return(BuscarFuncion(val)); }
	CFuncion* BuscarFuncion(CCadenaConHash& Nombre);

	bool Validar(void);

	void Imprimir(int n);
};
