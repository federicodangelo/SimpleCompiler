#pragma once

class CDefinicionObjeto;


class CTipoDato
{
public:
	enum TipoDatoEnum
	{
		TIPO_NINGUNO,
		TIPO_VOID,
		TIPO_INT,
		TIPO_FLOAT,
		TIPO_CADENA,
	};

private:
	TipoDatoEnum		m_Tipo;

public:
	CTipoDato();
	~CTipoDato();

	CTipoDato(TipoDatoEnum Tipo);
	CTipoDato(const char* pcTipo);

	void SetearTipoDato(TipoDatoEnum Tipo) { m_Tipo = Tipo; }

	TipoDatoEnum ObtenerTipoDato(void) { return(m_Tipo); }

	static TipoDatoEnum TraducirTipoDato(const char* pcTipo);

	const char* ToString();


};

