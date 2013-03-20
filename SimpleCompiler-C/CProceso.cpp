#include "CProceso.h"
#include "CErrorEjecucion.h"
#include "FuncionesUnix.h"

CProceso::FuncInstPtr const CProceso::m_ppInst[CListaInstrucciones::INST_LAST] = 
{
	&PrvInstNada,
	&PrvInstSumarInt,
	&PrvInstRestarInt,
	&PrvInstMultInt,
	&PrvInstDivInt,

	&PrvInstSumarFloat,
	&PrvInstRestarFloat,
	&PrvInstMultFloat,
	&PrvInstDivFloat,

	&PrvInstConcatString,
	&PrvInstCompareString,

	&PrvInstIsZeroInt,

	&PrvInstFloatAInt,
	&PrvInstIntAFloat,

	&PrvInstIncInt,
	&PrvInstDecInt,

	&PrvInstStackDup,
	
	&PrvInstReturn,

	&PrvInstStackPop,

	&PrvInstStackPopVarInt,
	&PrvInstStackPopVarFloat,
	&PrvInstStackPopVarString,

	&PrvInstStackPopGlobalVarInt,
	&PrvInstStackPopGlobalVarFloat,
	&PrvInstStackPopGlobalVarString,

	&PrvInstStackPushInt,
	&PrvInstStackPushFloat,
	&PrvInstStackPushString,

	&PrvInstStackPushVarInt,
	&PrvInstStackPushVarFloat,
	&PrvInstStackPushVarString,

	&PrvInstStackPushGlobalVarInt,
	&PrvInstStackPushGlobalVarFloat,
	&PrvInstStackPushGlobalVarString,

	&PrvInstJump,
	&PrvInstJumpIfZero,

	&PrvInstCallFunction,
	&PrvInstCallGlobalFunction	
};

/*CProceso::FuncInstRetPtr const CProceso::m_ppInstRet[5] = 
{
	&PrvInstRetVoid,
	&PrvInstRetVoid,
	&PrvInstRetInt,
	&PrvInstRetFloat,
	&PrvInstRetObj
};*/


CProceso::CProceso(void)
{
	m_Instruccion = 0;
	m_pDefinicionFunciones = NULL;
	m_pListaInstrucciones = NULL;
	m_pTablaConstantes = NULL;
	m_pFuncion = NULL;
	m_Estado = ESTADO_INICIALIZANDO;
	m_pFuncionesExternas = NULL;
	m_CantidadFuncionesExternas = 0;

/*
	m_ppInst[CListaInstrucciones::INST_NADA] = &PrvInstNada;
	m_ppInst[CListaInstrucciones::INST_SUMAR_INT] = &PrvInstSumarInt;
	m_ppInst[CListaInstrucciones::INST_RESTAR_INT] = &PrvInstRestarInt;
	m_ppInst[CListaInstrucciones::INST_MULT_INT] = &PrvInstMultInt;
	m_ppInst[CListaInstrucciones::INST_DIV_INT] = &PrvInstDivInt;

	m_ppInst[CListaInstrucciones::INST_SUMAR_FLOAT] = &PrvInstSumarFloat;
	m_ppInst[CListaInstrucciones::INST_RESTAR_FLOAT] = &PrvInstRestarFloat;
	m_ppInst[CListaInstrucciones::INST_MULT_FLOAT] = &PrvInstMultFloat;
	m_ppInst[CListaInstrucciones::INST_DIV_FLOAT] = &PrvInstDivFloat;

	m_ppInst[CListaInstrucciones::INST_FLOAT_A_INT] = &PrvInstFloatAInt;
	m_ppInst[CListaInstrucciones::INST_INT_A_FLOAT] = &PrvInstIntAFloat;

	m_ppInst[CListaInstrucciones::INST_INC_INT] = &PrvInstIncInt;
	m_ppInst[CListaInstrucciones::INST_DEC_INT] = &PrvInstDecInt;

	m_ppInst[CListaInstrucciones::INST_STACK_DUP] = &PrvInstStackDup;
	
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_NULL] = &PrvInstStackPushNull;
	
	m_ppInst[CListaInstrucciones::INST_RETURN] = &PrvInstReturn;

	m_ppInst[CListaInstrucciones::INST_STACK_POP] = &PrvInstStackPop;

	m_ppInst[CListaInstrucciones::INST_THROW] = &PrvInstThrow;

	m_ppInst[CListaInstrucciones::INST_STACK_POP_VAR_INT] = &PrvInstStackPopVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_VAR_FLOAT] = &PrvInstStackPopVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_VAR_OBJ] = &PrvInstStackPopVarObj;

	m_ppInst[CListaInstrucciones::INST_STACK_POP_CLASS_VAR_INT] = &PrvInstStackPopClassVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_CLASS_VAR_FLOAT] = &PrvInstStackPopClassVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_CLASS_VAR_OBJ] = &PrvInstStackPopClassVarObj;

	m_ppInst[CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_INT] = &PrvInstStackPopThisClassVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_FLOAT] = &PrvInstStackPopThisClassVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_OBJ] = &PrvInstStackPopThisClassVarObj;

	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_INT] = &PrvInstStackPushInt;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_FLOAT] = &PrvInstStackPushFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_STRING] = &PrvInstStackPushString;

	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_VAR_INT] = &PrvInstStackPushVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_VAR_FLOAT] = &PrvInstStackPushVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_VAR_OBJ] = &PrvInstStackPushVarObj;

	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_INT] = &PrvInstStackPushClassVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_FLOAT] = &PrvInstStackPushClassVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_OBJ] = &PrvInstStackPushClassVarObj;

	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_INT] = &PrvInstStackPushThisClassVarInt;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_FLOAT] = &PrvInstStackPushThisClassVarFloat;
	m_ppInst[CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_OBJ] = &PrvInstStackPushThisClassVarObj;

	m_ppInst[CListaInstrucciones::INST_NEW] = &PrvInstNew;

	m_ppInst[CListaInstrucciones::INST_JUMP] = &PrvInstJump;
	m_ppInst[CListaInstrucciones::INST_JUMP_IF_ZERO] = &PrvInstJumpIfZero;

	m_ppInst[CListaInstrucciones::INST_CALL_THIS_CLASS_FUNCTION] = &PrvInstCallThisClassFunction;
	m_ppInst[CListaInstrucciones::INST_CALL_CLASS_FUNCTION] = &PrvInstCallClassFunction;*/
}

