#pragma once

#include "CCadena.h"
#include "FuncionesUnix.h"

#include "CTablaSimbolos.h"

//#define DEBUG_TREE

class CSyntTree
{
public:
	enum SyntTypeEnum
	{
		SYNT_INSTRUCCION_NULA,
		SYNT_INSTRUCCION,
		SYNT_RETURN,
		SYNT_LISTA_FUNCIONES,
		SYNT_LISTA_INSTRUCCIONES,
		SYNT_DECLARACION_FUNCION,
		SYNT_DECLARACION_VARIABLE,
		SYNT_LLAMADO_FUNCION,
		SYNT_TABLA_SIMBOLOS,
		SYNT_VARIABLE,
		SYNT_IDENTIFICADOR,
		SYNT_CONSTANTE,
		SYNT_CONVERSION,
		SYNT_OP_COMPARACION_IGUAL,
		SYNT_OP_COMPARACION_DISTINTO,
		SYNT_OP_ASIGNACION,
		SYNT_OP_MAS,
		SYNT_OP_MENOS,
		SYNT_OP_POR,
		SYNT_OP_DIV,
		SYNT_IF,
		SYNT_FOR,
		SYNT_WHILE,
		SYNT_NONE
	};

private:
	SyntTypeEnum	m_Type;
	CSimbolo*		m_pReturnType;
	CSimbolo*		m_pSimboloDefinicionFuncion;
		
	int m_Childs;
#ifndef DEBUG_TREE
	CSyntTree** m_ppChilds;
#else
	CSyntTree*	m_ppChilds[24];
#endif

	long			m_Integer;
	float			m_Float;
	CCadena			m_String;
	CTablaSimbolos	*m_pTablaSimbolos;

public:
	CSyntTree();
	~CSyntTree();

	void Clear(void);

	void SetType(SyntTypeEnum t) { m_Type = t; }
	SyntTypeEnum GetType(void) { return(m_Type); }

	void SetReturnType(CSimbolo* pReturnType) { m_pReturnType = pReturnType; }
	CSimbolo* GetReturnType(void) { return(m_pReturnType); }

	void SetSimboloDefinicionFuncion(CSimbolo* pDefinicion) { m_pSimboloDefinicionFuncion = pDefinicion; }
	CSimbolo* GetSimboloDefinicionFuncion(void) { return(m_pSimboloDefinicionFuncion); }

	void AddChild(CSyntTree* pNode);

	int GetChilds(void) { return(m_Childs); }
	CSyntTree* GetChild(int n) { return(m_ppChilds[n]); }

	const char* GetString(void) { return(m_String.ObtenerCadena()); }
	void SetString(const char* pcString) { m_String.SetearCadena(pcString); }

	long GetInteger(void) { return(m_Integer); }
	void SetInteger(long i) { m_Integer = i; }

	float GetFloat(void) { return(m_Float); }
	void SetFloat(float f) { m_Float = f; }

	void SetTablaSimbolos(CTablaSimbolos* pTablaSimbolos) { m_pTablaSimbolos = pTablaSimbolos; }
	CTablaSimbolos* GetTablaSimbolos(void) { return(m_pTablaSimbolos); }

	void Imprimir(int n = 0);
};
