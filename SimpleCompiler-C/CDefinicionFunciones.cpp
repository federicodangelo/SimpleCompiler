#include "CDefinicionFunciones.h"
#include "FuncionesUnix.h"

CDefinicionFunciones::CDefinicionFunciones()
{
	m_ppFunciones = NULL;
	m_ppVariables = NULL;
	m_Funciones = 0;
	m_Variables = 0;
}

CDefinicionFunciones::~CDefinicionFunciones()
{
	Limpiar();
}

void CDefinicionFunciones::Limpiar(void)
{
	if (m_ppVariables)
	{
		while (m_Variables != 0)
			delete m_ppVariables[--m_Variables];

		free(m_ppVariables);
		m_ppVariables = NULL;
	}

	if (m_ppFunciones)
	{
		while (m_Funciones != 0)
			delete m_ppFunciones[--m_Funciones];

		free(m_ppFunciones);
		m_ppFunciones = NULL;
	}
}

void CDefinicionFunciones::Imprimir(int n)
{
	int i;

	for (i = 0; i < n; i++)
		printf("    ");

	printf("Listado de funciones: \n");

	for (i = 0; i < n + 1; i++)
		printf("    ");

	printf("Constantes:\n");
	m_TablaConstantes.Imprimir(n + 2);

	for (i = 0; i < n + 1; i++)
		printf("    ");

	printf("Variables:\n");
	for (i = 0; i < m_Variables; i++)
		m_ppVariables[i]->Imprimir(n + 2);

	for (i = 0; i < n + 1; i++)
		printf("    ");

	printf("Funciones:\n");
	for (i = 0; i < m_Funciones; i++)
		m_ppFunciones[i]->Imprimir(n + 2);
}

void CDefinicionFunciones::AgregarVariable(CDefinicionVariable *pVariable)
{
	m_ppVariables = (CDefinicionVariable**) realloc(m_ppVariables, sizeof(CDefinicionVariable*) * (m_Variables + 1));
	
	m_ppVariables[m_Variables] = pVariable;

	m_Variables++;
}

void CDefinicionFunciones::AgregarFuncion(CFuncion *pFuncion)
{
	m_ppFunciones = (CFuncion**) realloc(m_ppFunciones, sizeof(CFuncion*) * (m_Funciones + 1));
	
	m_ppFunciones[m_Funciones] = pFuncion;

	m_Funciones++;
}

int CDefinicionFunciones::BuscarPosicionFuncion(CCadenaConHash& Nombre)
{
	for (int i = 0; i < m_Funciones; i++)
		if (m_ppFunciones[i]->ObtenerNombre() == Nombre)
		{
			return(i);
			break;
		}
	return(-1);
}

int CDefinicionFunciones::BuscarPosicionVariable(CCadenaConHash& Nombre)
{
	for (int i = 0; i < m_Variables; i++)
		if (m_ppVariables[i]->ObtenerNombre() == Nombre)
		{
			return(i);
			break;
		}

	return(-1);
}

CFuncion* CDefinicionFunciones::BuscarFuncion(CCadenaConHash& Nombre)
{
	CFuncion* pFuncion = NULL;

	for (int i = 0; i < m_Funciones; i++)
		if (m_ppFunciones[i]->ObtenerNombre() == Nombre)
		{
			pFuncion = m_ppFunciones[i];
			break;
		}

	return(pFuncion);
}