CProceso::~CProceso()
{
	Clear();
}

void CProceso::Clear(void)
{
	m_Stack.Limpiar();

	m_Instruccion = 0;
	m_pDefinicionFunciones = NULL;
	m_pListaInstrucciones = NULL;
	m_pTablaConstantes = NULL;
	m_pFuncion = NULL;
	m_Runtime.Limpiar();
	m_pFuncionesExternas = NULL;
	m_CantidadFuncionesExternas = 0;
}

CListaInstrucciones::InstruccionesEnum CProceso::PrvObtenerInstruccion(void)
{
	CListaInstrucciones::InstruccionesEnum inst = m_pListaInstrucciones->ObtenerInstruccion(m_Instruccion);
	
	m_Instruccion += sizeof(CListaInstrucciones::InstruccionType);

	return(inst);
}

CListaInstrucciones::ParametroType CProceso::PrvObtenerParametro(void)
{
	CListaInstrucciones::ParametroType param = m_pListaInstrucciones->ObtenerParametroDesplazado(m_Instruccion);

	m_Instruccion += sizeof(CListaInstrucciones::ParametroType);

	return(param);
}

void CProceso::PrvEjecutarInstruccion(void)
{
	CListaInstrucciones::InstruccionesEnum inst = PrvObtenerInstruccion();

	m_ppInst[inst](this);
	
	return;

	/*long int_value, int_value1, int_value2;
	float float_value, float_value1, float_value2;
	CObjeto* pObjeto;

	switch(inst)
	{
		case CListaInstrucciones::INST_NADA:
			break;

		case CListaInstrucciones::INST_SUMAR_INT:
			int_value2 = m_Stack.PopInt();
			int_value1 = m_Stack.PopInt();
			int_value = int_value1 + int_value2;
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_RESTAR_INT:
			int_value2 = m_Stack.PopInt();
			int_value1 = m_Stack.PopInt();
			int_value = int_value1 - int_value2;
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_MULT_INT:
			int_value2 = m_Stack.PopInt();
			int_value1 = m_Stack.PopInt();
			int_value = int_value1 * int_value2;
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_DIV_INT:
			int_value2 = m_Stack.PopInt();
			int_value1 = m_Stack.PopInt();
			int_value = int_value1 / int_value2;
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_SUMAR_FLOAT:
			float_value2 = m_Stack.PopFloat();
			float_value1 = m_Stack.PopFloat();
			float_value = float_value1 + float_value2;
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_RESTAR_FLOAT:
			float_value2 = m_Stack.PopFloat();
			float_value1 = m_Stack.PopFloat();
			float_value = float_value1 - float_value2;
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_MULT_FLOAT:
			float_value2 = m_Stack.PopFloat();
			float_value1 = m_Stack.PopFloat();
			float_value = float_value1 * float_value2;
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_DIV_FLOAT:
			float_value2 = m_Stack.PopFloat();
			float_value1 = m_Stack.PopFloat();
			float_value = float_value1 / float_value2;
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_FLOAT_A_INT:
			float_value = m_Stack.PopFloat();
			int_value = (long) float_value;
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_INT_A_FLOAT:
			int_value = m_Stack.PopInt();
			float_value = (float) int_value;
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_STACK_PUSH_NULL:
			m_Stack.PushObjeto(NULL);
			break;

		case CListaInstrucciones::INST_STACK_PUSH_INT:
			int_value = m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetInt();
			m_Stack.PushInt(int_value);
			break;

		case CListaInstrucciones::INST_STACK_PUSH_FLOAT:
			float_value = m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetFloat();
			m_Stack.PushFloat(float_value);
			break;

		case CListaInstrucciones::INST_STACK_PUSH_STRING:
			//FIX_ME tengo que construir un objeto del tipo string y asignarle el string constante..
			break;

		case CListaInstrucciones::INST_STACK_PUSH_VAR_INT:
			m_Stack.PushInt(m_Stack.ObtenerVarInt(PrvObtenerParametro()));
			break;

		case CListaInstrucciones::INST_STACK_PUSH_VAR_FLOAT:
			m_Stack.PushFloat(m_Stack.ObtenerVarFloat(PrvObtenerParametro()));
			break;

		case CListaInstrucciones::INST_STACK_PUSH_VAR_OBJ:
			m_Stack.PushObjeto(m_Stack.ObtenerVarObjeto(PrvObtenerParametro()));
			break;

		case CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_INT:
			m_Stack.PushInt(m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->ObtenerInt());
			break;

		case CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_FLOAT:
			m_Stack.PushFloat(m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->ObtenerFloat());
			break;

		case CListaInstrucciones::INST_STACK_PUSH_THIS_CLASS_VAR_OBJ:
			m_Stack.PushObjeto(m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->ObtenerObjeto());
			break;

		case CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_INT:
		{
			pObjeto = m_Stack.PopObjeto(false);
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			m_Stack.PushInt(pObjeto->ObtenerVariables()->Variable(nVar)->ObtenerInt());
			pObjeto->DecrementarUso();
			break;
		}

		case CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_FLOAT:
		{
			pObjeto = m_Stack.PopObjeto(false);
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			m_Stack.PushFloat(pObjeto->ObtenerVariables()->Variable(nVar)->ObtenerFloat());
			pObjeto->DecrementarUso();
			break;
		}

		case CListaInstrucciones::INST_STACK_PUSH_CLASS_VAR_OBJ:
		{
			pObjeto = m_Stack.PopObjeto(false);
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			m_Stack.PushObjeto(pObjeto->ObtenerVariables()->Variable(nVar)->ObtenerObjeto());
			pObjeto->DecrementarUso();
			break;
		}
		
		case CListaInstrucciones::INST_STACK_DUP:
			m_Stack.Duplicar();
			break;

		case CListaInstrucciones::INST_STACK_POP:
			m_Stack.Pop();
			break;

		case CListaInstrucciones::INST_STACK_POP_VAR_INT:
			m_Stack.SetearVarInt(PrvObtenerParametro(), m_Stack.PopInt());
			break;

		case CListaInstrucciones::INST_STACK_POP_VAR_FLOAT:
			m_Stack.SetearVarFloat(PrvObtenerParametro(), m_Stack.PopFloat());
			break;

		case CListaInstrucciones::INST_STACK_POP_VAR_OBJ:
			pObjeto = m_Stack.PopObjeto(false);
			m_Stack.SetearVarObjeto(PrvObtenerParametro(), pObjeto);
			if (pObjeto != NULL)
				pObjeto->DecrementarUso();
			break;

		case CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_INT:
			m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->SetearInt(m_Stack.PopInt());
			break;

		case CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_FLOAT:
			m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->SetearFloat(m_Stack.PopFloat());
			break;

		case CListaInstrucciones::INST_STACK_POP_THIS_CLASS_VAR_OBJ:
			pObjeto = m_Stack.PopObjeto(false);
			m_pObjeto->ObtenerVariables()->Variable(PrvObtenerParametro())->SetearObjeto(pObjeto);
			pObjeto->DecrementarUso();
			break;

		case CListaInstrucciones::INST_STACK_POP_CLASS_VAR_INT:
		{
			pObjeto = m_Stack.PopObjeto(false);
			int_value = m_Stack.PopInt();
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto->ObtenerVariables()->Variable(nVar)->SetearInt(int_value);
			pObjeto->DecrementarUso();
			break;
		}

		case CListaInstrucciones::INST_STACK_POP_CLASS_VAR_FLOAT:
		{
			pObjeto = m_Stack.PopObjeto(false);
			float_value = m_Stack.PopFloat();
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto->ObtenerVariables()->Variable(nVar)->SetearFloat(float_value);
			pObjeto->DecrementarUso();
			break;
		}

		case CListaInstrucciones::INST_STACK_POP_CLASS_VAR_OBJ:
		{
			pObjeto = m_Stack.PopObjeto(false);
			CObjeto* pObjetoValue = m_Stack.PopObjeto(false);
			int nVar = pObjeto->ObtenerDefinicion()->BuscarPosicionVariable(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto->ObtenerVariables()->Variable(nVar)->SetearObjeto(pObjetoValue);
			pObjetoValue->DecrementarUso();
			pObjeto->DecrementarUso();
			break;
		}	
		
		case CListaInstrucciones::INST_JUMP:
			m_Instruccion += PrvObtenerParametro();
			break;

		case CListaInstrucciones::INST_JUMP_IF_ZERO:
			int_value = m_Stack.PopInt();
			if (int_value == 0)
				m_Instruccion += PrvObtenerParametro();
			else
				PrvObtenerParametro();
			break;

		case CListaInstrucciones::INST_CALL_THIS_CLASS_FUNCTION:
			PrvEjecutarFuncion(m_pObjeto, PrvObtenerParametro());
			break;

		case CListaInstrucciones::INST_CALL_CLASS_FUNCTION:
		{
			CObjeto* pObjeto = m_Stack.PopObjeto(false);
			CCadenaConHash& NombreFuncion = m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString();
			PrvEjecutarFuncion(pObjeto, NombreFuncion);
			pObjeto->DecrementarUso();
			break;
		}

		case CListaInstrucciones::INST_NEW:
			pObjeto = PrvConstruirObjeto(m_pTablaConstantes->GetSimbolo(PrvObtenerParametro())->GetString());
			m_Stack.PushObjeto(pObjeto);
			break;

		case CListaInstrucciones::INST_RETURN:
			PrvRetornarDeFuncion();
			break;

		case CListaInstrucciones::INST_THROW:
		{
			pObjeto = m_Stack.PopObjeto(false);

			int n = m_pFuncion->BuscarManejadorExcepcion(m_Instruccion, pObjeto->ObtenerDefinicion()->ObtenerNombre());

			while (n < 0 && m_Stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
			{
				PrvRetornarDeFuncion();
				n = m_pFuncion->BuscarManejadorExcepcion(m_Instruccion, pObjeto->ObtenerDefinicion()->ObtenerNombre());
			}

			if (m_Stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
			{
				//Se encontro un manejador
				m_Instruccion = m_pFuncion->GetManejadorExcepcion(n)->EjecutarLinea;
				m_Stack.PushObjeto(pObjeto);
				pObjeto->DecrementarUso();
			}
			else
			{
				pObjeto->DecrementarUso();
				printf("No hay manejador de excepciones por defecto!!!\n");
				//No se encontro ningun manejador.. estoy hasta las manos
			}
			break;
		}

		default:
			ThrowErrorEjecucion("Instruccion invalida o no implementada");
			break;
	}*/
}

