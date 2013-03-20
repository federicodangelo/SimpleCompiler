#pragma once

class CListaInstrucciones
{
public:
	typedef unsigned char InstruccionType;
	typedef short ParametroType;

	enum InstruccionesEnum
	{
		/* Instrucciones sin ningun parametro */
		INST_NADA, /* Toma: nada, Devuelve: nada */ /* Parametros: ninguno */
		INST_SUMAR_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
		INST_RESTAR_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
		INST_MULT_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
		INST_DIV_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
			
		INST_SUMAR_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */
		INST_RESTAR_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */
		INST_MULT_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */
		INST_DIV_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */

		INST_CONCAT_STRING, /* Toma: 2 string, Devuelve: 1 string */ /* Parametros: ninguno */
		INST_COMPARE_STRING, /* Toma: 2 string, Devuelve: 1 int */ /* Parametros: ninguno */

		INST_ISZERO_INT, /* Toma: 1 int, Devuelve: 1 int */ /* Parametros: ninguno */

		INST_FLOAT_A_INT, /* Toma: 1 float, Devuelve: 1 int */ /* Parametros: ninguno */
		INST_INT_A_FLOAT, /* Toma: 1 int, Deuvuelve: 1 float */ /* Parametros: ninguno */
	
		INST_INC_INT, /* Toma: 1 int, Devuelve: 1 int */ /* Parametros: ninguno */
		INST_DEC_INT, /* Toma: 1 int, Devuelve: 1 int */ /* Parametros: ninguno */
			
		INST_STACK_DUP, /* Toma: 1 objeto/int/float, Devuelve: 2 objeto/int/float (duplica el objeto) */ /* Parametros: ninguno */

		INST_RETURN, /* Toma: Nada, Devuelve: Nada */ /* Parametros: ninguno */

		INST_STACK_POP, /* Toma: 1 objeto/int/float, Devuelve: Nada */ /* Parametros: ninguno */

		/* Instrucciones con 1 parametro */
		INST_STACK_POP_VAR_INT, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: número de variable local */
		INST_STACK_POP_VAR_FLOAT, /* Toma: 1 float, Devuelve: Nada */ /* Parametros: número de variable local */
		INST_STACK_POP_VAR_STRING, /* Toma: 1 string, Devuelve: Nada */ /* Parametros: número de variable local */

		INST_STACK_POP_GLOBAL_VAR_INT, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: número de constante donde esta el nombre de la variable */
		INST_STACK_POP_GLOBAL_VAR_FLOAT, /* Toma: 1 float, Devuelve: Nada */ /* Parametros: número de constante donde esta el nombre de la variable */
		INST_STACK_POP_GLOBAL_VAR_STRING, /* Toma: 1 string, Devuelve: Nada */ /* Parametros: número de constante donde esta el nombre de la variable */

		INST_STACK_PUSH_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: número de constante */
		INST_STACK_PUSH_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: número de constante */
		INST_STACK_PUSH_STRING, /* Toma: Nada, Devuelve: 1 objeto del tipo string */ /* Parametros: número de constante */

		INST_STACK_PUSH_VAR_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: número de variable local */
		INST_STACK_PUSH_VAR_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: número de variable local */
		INST_STACK_PUSH_VAR_STRING, /* Toma: Nada, Devuelve: 1 string */ /* Parametros: número de variable local */

		INST_STACK_PUSH_GLOBAL_VAR_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: número de constante donde esta el nombre de la variable */
		INST_STACK_PUSH_GLOBAL_VAR_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: número de constante donde esta el nombre de la variable */
		INST_STACK_PUSH_GLOBAL_VAR_STRING, /* Toma: Nada, Devuelve: 1 string */ /* Parametros: número de constante donde esta el nombre de la variable */

		INST_JUMP, /* Toma: Nada, Devuelve: Nada */ /* Parametros: número de lineas a saltar */
		INST_JUMP_IF_ZERO, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: número de lineas a saltar */
	
		INST_CALL_FUNCTION, /* Toma: Nada, Devuelve: Nada */ /* Parametros: número de función de instancia a ejecutar */
		INST_CALL_GLOBAL_FUNCTION, /* Toma: Nada, Devuelve: Nada */ /* Parametros: número de constante donde esta el nombre de la fuinción a ejecutar */
		INST_LAST
	};

private:
	unsigned char* m_pInstrucciones;
	int m_Instrucciones;

public:
	CListaInstrucciones();
	~CListaInstrucciones();

	void Limpiar(void);

	int AgregarInstruccion(InstruccionesEnum Instruccion);

	int AgregarInstruccion(InstruccionesEnum Instruccion, ParametroType Parametro);

	int	ObtenerInstrucciones(void) { return(m_Instrucciones); }

	bool TieneParametro(int nInstruccion);

	void SetearInstruccion(int nInstruccion, InstruccionesEnum Instruccion);
	void SetearParametro(int nInstruccion, ParametroType Parametro);

	inline InstruccionesEnum ObtenerInstruccion(int nInstruccion)
	{
		return((InstruccionesEnum) *(InstruccionType*) &m_pInstrucciones[nInstruccion]);
	}
	
	inline ParametroType ObtenerParametroDesplazado(int nInstruccion)
	{
		return(*(ParametroType*) &m_pInstrucciones[nInstruccion]);
	}
	
	inline ParametroType ObtenerParametro(int nInstruccion)
	{
		return(*(ParametroType*) &m_pInstrucciones[nInstruccion + sizeof(InstruccionType)]);
	}
	
	
	void Imprimir(int n);
};
