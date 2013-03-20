#pragma once
#include "CListaInstrucciones.h"
#include "CDefinicionVariable.h"
#include "CCadena.h"
#include "FuncionesUnix.h"

class CObjeto;
class CProceso;
class CStack;

class CFuncion
{
public:
	enum TipoFuncionEnum { FUNCION_NORMAL, FUNCION_NATIVA };
	
	typedef bool FuncionNativa(CProceso* pProceso, CStack* pStack, CCadenaConHash& mensajeError);

private:
	CCadenaConHash			m_Nombre;
	CCadenaConHash			m_NombreCorto;
	CListaInstrucciones		m_ListaInstrucciones;
	CTipoDato				m_TipoDevuelto;
	CDefinicionVariable**	m_ppParametros;
	int						m_Parametros;
	int						m_ManejadoresExcepciones;
	int						m_CantVariables;
	TipoFuncionEnum			m_TipoFuncion;
	FuncionNativa*			m_pFuncionNativa;

public:
	CFuncion();
	~CFuncion();

	CFuncion(CFuncion& Funcion);
	CFuncion& operator=(CFuncion& Funcion);

	void Limpiar(void);

	void ConfigurarDesdeDefinicion(const char* pcDefinicion);
	void ActualizarNombreCompleto();

	void SetearNombre(const char* pcNombre) { m_Nombre.SetearCadena(pcNombre); }
	CCadenaConHash& ObtenerNombre(void) { return(m_Nombre); }
	CCadenaConHash& ObtenerNombreCorto(void) { return(m_NombreCorto); }

	void SetCantVariables(int n) { m_CantVariables = n; }
	int GetCantVariables(void) { return(m_CantVariables); }

	CListaInstrucciones* ObtenerListaInstrucciones(void) { return(&m_ListaInstrucciones); }
	void SetearListaInstrucciones(CListaInstrucciones& ListaInstrucciones) { m_ListaInstrucciones = ListaInstrucciones; }

	void SetearTipoFuncion(TipoFuncionEnum Tipo) { m_TipoFuncion = Tipo; }
	TipoFuncionEnum ObtenerTipoFuncion(void) { return(m_TipoFuncion); }

	CTipoDato* TipoDevuelto(void) { return(&m_TipoDevuelto) ;}

	void AddParametro(CDefinicionVariable* pParametro);
	int	GetParametros(void) { return(m_Parametros); }
	CDefinicionVariable* GetParametro(int n) { return(m_ppParametros[n]); }

	void SetearFuncionNativa(FuncionNativa* pFuncionNativa) { m_pFuncionNativa = pFuncionNativa; }
	FuncionNativa* ObtenerFuncioNativa(void) { return(m_pFuncionNativa); }

	void Imprimir(int n);
};