void CProceso::PrvConstruirRuntime()
{
	m_Runtime.Inicializar(m_pDefinicionFunciones);
	m_pTablaConstantes = m_pDefinicionFunciones->GetTablaConstantes();
}

void CProceso::PrvEjecutarFuncion(int n)
{
	CFuncion* pFuncion = m_pDefinicionFunciones->ObtenerFuncion(n);

	PrvEjecutarFuncion(pFuncion);
}

void CProceso::PrvEjecutarFuncion(CCadenaConHash& NombreFuncion)
{
	CFuncion* pFuncion = m_pDefinicionFunciones->BuscarFuncion(NombreFuncion);

	ThrowErrorEjecucionSi(pFuncion == NULL, "No se encontro la función");

	PrvEjecutarFuncion(pFuncion);
}

void CProceso::PrvEjecutarFuncion(CFuncion* pFuncion)
{
	m_Stack.PushLlamadoFuncion(pFuncion->GetParametros(), pFuncion->GetCantVariables(), m_pFuncion, m_Instruccion);

	m_pFuncion = pFuncion;

	if (pFuncion->ObtenerTipoFuncion() == CFuncion::FUNCION_NORMAL)
	{
		m_pListaInstrucciones = m_pFuncion->ObtenerListaInstrucciones();

		m_Instruccion = 0;
	}
	else
	{
		CCadenaConHash descripcionError;

		bool ok = pFuncion->ObtenerFuncioNativa()(this, &m_Stack, descripcionError);

		if (pFuncion->TipoDevuelto() != NULL &&
			pFuncion->TipoDevuelto()->ObtenerTipoDato() != CTipoDato::TIPO_VOID &&
			pFuncion->TipoDevuelto()->ObtenerTipoDato() != CTipoDato::TIPO_NINGUNO)
		{
			if (m_Stack.ObtenerElementosEnStack() == 0 ||
				m_Stack.ObtenerTipoDato() != pFuncion->TipoDevuelto()->ObtenerTipoDato())
			{
				ThrowErrorEjecucion("El tipo devuelto por la función nativa no coincide con lo que declara en su definición");
			}
		}

		if (ok == false)
			ThrowErrorEjecucion(descripcionError.ObtenerCadena());

		PrvRetornarDeFuncion();
	}
}

