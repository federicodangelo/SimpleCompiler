#include "CSimbolo.h"
#include "CTablaSimbolos.h"
#include "FuncionesUnix.h"

CSimbolo::CSimbolo()
{
	m_Type = SIMBOLO_TIPO_VARIABLE;
	m_pReturnType = NULL;
	m_ppListaParametros = NULL;
	m_Parametros = 0;
	m_pTablaSimbolos = NULL;
	m_bFuncionNativa = false;
	m_CantVariables = 0;
}

CSimbolo::~CSimbolo()
{
	Clear();
}

CSimbolo::CSimbolo(CSimbolo& Simbolo)
{
	*this = Simbolo;
}

CSimbolo& CSimbolo::operator=(CSimbolo& Simbolo)
{
	if (&Simbolo != this)
	{
		Clear();

		SetNombre(Simbolo.GetNombre());
		SetNombreFuncionCompleto(Simbolo.GetNombreFuncionCompleto());
		SetType(Simbolo.GetType());
		SetReturnType(Simbolo.GetReturnType());
		SetTablaSimbolos(Simbolo.GetTablaSimbolos());
		SetFuncionNativa(Simbolo.GetFuncionNativa());

		for (int i = 0; i < Simbolo.GetParametros(); i++)
			AddParametro(new CSimbolo(*Simbolo.GetParametro(i)));
	}

	return(*this);
}

void CSimbolo::Clear(void)
{
	m_Nombre.Limpiar();
	m_NombreFuncionCompleto.Limpiar();

	if (m_ppListaParametros)
	{
		//Todos los simbolos dentro de la lista de parametros tambien pertenecen
		//a alguna tabla de simbolos, asi que seran limpiados en ese momento..
		/*while (m_Parametros != 0)
			delete m_ppListaParametros[--m_Parametros];*/
		

		free(m_ppListaParametros);

		m_ppListaParametros = NULL;
	}

	m_pTablaSimbolos = NULL;

	m_Parametros = 0;

	m_bFuncionNativa = false;
}

void CSimbolo::AddParametro(CSimbolo* pParametro)
{
	m_ppListaParametros = (CSimbolo**) realloc(m_ppListaParametros, sizeof(CSimbolo*) * (m_Parametros + 1));
	
	m_ppListaParametros[m_Parametros] = pParametro;

	m_Parametros++;
}

void CSimbolo::Imprimir(int n)
{
	for (int i = 0; i < n; i++)
		printf("    ");

	char* pcTipo;

	switch(GetType())
	{
		case SIMBOLO_TIPO_DATO:
			pcTipo = "Tipo de Dato";
			break;

		case SIMBOLO_TIPO_VARIABLE:
			pcTipo = "Variable";
			break;

		case SIMBOLO_TIPO_FUNCION:
			pcTipo = "Funcion";
			break;
	}

	printf("Simbolo: %s, Tipo: %s", GetNombre(), pcTipo);

	if (GetReturnType() != NULL)
		printf(", Devuelve: %s\n", GetReturnType()->GetNombre());
	else
		printf("\n");

	if (m_Parametros)
	{
		int i;

		for (i = 0; i < n + 1; i++)
			printf("    ");

		printf("Parametros: \n");
		
		for (i = 0; i < m_Parametros; i++)
			m_ppListaParametros[i]->Imprimir(n + 2);
	}

	if (GetType() == SIMBOLO_TIPO_FUNCION)
	{
		for (int i = 0; i < n + 1; i++)
			printf("    ");

		printf("Cantidad de variables locales usadas: %d\n", GetCantVariables());
	}

	if (m_pTablaSimbolos)
	{
		for (int i = 0; i < n + 1; i++)
			printf("    ");

		printf("Simbolos adicionales: \n");

		m_pTablaSimbolos->Imprimir(n + 2);
	}
}

void CSimbolo::ActualizarNombreCompletoFuncion(void)
{
	char pcNombre[1024];
	char pcSeparador[2];
	char pcSeparadorFuncion[2];

	pcSeparador[0] = SIMBOLO_SEPARADOR_PARAMETRO;
	pcSeparador[1] = '\0';

	pcSeparadorFuncion[0] = SIMBOLO_SEPARADOR_FUNCION;
	pcSeparadorFuncion[1] = '\0';

	pcNombre[0] = '\0';

	if (m_pReturnType == NULL)
		strcat(pcNombre, SIMBOLO_VOID);
	else
		strcat(pcNombre, m_pReturnType->GetNombre());

	strcat(pcNombre, pcSeparadorFuncion);
	strcat(pcNombre, GetNombre());
	strcat(pcNombre, "(");

	for (int i = 0; i < GetParametros(); i++)
	{
		if (i != 0)
			strcat(pcNombre, pcSeparador);
		strcat(pcNombre, GetParametro(i)->GetReturnType()->GetNombre());
	}

	strcat(pcNombre, ")");

	m_NombreFuncionCompleto.SetearCadena(pcNombre);
}
