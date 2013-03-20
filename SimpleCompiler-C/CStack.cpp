#include "CStack.h"

CStack::CStack()
{
	m_StackPos = -1;
	m_OffsetStack = -1;
	m_OffsetVars = 0;
	m_CantVars = 0;
	m_StackFuncionesPos = 0;
	
	memset(m_pTipoDato, 0, sizeof(m_pTipoDato));
}

CStack::~CStack()
{
	Limpiar();
}

void CStack::Limpiar()
{
	m_OffsetStack = -1;
	m_OffsetVars = 0;
	m_CantVars = 0;
	m_StackFuncionesPos = 0;
	
	while(m_StackPos >= 0)
		Pop();

	memset(m_pTipoDato, 0, sizeof(m_pTipoDato));
}
