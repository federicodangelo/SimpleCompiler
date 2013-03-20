#pragma once
#include "CCadena.h"
#include "FuncionesUnix.h"

#define SIMBOLO_INT	"int"
#define SIMBOLO_FLOAT "float"
#define SIMBOLO_STRING "string"
#define SIMBOLO_VOID "void"

class CProceso;
class CStack;

typedef int FuncionNativa(CProceso* pProceso, CStack* pStack);

class CTablaSimbolos;

class CSimbolo
{
public:
	enum SimboloTypeEnum
	{
		SIMBOLO_TIPO_DATO,
		SIMBOLO_TIPO_VARIABLE,
		SIMBOLO_TIPO_FUNCION
	};

	enum { SIMBOLO_SEPARADOR_FUNCION = '#',
		SIMBOLO_SEPARADOR_PARAMETRO = ',' };

private:
	CCadena		m_Nombre;
	CCadena		m_NombreFuncionCompleto;
	SimboloTypeEnum m_Type;
	CSimbolo*	m_pReturnType;
	CSimbolo**	m_ppListaParametros;
	int			m_Parametros;
	CTablaSimbolos* m_pTablaSimbolos;
	int			m_CantVariables;
	bool		m_bFuncionNativa;

public:
	CSimbolo();
	~CSimbolo();

	CSimbolo(CSimbolo& Simbolo);
	CSimbolo& operator=(CSimbolo& Simbolo);

	void Clear(void);

	const char* GetNombre(void) { return(m_Nombre.ObtenerCadena()); }
	void SetNombre(const char* pcNombre) { m_Nombre.SetearCadena(pcNombre); }

	const char* GetNombreFuncionCompleto(void) { return(m_NombreFuncionCompleto.ObtenerCadena()); }
	void SetNombreFuncionCompleto(const char* pcNombre) { m_NombreFuncionCompleto.SetearCadena(pcNombre); }

	void SetType(SimboloTypeEnum Type) { m_Type = Type; }
	SimboloTypeEnum GetType(void) { return(m_Type); }

	void SetReturnType(CSimbolo* pReturnType) { m_pReturnType = pReturnType; }
	CSimbolo* GetReturnType(void) { return(m_pReturnType); }

	void SetCantVariables(int n) { m_CantVariables = n; }
	int GetCantVariables(void) { return(m_CantVariables); }

	void AddParametro(CSimbolo* pParametro);
	int	GetParametros(void) { return(m_Parametros); }
	CSimbolo* GetParametro(int n) { return(m_ppListaParametros[n]); }

	CTablaSimbolos* GetTablaSimbolos(void) { return(m_pTablaSimbolos); }
	void SetTablaSimbolos(CTablaSimbolos* pTabla) { m_pTablaSimbolos = pTabla; }

	void SetFuncionNativa(bool bFuncionNativa) { m_bFuncionNativa = bFuncionNativa; }
	bool GetFuncionNativa(void) { return(m_bFuncionNativa); }

	void ActualizarNombreCompletoFuncion(void);

	void Imprimir(int n);
};
