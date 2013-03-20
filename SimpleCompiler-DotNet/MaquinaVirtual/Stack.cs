using System;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.MaquinaVirtual
{
	public class Stack
	{
		const string ERROR_STACK_VACIO = "El stack esta vacio";
		const string ERROR_STACK_LLENO = "El stack esta lleno";
		const string ERROR_STACK_FUNCIONES_LLENO = "El stack de llamado de funciones esta lleno";
		const string ERROR_STACK_TIPO_INVALIDO = "El tipo de dato que se pidio sacar del stack no es compatible con el tipo de dato presente en el stack";

		const int MAX_STACK = 1024;
		const int MAX_STACK_FUNCIONES = 512;

		public class InfoStackFunciones
		{
			public int cantVars;
			public int OffsetVars;
			public int OffsetStack;
			public int instruccion;
			public Funcion funcion;
		};

		int	stackPos;
		TipoDato.TipoDatoEnum[] tipoDatos = new TipoDato.TipoDatoEnum[MAX_STACK + 1];
		object[] valores = new object[MAX_STACK + 1];

		int stackFuncionesPos;
		InfoStackFunciones[] stackFunciones = new InfoStackFunciones[MAX_STACK_FUNCIONES];

		int cantVars;
		int offsetVars;
		int offsetStack;

		public Stack()
		{
			this.stackPos = -1;
			this.offsetStack = -1;
			this.offsetVars = 0;
			this.cantVars = 0;
			this.stackFuncionesPos = 0;

			for (int i = 0; i < MAX_STACK_FUNCIONES; i++)
				stackFunciones[i] = new InfoStackFunciones();
		}
		
		public void Limpiar()
		{
			this.offsetStack = -1;
			this.offsetVars = 0;
			this.cantVars = 0;
			this.stackFuncionesPos = 0;
	
			for (int i = 0; i < MAX_STACK_FUNCIONES; i++)
				stackFunciones[i].funcion = null;

			while(this.stackPos >= 0)
				Pop();
		}

		private void ThrowErrorEjecucionSi(bool condicion, string descripcion)
		{
			if (condicion)
				throw new ErrorEjecucion(descripcion);
		}

		public void PushInt(int valor)
		{
			ThrowErrorEjecucionSi(this.stackPos == MAX_STACK, ERROR_STACK_LLENO);
			this.stackPos++;
			this.tipoDatos[this.stackPos] = TipoDato.TipoDatoEnum.TIPO_INT;
			this.valores[this.stackPos] = valor;
		}

		public void PushFloat(float valor)
		{
			ThrowErrorEjecucionSi(this.stackPos == MAX_STACK, ERROR_STACK_LLENO);
			this.stackPos++;
			this.tipoDatos[this.stackPos] = TipoDato.TipoDatoEnum.TIPO_FLOAT;
			this.valores[this.stackPos] = valor;
		}

		public void PushString(string valor)
		{
			ThrowErrorEjecucionSi(this.stackPos == MAX_STACK, ERROR_STACK_LLENO);
			this.stackPos++;
			this.tipoDatos[this.stackPos] = TipoDato.TipoDatoEnum.TIPO_STRING;
	
			this.valores[this.stackPos] = valor;
		}

		public void Pop()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
	
			this.stackPos--;
		}

		public int PopInt()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.tipoDatos[this.stackPos] != TipoDato.TipoDatoEnum.TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

			return((int) this.valores[this.stackPos--]);
		}

		public float PopFloat()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.tipoDatos[this.stackPos] != TipoDato.TipoDatoEnum.TIPO_FLOAT, ERROR_STACK_TIPO_INVALIDO);

			return((float) this.valores[this.stackPos--]);
		}

		public string PopString()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.tipoDatos[this.stackPos] != TipoDato.TipoDatoEnum.TIPO_STRING, ERROR_STACK_TIPO_INVALIDO);

			return (string) this.valores[this.stackPos--];
		}

		public void Duplicar()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.stackPos == MAX_STACK, ERROR_STACK_LLENO);

			this.tipoDatos[this.stackPos + 1] = this.tipoDatos[this.stackPos];
			this.valores[this.stackPos + 1] = this.valores[this.stackPos];
			this.stackPos++;
		}

		public int ObtenerElementosEnStack() 
		{ 
			return (this.stackPos - this.offsetStack); 
		}

		public TipoDato.TipoDatoEnum ObtenerTipoDato()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);

			return(this.tipoDatos[this.stackPos]);
		}

		public void PushLlamadoFuncion(int cantParametros, int cantVars, Funcion funcion, int instruccion)
		{
			ThrowErrorEjecucionSi(this.stackPos - cantParametros + cantVars >= MAX_STACK, ERROR_STACK_LLENO);
			ThrowErrorEjecucionSi(this.stackFuncionesPos + 1 == MAX_STACK_FUNCIONES, ERROR_STACK_FUNCIONES_LLENO);

			this.stackFuncionesPos++;
	
			InfoStackFunciones infoStack = this.stackFunciones[this.stackFuncionesPos];

			infoStack.cantVars = this.cantVars;
			infoStack.OffsetStack = this.offsetStack;
			infoStack.OffsetVars = this.offsetVars;
			infoStack.instruccion = instruccion;
			infoStack.funcion = funcion;

			this.offsetVars = this.stackPos + 1 - cantParametros;
			this.offsetStack = this.offsetVars + cantVars - 1;

			this.cantVars = cantVars;
			this.stackPos = this.offsetStack;
		}

		public void IncrementarInt()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.tipoDatos[this.stackPos] != TipoDato.TipoDatoEnum.TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

			int v = (int) this.valores[this.stackPos];
			v++;
			this.valores[this.stackPos] = v;
		}

		public void DecrementarInt()
		{
			ThrowErrorEjecucionSi(this.stackPos == this.offsetStack, ERROR_STACK_VACIO);
			ThrowErrorEjecucionSi(this.tipoDatos[this.stackPos] != TipoDato.TipoDatoEnum.TIPO_INT, ERROR_STACK_TIPO_INVALIDO);

			int v = (int) this.valores[this.stackPos];
			v--;
			this.valores[this.stackPos] = v;
		}

		public InfoStackFunciones GetLastLlamadoFuncion()
		{
			return(this.stackFunciones[this.stackFuncionesPos]);
		}

		public void PopLlamadoFuncion()
		{
			while(this.stackPos >= this.offsetVars)
				this.valores[this.stackPos--] = null;
	
			InfoStackFunciones infoStack = this.stackFunciones[this.stackFuncionesPos];

			this.cantVars = infoStack.cantVars;
			this.offsetVars = infoStack.OffsetVars;
			this.offsetStack = infoStack.OffsetStack;

			this.stackFuncionesPos--;
		}

		public int ObtenerElementosEnStackLlamadoFunciones() 
		{ 
			return(this.stackFuncionesPos); 
		}

		public TipoDato.TipoDatoEnum VarTipoDato(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			return(this.tipoDatos[n + this.offsetVars]); 
		}

		public void SetearVarInt(int n, int valor)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.tipoDatos[n] = TipoDato.TipoDatoEnum.TIPO_INT;

			this.valores[n] = valor;
		}

		public void SetearVarFloat(int n, float valor)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.tipoDatos[n] = TipoDato.TipoDatoEnum.TIPO_FLOAT;

			this.valores[n] = valor;
		}

		public void SetearVarString(int n, string valor)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			this.tipoDatos[n] = TipoDato.TipoDatoEnum.TIPO_STRING;

			this.valores[n] = valor;
		}

		public int ObtenerVarInt(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowErrorEjecucionSi(this.tipoDatos[n] != TipoDato.TipoDatoEnum.TIPO_INT, "Tipo de variable incorrecto");

			return (int) this.valores[n];
		}

		public float ObtenerVarFloat(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowErrorEjecucionSi(this.tipoDatos[n] != TipoDato.TipoDatoEnum.TIPO_FLOAT, "Tipo de variable incorrecto");

			return (float) this.valores[n];
		}

		public string ObtenerVarString(int n)
		{
			//Chequeo no necesario porque chequeo todos los indices en CDefinicionObjeto::PrvValidarFuncion
			//ThrowErrorEjecucionSi(n >= this.cantVars, "Numero de variable invalido");

			n += this.offsetVars;

			ThrowErrorEjecucionSi(this.tipoDatos[n] != TipoDato.TipoDatoEnum.TIPO_STRING, "Tipo de variable incorrecto");

			ThrowErrorEjecucionSi(this.valores[n] == null, "El valor almacenado en la variable es NULL, no puede obtenerse este valor");

			return (string) this.valores[n];
		}
	}
}