void CProceso::PrvEjecutarFuncionGlobal(CCadenaConHash& nombreFuncion)
{
	CFuncion *pFuncion = NULL;

	for (int i = 0; i < m_CantidadFuncionesExternas; i++)
		if (m_pFuncionesExternas[i].ObtenerNombre() == nombreFuncion)
		{
			pFuncion = &m_pFuncionesExternas[i];
			break;
		}

		ThrowErrorEjecucionSi(pFuncion == NULL, "No se encontro la función global solicitada");

	PrvEjecutarFuncion(pFuncion);
}


void CProceso::PrvRetornarDeFuncion(void)
{
	CFuncion* pCalledFunc = m_pFuncion;
	CStack* pStack = &m_Stack;

	CStack::InfoStackFunciones* pInfo = pStack->GetLastLlamadoFuncion();

	m_Instruccion = pInfo->Instruccion;
	m_pFuncion = pInfo->pFuncion;
	m_pListaInstrucciones = m_pFuncion->ObtenerListaInstrucciones();

	switch(pCalledFunc->TipoDevuelto()->ObtenerTipoDato())
	{
		case CTipoDato::TIPO_NINGUNO:
		case CTipoDato::TIPO_VOID:
			pStack->PopLlamadoFuncion();
			break;

		case CTipoDato::TIPO_INT:
		{
			long l = pStack->PopInt();
			//printf("Resultado llamdo función: %d\n", l);
			pStack->PopLlamadoFuncion();
			pStack->PushInt(l);
			break;
		}

		case CTipoDato::TIPO_FLOAT:
		{
			float f = pStack->PopFloat();
			//printf("Resultado llamdo función: %f\n", f);
			pStack->PopLlamadoFuncion();
			pStack->PushFloat(f);
			break;
		}

		case CTipoDato::TIPO_CADENA:
		{
			CObjetoCadena* pCadena = pStack->PopString(false);
			//printf("Resultado llamdo función: %s\n", (const char*) pCadena->ObtenerCadena());
			pStack->PopLlamadoFuncion();
			pStack->PushString(pCadena);
			pCadena->DecrementarUso();
			break;
		}
	}
}

