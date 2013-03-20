#pragma once
#include "CTipoDato.h"

class CObjeto;
class CObjetoCadena;

class CVariable
{
private:
	CTipoDato	m_TipoDato;
	long		m_Valor;

public:
	CVariable();
	~CVariable();

	CTipoDato* TipoDato(void) { return(&m_TipoDato); }

	void SetearInt(long value);
	void SetearFloat(float value);
	void SetearObjeto(CObjeto* value);
	void SetearString(CObjetoCadena* value);

	long ObtenerInt(void);
	float ObtenerFloat(void);
	CObjeto* ObtenerObjeto(void);
	CObjetoCadena* ObtenerString(void);
};

