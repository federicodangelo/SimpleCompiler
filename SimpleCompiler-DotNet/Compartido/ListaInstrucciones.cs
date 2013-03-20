using System;
using System.Collections;

namespace CompiladorReducido.Compartido
{
	public class ListaInstrucciones
	{
		public const int lenInstruccion = 1;
		public const int lenParametro = 2;

		public enum InstruccionesEnum
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
			INST_STACK_POP_VAR_OBJ, /* Toma: 1 objeto, Devuelve: Nada */ /* Parametros: número de variable local */
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
		}

		private byte[] instrucciones;
		private int nInstrucciones;
				
		public ListaInstrucciones(int maxInstrucciones)
		{
            instrucciones = new byte[maxInstrucciones];
			nInstrucciones = 0;
		}

		public void Crear(int maxInstrucciones)
		{
			Limpiar();

            instrucciones = new byte[maxInstrucciones];
		}
		
		public void Limpiar()
		{
			instrucciones = null;
            nInstrucciones = 0;
		}

		public int AgregarInstruccion(InstruccionesEnum instruccion)
		{
			int pos = nInstrucciones;

			instrucciones[nInstrucciones++] = (byte) instruccion;

			return pos;
		}

		public int AgregarInstruccion(InstruccionesEnum instruccion, short parametro)
		{
			int pos = nInstrucciones;

			instrucciones[nInstrucciones++] = (byte) instruccion;

			instrucciones[nInstrucciones++] = (byte) (((ushort) parametro) & 255);
			instrucciones[nInstrucciones++] = (byte) (((ushort) parametro) >> 8);

			return pos;
		}

		public int	ObtenerInstrucciones() 
		{ 
			return nInstrucciones; 
		}

		public bool TieneParametro(int nInstruccion)
		{
			return (ObtenerInstruccion(nInstruccion) >= InstruccionesEnum.INST_STACK_POP_VAR_INT);
		}

		public void SetearInstruccion(int nInstruccion, InstruccionesEnum instruccion)
		{
			instrucciones[nInstruccion] = (byte) instruccion;
		}

		public void SetearParametro(int nInstruccion, short parametro)
		{
			instrucciones[nInstruccion + 1] = (byte) (((ushort) parametro) & 255);
			instrucciones[nInstruccion + 2] = (byte) (((ushort) parametro) >> 8);
		}

		public InstruccionesEnum ObtenerInstruccion(int nInstruccion)
		{
			return (InstruccionesEnum) instrucciones[nInstruccion];
		}
	
		public short ObtenerParametroDesplazado(int nInstruccion)
		{
			ushort parametro = (ushort) instrucciones[nInstruccion++];
			parametro |= (ushort) (((ushort) instrucciones[nInstruccion]) << 8);

			return (short) parametro;
		}
	
		public short ObtenerParametro(int nInstruccion)
		{
			nInstruccion += lenInstruccion;
			ushort parametro = (ushort) instrucciones[nInstruccion++];
			parametro |= (ushort) (((ushort) instrucciones[nInstruccion]) << 8);

			return (short) parametro;
		}
		
		public void Imprimir(int n)
		{
			int i = 0;

			while(i < nInstrucciones)
			{
				for (int j = 0; j < n; j++)
					Console.Write("    ");

				Console.Write("{0}: ", i);

				switch(ObtenerInstruccion(i))
				{
					case InstruccionesEnum.INST_NADA:
						Console.Write("Nada\n");
						break;

					case InstruccionesEnum.INST_SUMAR_INT:
						Console.Write("Sumar int\n");
						break;

					case InstruccionesEnum.INST_RESTAR_INT:
						Console.Write("Restar int\n");
						break;

					case InstruccionesEnum.INST_MULT_INT:
						Console.Write("Multiplicar int\n");
						break;

					case InstruccionesEnum.INST_DIV_INT:
						Console.Write("Dividir int\n");
						break;

					case InstruccionesEnum.INST_SUMAR_FLOAT:
						Console.Write("Sumar float\n");
						break;

					case InstruccionesEnum.INST_RESTAR_FLOAT:
						Console.Write("Restar float\n");
						break;

					case InstruccionesEnum.INST_MULT_FLOAT:
						Console.Write("Multiplicar float\n");
						break;

					case InstruccionesEnum.INST_DIV_FLOAT:
						Console.Write("Dividir float\n");
						break;

					case InstruccionesEnum.INST_CONCAT_STRING:
						Console.Write("Concatenar strings\n");
						break;

					case InstruccionesEnum.INST_COMPARE_STRING:
						Console.Write("Comparar strings\n");
						break;

					case InstruccionesEnum.INST_ISZERO_INT:
						Console.Write("El int es igual a 0\n");
						break;

					case InstruccionesEnum.INST_FLOAT_A_INT:
						Console.Write("Convertir float a int\n");
						break;

					case InstruccionesEnum.INST_INT_A_FLOAT:
						Console.Write("Convertir int a float\n");
						break;

					case InstruccionesEnum.INST_STACK_PUSH_INT:
						Console.Write("Push constante int: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_FLOAT:
						Console.Write("Push constante float: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_STRING:
						Console.Write("Push constante string: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_VAR_INT:
						Console.Write("Push variable int: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_VAR_FLOAT:
						Console.Write("Push variable float: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_VAR_STRING:
						Console.Write("Push variable string: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
						Console.Write("Push variable global int: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
						Console.Write("Push variable global float: {0}\n", ObtenerParametro(i));
						break;
			
					case InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
						Console.Write("Push variable global string: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_DUP:
						Console.Write("Duplicar\n");
						break;

					case InstruccionesEnum.INST_STACK_POP:
						Console.Write("Pop\n");
						break;

					case InstruccionesEnum.INST_STACK_POP_VAR_INT:
						Console.Write("Pop variable int: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_VAR_FLOAT:
						Console.Write("Pop variable float: {0} \n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_VAR_OBJ:
						Console.Write("Pop variable objeto: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_VAR_STRING:
						Console.Write("Pop variable string: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_INT:
						Console.Write("Pop variable global int: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
						Console.Write("Pop variable global float: {0} \n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
						Console.Write("Pop variable global string: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_JUMP:
						Console.Write("Salto: {0} Destino: {1}\n", ObtenerParametro(i), i + ListaInstrucciones.lenInstruccion + ListaInstrucciones.lenParametro + ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_JUMP_IF_ZERO:
						Console.Write("Saltar si int == 0: {0} Destino: {1}\n", ObtenerParametro(i), i + ListaInstrucciones.lenInstruccion + ListaInstrucciones.lenParametro + ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_CALL_FUNCTION:
						Console.Write("Llamado a funcion local: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_CALL_GLOBAL_FUNCTION:
						Console.Write("Llamado a función global: {0}\n", ObtenerParametro(i));
						break;

					case InstruccionesEnum.INST_RETURN:
						Console.Write("Retornar de función\n");
						break;

					case InstruccionesEnum.INST_INC_INT:
						Console.Write("Incrementar int en 1\n");
						break;

					case InstruccionesEnum.INST_DEC_INT:
						Console.Write("Decrementar int en 1\n");
						break;
				}

				if (!TieneParametro(i))
					i += lenInstruccion;
				else
					i += lenInstruccion + lenParametro;
			}
		}
	}
}