/*void CProceso::PrvInstRetVoid(CStack* const pStack)
{
	pStack->PopLlamadoFuncion();
}

void CProceso::PrvInstRetInt(CStack* const pStack)
{
	long l = pStack->PopInt();
	pStack->PopLlamadoFuncion();
	pStack->PushInt(l);
}

void CProceso::PrvInstRetFloat(CStack* const pStack)
{
	float f = pStack->PopFloat();
	pStack->PopLlamadoFuncion();
	pStack->PushFloat(f);
}

void CProceso::PrvInstRetObj(CStack* const pStack)
{
	CObjeto* pObj = pStack->PopObjeto(false);
	pStack->PopLlamadoFuncion();
	pStack->PushObjeto(pObj);
	pObj->DecrementarUso();
}*/

void CProceso::AgregarDefiniciones(CDefinicionFunciones* pDefinicionFunciones)
{
	m_pDefinicionFunciones = pDefinicionFunciones;
}

void CProceso::AgregarFuncionesExternas(CFuncion* pFuncionesExternas, int CantidadFuncionesExternas)
{
	m_pFuncionesExternas = pFuncionesExternas;
	m_CantidadFuncionesExternas = CantidadFuncionesExternas;
}

void CProceso::Inicializar(void)
{
	try
	{
		PrvConstruirRuntime();
	}
	catch(CErrorEjecucion err)
	{
		printf("Error de ejecución: %s\n", err.GetDescription());
		printf("Stack de llamadas: \n");

		if (m_pFuncion)
			printf("\tFuncion %s, Linea %d\n", m_pFuncion->ObtenerNombre(), m_Instruccion);
		
		while(m_Stack.ObtenerElementosEnStackLlamadoFunciones())
		{
			if (m_Stack.GetLastLlamadoFuncion()->pFuncion)
				printf("\tFuncion %s, Linea %d\n", m_Stack.GetLastLlamadoFuncion()->pFuncion->ObtenerNombre(), m_Stack.GetLastLlamadoFuncion()->Instruccion);
			m_Stack.PopLlamadoFuncion();
		}
		printf("\n");
	}
}

