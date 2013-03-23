using System;
using SimpleCompiler.Shared;

namespace SimpleCompiler.VirtualMachine
{
	public class Process
	{
		public enum ProcessStateEnum
		{
			STATE_INITIALIZED,
			STATE_RUNNING,
			STATE_FINISHED,
			STATE_ERROR,
		};

		private Stack stack;
        private int nInstruction;

        private Runtime	runtime;
        private Function function;
        private InstructionsList instructionsList;
        private SymbolConstantTable	constantsTable;
        private FunctionsDefinitions functionsDefinitions;
        private Function[] externalFunctions;

        private ProcessStateEnum state;

		private Runtime PrvBuildRuntime()
		{
			return new Runtime(functionsDefinitions);
		}
		
		public Process()
		{
			state = ProcessStateEnum.STATE_INITIALIZED;
			stack = new Stack();
			nInstruction = 0;
		}

		public void Clear()
		{
			stack.Clear();
			state = ProcessStateEnum.STATE_INITIALIZED;
			nInstruction = 0;
			function = null;
			instructionsList = null;
			functionsDefinitions = null;
			constantsTable = null;
		}

		public void Init()
		{
			runtime = PrvBuildRuntime();
		}

		public void AddDefinitions(FunctionsDefinitions definicionFunciones)
		{
			this.functionsDefinitions = definicionFunciones;
			this.constantsTable = definicionFunciones.ConstantsTable;
		}

		public void AddExternalFunctions(Function[] funcionesExternas)
		{
			this.externalFunctions = funcionesExternas;
		}

		public ProcessStateEnum CallFunction(string functionName)
		{
			int cantidadStackOriginal = stack.GetNumberOfElementsInStack();
			PrvCallFunction(functionName);

			state = ProcessStateEnum.STATE_RUNNING;

			try
			{
				if (this.state == ProcessStateEnum.STATE_RUNNING)
				{
					while (this.stack.GetNumberOfElementsInFunctionCallStack() > 0)
					{
						PrvRunInstruction();
						if (this.stack.GetNumberOfElementsInFunctionCallStack() == 0)	break;
						PrvRunInstruction();
						if (this.stack.GetNumberOfElementsInFunctionCallStack() == 0)	break;
						PrvRunInstruction();
						if (this.stack.GetNumberOfElementsInFunctionCallStack() == 0)	break;
						PrvRunInstruction();
						if (this.stack.GetNumberOfElementsInFunctionCallStack() == 0)	break;
						PrvRunInstruction();
					}
				}

				if (this.stack.GetNumberOfElementsInFunctionCallStack() == 0)
				{
					this.state = ProcessStateEnum.STATE_FINISHED;

					while(this.stack.GetNumberOfElementsInStack() != cantidadStackOriginal)
					{
						this.stack.Pop();
					}
				}
			}
			catch(RuntimeError err)
			{
				Console.Write("Runtime error: {0}\n", err.Message);
				Console.Write("Call stack: \n");
				
				Console.Write("\tFunction {0}, Line {1}\n", function.Name, nInstruction);

				while (this.stack.GetNumberOfElementsInFunctionCallStack() > 0)
				{
					if (this.stack.GetLastFunctionCall().functions != null)
						Console.Write("Function {0}, Line {0}\n", this.stack.GetLastFunctionCall().functions.Name, this.stack.GetLastFunctionCall().instruction);
					this.stack.PopFunctionCall();
				}
				Console.Write("\n");

				Clear();
				this.state = ProcessStateEnum.STATE_ERROR;
			}

			return(this.state);
		}

		private void ThrowRuntimeErrorIf(bool condition, string descripcion)
		{
			if (condition)
				throw new RuntimeError(descripcion);
		}

		#region Functions call

		private void PrvCallFunction(string functionName)
		{
			Function function = functionsDefinitions.FindFunction(functionName);

			if (function == null)
				throw new RuntimeError("No se encontro la funci�n [" + functionName + "]");

			PrvCallFunction(function);
		}

		private void PrvCallExternalFunction(string functionName)
		{
			Function funcion = null;

			foreach(Function f in externalFunctions)
            {
				if (f.Name == functionName)
				{
					funcion = f;
					break;
				}
            }

			if (funcion == null)
				throw new RuntimeError("External function not found [" + functionName + "]");

			PrvCallFunction(funcion);
		}

