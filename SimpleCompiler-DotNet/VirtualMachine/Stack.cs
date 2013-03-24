using System;
using SimpleCompiler.Shared;

namespace SimpleCompiler.VirtualMachine
{
	public class Stack
	{
		const string ERROR_STACK_EMPTY = "Empty stack";
		const string ERROR_STACK_FULL = "Full stack";
		const string ERROR_STACK_FUNCTIONS_FULL = "Full functions call stack";
		const string ERROR_STACK_INVALID_TYPE = "Invalid type";

		const int MAX_STACK = 1024;
		const int MAX_STACK_FUNCTIONS = 512;

		public class InfoStackFunctions
		{
			public int numberVars;
			public int offsetVars;
			public int offsetStack;
			public int instruction;
			public Function functions;
		};

		private int	stackPos;
        private DataType.DataTypeEnum[] dataTypes = new DataType.DataTypeEnum[MAX_STACK + 1];
        private object[] values = new object[MAX_STACK + 1];

        private int stackFunctionsPos;
        private InfoStackFunctions[] stackFunctions = new InfoStackFunctions[MAX_STACK_FUNCTIONS];

        private int numberOfVars;
        private int offsetVars;
        private int offsetStack;

		public Stack()
		{
			this.stackPos = -1;
			this.offsetStack = -1;
			this.offsetVars = 0;
			this.numberOfVars = 0;
			this.stackFunctionsPos = 0;

			for (int i = 0; i < MAX_STACK_FUNCTIONS; i++)
				stackFunctions[i] = new InfoStackFunctions();
		}
		
		public void Clear()
		{
			this.offsetStack = -1;
			this.offsetVars = 0;
			this.numberOfVars = 0;
			this.stackFunctionsPos = 0;
	
			for (int i = 0; i < MAX_STACK_FUNCTIONS; i++)
				stackFunctions[i].functions = null;

			while(this.stackPos >= 0)
				Pop();
		}

		private void ThrowRuntimeErrorIf(bool condition, string description)
		{
			if (condition)
				throw new RuntimeError(description);
		}

		public void PushInt(int val)
		{
			ThrowRuntimeErrorIf(this.stackPos == MAX_STACK, ERROR_STACK_FULL);
			this.stackPos++;
			this.dataTypes[this.stackPos] = DataType.DataTypeEnum.TYPE_INT;
			this.values[this.stackPos] = val;
		}

		public void PushFloat(float val)
		{
			ThrowRuntimeErrorIf(this.stackPos == MAX_STACK, ERROR_STACK_FULL);
			this.stackPos++;
			this.dataTypes[this.stackPos] = DataType.DataTypeEnum.TYPE_FLOAT;
			this.values[this.stackPos] = val;
		}

		public void PushString(string val)
		{
			ThrowRuntimeErrorIf(this.stackPos == MAX_STACK, ERROR_STACK_FULL);
			this.stackPos++;
			this.dataTypes[this.stackPos] = DataType.DataTypeEnum.TYPE_STRING;
	
			this.values[this.stackPos] = val;
		}

		public void Pop()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
	
			this.stackPos--;
		}