CProceso::EstadoProcesoEnum CProceso::EjecutarFuncion(const char* pcFuncion)
{
	int cantidadStackOriginal = m_Stack.ObtenerElementosEnStack();
	PrvEjecutarFuncion(pcFuncion);

	m_Estado = ESTADO_CORRIENDO;

	try
	{
		if (m_Estado == ESTADO_CORRIENDO)
		{
			while (m_Stack.ObtenerElementosEnStackLlamadoFunciones())
			{
				PrvEjecutarInstruccion();
				if (!m_Stack.ObtenerElementosEnStackLlamadoFunciones())	break;
				PrvEjecutarInstruccion();
				if (!m_Stack.ObtenerElementosEnStackLlamadoFunciones())	break;
				PrvEjecutarInstruccion();
				if (!m_Stack.ObtenerElementosEnStackLlamadoFunciones())	break;
				PrvEjecutarInstruccion();
				if (!m_Stack.ObtenerElementosEnStackLlamadoFunciones())	break;
				PrvEjecutarInstruccion();
			}
		}

		if (m_Stack.ObtenerElementosEnStackLlamadoFunciones() == 0)
		{
			m_Estado = ESTADO_TERMINADO;

			while(m_Stack.ObtenerElementosEnStack() != cantidadStackOriginal)
			{
				m_Stack.Pop();
			}
		}
	}
	catch(CErrorEjecucion err)
	{
		printf("Error de ejecución: %s\n", err.GetDescription());
		printf("Stack de llamadas: \n");
		if (m_pFuncion)
			printf("\tFuncion %s, Linea %d\n", (const char*) m_pFuncion->ObtenerNombre(), m_Instruccion);

		while(m_Stack.ObtenerElementosEnStackLlamadoFunciones())
		{
			if (m_Stack.GetLastLlamadoFuncion()->pFuncion)
				printf("\tFuncion %s, Linea %d\n", m_Stack.GetLastLlamadoFuncion()->pFuncion->ObtenerNombre(), m_Stack.GetLastLlamadoFuncion()->Instruccion);
			m_Stack.PopLlamadoFuncion();
		}
		printf("\n");

		Clear();
		m_Estado = ESTADO_ERROR;
	}

	return(m_Estado);
}

/* FUNCIONES QUE REPRESENTAN INSTRUCCIONES */

void CProceso::PrvInstNada(CProceso* const pProceso)
{
}

void CProceso::PrvInstSumarInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value2 = pStack->PopInt();
	long int_value1 = pStack->PopInt();
	long int_value = int_value1 + int_value2;
	pStack->PushInt(int_value);
}

