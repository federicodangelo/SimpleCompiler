#include "CFuncion.h"
#include "FuncionesUnix.h"

CFuncion::CFuncion()
{
	m_ppParametros = NULL;
	m_Parametros = 0;
	m_ManejadoresExcepciones = 0;
	m_TipoFuncion = FUNCION_NORMAL;
	m_pFuncionNativa = NULL;
	m_CantVariables = 0;
}

CFuncion::~CFuncion()
{
	Limpiar();
}

void CFuncion::Limpiar(void)
{
	if (m_ppParametros)
	{
		while(m_Parametros != 0)
			delete m_ppParametros[--m_Parametros];

		free(m_ppParametros);

		m_ppParametros = NULL;
	}
}

void CFuncion::ConfigurarDesdeDefinicion(const char* pcDef)
{
	bool ok = false;

	char* pcTemp = (char*) malloc(sizeof(char) * (strlen(pcDef) + 1));

	try
	{
		//Inicio
		while(*pcDef == ' ')
			pcDef++;
		//

		//Tipo Dato
		char* pcDest = pcTemp;
        
		while(*pcDef != ' ' && *pcDef != '\0')
			*(pcDest++) = *(pcDef++);

		*pcDest = '\0';

		m_TipoDevuelto.SetearTipoDato(CTipoDato::TraducirTipoDato(pcTemp));

		if (*pcDef == ' ')
		{
            *(pcDest++) = ' ';

			while(*pcDef == ' ')
				pcDef++;
		}
		//

		//Nombre Funcion
		pcDest = pcTemp;

        while(*pcDef != ' ' && *pcDef != '(' && *pcDef != '\0')
			*(pcDest++) = *(pcDef++);

		*pcDest = '\0';

		m_NombreCorto.SetearCadena(pcTemp);

		while(*pcDef == ' ')
			pcDef++;

		//Parametros
        if (*pcDef == '(')
			*(pcDest++) = *(pcDef++);
        
		while(*pcDef != ')' && *pcDef != '\0')
		{
            while(*pcDef == ' ')
				pcDef++;

			pcDest = pcTemp;
			//tipo de dato
			while(*pcDef != ' ' && *pcDef != '\0')
				*(pcDest++) = *(pcDef++);

			*pcDest = '\0';

			CDefinicionVariable* pParametro = new CDefinicionVariable();

			AddParametro(pParametro);

			pParametro->TipoDato()->SetearTipoDato(CTipoDato::TraducirTipoDato(pcTemp));

			if (*pcDef == ' ')
			{
				while(*pcDef == ' ')
					pcDef++;

				//nombre parametro
				pcDest = pcTemp;
				while(*pcDef != ' ' && *pcDef != '\0' && *pcDef != ',')
					*(pcDest++) = *(pcDef++);

				*pcDest = '\0';
				pParametro->SetearNombre(pcTemp);

				while(*pcDef == ' ')
					pcDef++;

				if (*pcDef == ',')
                    pcDef++;

				ActualizarNombreCompleto();
			}
		}
	}
	catch(...)
	{
		free(pcTemp);
		throw;
	}
	free(pcTemp);
}

void CFuncion::ActualizarNombreCompleto()
{
	CCadenaConHash nombre;

	nombre.ConcatenarCadena(m_TipoDevuelto.ToString());

	nombre.ConcatenarCadena("#");
	nombre.ConcatenarCadena(m_NombreCorto);
	nombre.ConcatenarCadena("(");

	for (int i = 0; i < GetParametros(); i++)
	{
		if (i != 0)
			nombre.ConcatenarCadena(",");

		nombre.ConcatenarCadena(GetParametro(i)->TipoDato()->ToString());
	}

	nombre.ConcatenarCadena(")");

	m_Nombre = nombre;
}

void CFuncion::AddParametro(CDefinicionVariable* pParametro)
{
	m_ppParametros = (CDefinicionVariable**) realloc(m_ppParametros, sizeof(CDefinicionVariable*) * (m_Parametros + 1));
	
	m_ppParametros[m_Parametros] = pParametro;
	
	m_Parametros++;
}

void CFuncion::Imprimir(int n)
{
	int i;

	for (i = 0; i < n; i++)
		printf("    ");

	printf("Funcion: %s\n", ObtenerNombre());

	for (i = 0; i < n + 1; i++)
		printf("    ");

	printf("Parametros:\n");
	for (i = 0; i < m_Parametros; i++)
		m_ppParametros[i]->Imprimir(n + 2);
	
	for (i = 0; i < n + 1; i++)
		printf("    ");
	
	printf("Cantidad de variables locales: %d\n", GetCantVariables());
	
	for (i = 0; i < n + 1; i++)
		printf("    ");
	
	printf("Instrucciones:\n");
	m_ListaInstrucciones.Imprimir(n + 2);
}

