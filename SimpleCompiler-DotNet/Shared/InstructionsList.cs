using System;
using System.Collections;

namespace SimpleCompiler.Shared
{
	public class InstructionsList
	{
		public const int LEN_INSTRUCTION = 1;
		public const int LEN_PARAMETER = 2;

		public enum InstructionsEnum
		{
			/* Instrucciones sin ningun parametro */
			INST_NULL, /* Toma: nada, Devuelve: nada */ /* Parametros: ninguno */
			INST_ADD_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
			INST_SUB_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
			INST_MULT_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
			INST_DIV_INT, /* Toma: 2 int, Devuelve: 1 int */ /* Parametros: ninguno */
				
			INST_ADD_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */
			INST_SUB_FLOAT, /* Toma: 2 float, Devuelve: 1 float */ /* Parametros: ninguno */
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
			INST_STACK_POP_VAR_INT, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: n�mero de variable local */
			INST_STACK_POP_VAR_FLOAT, /* Toma: 1 float, Devuelve: Nada */ /* Parametros: n�mero de variable local */
			INST_STACK_POP_VAR_OBJ, /* Toma: 1 objeto, Devuelve: Nada */ /* Parametros: n�mero de variable local */
			INST_STACK_POP_VAR_STRING, /* Toma: 1 string, Devuelve: Nada */ /* Parametros: n�mero de variable local */

			INST_STACK_POP_GLOBAL_VAR_INT, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: n�mero de constante donde esta el nombre de la variable */
			INST_STACK_POP_GLOBAL_VAR_FLOAT, /* Toma: 1 float, Devuelve: Nada */ /* Parametros: n�mero de constante donde esta el nombre de la variable */
			INST_STACK_POP_GLOBAL_VAR_STRING, /* Toma: 1 string, Devuelve: Nada */ /* Parametros: n�mero de constante donde esta el nombre de la variable */

			INST_STACK_PUSH_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: n�mero de constante */
			INST_STACK_PUSH_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: n�mero de constante */
			INST_STACK_PUSH_STRING, /* Toma: Nada, Devuelve: 1 objeto del tipo string */ /* Parametros: n�mero de constante */

			INST_STACK_PUSH_VAR_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: n�mero de variable local */
			INST_STACK_PUSH_VAR_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: n�mero de variable local */
			INST_STACK_PUSH_VAR_STRING, /* Toma: Nada, Devuelve: 1 string */ /* Parametros: n�mero de variable local */

			INST_STACK_PUSH_GLOBAL_VAR_INT, /* Toma: Nada, Devuelve: 1 int */ /* Parametros: n�mero de constante donde esta el nombre de la variable */
			INST_STACK_PUSH_GLOBAL_VAR_FLOAT, /* Toma: Nada, Devuelve: 1 float */ /* Parametros: n�mero de constante donde esta el nombre de la variable */
			INST_STACK_PUSH_GLOBAL_VAR_STRING, /* Toma: Nada, Devuelve: 1 string */ /* Parametros: n�mero de constante donde esta el nombre de la variable */

			INST_JUMP, /* Toma: Nada, Devuelve: Nada */ /* Parametros: n�mero de lineas a saltar */
			INST_JUMP_IF_ZERO, /* Toma: 1 int, Devuelve: Nada */ /* Parametros: n�mero de lineas a saltar */
		
			INST_CALL_FUNCTION, /* Toma: Nada, Devuelve: Nada */ /* Parametros: n�mero de funci�n de instancia a ejecutar */
			INST_CALL_GLOBAL_FUNCTION, /* Toma: Nada, Devuelve: Nada */ /* Parametros: n�mero de constante donde esta el nombre de la fuinci�n a ejecutar */
			INST_LAST
		}

		private byte[] instructions;
		private int nInstructions;
				
		public InstructionsList(int maxInstructions)
		{
            instructions = new byte[maxInstructions];
			nInstructions = 0;
		}

		public void Create(int maxInstructions)
		{
			Clear();

            instructions = new byte[maxInstructions];
		}
		
		public void Clear()
		{
			instructions = null;
            nInstructions = 0;
		}

		public int AddInstruction(InstructionsEnum instruction)
		{
			int pos = nInstructions;

			instructions[nInstructions++] = (byte) instruction;

			return pos;
		}

		public int AddInstruction(InstructionsEnum instruccion, short parametro)
		{
			int pos = nInstructions;

			instructions[nInstructions++] = (byte) instruccion;

			instructions[nInstructions++] = (byte) (((ushort) parametro) & 255);
			instructions[nInstructions++] = (byte) (((ushort) parametro) >> 8);

			return pos;
		}

		public int GetInstructions() 
		{ 
			return nInstructions; 
		}

		public bool HasParameter(int nInstruccion)
		{
			return (GetInstruction(nInstruccion) >= InstructionsEnum.INST_STACK_POP_VAR_INT);
		}

		public void SetInstruction(int nInstruccion, InstructionsEnum instruccion)
		{
			instructions[nInstruccion] = (byte) instruccion;
		}

