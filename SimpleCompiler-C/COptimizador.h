#pragma once
#include "CSyntTree.h"

class COptimizador
{
public:
	enum NivelOptimizacionEnum
	{
		OPTIMIZACION_NIVEL_0,	//Elimina el c�digo que no se utiliza
		OPTIMIZACION_NIVEL_1
	};

private:
	NivelOptimizacionEnum m_NivelOptimizacion;

public:
	COptimizador();
	~COptimizador();

	CSyntTree* Optimizar(CSyntTree* pTree, NivelOptimizacionEnum Nivel);
};