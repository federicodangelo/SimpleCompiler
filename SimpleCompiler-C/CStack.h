#pragma once
#include "CTipoDato.h"
#include "CFuncion.h"
#include "CErrorEjecucion.h"
#include "CObjetoCadena.h"

#define ERROR_STACK_VACIO "El stack esta vacio"
#define ERROR_STACK_LLENO "El stack esta lleno"
#define ERROR_STACK_FUNCIONES_LLENO "El stack de llamado de funciones esta lleno"
#define ERROR_STACK_TIPO_INVALIDO "El tipo de dato que se pidio sacar del stack no es compatible con el tipo de dato presente en el stack"

class CObjeto;

class CStack
{
public:
	enum { MAX_STACK = 1024 };
	enum { MAX_STACK_FUNCIONES = 512 };

	struct InfoStackFunciones
	{
		int CantVars;
		int OffsetVars;
		int OffsetStack;
		int Instruccion;
		CFuncion* pFuncion;
	};

private:
	int	m_StackPos;
	CTipoDato::TipoDatoEnum	m_pTipoDato[MAX_STACK + 1];
	long m_pValor[MAX_STACK + 1];
	
	int m_StackFuncionesPos;
	InfoStackFunciones m_pStackFunciones[MAX_STACK_FUNCIONES];

	int m_CantVars;
	int m_OffsetVars;
	int m_OffsetStack;
	
	
public:
	CStack();
	~CStack();

	void Limpiar(void);

	inline void PushInt(long value)
	{
		ThrowErrorEjecucionSi(m_StackPos == MAX_STACK, ERROR_STACK_LLENO);
		m_StackPos++;
		m_pTipoDato[m_StackPos] = CTipoDato::TIPO_INT;
		(*(long*) &m_pValor[m_StackPos]) = value;
	}

	inline void PushFloat(float value)
	{
		ThrowErrorEjecucionSi(m_StackPos == MAX_STACK, ERROR_STACK_LLENO);
		m_StackPos++;
		m_pTipoDato[m_StackPos] = CTipoDato::TIPO_FLOAT;
		(*(float*) &m_pValor[m_StackPos]) = value;
	}

	inline void PushString(CObjetoCadena* value)
	{
		ThrowErrorEjecucionSi(m_StackPos == MAX_STACK, ERROR_STACK_LLENO);
		m_StackPos++;
		m_pTipoDato[m_StackPos] = CTipoDato::TIPO_CADENA;
		
		if (value != NULL)
		{
			(*(CObjetoCadena**) &m_pValor[m_StackPos]) = value;
			value->IncrementarUso();
		}
		else
		{
			(*(CObjeto**) &m_pValor[m_StackPos]) = NULL;
		}
	}

	inline void Pop(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		
		if (m_pTipoDato[m_StackPos] == CTipoDato::TIPO_CADENA)
		{
			m_pTipoDato[m_StackPos] = CTipoDato::TIPO_NINGUNO;

			CObjetoCadena* pObjetoCadena = *(CObjetoCadena**) &m_pValor[m_StackPos];

			if (pObjetoCadena != NULL)
				pObjetoCadena->DecrementarUso();
		}
		
		m_StackPos--;
	}

	inline long PopInt(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_pTipoDato[m_StackPos] != CTipoDato::TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

		return(*(long*) &m_pValor[m_StackPos--]);
	}

	inline float PopFloat(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_pTipoDato[m_StackPos] != CTipoDato::TIPO_FLOAT, ERROR_STACK_TIPO_INVALIDO);