void CProceso::PrvInstRestarInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value2 = pStack->PopInt();
	long int_value1 = pStack->PopInt();
	long int_value = int_value1 - int_value2;
	pStack->PushInt(int_value);
}

void CProceso::PrvInstMultInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value2 = pStack->PopInt();
	long int_value1 = pStack->PopInt();
	long int_value = int_value1 * int_value2;
	pStack->PushInt(int_value);
}

void CProceso::PrvInstDivInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value2 = pStack->PopInt();
	long int_value1 = pStack->PopInt();
	long int_value = int_value1 / int_value2;
	pStack->PushInt(int_value);
}

void CProceso::PrvInstSumarFloat(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	float float_value2 = pStack->PopFloat();
	float float_value1 = pStack->PopFloat();
	float float_value = float_value1 + float_value2;
	pStack->PushFloat(float_value);
}

void CProceso::PrvInstRestarFloat(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	float float_value2 = pStack->PopFloat();
	float float_value1 = pStack->PopFloat();
	float float_value = float_value1 - float_value2;
	pStack->PushFloat(float_value);
}

void CProceso::PrvInstMultFloat(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	float float_value2 = pStack->PopFloat();
	float float_value1 = pStack->PopFloat();
	float float_value = float_value1 * float_value2;
	pStack->PushFloat(float_value);
}

void CProceso::PrvInstDivFloat(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	float float_value2 = pStack->PopFloat();
	float float_value1 = pStack->PopFloat();
	float float_value = float_value1 / float_value2;
	pStack->PushFloat(float_value);
}

void CProceso::PrvInstConcatString(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	CObjetoCadena* cadena_value2 = pStack->PopString(false);
	CObjetoCadena* cadena_value1 = pStack->PopString(false);
	
	CObjetoCadena* cadena_value = new CObjetoCadena(cadena_value1->ObtenerCadena());
	cadena_value->ObtenerCadena().ConcatenarCadena(cadena_value2->ObtenerCadena());

	cadena_value2->DecrementarUso();
	cadena_value1->DecrementarUso();

	pStack->PushString(cadena_value);
}

void CProceso::PrvInstCompareString(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	CObjetoCadena* cadena_value2 = pStack->PopString(false);
	CObjetoCadena* cadena_value1 = pStack->PopString(false);

	int n = strcmp(cadena_value1->ObtenerCadena(), cadena_value2->ObtenerCadena());
	
	cadena_value2->DecrementarUso();
	cadena_value1->DecrementarUso();

	pStack->PushInt(n);
}

void CProceso::PrvInstIsZeroInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value = pStack->PopInt();
	
	int_value = !int_value; //Si es igual a 0, la negación va a dar distinto de cero (verdadero), sino 0 (falso)
	
	pStack->PushInt(int_value);
}

void CProceso::PrvInstFloatAInt(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	float float_value = pStack->PopFloat();
	long int_value = (long) float_value;
	pStack->PushInt(int_value);
}

void CProceso::PrvInstIntAFloat(CProceso* const pProceso)
{
	CStack* pStack = &pProceso->m_Stack;
	long int_value = pStack->PopInt();
	float float_value = (float) int_value;
	pStack->PushFloat(float_value);
}

void CProceso::PrvInstStackPushInt(CProceso* const pProceso)
{
	long int_value = pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetInt();
	pProceso->m_Stack.PushInt(int_value);
}

void CProceso::PrvInstStackPushFloat(CProceso* const pProceso)
{
	float float_value = pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetFloat();
	pProceso->m_Stack.PushFloat(float_value);
}

void CProceso::PrvInstStackPushString(CProceso* const pProceso)
{
	const char* string_value = pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString();
	pProceso->m_Stack.PushString(new CObjetoCadena(string_value));
}

void CProceso::PrvInstStackPushVarInt(CProceso* const pProceso)
{
	pProceso->m_Stack.PushInt(pProceso->m_Stack.ObtenerVarInt(pProceso->PrvObtenerParametro()));
}

void CProceso::PrvInstStackPushVarFloat(CProceso* const pProceso)
{
	pProceso->m_Stack.PushFloat(pProceso->m_Stack.ObtenerVarFloat(pProceso->PrvObtenerParametro()));
}

