#include "COptimizador.h"

COptimizador::COptimizador()
{
}

COptimizador::~COptimizador()
{
}

CSyntTree* COptimizador::Optimizar(CSyntTree* pTree, COptimizador::NivelOptimizacionEnum Nivel)
{
	m_NivelOptimizacion = Nivel;

	return(pTree);
}