		public int PopInt()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.dataTypes[this.stackPos] != DataType.DataTypeEnum.TYPE_INT, ERROR_STACK_INVALID_TYPE);

			return((int) this.values[this.stackPos--]);
		}

		public float PopFloat()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.dataTypes[this.stackPos] != DataType.DataTypeEnum.TYPE_FLOAT, ERROR_STACK_INVALID_TYPE);

			return((float) this.values[this.stackPos--]);
		}

		public string PopString()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.dataTypes[this.stackPos] != DataType.DataTypeEnum.TYPE_STRING, ERROR_STACK_INVALID_TYPE);

			return (string) this.values[this.stackPos--];
		}

		public void Duplicate()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.stackPos == MAX_STACK, ERROR_STACK_FULL);

			this.dataTypes[this.stackPos + 1] = this.dataTypes[this.stackPos];
			this.values[this.stackPos + 1] = this.values[this.stackPos];
			this.stackPos++;
		}

		public int GetNumberOfElementsInStack() 
		{ 
			return (this.stackPos - this.offsetStack); 
		}

		public DataType.DataTypeEnum GetLastElementDataType()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);

			return(this.dataTypes[this.stackPos]);
		}

		public void PushFunctionCall(int parameters, int vars, Function function, int instruction)
		{
			ThrowRuntimeErrorIf(this.stackPos - parameters + vars >= MAX_STACK, ERROR_STACK_FULL);
			ThrowRuntimeErrorIf(this.stackFunctionsPos + 1 == MAX_STACK_FUNCTIONS, ERROR_STACK_FUNCTIONS_FULL);

			this.stackFunctionsPos++;
	
			InfoStackFunctions infoStack = this.stackFunctions[this.stackFunctionsPos];

			infoStack.numberVars = this.numberOfVars;
			infoStack.offsetStack = this.offsetStack;
			infoStack.offsetVars = this.offsetVars;
			infoStack.instruction = instruction;
			infoStack.functions = function;

			this.offsetVars = this.stackPos + 1 - parameters;
			this.offsetStack = this.offsetVars + vars - 1;

			this.numberOfVars = vars;
			this.stackPos = this.offsetStack;
		}

		public void IncrementInt()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.dataTypes[this.stackPos] != DataType.DataTypeEnum.TYPE_INT, ERROR_STACK_INVALID_TYPE);

			int v = (int) this.values[this.stackPos];
			v++;
			this.values[this.stackPos] = v;
		}

		public void DecrementInt()
		{
			ThrowRuntimeErrorIf(this.stackPos == this.offsetStack, ERROR_STACK_EMPTY);
			ThrowRuntimeErrorIf(this.dataTypes[this.stackPos] != DataType.DataTypeEnum.TYPE_INT, ERROR_STACK_INVALID_TYPE);

			int v = (int) this.values[this.stackPos];
			v--;
			this.values[this.stackPos] = v;
		}

		public InfoStackFunctions GetLastFunctionCall()
		{
			return(this.stackFunctions[this.stackFunctionsPos]);
		}

		public void PopFunctionCall()
		{
			while(this.stackPos >= this.offsetVars)
				this.values[this.stackPos--] = null;
	
			InfoStackFunctions infoStack = this.stackFunctions[this.stackFunctionsPos];

			this.numberOfVars = infoStack.numberVars;
			this.offsetVars = infoStack.offsetVars;
			this.offsetStack = infoStack.offsetStack;

			this.stackFunctionsPos--;
		}

		public int GetNumberOfElementsInFunctionCallStack() 
		{ 
			return(this.stackFunctionsPos); 
		}

		public DataType.DataTypeEnum GetVarDataType(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			return(this.dataTypes[n + this.offsetVars]); 
		}

		public void SetVarInt(int n, int val)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.dataTypes[n] = DataType.DataTypeEnum.TYPE_INT;

			this.values[n] = val;
		}

		public void SetVarFloat(int n, float val)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.dataTypes[n] = DataType.DataTypeEnum.TYPE_FLOAT;

			this.values[n] = val;
		}

		public void SetVarString(int n, string val)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.dataTypes[n] = DataType.DataTypeEnum.TYPE_STRING;

			this.values[n] = val;
		}

		public int GetVarInt(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowRuntimeErrorIf(this.dataTypes[n] != DataType.DataTypeEnum.TYPE_INT, "Tipo de variable incorrecto");

			return (int) this.values[n];
		}

		public float GetVarFloat(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowRuntimeErrorIf(this.dataTypes[n] != DataType.DataTypeEnum.TYPE_FLOAT, "Tipo de variable incorrecto");

			return (float) this.values[n];
		}

		public string GetVarString(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowRuntimeErrorIf(this.dataTypes[n] != DataType.DataTypeEnum.TYPE_STRING, "Tipo de variable incorrecto");

			ThrowRuntimeErrorIf(this.values[n] == null, "El valor almacenado en la variable es NULL, no puede obtenerse este valor");

			return (string) this.values[n];
		}
	}
}