		private void PrvCallFunction(int n)
		{
			Function funcion = functionsDefinitions.GetFunction(n);

			PrvCallFunction(funcion);
		}

		private void PrvCallFunction(Function funcion)
		{
			this.stack.PushFunctionCall(funcion.GetParameters(), funcion.NumberOfVariables, this.function, nInstruction);

			this.function = funcion;

			if (funcion.FunctionType == Function.FuncionTypeEnum.FUNCTION_NORMAL)
			{
				instructionsList = this.function.InstructionsList;

				nInstruction = 0;
			}
			else
			{
				string descripcionError;

				bool ok = funcion.NativeFunction(this, stack, out descripcionError);

				if (funcion.ReturnType != null &&
					funcion.ReturnType.Type != DataType.DataTypeEnum.TYPE_VOID &&
					funcion.ReturnType.Type != DataType.DataTypeEnum.TYPE_NONE)
				{
					if (stack.GetNumberOfElementsInStack() == 0 ||
						stack.GetLastElementDataType() != funcion.ReturnType.Type)
					{
						throw new RuntimeError(string.Format("El tipo devuelto por la funci�n nativa {0} no coincide con lo que declara en su definici�n", funcion.Name));
					}
				}

				if (ok == false)
					throw new RuntimeError(descripcionError);

				PrvReturnFromFunction();
			}
		}

		private void PrvReturnFromFunction()
		{
			Function funcionLlamada = this.function;
			Stack stack = this.stack;

			Stack.InfoStackFunctions info = stack.GetLastFunctionCall();

			this.nInstruction = info.instruction;
			this.function = info.functions;

			if (this.function != null)
				this.instructionsList = this.function.InstructionsList;
			else
				this.instructionsList = null;

			switch(funcionLlamada.ReturnType.Type)
			{
				case DataType.DataTypeEnum.TYPE_NONE:
					stack.PopFunctionCall();
					break;

				case DataType.DataTypeEnum.TYPE_VOID:
					stack.PopFunctionCall();
					break;

				case DataType.DataTypeEnum.TYPE_INT:
				{
					int l = stack.PopInt();
					//Console.WriteLine("Resultado llamdo funci�n: {0}", l);
					stack.PopFunctionCall();
					stack.PushInt(l);
					break;
				}

				case DataType.DataTypeEnum.TYPE_FLOAT:
				{
					float f = stack.PopFloat();
					//Console.WriteLine("Resultado llamdo funci�n: {0}", f);
					stack.PopFunctionCall();
					stack.PushFloat(f);
					break;
				}

				case DataType.DataTypeEnum.TYPE_STRING:
				{
					string cadena = stack.PopString();
					//Console.WriteLine("Resultado llamdo funci�n: {0}", cadena);

					stack.PopFunctionCall();
					stack.PushString(cadena);
					break;
				}
			}
		}

		#endregion

		#region Instructions

		#region Instructions Selector

		private InstructionsList.InstructionsEnum PrvGetInstruction()
		{
			InstructionsList.InstructionsEnum inst = instructionsList.GetInstruction(nInstruction);
			
			nInstruction += InstructionsList.LEN_INSTRUCTION;

			return inst;
		}

		private short PrvGetParameter()
		{
			short param = instructionsList.GetParameterOffsetted(nInstruction);
			
			nInstruction += InstructionsList.LEN_PARAMETER;

			return param;
		}
        
