#include "CDefinicionVariable.h"
#include "FuncionesUnix.h"

CDefinicionVariable::CDefinicionVariable()
{
}

CDefinicionVariable::~CDefinicionVariable()
{
}

void CDefinicionVariable::Imprimir(int n)
{
	int i;

	for (i = 0; i < n; i++)
		printf("    ");

	const char* pcTipo;

	switch(m_Tipo.ObtenerTipoDato())
	{
		case CTipoDato::TIPO_INT:
			pcTipo = "int";
			break;
		case CTipoDato::TIPO_FLOAT:
			pcTipo = "float";
			break;
		case CTipoDato::TIPO_CADENA:
			pcTipo = "string";
			break;
	}

	printf("Nombre: %s, Tipo de Dato: %s\n", ObtenerNombre().ObtenerCadena(), pcTipo);
}