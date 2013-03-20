#include "CSyntTree.h"
#include "FuncionesUnix.h"

CSyntTree::CSyntTree()
{
	m_Childs = 0;
	#ifndef DEBUG_TREE
		m_ppChilds = NULL;
	#endif
	m_pTablaSimbolos = NULL;
	m_pReturnType = NULL;
	m_pSimboloDefinicionFuncion = NULL;
}

CSyntTree::~CSyntTree()
{
	Clear();
}

void CSyntTree::Clear(void)
{
	if (m_ppChilds)
	{
		while (m_Childs != 0)
			delete m_ppChilds[--m_Childs];
		
		#ifndef DEBUG_TREE
			free(m_ppChilds);

			m_ppChilds = NULL;
		#endif
	}

	m_String.Limpiar();

	if (m_pTablaSimbolos)
	{
		delete m_pTablaSimbolos;
		m_pTablaSimbolos = NULL;
	}

	m_pSimboloDefinicionFuncion = NULL;
	m_pReturnType = NULL;
}

void CSyntTree::AddChild(CSyntTree* pNode)
{
	#ifndef DEBUG_TREE
		m_ppChilds = (CSyntTree**) realloc(m_ppChilds, sizeof(CSyntTree*) * (m_Childs + 1));
	#endif

	m_ppChilds[m_Childs] = pNode;

	m_Childs++;
}

void CSyntTree::Imprimir(int n)
{
	int i;

	for (i = 0; i < n; i++)
		printf("    ");

	int hijo = 0;

	switch(GetType())
	{
		case SYNT_INSTRUCCION:
			printf("Instrucción: \n");
			break;

		case SYNT_RETURN:
			printf("Return: \n");
			break;

		case SYNT_FOR:
			printf("Instrucción For:\n");
			
			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Inicialización:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Condicion:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Incremento:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Ciclo:\n");
			GetChild(hijo++)->Imprimir(n + 2);
			break;

		case SYNT_WHILE:
			printf("Instrucción While:\n");
			
			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Condición:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Ciclo:\n");
			GetChild(hijo++)->Imprimir(n + 2);
			break;

		case SYNT_IF:
			printf("Instrucción If:\n");
			
			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Condición:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			for (i = 0; i < n + 1; i++)
				printf("    ");
			printf("Si es verdad:\n");
			GetChild(hijo++)->Imprimir(n + 2);

			if (GetChilds() == 3)
			{
				for (i = 0; i < n + 1; i++)
					printf("    ");
				printf("Si es falso:\n");
				GetChild(hijo++)->Imprimir(n + 2);
			}
			break;

		case SYNT_INSTRUCCION_NULA:
			printf("Instrucción nula\n");
			break;

		case SYNT_LISTA_FUNCIONES:
			printf("Lista de funciones:\n");
			break;

		case SYNT_LISTA_INSTRUCCIONES:
			printf("Lista de instrucciones:\n");
			break;

		case SYNT_IDENTIFICADOR:
			printf("Identificador: %s\n", GetString());
			break;

		case SYNT_VARIABLE:
			printf("Variable: %s\n", GetString());
			break;
		
		case SYNT_CONSTANTE:
			printf("Valor constante ");

			if (strcmp(GetReturnType()->GetNombre(), SIMBOLO_INT) == 0)
				printf("(entero) %ld\n", GetInteger());
			else
				if (strcmp(GetReturnType()->GetNombre(), SIMBOLO_FLOAT) == 0)
					printf("(float) %f\n", GetFloat());
				else
					if (strcmp(GetReturnType()->GetNombre(), SIMBOLO_STRING) == 0)
						printf("(cadena) %s\n", GetString());
			break;

		case SYNT_OP_ASIGNACION:
			printf ("Operador =\n");
			break;

		case SYNT_OP_MAS:
			printf ("Operador +\n");
			break;

		case SYNT_OP_MENOS:
			printf ("Operador -\n");
			break;

		case SYNT_OP_POR:
			printf ("Operador *\n");
			break;

		case SYNT_OP_DIV:
			printf ("Operador /\n");
			break;

		case SYNT_LLAMADO_FUNCION:
			printf("LLamado a función: %s\n", GetString());
			break;

		case SYNT_DECLARACION_VARIABLE:
			printf("Declaración de variables del tipo %s\n", GetString());
			break;

		case SYNT_DECLARACION_FUNCION:
			printf("Declaración de la función: %s\n", GetString());
			break;
		
		case SYNT_CONVERSION:
			printf("Conversion desde %s a %s\n", GetChild(0)->GetReturnType()->GetNombre(), GetReturnType()->GetNombre());
			break;

		case SYNT_TABLA_SIMBOLOS:
			printf("Tabla de simbolos\n");
			m_pTablaSimbolos->Imprimir(n + 1);
			break;
	}

	n++;

	for (; hijo < GetChilds(); hijo++)
		GetChild(hijo)->Imprimir(n);
}