		return(*(float*) &m_pValor[m_StackPos--]);
	}

	inline CObjetoCadena* PopString(bool bAutoDecrement = true)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_pTipoDato[m_StackPos] != CTipoDato::TIPO_CADENA, ERROR_STACK_TIPO_INVALIDO);

		CObjetoCadena* pObjetoCadena = *(CObjetoCadena**) &m_pValor[m_StackPos];
		
		m_pTipoDato[m_StackPos] = CTipoDato::TIPO_NINGUNO;
		
		m_StackPos--;

		if (bAutoDecrement && pObjetoCadena != NULL)
			pObjetoCadena->DecrementarUso();

		return(pObjetoCadena);
	}

	inline void Duplicar(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_StackPos == MAX_STACK, ERROR_STACK_LLENO);

		m_pTipoDato[m_StackPos + 1] = m_pTipoDato[m_StackPos];
		m_pValor[m_StackPos + 1] = m_pValor[m_StackPos];
		m_StackPos++;

		if (m_pTipoDato[m_StackPos] == CTipoDato::TIPO_CADENA)
		{
			CObjetoCadena* pObjetoCadena = *(CObjetoCadena**) &m_pValor[m_StackPos];
			if (pObjetoCadena != NULL)
				pObjetoCadena->IncrementarUso();
		}
	}

	int ObtenerElementosEnStack(void) { return (m_StackPos - m_OffsetStack); }

	inline CTipoDato::TipoDatoEnum ObtenerTipoDato(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);

		return(m_pTipoDato[m_StackPos]);
	}

	inline void PushLlamadoFuncion(int CantParametros, int CantVars, CFuncion* pFuncion, int Instruccion)
	{
		ThrowErrorEjecucionSi(m_StackPos - CantParametros + CantVars >= MAX_STACK, ERROR_STACK_LLENO);
		ThrowErrorEjecucionSi(m_StackFuncionesPos + 1 == MAX_STACK_FUNCIONES, ERROR_STACK_FUNCIONES_LLENO);

		m_StackFuncionesPos++;
		
		InfoStackFunciones* pInfoStack = &m_pStackFunciones[m_StackFuncionesPos];

		pInfoStack->CantVars = m_CantVars;
		pInfoStack->OffsetStack = m_OffsetStack;
		pInfoStack->OffsetVars = m_OffsetVars;
		pInfoStack->Instruccion = Instruccion;
		pInfoStack->pFuncion = pFuncion;

		m_OffsetVars = m_StackPos + 1 - CantParametros;
		m_OffsetStack = m_OffsetVars + CantVars - 1;

		m_CantVars = CantVars;
		m_StackPos = m_OffsetStack;
	}

	inline void IncrementarInt(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_pTipoDato[m_StackPos] != CTipoDato::TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

		(*(long*) &m_pValor[m_StackPos])++;
	}

	inline void DecrementarInt(void)
	{
		ThrowErrorEjecucionSi(m_StackPos == m_OffsetStack, ERROR_STACK_VACIO);
		ThrowErrorEjecucionSi(m_pTipoDato[m_StackPos] != CTipoDato::TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

		(*(long*) &m_pValor[m_StackPos])--;
	}

	inline InfoStackFunciones* GetLastLlamadoFuncion(void)
	{
		return(&m_pStackFunciones[m_StackFuncionesPos]);
	}

	inline void PopLlamadoFuncion(void)
	{
		while(m_StackPos >= m_OffsetVars)
		{
			if (m_pTipoDato[m_StackPos] == CTipoDato::TIPO_CADENA)
			{
				m_pTipoDato[m_StackPos] = CTipoDato::TIPO_NINGUNO;
				
				CObjetoCadena* pObjetoCadena = *(CObjetoCadena**) &m_pValor[m_StackPos];

				if (pObjetoCadena != NULL)
					pObjetoCadena->DecrementarUso();
			}

			m_StackPos--;
		}
		
		InfoStackFunciones* pInfoStack = &m_pStackFunciones[m_StackFuncionesPos];

		m_CantVars = pInfoStack->CantVars;
		m_OffsetVars = pInfoStack->OffsetVars;
		m_OffsetStack = pInfoStack->OffsetStack;

		m_StackFuncionesPos--;
	}

	int ObtenerElementosEnStackLlamadoFunciones(void) { return(m_StackFuncionesPos); }

	inline CTipoDato::TipoDatoEnum VarTipoDato(int n)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		return(m_pTipoDato[n + m_OffsetVars]); 
	}

	inline void SetearVarInt(int n, long value)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		if (m_pTipoDato[n] == CTipoDato::TIPO_CADENA)
		{
			if (m_pValor[n] != 0)
				(*(CObjetoCadena**) &m_pValor[n])->DecrementarUso();
		}

		m_pTipoDato[n] = CTipoDato::TIPO_INT;

		(*(long*) &m_pValor[n]) = value;
	}

	inline void SetearVarFloat(int n, float value)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		if (m_pTipoDato[n] == CTipoDato::TIPO_CADENA)
		{
			if (m_pValor[n] != 0)
				(*(CObjetoCadena**) &m_pValor[n])->DecrementarUso();
		}

		m_pTipoDato[n] = CTipoDato::TIPO_FLOAT;

		(*(float*) &m_pValor[n]) = value;
	}

	inline void SetearVarString(int n, CObjetoCadena* value)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		if (m_pTipoDato[n] == CTipoDato::TIPO_CADENA)
		{
			if (m_pValor[n] != 0)
				(*(CObjetoCadena**) &m_pValor[n])->DecrementarUso();
		}

		m_pTipoDato[n] = CTipoDato::TIPO_CADENA;

		if (value != NULL)
		{
			(*(CObjetoCadena**) &m_pValor[n]) = value;
			value->IncrementarUso();
		}
		else
		{
			m_pValor[n] = NULL;
		}
	}

	inline long ObtenerVarInt(int n)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		ThrowErrorEjecucionSi(m_pTipoDato[n] != CTipoDato::TIPO_INT, "Tipo de variable incorrecto");

		return(*(long*) &m_pValor[n]);
	}

	inline float ObtenerVarFloat(int n)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		ThrowErrorEjecucionSi(m_pTipoDato[n] != CTipoDato::TIPO_FLOAT, "Tipo de variable incorrecto");

		return(*(float*) &m_pValor[n]);
	}

	inline CObjetoCadena* ObtenerVarString(int n)
	{
		//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
		//ThrowErrorEjecucionSi(n >= m_CantVars, "Numero de variable invalido");

		n += m_OffsetVars;

		ThrowErrorEjecucionSi(m_pTipoDato[n] != CTipoDato::TIPO_CADENA, "Tipo de variable incorrecto");

		ThrowErrorEjecucionSi(m_pValor[n] == NULL, "El valor almacenado en la variable es NULL, no puede obtenerse este valor");

		return(*(CObjetoCadena**) &m_pValor[n]);
	}
};
