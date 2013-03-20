#include "CSimboloSimple.h"
#include "CTablaSimbolos.h"
#include "FuncionesUnix.h"

CSimboloSimple::CSimboloSimple()
{
	m_Type = SIMBOLO_TIPO_CONSTANTE_INT;
}

void CSimboloSimple::Imprimir(int n)
{
	for (int i = 0; i < n; i++)
		printf("    ");

	switch(GetType())
	{
		case SIMBOLO_TIPO_CONSTANTE_INT:
			printf("Constante int: %ld\n", GetInt());
			break;

		case SIMBOLO_TIPO_CONSTANTE_FLOAT:
			printf("Constante float: %f\n", GetFloat());
			break;

		case SIMBOLO_TIPO_CONSTANTE_STRING:
			printf("Constante string: %s\n", GetString());
			break;
	}
}