void CProceso::PrvInstStackPushVarString(CProceso* const pProceso)
{
	pProceso->m_Stack.PushString(pProceso->m_Stack.ObtenerVarString(pProceso->PrvObtenerParametro()));
}

void CProceso::PrvInstStackPushGlobalVarInt(CProceso* const pProceso)
{
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Stack.PushInt(pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->ObtenerInt());
}

void CProceso::PrvInstStackPushGlobalVarFloat(CProceso* const pProceso)
{
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Stack.PushFloat(pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->ObtenerFloat());
}

void CProceso::PrvInstStackPushGlobalVarString(CProceso* const pProceso)
{
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Stack.PushString(pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->ObtenerString());
}

void CProceso::PrvInstStackDup(CProceso* const pProceso)
{
	//FIX_ME
	/*if (pProceso->m_Stack.ObtenerTipoDato() == CTipoDato::TIPO_INT)
	{
		pProceso->m_Stack.Duplicar();
		printf("Valor: %d\n", pProceso->m_Stack.PopInt());
	}*/

	pProceso->m_Stack.Duplicar();
}

void CProceso::PrvInstStackPop(CProceso* const pProceso)
{
	pProceso->m_Stack.Pop();
}

void CProceso::PrvInstStackPopVarInt(CProceso* const pProceso)
{
	pProceso->m_Stack.SetearVarInt(pProceso->PrvObtenerParametro(), pProceso->m_Stack.PopInt());
}

void CProceso::PrvInstStackPopVarFloat(CProceso* const pProceso)
{
	pProceso->m_Stack.SetearVarFloat(pProceso->PrvObtenerParametro(), pProceso->m_Stack.PopFloat());
}

void CProceso::PrvInstStackPopVarString(CProceso* const pProceso)
{
	CObjetoCadena* pObjetoCadena = pProceso->m_Stack.PopString(false);
	pProceso->m_Stack.SetearVarString(pProceso->PrvObtenerParametro(), pObjetoCadena);
	if (pObjetoCadena != NULL)
		pObjetoCadena->DecrementarUso();
}

void CProceso::PrvInstStackPopGlobalVarInt(CProceso* const pProceso)
{
	long int_value = pProceso->m_Stack.PopInt();
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->SetearInt(int_value);
}

void CProceso::PrvInstStackPopGlobalVarFloat(CProceso* const pProceso)
{
	float float_value = pProceso->m_Stack.PopFloat();
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->SetearFloat(float_value);
}

void CProceso::PrvInstStackPopGlobalVarString(CProceso* const pProceso)
{
	CObjetoCadena* pObjetoCadenaValue = pProceso->m_Stack.PopString(false);
	int nVar = pProceso->m_pDefinicionFunciones->BuscarPosicionVariable(pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString());
	ThrowErrorEjecucionSi(nVar == CDefinicionFunciones::NO_ENCONTRADO, "No se encontro la variable");
	pProceso->m_Runtime.ObtenerVariables()->Variable(nVar)->SetearString(pObjetoCadenaValue);
	pObjetoCadenaValue->DecrementarUso();
}

void CProceso::PrvInstJump(CProceso* const pProceso)
{
	pProceso->m_Instruccion += pProceso->PrvObtenerParametro();
}

void CProceso::PrvInstJumpIfZero(CProceso* const pProceso)
{
	long int_value = pProceso->m_Stack.PopInt();
	if (int_value == 0)
		pProceso->m_Instruccion += pProceso->PrvObtenerParametro();
	else
		pProceso->PrvObtenerParametro();
}

void CProceso::PrvInstCallFunction(CProceso* const pProceso)
{
	pProceso->PrvEjecutarFuncion(pProceso->PrvObtenerParametro());
}

void CProceso::PrvInstCallGlobalFunction(CProceso* const pProceso)
{
	CCadenaConHash& NombreFuncion = pProceso->m_pTablaConstantes->GetSimbolo(pProceso->PrvObtenerParametro())->GetString();
	pProceso->PrvEjecutarFuncionGlobal(NombreFuncion);
}

void CProceso::PrvInstReturn(CProceso* const pProceso)
{
	pProceso->PrvRetornarDeFuncion();
}

void CProceso::PrvInstIncInt(CProceso* const pProceso)
{
	pProceso->m_Stack.IncrementarInt();
}

void CProceso::PrvInstDecInt(CProceso* const pProceso)
{
	pProceso->m_Stack.DecrementarInt();
}
