#pragma once
#include "CLexema.h"
#include "CLexer.h"
#include "CSyntTree.h"
#include "CTablaSimbolos.h"
#include "CFuncion.h"

class CParserError
{
private:
	long	m_Line;
	long	m_Column;
	CCadena	m_Description;

public:
	CParserError(const char* pcDescription, long Line, long Column) { m_Description.SetearCadena(pcDescription); m_Line = Line; m_Column = Column; }

	long GetLine(void) { return(m_Line); }
	long GetColumn(void) { return(m_Column); }
	const char* GetDescription(void) { return(m_Description.ObtenerCadena()); }
};

#define ThrowParserError(Description) throw CParserError(Description, m_Lexema.GetLine(), m_Lexema.GetColumn())
#define ThrowParserErrorIf(Condition, Description) if (Condition) ThrowParserError(Description)

class CParser
{
private:
	enum { MAX_STACK_LEXEMAS = 12 };
private:
	CLexer m_Lexer;
	CLexema m_Lexema;

	CLexema m_pStackLexemas[MAX_STACK_LEXEMAS];
	int		m_StackLexemas;

	CTablaSimbolos *m_pTablaSimbolos;
	CSimbolo*	m_pSimboloInt;
	CSimbolo*	m_pSimboloFloat;
	CSimbolo*	m_pSimboloString;
	CSimbolo*	m_pSimboloVoid;

	CSimbolo*	m_pTipoReturn;

	int m_StackVariables;
	int m_MaxVariables;

	CFuncion* m_pFuncionesExternas;
	int	m_CantidadFuncionesExternas;

private:
	void PrvGetLexema(void);
	void PrvReturnLexema(CLexema& Lexema);
	
	void PrvCrearTablaSimbolos(void);
	CSyntTree* PrvAddTablaSimbolos(CTablaSimbolos* pTablaSimbolos);

	CSyntTree* PrvConvertirReturnType(CSyntTree* pNodo, CSimbolo* pReturnType);

	
	
	CSyntTree* PrvFunciones(void);
		CSyntTree* PrvFuncion(void);
  	 /*CSyntTree* PrvDeclaracionVariable(void);*/
			CSyntTree* PrvInstrucciones(void);
				CSyntTree* PrvInstruccion(void);
					CSyntTree* PrvDeclaracionVariable(bool bPermitirInicializacion);
					CSyntTree* PrvIf(void);
					CSyntTree* PrvFor(void);
					CSyntTree* PrvWhile(void);
					CSyntTree* PrvReturn(void);
					CSyntTree* PrvExpresion(void);
						CSyntTree* PrvSumaResta(void); 
							CSyntTree* PrvOperadorLogico(CSyntTree* pNodo);
							CSyntTree* PrvMultiplicacionDivision(void);
								CSyntTree* PrvValor(void);
									CSyntTree* PrvValor2(void);
										CSyntTree* PrvConstante(void);
										CSyntTree* PrvVariable(void);
											CSyntTree* PrvVariable2(CSimbolo* pSimbolo);
										CSyntTree* PrvLlamadoFuncion(void);
											CSyntTree* PrvLlamadoFuncion2(const char* pcNombreFuncion, CTablaSimbolos* pTablaSimbolos);

public:
	CParser();
	~CParser();

	void Clear(void);

	void SetProgram(char* pcText);
	void SetFuncionesExternas(CFuncion* pFuncionesExternas, int CantidadFuncionesExternas);

	bool GenSyntTree(CSyntTree** ppRoot);
};
