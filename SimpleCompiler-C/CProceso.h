#pragma once

#include "CRuntime.h"
#include "CDefinicionFunciones.h"
#include "CTablaSimbolosSimple.h"
#include "CStack.h"

class CProceso
{
public:
	enum EstadoProcesoEnum
	{
		ESTADO_INICIALIZANDO,
		ESTADO_CORRIENDO,
		ESTADO_TERMINADO,
		ESTADO_ERROR,
	};

private:
	typedef void FuncInst(CProceso* const);
	typedef FuncInst* FuncInstPtr;

	static inline void	PrvInstNada(CProceso* const pProceso);
	static inline void	PrvInstSumarInt(CProceso* const pProceso);
	static inline void	PrvInstRestarInt(CProceso* const pProceso);
	static inline void	PrvInstMultInt(CProceso* const pProceso);
	static inline void	PrvInstDivInt(CProceso* const pProceso);
	static inline void	PrvInstSumarFloat(CProceso* const pProceso);
	static inline void	PrvInstRestarFloat(CProceso* const pProceso);
	static inline void	PrvInstMultFloat(CProceso* const pProceso);
	static inline void	PrvInstDivFloat(CProceso* const pProceso);
	static inline void	PrvInstConcatString(CProceso* const pProceso);
	static inline void	PrvInstCompareString(CProceso* const pProceso);
	static inline void	PrvInstIsZeroInt(CProceso* const pProceso);
	static inline void	PrvInstFloatAInt(CProceso* const pProceso);
	static inline void	PrvInstIntAFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPushInt(CProceso* const pProceso);
	static inline void	PrvInstStackPushFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPushString(CProceso* const pProceso);
	static inline void	PrvInstStackPushVarInt(CProceso* const pProceso);
	static inline void	PrvInstStackPushVarFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPushVarString(CProceso* const pProceso);
	static inline void	PrvInstStackPushGlobalVarInt(CProceso* const pProceso);
	static inline void	PrvInstStackPushGlobalVarFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPushGlobalVarString(CProceso* const pProceso);
	static inline void	PrvInstStackDup(CProceso* const pProceso);
	static inline void	PrvInstStackPop(CProceso* const pProceso);
	static inline void	PrvInstStackPopVarInt(CProceso* const pProceso);
	static inline void	PrvInstStackPopVarFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPopVarString(CProceso* const pProceso);
	static inline void	PrvInstStackPopGlobalVarInt(CProceso* const pProceso);
	static inline void	PrvInstStackPopGlobalVarFloat(CProceso* const pProceso);
	static inline void	PrvInstStackPopGlobalVarString(CProceso* const pProceso);
	static inline void	PrvInstJump(CProceso* const pProceso);
	static inline void	PrvInstJumpIfZero(CProceso* const pProceso);
	static inline void	PrvInstCallFunction(CProceso* const pProceso);
	static inline void	PrvInstCallGlobalFunction(CProceso* const pProceso);
	static inline void	PrvInstReturn(CProceso* const pProceso);
	static inline void	PrvInstIncInt(CProceso* const pProceso);
	static inline void	PrvInstDecInt(CProceso* const pProceso);

	/*typedef void FuncInstRet(CStack* const);
	typedef FuncInstRet* FuncInstRetPtr;

	static inline void	PrvInstRetVoid(CStack* const pStack);
	static inline void	PrvInstRetInt(CStack* const pStack);
	static inline void	PrvInstRetFloat(CStack* const pStack);
	static inline void	PrvInstRetObj(CStack* const pStack);*/

private:
	CStack m_Stack;
	int m_Instruccion;

public:
	static FuncInstPtr const m_ppInst[CListaInstrucciones::INST_LAST];
	//static FuncInstRetPtr const m_ppInstRet[5];
	
private:
	CFuncion*				m_pFuncion;
	CListaInstrucciones*	m_pListaInstrucciones;
	CTablaSimbolosSimple*	m_pTablaConstantes;
	CDefinicionFunciones*	m_pDefinicionFunciones;
	CRuntime				m_Runtime;
	CFuncion*				m_pFuncionesExternas;
	int						m_CantidadFuncionesExternas;

	EstadoProcesoEnum m_Estado;

private:
	inline CListaInstrucciones::InstruccionesEnum PrvObtenerInstruccion(void);
	inline CListaInstrucciones::ParametroType PrvObtenerParametro(void);
	inline void PrvEjecutarInstruccion(void);

	inline void PrvConstruirRuntime();

	inline void PrvEjecutarFuncion(const char* pcNombreFuncion) { CCadenaConHash val(pcNombreFuncion); PrvEjecutarFuncion(val); }
	inline void PrvEjecutarFuncion(CCadenaConHash& NombreFuncion);
	inline void PrvEjecutarFuncion(int n);
	inline void PrvEjecutarFuncion(CFuncion* pFuncion);
	inline void PrvEjecutarFuncionGlobal(const char* pcNombreFuncion) { CCadenaConHash val(pcNombreFuncion); PrvEjecutarFuncionGlobal(val); }
	inline void PrvEjecutarFuncionGlobal(CCadenaConHash& NombreFuncion);
	inline void PrvRetornarDeFuncion(void);
	
public:
	CProceso(void);
	~CProceso();

	void Clear(void);

	void AgregarDefiniciones(CDefinicionFunciones* pDefinicionFunciones);
	void AgregarFuncionesExternas(CFuncion* pFuncionesExternas, int CantidadFuncionesExternas);

	void Inicializar(void);

	EstadoProcesoEnum EjecutarFuncion(const char* pcFuncion);
};