		private void PrvRunInstruction()
		{
			InstructionsList.InstructionsEnum inst = PrvGetInstruction();

			/*m_ppInst[inst](this);
	
			return;*/

			int int_value, int_value1, int_value2;
			float float_value, float_value1, float_value2;
			string string_value, string_value1, string_value2;

			switch(inst)
			{
				case InstructionsList.InstructionsEnum.INST_NULL:
					break;

				case InstructionsList.InstructionsEnum.INST_ADD_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 + int_value2;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_SUB_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 - int_value2;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_MULT_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 * int_value2;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_DIV_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 / int_value2;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_ADD_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 + float_value2;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_SUB_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 - float_value2;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_MULT_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 * float_value2;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_DIV_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 / float_value2;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_CONCAT_STRING:
					string_value2 = stack.PopString();
					string_value1 = stack.PopString();

					string_value = string_value1 + string_value2;
			
					stack.PushString(string_value);
					break;

				case InstructionsList.InstructionsEnum.INST_COMPARE_STRING:
				{
					string_value2 = stack.PopString();
					string_value1 = stack.PopString();

					int n = string_value1.CompareTo(string_value2);
			
					stack.PushInt(n);
					break;
				}

				case InstructionsList.InstructionsEnum.INST_ISZERO_INT:
					int_value = stack.PopInt();

					if (int_value == 0)
						int_value = 1;
					else
						int_value = 0;
					
					stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_FLOAT_A_INT:
					float_value = this.stack.PopFloat();
					int_value = (int) float_value;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_INT_A_FLOAT:
					int_value = this.stack.PopInt();
					float_value = (float) int_value;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_INT:
					int_value = this.constantsTable.GetSymbol(PrvGetParameter()).Integer;
					this.stack.PushInt(int_value);
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_FLOAT:
					float_value = this.constantsTable.GetSymbol(PrvGetParameter()).Float;
					this.stack.PushFloat(float_value);
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_STRING:
					string_value = this.constantsTable.GetSymbol(PrvGetParameter()).String;
					stack.PushString(string_value);
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_INT:
					this.stack.PushInt(this.stack.GetVarInt(PrvGetParameter()));
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_FLOAT:
					this.stack.PushFloat(this.stack.GetVarFloat(PrvGetParameter()));
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_VAR_STRING:
					this.stack.PushString(this.stack.GetVarString(PrvGetParameter()));
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
				{
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);
					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					this.stack.PushInt(runtime.ObtenerVariables().Variable(nVar).GetInt());
					break;
				}

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
				{
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);
					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					this.stack.PushFloat(runtime.ObtenerVariables().Variable(nVar).GetFloat());
					break;
				}

				case InstructionsList.InstructionsEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
				{
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);

					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					this.stack.PushString(runtime.ObtenerVariables().Variable(nVar).GetString());
					break;
				}
		
				case InstructionsList.InstructionsEnum.INST_STACK_DUP:
					this.stack.Duplicate();
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_POP:
					this.stack.Pop();
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_INT:
					this.stack.SetVarInt(PrvGetParameter(), this.stack.PopInt());
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_FLOAT:
					this.stack.SetVarFloat(PrvGetParameter(), this.stack.PopFloat());
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_POP_VAR_STRING:
					this.stack.SetVarString(PrvGetParameter(), this.stack.PopString());
					break;

				case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_INT:
				{
					int_value = this.stack.PopInt();
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);
					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetInt(int_value);
					break;
				}

				case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
				{
					float_value = this.stack.PopFloat();
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);
					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetFloat(float_value);
					break;
				}

				case InstructionsList.InstructionsEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
				{
					string_value = this.stack.PopString();
					int nVar = functionsDefinitions.FindVariablePosition(this.constantsTable.GetSymbol(PrvGetParameter()).String);
					ThrowRuntimeErrorIf(nVar == FunctionsDefinitions.NOT_FOUND, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetString(string_value);
					break;
				}

				case InstructionsList.InstructionsEnum.INST_JUMP:
				{
					short salto = PrvGetParameter();
					nInstruction += salto;
					break;
				}

				case InstructionsList.InstructionsEnum.INST_JUMP_IF_ZERO:
				{
					int_value = this.stack.PopInt();
					short salto = PrvGetParameter();
					if (int_value == 0)
						nInstruction += salto;
					break;
				}

				case InstructionsList.InstructionsEnum.INST_CALL_FUNCTION:
					PrvCallFunction(PrvGetParameter());
					break;

				case InstructionsList.InstructionsEnum.INST_CALL_GLOBAL_FUNCTION:
				{
					string nombreFuncion = this.constantsTable.GetSymbol(PrvGetParameter()).String;
					PrvCallExternalFunction(nombreFuncion);
					break;
				}

				case InstructionsList.InstructionsEnum.INST_RETURN:
					PrvReturnFromFunction();
					break;

				case InstructionsList.InstructionsEnum.INST_INC_INT:
					stack.IncrementInt();
					break;

				case InstructionsList.InstructionsEnum.INST_DEC_INT:
					stack.DecrementInt();
					break;

				default:
					ThrowRuntimeErrorIf(true, "Instruccion invalida o no implementada");
					break;
			}
		}

		#endregion

		#endregion
	}
}
