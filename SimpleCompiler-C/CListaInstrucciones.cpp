#include "CListaInstrucciones.h"
#include "FuncionesUnix.h"

CListaInstrucciones::CListaInstrucciones()
{
	m_pInstrucciones = NULL;
	m_Instrucciones = 0;
}

CListaInstrucciones::~CListaInstrucciones()
{
	Limpiar();
}

void CListaInstrucciones::Limpiar(void)
{
	if (m_pInstrucciones)
	{
		free(m_pInstrucciones);

		m_pInstrucciones = NULL;

		m_Instrucciones = 0;
	}
}

int CListaInstrucciones::AgregarInstruccion(InstruccionesEnum Instruccion)
{
	m_pInstrucciones = (unsigned char*) realloc(m_pInstrucciones, m_Instrucciones + sizeof(InstruccionType));
	
	*(InstruccionType*) &m_pInstrucciones[m_Instrucciones] = (InstruccionType) Instruccion;

	int Pos = m_Instrucciones;

	m_Instrucciones += sizeof(InstruccionType);

	return(Pos);
}

int CListaInstrucciones::AgregarInstruccion(InstruccionesEnum Instruccion, ParametroType Parametro)
{
	m_pInstrucciones = (unsigned char*) realloc(m_pInstrucciones, m_Instrucciones + sizeof(InstruccionType) + sizeof(ParametroType));
	
	*(InstruccionType*) &m_pInstrucciones[m_Instrucciones] = (InstruccionType) Instruccion;
	*(ParametroType*) &m_pInstrucciones[m_Instrucciones + sizeof(InstruccionType)] = Parametro;

	int Pos = m_Instrucciones;

	m_Instrucciones += sizeof(InstruccionType) + sizeof(ParametroType);

	return(Pos);
}


void CListaInstrucciones::Imprimir(int n)
{
	int i = 0;

	while(i < m_Instrucciones)
	{
		for (int j = 0; j < n; j++)
			printf("    ");

		printf("%4d: ", i);

		switch(ObtenerInstruccion(i))
		{
			case INST_NADA:
				printf("Nada\n");
				break;

			case INST_SUMAR_INT:
				printf("Sumar int\n");
				break;

			case INST_RESTAR_INT:
				printf("Restar int\n");
				break;

			case INST_MULT_INT:
				printf("Multiplicar int\n");
				break;

			case INST_DIV_INT:
				printf("Dividir int\n");
				break;

			case INST_SUMAR_FLOAT:
				printf("Sumar float\n");
				break;

			case INST_RESTAR_FLOAT:
				printf("Restar float\n");
				break;

			case INST_MULT_FLOAT:
				printf("Multiplicar float\n");
				break;

			case INST_DIV_FLOAT:
				printf("Dividir float\n");
				break;

			case INST_CONCAT_STRING:
				printf("Concatenar strings\n");
				break;

			case INST_COMPARE_STRING:
				printf("Comparar strings\n");
				break;

			case INST_ISZERO_INT:
				printf("El int es igual a 0\n");
				break;

			case INST_FLOAT_A_INT:
				printf("Convertir float a int\n");
				break;

			case INST_INT_A_FLOAT:
				printf("Convertir int a float\n");
				break;

			case INST_STACK_PUSH_INT:
				printf("Push constante int: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_FLOAT:
				printf("Push constante float: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_STRING:
				printf("Push constante string: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_VAR_INT:
				printf("Push variable int: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_VAR_FLOAT:
				printf("Push variable float: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_VAR_STRING:
				printf("Push variable string: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_GLOBAL_VAR_INT:
				printf("Push variable global int: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
				printf("Push variable global float: %ld\n", ObtenerParametro(i));
				break;
			
			case INST_STACK_PUSH_GLOBAL_VAR_STRING:
				printf("Push variable global string: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_DUP:
				printf("Duplicar\n");
				break;

			case INST_STACK_POP:
				printf("Pop\n");
				break;

			case INST_STACK_POP_VAR_INT:
				printf("Pop variable int: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_POP_VAR_FLOAT:
				printf("Pop variable float: %ld \n", ObtenerParametro(i));
				break;

			case INST_STACK_POP_VAR_STRING:
				printf("Pop variable string: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_POP_GLOBAL_VAR_INT:
				printf("Pop variable global int: %ld\n", ObtenerParametro(i));
				break;

			case INST_STACK_POP_GLOBAL_VAR_FLOAT:
				printf("Pop variable global float: %ld \n", ObtenerParametro(i));
				break;

			case INST_STACK_POP_GLOBAL_VAR_STRING:
				printf("Pop variable global string: %ld\n", ObtenerParametro(i));
				break;

			case INST_JUMP:
				printf("Salto: %ld Destino: %d\n", ObtenerParametro(i), i + sizeof(InstruccionType) + sizeof(ParametroType) + ObtenerParametro(i));
				break;

			case INST_JUMP_IF_ZERO:
				printf("Saltar si int == 0: %ld Destino: %d\n", ObtenerParametro(i), i + sizeof(InstruccionType) + sizeof(ParametroType) + ObtenerParametro(i));
				break;

			case INST_CALL_FUNCTION:
				printf("Llamado a funcion local: %ld\n", ObtenerParametro(i));
				break;

			case INST_CALL_GLOBAL_FUNCTION:
				printf("Llamado a función global: %ld\n", ObtenerParametro(i));
				break;

			case INST_RETURN:
				printf("Retornar de función\n");
				break;

			case INST_INC_INT:
				printf("Incrementar int en 1\n");
				break;

			case INST_DEC_INT:
				printf("Decrementar int en 1\n");
				break;
		}

		if (!TieneParametro(i))
			i += sizeof(InstruccionType);
		else
			i += sizeof(InstruccionType) + sizeof(ParametroType);
	}
}

bool CListaInstrucciones::TieneParametro(int nInstruccion)
{
	return(*(InstruccionType*) &m_pInstrucciones[nInstruccion] >= INST_STACK_POP_VAR_INT);
}

void CListaInstrucciones::SetearInstruccion(int nInstruccion, InstruccionesEnum Instruccion)
{
	*(InstruccionType*) m_pInstrucciones[nInstruccion] = (InstruccionType) Instruccion;
}

void CListaInstrucciones::SetearParametro(int nInstruccion, ParametroType Parametro)
{
	*(ParametroType*) &m_pInstrucciones[nInstruccion + sizeof(InstruccionType)] = Parametro;
}
