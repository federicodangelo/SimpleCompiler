using System;
using System.Collections.Generic;

namespace SimpleCompiler.Shared
{
	public class FunctionsDefinitions
	{
		public const int NOT_FOUND = -1;

		public SymbolConstantTable ConstantsTable;

		private List<VariableDefinition> variables;
		private List<Function> functions;
				
		public FunctionsDefinitions()
		{
            variables = new List<VariableDefinition>();
            functions = new List<Function>();
			ConstantsTable = new SymbolConstantTable();
		}

		public void Limpiar()
		{
			variables.Clear();
			functions.Clear();
			ConstantsTable.Clear();
		}

		public int GetVariables() 
		{ 
			return variables.Count; 
		}
		
		public VariableDefinition GetVariable(int n) 
		{ 
			return (VariableDefinition) variables[n]; 
		}

		public void AddVariable(VariableDefinition variable)
		{
			variables.Add(variable);
		}

		public int GetFunctions() 
		{ 
			return functions.Count;
		}

		public Function GetFunction(int n) 
		{ 
			return (Function) functions[n];
		}

		public void AddFunction(Function funcion)
		{
			functions.Add(funcion);
		}

		public int FunctionFunctionPosition(string name)
		{
			for (int i = 0; i < functions.Count; i++)
				if (((Function) functions[i]).Name == name)
					return(i);

			return -1;
		}

		public int FindVariablePosition(string name)
		{
			for (int i = 0; i < variables.Count; i++)
				if (((VariableDefinition) variables[i]).Name == name)
					return(i);

			return -1;
		}

		public Function FindFunction(string nombre)
		{
			for (int i = 0; i < functions.Count; i++)
				if (((Function) functions[i]).Name == nombre)
					return (Function) functions[i];

			return null;
		}

		public void Print(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Constants:\n");
			ConstantsTable.Print(n + 1);

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Variables:\n");
			foreach(VariableDefinition variable in variables)
				variable.Print(n + 2);

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Functions:\n");
			foreach(Function funcion in functions)
				funcion.Print(n + 1);
		}

		public bool Validate()
		{
			bool valid = true;

			foreach(Function funcion in functions)
            {
				if (!PrvValidateFunction(funcion))
				{
					valid = false;
					break;
				}
            }

			return valid;
		}

		private bool PrvValidateFunction(Function funcion)
		{
			bool valid = true;

			if (funcion.FunctionType == Function.FuncionTypeEnum.FUNCTION_NORMAL)
			{
				int i = 0;

				while(valid && i < funcion.InstructionsList.GetInstructions())
				{
					short param = 0;

					if (funcion.InstructionsList.HasParameter(i))
						param = funcion.InstructionsList.GetParameter(i);

					switch(funcion.InstructionsList.GetInstruction(i))
					{
						case InstructionsList.InstructionsEnum.INST_NULL:
							break;

						case InstructionsList.InstructionsEnum.INST_ADD_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_SUB_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_MULT_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_DIV_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_ADD_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_SUB_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_MULT_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_DIV_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_CONCAT_STRING:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_COMPARE_STRING:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_ISZERO_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_FLOAT_A_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_INT_A_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT:
							if (param < 0 ||
								param >= ConstantsTable.GetSimbolos() ||
								ConstantsTable.GetSymbol(param).Type != SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_INT)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT:
							if (param < 0 ||
								param >= ConstantsTable.GetSimbolos() ||
								ConstantsTable.GetSymbol(param).Type != SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_FLOAT)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING:
							if (param < 0 ||
								param >= ConstantsTable.GetSimbolos() ||
								ConstantsTable.GetSymbol(param).Type != SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_INT:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_FLOAT:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_STRING:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;
				
						case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_DUP:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_OBJ:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING:
							if (param < 0 ||
								param >= funcion.NumberOfVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_INT:
							//FIX_ME Validar contra la otra definicion
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
							//FIX_ME Validar contra la otra definicion
							break;

						case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
							//FIX_ME Validar contra la otra definicion
							break;

						case InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO:
							//FIX_ME Falta simulacion de stack..
							//El break no esta a proposito
						case InstructionsList.InstructionsEnum.INST_JUMP:
						{
							int dest = i + InstructionsList.LEN_INSTRUCTION + InstructionsList.LEN_PARAMETER + param;
							if (dest < 0 ||
								dest > funcion.InstructionsList.GetInstructions() - InstructionsList.LEN_INSTRUCTION) //Tiene que haber espacio para el return
								valid = false;
							//FIX_ME Falta chequear que no salte al medio de una instruccion
							//FIX_ME Falta simulacion de stack..
							break;
						}

						case InstructionsList.InstructionsEnum.INST_CALL_FUNCTION:
							if (param < 0 ||
								param >= functions.Count)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_CALL_GLOBAL_FUNCTION:
							if (param < 0 ||
								param >= ConstantsTable.GetSimbolos() ||
								ConstantsTable.GetSymbol(param).Type != SymbolConstant.SymbolTypeEnum.SYMBOL_TYPE_CONSTANT_STRING)
							{
								valid = false;
							}
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_RETURN:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_INC_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case InstructionsList.InstructionsEnum.INST_DEC_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						default:
							valid = false;
							break;
					}

					if (!funcion.InstructionsList.HasParameter(i))
						i += InstructionsList.LEN_INSTRUCTION;
					else
						i += InstructionsList.LEN_INSTRUCTION + InstructionsList.LEN_PARAMETER;
				}

				if (valid)
				{
					if (funcion.InstructionsList.GetInstructions() >= 1)
					{
						if (funcion.InstructionsList.GetInstruction(funcion.InstructionsList.GetInstructions() - 1) != InstructionsList.InstructionsEnum.INST_RETURN)
							valid = false;
					}
					else
					{
						valid = false;
					}
				}
			}

			return(valid);
		}

	}
}
