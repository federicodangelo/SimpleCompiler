#include "CVariable.h"
#include "CErrorEjecucion.h"
#include "CObjetoCadena.h"
#include "FuncionesUnix.h"

CVariable::CVariable()
{
	m_Valor = 0;
}

CVariable::~CVariable()
{
	if (m_TipoDato.ObtenerTipoDato() == CTipoDato::TIPO_CADENA)
		if (m_Valor != 0)
			(*(CObjetoCadena**) &m_Valor)->DecrementarUso();
}

void CVariable::SetearInt(long value)
{
	if (m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_NINGUNO)
	{
		ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_INT, "Tipo de variable incorrecto");
	}
	else
		m_TipoDato.SetearTipoDato(CTipoDato::TIPO_INT);

	(*(long*) &m_Valor) = value;
}

void CVariable::SetearFloat(float value)
{
	if (m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_NINGUNO)
	{
		ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_FLOAT, "Tipo de variable incorrecto");
	}
	else
		m_TipoDato.SetearTipoDato(CTipoDato::TIPO_FLOAT);

	(*(float*) &m_Valor) = value;
}

void CVariable::SetearString(CObjetoCadena* value)
{
	if (m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_NINGUNO)
	{
		ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_CADENA, "Tipo de variable incorrecto");

		if (m_Valor != 0)
			(*(CObjetoCadena**) &m_Valor)->DecrementarUso();
	}
	else
	{
		m_TipoDato.SetearTipoDato(CTipoDato::TIPO_CADENA);
	}

	(*(CObjetoCadena**) &m_Valor) = value;

	if (value != NULL)
		value->IncrementarUso();
}

long CVariable::ObtenerInt(void)
{
	ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_INT, "Tipo de variable incorrecto");
	return(*(long*) &m_Valor);
}

float CVariable::ObtenerFloat(void)
{
	ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_FLOAT, "Tipo de variable incorrecto");
	return(*(float*) &m_Valor);
}

CObjetoCadena* CVariable::ObtenerString(void)
{
	ThrowErrorEjecucionSi(m_TipoDato.ObtenerTipoDato() != CTipoDato::TIPO_CADENA, "Tipo de variable incorrecto");

	ThrowErrorEjecucionSi(m_Valor == NULL, "El valor almacenado en la variable es NULL, no puede obtenerse este valor");

	return(*(CObjetoCadena**) &m_Valor);
}