bool CDefinicionFunciones::PrvValidarFuncion(CFuncion* pFuncion)
{
	bool bValid = true;

	if (pFuncion->ObtenerTipoFuncion() == CFuncion::FUNCION_NORMAL)
	{
		int i = 0;

		while(bValid && i < pFuncion->ObtenerListaInstrucciones()->ObtenerInstrucciones())
		{
			CListaInstrucciones::ParametroType param;

			if (pFuncion->ObtenerListaInstrucciones()->TieneParametro(i))
				param = pFuncion->ObtenerListaInstrucciones()->ObtenerParametro(i);

			switch(pFuncion->ObtenerListaInstrucciones()->ObtenerInstruccion(i))
			{
				case CListaInstrucciones::INST_NADA:
					break;

				case CListaInstrucciones::INST_SUMAR_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_RESTAR_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_MULT_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_DIV_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_SUMAR_FLOAT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_RESTAR_FLOAT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_MULT_FLOAT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_DIV_FLOAT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_CONCAT_STRING:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_COMPARE_STRING:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_ISZERO_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_FLOAT_A_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_INT_A_FLOAT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_INT:
					if (param < 0 ||
						param >= m_TablaConstantes.GetSimbolos() ||
						m_TablaConstantes.GetSimbolo(param)->GetType() != CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_INT)
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_FLOAT:
					if (param < 0 ||
						param >= m_TablaConstantes.GetSimbolos() ||
						m_TablaConstantes.GetSimbolo(param)->GetType() != CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_FLOAT)
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_STRING:
					if (param < 0 ||
						param >= m_TablaConstantes.GetSimbolos() ||
						m_TablaConstantes.GetSimbolo(param)->GetType() != CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_STRING)
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_VAR_INT:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_VAR_FLOAT:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_VAR_STRING:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_GLOBAL_VAR_INT:
					//FIX_ME Validar contra la otra definicion
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
					//FIX_ME Validar contra la otra definicion
					//FIX_ME Falta simulacion de stack..
					break;
				
				case CListaInstrucciones::INST_STACK_PUSH_GLOBAL_VAR_STRING:
					//FIX_ME Validar contra la otra definicion
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_DUP:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_POP:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_POP_VAR_INT:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_POP_VAR_FLOAT:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_POP_VAR_STRING:
					if (param < 0 ||
						param >= pFuncion->GetCantVariables())
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_STACK_POP_GLOBAL_VAR_INT:
					//FIX_ME Validar contra la otra definicion
					break;

				case CListaInstrucciones::INST_STACK_POP_GLOBAL_VAR_FLOAT:
					//FIX_ME Validar contra la otra definicion
					break;

				case CListaInstrucciones::INST_STACK_POP_GLOBAL_VAR_STRING:
					//FIX_ME Validar contra la otra definicion
					break;
				
				case CListaInstrucciones::INST_JUMP_IF_ZERO:
					//FIX_ME Falta simulacion de stack..
					//El break no esta a proposito
				case CListaInstrucciones::INST_JUMP:
				{
					int dest = i + sizeof(CListaInstrucciones::InstruccionType) + sizeof(CListaInstrucciones::ParametroType) + param;
					if (dest < 0 ||
						dest > pFuncion->ObtenerListaInstrucciones()->ObtenerInstrucciones() - (int) sizeof(CListaInstrucciones::InstruccionType)) //Tiene que haber espacio para el return
						bValid = false;
					//FIX_ME Falta chequear que no salte al medio de una instruccion
					//FIX_ME Falta simulacion de stack..
					break;
				}

				case CListaInstrucciones::INST_CALL_FUNCTION:
					if (param < 0 ||
						param >= m_Funciones)
						bValid = false;
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_CALL_GLOBAL_FUNCTION:
					if (param < 0 ||
						param >= m_TablaConstantes.GetSimbolos() ||
						m_TablaConstantes.GetSimbolo(param)->GetType() != CSimboloSimple::SIMBOLO_TIPO_CONSTANTE_STRING)
					//FIX_ME Validar contra la otra definicion
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_RETURN:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_INC_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				case CListaInstrucciones::INST_DEC_INT:
					//FIX_ME Falta simulacion de stack..
					break;

				default:
					bValid = false;
					break;
			}

			if (!pFuncion->ObtenerListaInstrucciones()->TieneParametro(i))
				i += sizeof(CListaInstrucciones::InstruccionType);
			else
				i += sizeof(CListaInstrucciones::InstruccionType) + sizeof(CListaInstrucciones::ParametroType);
		}

		if (bValid)
		{
			if (pFuncion->ObtenerListaInstrucciones()->ObtenerInstrucciones() >= sizeof(CListaInstrucciones::InstruccionType))
			{
				if (pFuncion->ObtenerListaInstrucciones()->ObtenerInstruccion(pFuncion->ObtenerListaInstrucciones()->ObtenerInstrucciones() - sizeof(CListaInstrucciones::InstruccionType)) != CListaInstrucciones::INST_RETURN)
					bValid = false;
			}
			else
			{
				bValid = false;
			}
		}
	}



	return(bValid);
}

bool CDefinicionFunciones::Validar(void)
{
	bool bValid = true;

	for (int i = 0; i < m_Funciones; i++)
	{
		if (!PrvValidarFuncion(m_ppFunciones[i]))
		{
			bValid = false;
			break;
		}
	}

	return(bValid);
}