		public void SetParameter(int nInstruccion, short parametro)
		{
			instructions[nInstruccion + 1] = (byte) (((ushort) parametro) & 255);
			instructions[nInstruccion + 2] = (byte) (((ushort) parametro) >> 8);
		}

		public InstructionsEnum GetInstruction(int nInstruccion)
		{
			return (InstructionsEnum) instructions[nInstruccion];
		}
	
		public short GetParameterOffsetted(int nInstruccion)
		{
			ushort parametro = (ushort) instructions[nInstruccion++];
			parametro |= (ushort) (((ushort) instructions[nInstruccion]) << 8);

			return (short) parametro;
		}
	
		public short GetParameter(int nInstruccion)
		{
			nInstruccion += LEN_INSTRUCTION;
			ushort parametro = (ushort) instructions[nInstruccion++];
			parametro |= (ushort) (((ushort) instructions[nInstruccion]) << 8);

			return (short) parametro;
		}
		
		public void Print(int n)
		{
			int i = 0;

			while(i < nInstructions)
			{
				for (int j = 0; j < n; j++)
					Console.Write("    ");

				Console.Write("{0}: ", i);

				switch(GetInstruction(i))
				{
					case InstructionsEnum.INST_NULL:
						Console.Write("Null\n");
						break;

					case InstructionsEnum.INST_ADD_INT:
						Console.Write("Add int\n");
						break;

					case InstructionsEnum.INST_SUB_INT:
						Console.Write("Subtract int\n");
						break;

					case InstructionsEnum.INST_MULT_INT:
						Console.Write("Multiply int\n");
						break;

					case InstructionsEnum.INST_DIV_INT:
						Console.Write("Divide int\n");
						break;

					case InstructionsEnum.INST_ADD_FLOAT:
						Console.Write("Add float\n");
						break;

					case InstructionsEnum.INST_SUB_FLOAT:
						Console.Write("Subtract float\n");
						break;

					case InstructionsEnum.INST_MULT_FLOAT:
						Console.Write("Multiply float\n");
						break;

					case InstructionsEnum.INST_DIV_FLOAT:
						Console.Write("Divide float\n");
						break;

					case InstructionsEnum.INST_CONCAT_STRING:
						Console.Write("Concatenate strings\n");
						break;

					case InstructionsEnum.INST_COMPARE_STRING:
						Console.Write("Compare strings\n");
						break;

					case InstructionsEnum.INST_ISZERO_INT:
						Console.Write("Int is 0 (zero)\n");
						break;

					case InstructionsEnum.INST_FLOAT_A_INT:
						Console.Write("Convert float to int\n");
						break;

					case InstructionsEnum.INST_INT_A_FLOAT:
						Console.Write("Convert int to float\n");
						break;

					case InstructionsEnum.INST_STACK_PUSH_INT:
						Console.Write("Push constant int: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_FLOAT:
                        Console.Write("Push constant float: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_STRING:
                        Console.Write("Push constant string: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_VAR_INT:
						Console.Write("Push variable int: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_VAR_FLOAT:
						Console.Write("Push variable float: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_VAR_STRING:
						Console.Write("Push variable string: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
						Console.Write("Push variable global int: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
						Console.Write("Push variable global float: {0}\n", GetParameter(i));
						break;
			
					case InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
						Console.Write("Push variable global string: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_DUP:
						Console.Write("Duplicate\n");
						break;

					case InstructionsEnum.INST_STACK_POP:
						Console.Write("Pop\n");
						break;

					case InstructionsEnum.INST_STACK_POP_VAR_INT:
						Console.Write("Pop variable int: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_VAR_FLOAT:
						Console.Write("Pop variable float: {0} \n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_VAR_OBJ:
						Console.Write("Pop variable object: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_VAR_STRING:
						Console.Write("Pop variable string: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_INT:
						Console.Write("Pop variable global int: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
						Console.Write("Pop variable global float: {0} \n", GetParameter(i));
						break;

					case InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
						Console.Write("Pop variable global string: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_JUMP:
						Console.Write("Jump: {0} Destination: {1}\n", GetParameter(i), i + InstructionsList.LEN_INSTRUCTION + InstructionsList.LEN_PARAMETER + GetParameter(i));
						break;

					case InstructionsEnum.INST_JUMP_IF_ZERO:
						Console.Write("Jump if int == 0: {0} Defination: {1}\n", GetParameter(i), i + InstructionsList.LEN_INSTRUCTION + InstructionsList.LEN_PARAMETER + GetParameter(i));
						break;

					case InstructionsEnum.INST_CALL_FUNCTION:
						Console.Write("Local function call: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_CALL_GLOBAL_FUNCTION:
						Console.Write("Global function call: {0}\n", GetParameter(i));
						break;

					case InstructionsEnum.INST_RETURN:
						Console.Write("Return from function\n");
						break;

					case InstructionsEnum.INST_INC_INT:
						Console.Write("Add 1 to int\n");
						break;

					case InstructionsEnum.INST_DEC_INT:
						Console.Write("Remove 1 from int\n");
						break;
				}

				if (!HasParameter(i))
					i += LEN_INSTRUCTION;
				else
					i += LEN_INSTRUCTION + LEN_PARAMETER;
			}
		}
	}
}
