#include "CParser.h"
#include "COptimizador.h"
#include "CGeneradorCodigo.h"
#include "CProceso.h"
#include <stdlib.h>
#include <stdio.h>
#include <windows.h>

void LeerPrograma(char* pcBuffer, char* pcFileName)
{
	FILE* pfFile = fopen(pcFileName, "rb");

	pcBuffer[0] = '\0';

	int Offset = 0;

	if (pfFile)
	{
		while (!feof(pfFile))
			Offset += fread(&pcBuffer[Offset], 1, 8192, pfFile);

		pcBuffer[Offset] = '\0';

		fclose(pfFile);
	}
}

bool FuncNativaMostrarMensaje(CProceso* pProceso, CStack* pStack, CCadenaConHash& mensajeError)
{
	CObjetoCadena* pCadena = pStack->ObtenerVarString(0);

	printf("%s\n", (const char*) pCadena->ObtenerCadena());

	pStack->PushInt(123);

	return(true);
}

int __cdecl main(int argc, char* argv[])
{
	CFuncion funcionesExternas[1];

	funcionesExternas[0].ConfigurarDesdeDefinicion("int MostrarMensaje(string mensaje)");
	funcionesExternas[0].SetearFuncionNativa(&FuncNativaMostrarMensaje);
	funcionesExternas[0].SetearTipoFuncion(CFuncion::FUNCION_NATIVA);


	char pcProgram[8192];

	LeerPrograma(pcProgram, "Programa.txt");

	CParser Parser;
	COptimizador Optimizador;
	CGeneradorCodigo GeneradorCodigo;
	CSyntTree* pTree = NULL;

	Parser.SetProgram(pcProgram);
	Parser.SetFuncionesExternas(funcionesExternas, 1);

	if (Parser.GenSyntTree(&pTree))
	{
		pTree = Optimizador.Optimizar(pTree, COptimizador::OPTIMIZACION_NIVEL_0);

		pTree->Imprimir();

		if (GeneradorCodigo.GenerarCodigo(pTree))
		{
			printf("\nObjetos definidos: \n");

			GeneradorCodigo.GetDefinicionFunciones()->Imprimir(0);

			if (GeneradorCodigo.GetDefinicionFunciones()->Validar() == false)
			{
				printf("Lista de definiciones invalida\n");
				return(0);
			}

			delete pTree;

			printf("\nEjecutando el programa\n");

			CProceso proceso;

			proceso.AgregarDefiniciones(GeneradorCodigo.GetDefinicionFunciones());
			proceso.AgregarFuncionesExternas(funcionesExternas, 1);

			proceso.Inicializar();

			LARGE_INTEGER v1, v2, freq;

			QueryPerformanceCounter(&v1);

			proceso.EjecutarFuncion("void#main()");

/*			proceso.EjecutarFuncion("void#main()");

			proceso.EjecutarFuncion("void#main()");

			proceso.EjecutarFuncion("void#main()");*/

			QueryPerformanceCounter(&v2);

			QueryPerformanceFrequency(&freq);

			long t = (long) ((v2.QuadPart - v1.QuadPart) * 1000 / freq.QuadPart);
			
			printf("\nFin de ejecución. Tiempo: %d ms\n", t);
		}
	}

	return 0;
}
