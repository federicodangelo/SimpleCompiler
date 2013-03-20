using System;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.MaquinaVirtual
{
	public class Proceso
	{
		public enum EstadoProcesoEnum
		{
			ESTADO_INICIALIZANDO,
			ESTADO_CORRIENDO,
			ESTADO_TERMINADO,
			ESTADO_ERROR,
		};

		Stack stack;
		int nInstruccion;

		Runtime	runtime;
		Funcion funcion;
		ListaInstrucciones listaInstrucciones;
		TablaSimbolosSimple	tablaConstantes;
		DefinicionFunciones definicionFunciones;
		Funcion[] funcionesExternas;

		EstadoProcesoEnum estado;

		Runtime PrvConstruirRuntime()
		{
			return new Runtime(definicionFunciones);
		}
		
		public Proceso()
		{
			estado = EstadoProcesoEnum.ESTADO_INICIALIZANDO;
			stack = new Stack();
			nInstruccion = 0;
		}

		public void Limpiar()
		{
			stack.Limpiar();
			estado = EstadoProcesoEnum.ESTADO_INICIALIZANDO;
			nInstruccion = 0;
			funcion = null;
			listaInstrucciones = null;
			definicionFunciones = null;
			tablaConstantes = null;
		}

		public void Inicializar()
		{
			runtime = PrvConstruirRuntime();
		}

		public void AgregarDefiniciones(DefinicionFunciones definicionFunciones)
		{
			this.definicionFunciones = definicionFunciones;
			this.tablaConstantes = definicionFunciones.TablaConstantes;
		}

		public void AgregarFuncionesExternas(Funcion[] funcionesExternas)
		{
			this.funcionesExternas = funcionesExternas;
		}

		public EstadoProcesoEnum EjecutarFuncion(string nombreFuncion)
		{
			int cantidadStackOriginal = stack.ObtenerElementosEnStack();
			PrvEjecutarFuncion(nombreFuncion);

			estado = EstadoProcesoEnum.ESTADO_CORRIENDO;

			try
			{
				if (this.estado == EstadoProcesoEnum.ESTADO_CORRIENDO)
				{
					while (this.stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
					{
						PrvEjecutarInstruccion();
						if (this.stack.ObtenerElementosEnStackLlamadoFunciones() == 0)	break;
						PrvEjecutarInstruccion();
						if (this.stack.ObtenerElementosEnStackLlamadoFunciones() == 0)	break;
						PrvEjecutarInstruccion();
						if (this.stack.ObtenerElementosEnStackLlamadoFunciones() == 0)	break;
						PrvEjecutarInstruccion();
						if (this.stack.ObtenerElementosEnStackLlamadoFunciones() == 0)	break;
						PrvEjecutarInstruccion();
					}
				}

				if (this.stack.ObtenerElementosEnStackLlamadoFunciones() == 0)
				{
					this.estado = EstadoProcesoEnum.ESTADO_TERMINADO;

					while(this.stack.ObtenerElementosEnStack() != cantidadStackOriginal)
					{
						this.stack.Pop();
					}
				}
			}
			catch(ErrorEjecucion err)
			{
				Console.Write("Error de ejecución: {0}\n", err.Message);
				Console.Write("Stack de llamadas: \n");
				
				Console.Write("\tFuncion {0}, Linea {1}\n", funcion.Nombre, nInstruccion);

				while (this.stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
				{
					if (this.stack.GetLastLlamadoFuncion().funcion != null)
						Console.Write("Funcion {0}, Linea {0}\n", this.stack.GetLastLlamadoFuncion().funcion.Nombre, this.stack.GetLastLlamadoFuncion().instruccion);
					this.stack.PopLlamadoFuncion();
				}
				Console.Write("\n");

				Limpiar();
				this.estado = EstadoProcesoEnum.ESTADO_ERROR;
			}

			return(this.estado);
		}

		void ThrowErrorEjecucionSi(bool condicion, string descripcion)
		{
			if (condicion)
				throw new ErrorEjecucion(descripcion);
		}

		#region Ejecución de funciones

		void PrvEjecutarFuncion(string nombreFuncion)
		{
			Funcion funcion = definicionFunciones.BuscarFuncion(nombreFuncion);

			if (funcion == null)
				throw new ErrorEjecucion("No se encontro la función [" + nombreFuncion + "]");

			PrvEjecutarFuncion(funcion);
		}

		void PrvEjecutarFuncionGlobal(string nombreFuncion)
		{
			Funcion funcion = null;

			foreach(Funcion f in funcionesExternas)
				if (f.Nombre == nombreFuncion)
				{
					funcion = f;
					break;
				}

			if (funcion == null)
				throw new ErrorEjecucion("No se encontro la función [" + nombreFuncion + "]");

			PrvEjecutarFuncion(funcion);
		}

		void PrvEjecutarFuncion(int n)
		{
			Funcion funcion = definicionFunciones.ObtenerFuncion(n);

			PrvEjecutarFuncion(funcion);
		}

		void PrvEjecutarFuncion(Funcion funcion)
		{
			this.stack.PushLlamadoFuncion(funcion.GetParametros(), funcion.CantVariables, this.funcion, nInstruccion);

			this.funcion = funcion;

			if (funcion.TipoFuncion == Funcion.TipoFuncionEnum.FUNCION_NORMAL)
			{
				listaInstrucciones = this.funcion.ListaInstrucciones;

				nInstruccion = 0;
			}
			else
			{
				string descripcionError;

				bool ok = funcion.FuncionNativa(this, stack, out descripcionError);

				if (funcion.TipoDevuelto != null &&
					funcion.TipoDevuelto.Tipo != TipoDato.TipoDatoEnum.TIPO_VOID &&
					funcion.TipoDevuelto.Tipo != TipoDato.TipoDatoEnum.TIPO_NINGUNO)
				{
					if (stack.ObtenerElementosEnStack() == 0 ||
						stack.ObtenerTipoDato() != funcion.TipoDevuelto.Tipo)
					{
						throw new ErrorEjecucion(string.Format("El tipo devuelto por la función nativa {0} no coincide con lo que declara en su definición", funcion.Nombre));
					}
				}

				if (ok == false)
					throw new ErrorEjecucion(descripcionError);

				PrvRetornarDeFuncion();
			}
		}

		void PrvRetornarDeFuncion()
		{
			Funcion funcionLlamada = this.funcion;
			Stack stack = this.stack;

			Stack.InfoStackFunciones info = stack.GetLastLlamadoFuncion();

			this.nInstruccion = info.instruccion;
			this.funcion = info.funcion;

			if (this.funcion != null)
				this.listaInstrucciones = this.funcion.ListaInstrucciones;
			else
				this.listaInstrucciones = null;

			switch(funcionLlamada.TipoDevuelto.Tipo)
			{
				case TipoDato.TipoDatoEnum.TIPO_NINGUNO:
					stack.PopLlamadoFuncion();
					break;

				case TipoDato.TipoDatoEnum.TIPO_VOID:
					stack.PopLlamadoFuncion();
					break;

				case TipoDato.TipoDatoEnum.TIPO_INT:
				{
					int l = stack.PopInt();
					//Console.WriteLine("Resultado llamdo función: {0}", l);
					stack.PopLlamadoFuncion();
					stack.PushInt(l);
					break;
				}

				case TipoDato.TipoDatoEnum.TIPO_FLOAT:
				{
					float f = stack.PopFloat();
					//Console.WriteLine("Resultado llamdo función: {0}", f);
					stack.PopLlamadoFuncion();
					stack.PushFloat(f);
					break;
				}

				case TipoDato.TipoDatoEnum.TIPO_STRING:
				{
					string cadena = stack.PopString();
					//Console.WriteLine("Resultado llamdo función: {0}", cadena);

					stack.PopLlamadoFuncion();
					stack.PushString(cadena);
					break;
				}
			}
		}

		#endregion

		#region Instrucciones

		#region Selector de instrucción

		ListaInstrucciones.InstruccionesEnum PrvObtenerInstruccion()
		{
			ListaInstrucciones.InstruccionesEnum inst = listaInstrucciones.ObtenerInstruccion(nInstruccion);
			
			nInstruccion += ListaInstrucciones.lenInstruccion;

			return inst;
		}

		short PrvObtenerParametro()
		{
			short param = listaInstrucciones.ObtenerParametroDesplazado(nInstruccion);
			
			nInstruccion += ListaInstrucciones.lenParametro;

			return param;
		}
        
		void PrvEjecutarInstruccion()
		{
			ListaInstrucciones.InstruccionesEnum inst = PrvObtenerInstruccion();

			/*m_ppInst[inst](this);
	
			return;*/

			int int_value, int_value1, int_value2;
			float float_value, float_value1, float_value2;
			string string_value, string_value1, string_value2;

			switch(inst)
			{
				case ListaInstrucciones.InstruccionesEnum.INST_NADA:
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_SUMAR_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 + int_value2;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_RESTAR_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 - int_value2;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_MULT_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 * int_value2;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_DIV_INT:
					int_value2 = this.stack.PopInt();
					int_value1 = this.stack.PopInt();
					int_value = int_value1 / int_value2;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_SUMAR_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 + float_value2;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_RESTAR_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 - float_value2;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_MULT_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 * float_value2;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_DIV_FLOAT:
					float_value2 = this.stack.PopFloat();
					float_value1 = this.stack.PopFloat();
					float_value = float_value1 / float_value2;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_CONCAT_STRING:
					string_value2 = stack.PopString();
					string_value1 = stack.PopString();

					string_value = string_value1 + string_value2;
			
					stack.PushString(string_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_COMPARE_STRING:
				{
					string_value2 = stack.PopString();
					string_value1 = stack.PopString();

					int n = string_value1.CompareTo(string_value2);
			
					stack.PushInt(n);
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_ISZERO_INT:
					int_value = stack.PopInt();

					if (int_value == 0)
						int_value = 1;
					else
						int_value = 0;
					
					stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_FLOAT_A_INT:
					float_value = this.stack.PopFloat();
					int_value = (int) float_value;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_INT_A_FLOAT:
					int_value = this.stack.PopInt();
					float_value = (float) int_value;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_INT:
					int_value = this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).Integer;
					this.stack.PushInt(int_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_FLOAT:
					float_value = this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).Float;
					this.stack.PushFloat(float_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_STRING:
					string_value = this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String;
					stack.PushString(string_value);
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_INT:
					this.stack.PushInt(this.stack.ObtenerVarInt(PrvObtenerParametro()));
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_FLOAT:
					this.stack.PushFloat(this.stack.ObtenerVarFloat(PrvObtenerParametro()));
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_STRING:
					this.stack.PushString(this.stack.ObtenerVarString(PrvObtenerParametro()));
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
				{
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);
					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					this.stack.PushInt(runtime.ObtenerVariables().Variable(nVar).ObtenerInt());
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
				{
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);
					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					this.stack.PushFloat(runtime.ObtenerVariables().Variable(nVar).ObtenerFloat());
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
				{
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);

					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					this.stack.PushString(runtime.ObtenerVariables().Variable(nVar).ObtenerString());
					break;
				}
		
				case ListaInstrucciones.InstruccionesEnum.INST_STACK_DUP:
					this.stack.Duplicar();
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP:
					this.stack.Pop();
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_INT:
					this.stack.SetearVarInt(PrvObtenerParametro(), this.stack.PopInt());
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_FLOAT:
					this.stack.SetearVarFloat(PrvObtenerParametro(), this.stack.PopFloat());
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_STRING:
					this.stack.SetearVarString(PrvObtenerParametro(), this.stack.PopString());
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_INT:
				{
					int_value = this.stack.PopInt();
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);
					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetearInt(int_value);
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
				{
					float_value = this.stack.PopFloat();
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);
					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetearFloat(float_value);
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
				{
					string_value = this.stack.PopString();
					int nVar = definicionFunciones.BuscarPosicionVariable(this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String);
					ThrowErrorEjecucionSi(nVar == DefinicionFunciones.NO_ENCONTRADO, "No se encontro la variable");
					runtime.ObtenerVariables().Variable(nVar).SetearString(string_value);
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_JUMP:
				{
					short salto = PrvObtenerParametro();
					nInstruccion += salto;
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_JUMP_IF_ZERO:
				{
					int_value = this.stack.PopInt();
					short salto = PrvObtenerParametro();
					if (int_value == 0)
						nInstruccion += salto;
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_CALL_FUNCTION:
					PrvEjecutarFuncion(PrvObtenerParametro());
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_CALL_GLOBAL_FUNCTION:
				{
					string nombreFuncion = this.tablaConstantes.GetSimbolo(PrvObtenerParametro()).String;
					PrvEjecutarFuncionGlobal(nombreFuncion);
					break;
				}

				case ListaInstrucciones.InstruccionesEnum.INST_RETURN:
					PrvRetornarDeFuncion();
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_INC_INT:
					stack.IncrementarInt();
					break;

				case ListaInstrucciones.InstruccionesEnum.INST_DEC_INT:
					stack.DecrementarInt();
					break;

				default:
					ThrowErrorEjecucionSi(true, "Instruccion invalida o no implementada");
					break;
			}
		}

		#endregion

		
		/* FUNCIONES QUE REPRESENTAN INSTRUCCIONES */

/*
		void PrvInstNada(Proceso proceso)
		{
		}

		void PrvInstSumarInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			int int_value2 = stack.PopInt();
			int int_value1 = stack.PopInt();
			int int_value = int_value1 + int_value2;
			stack.PushInt(int_value);
		}

		void PrvInstRestarInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			int int_value2 = stack.PopInt();
			int int_value1 = stack.PopInt();
			int int_value = int_value1 - int_value2;
			stack.PushInt(int_value);
		}

		void PrvInstMultInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			int int_value2 = stack.PopInt();
			int int_value1 = stack.PopInt();
			long int_value = int_value1 * int_value2;
			stack.PushInt(int_value);
		}

		void PrvInstDivInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			long int_value2 = stack.PopInt();
			long int_value1 = stack.PopInt();
			long int_value = int_value1 / int_value2;
			stack.PushInt(int_value);
		}

		void PrvInstSumarFloat(Proceso proceso)
		{
			Stack stack = proceso.stack;
			float float_value2 = stack.PopFloat();
			float float_value1 = stack.PopFloat();
			float float_value = float_value1 + float_value2;
			stack.PushFloat(float_value);
		}

		void PrvInstRestarFloat(Proceso proceso)
		{
			Stack stack = proceso.stack;
			float float_value2 = stack.PopFloat();
			float float_value1 = stack.PopFloat();
			float float_value = float_value1 - float_value2;
			stack.PushFloat(float_value);
		}

		void PrvInstMultFloat(Proceso proceso)
		{
			Stack stack = proceso.stack;
			float float_value2 = stack.PopFloat();
			float float_value1 = stack.PopFloat();
			float float_value = float_value1 * float_value2;
			stack.PushFloat(float_value);
		}

		void PrvInstDivFloat(Proceso proceso)
		{
			Stack stack = proceso.stack;
			float float_value2 = stack.PopFloat();
			float float_value1 = stack.PopFloat();
			float float_value = float_value1 / float_value2;
			stack.PushFloat(float_value);
		}

		void PrvInstConcatString(Proceso proceso)
		{
			Stack stack = proceso.stack;
			CObjetoCadena* cadena_value2 = stack.PopString(false);
			CObjetoCadena* cadena_value1 = stack.PopString(false);
	
			CObjetoCadena* cadena_value = new CObjetoCadena(cadena_value1.ObtenerCadena());
			cadena_value.ObtenerCadena().ConcatenarCadena(cadena_value2.ObtenerCadena());

			cadena_value2.DecrementarUso();
			cadena_value1.DecrementarUso();

			stack.PushString(cadena_value);
		}

		void PrvInstCompareString(Proceso proceso)
		{
			Stack stack = proceso.stack;
			CObjetoCadena* cadena_value2 = stack.PopString(false);
			CObjetoCadena* cadena_value1 = stack.PopString(false);

			int n = strcmp(cadena_value1.ObtenerCadena(), cadena_value2.ObtenerCadena());
	
			cadena_value2.DecrementarUso();
			cadena_value1.DecrementarUso();

			stack.PushInt(n);
		}

		void PrvInstIsZeroInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			long int_value = stack.PopInt();
	
			int_value = !int_value; //Si es igual a 0, la negación va a dar distinto de cero (verdadero), sino 0 (falso)
	
			stack.PushInt(int_value);
		}

		void PrvInstFloatAInt(Proceso proceso)
		{
			Stack stack = proceso.stack;
			float float_value = stack.PopFloat();
			long int_value = (long) float_value;
			stack.PushInt(int_value);
		}

		void PrvInstIntAFloat(Proceso proceso)
		{
			Stack stack = proceso.stack;
			long int_value = stack.PopInt();
			float float_value = (float) int_value;
			stack.PushFloat(float_value);
		}

		void PrvInstStackPushNull(Proceso proceso)
		{
			proceso.stack.PushObjeto(null);
		}

		void PrvInstStackPushInt(Proceso proceso)
		{
			long int_value = proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetInt();
			proceso.stack.PushInt(int_value);
		}

		void PrvInstStackPushFloat(Proceso proceso)
		{
			float float_value = proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetFloat();
			proceso.stack.PushFloat(float_value);
		}

		void PrvInstStackPushString(Proceso proceso)
		{
			const char* string_value = proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString();
			proceso.stack.PushString(new CObjetoCadena(string_value));
		}

		void PrvInstStackPushVarInt(Proceso proceso)
		{
			proceso.stack.PushInt(proceso.stack.ObtenerVarInt(proceso.PrvObtenerParametro()));
		}

		void PrvInstStackPushVarFloat(Proceso proceso)
		{
			proceso.stack.PushFloat(proceso.stack.ObtenerVarFloat(proceso.PrvObtenerParametro()));
		}

		void PrvInstStackPushVarObj(Proceso proceso)
		{
			proceso.stack.PushObjeto(proceso.stack.ObtenerVarObjeto(proceso.PrvObtenerParametro()));
		}

		void PrvInstStackPushVarString(Proceso proceso)
		{
			proceso.stack.PushString(proceso.stack.ObtenerVarString(proceso.PrvObtenerParametro()));
		}

		void PrvInstStackPushThisClassVarInt(Proceso proceso)
		{
			proceso.stack.PushInt(proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).ObtenerInt());
		}

		void PrvInstStackPushThisClassVarFloat(Proceso proceso)
		{
			proceso.stack.PushFloat(proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).ObtenerFloat());
		}

		void PrvInstStackPushThisClassVarObj(Proceso proceso)
		{
			proceso.stack.PushObjeto(proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).ObtenerObjeto());
		}

		void PrvInstStackPushThisClassVarString(Proceso proceso)
		{
			proceso.stack.PushString(proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).ObtenerString());
		}

		void PrvInstStackPushClassVarInt(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			proceso.stack.PushInt(pObjeto.ObtenerVariables().Variable(nVar).ObtenerInt());
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPushClassVarFloat(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			proceso.stack.PushFloat(pObjeto.ObtenerVariables().Variable(nVar).ObtenerFloat());
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPushClassVarObj(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			proceso.stack.PushObjeto(pObjeto.ObtenerVariables().Variable(nVar).ObtenerObjeto());
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPushClassVarString(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			proceso.stack.PushString(pObjeto.ObtenerVariables().Variable(nVar).ObtenerString());
			pObjeto.DecrementarUso();
		}

		void PrvInstStackDup(Proceso proceso)
		{
			proceso.stack.Duplicar();
		}

		void PrvInstStackPop(Proceso proceso)
		{
			proceso.stack.Pop();
		}

		void PrvInstStackPopVarInt(Proceso proceso)
		{
			proceso.stack.SetearVarInt(proceso.PrvObtenerParametro(), proceso.stack.PopInt());
		}

		void PrvInstStackPopVarFloat(Proceso proceso)
		{
			proceso.stack.SetearVarFloat(proceso.PrvObtenerParametro(), proceso.stack.PopFloat());
		}

		void PrvInstStackPopVarObj(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			proceso.stack.SetearVarObjeto(proceso.PrvObtenerParametro(), pObjeto);
			if (pObjeto != null)
				pObjeto.DecrementarUso();
		}

		void PrvInstStackPopVarString(Proceso proceso)
		{
			CObjetoCadena* pObjetoCadena = proceso.stack.PopString(false);
			proceso.stack.SetearVarString(proceso.PrvObtenerParametro(), pObjetoCadena);
			if (pObjetoCadena != null)
				pObjetoCadena.DecrementarUso();
		}

		void PrvInstStackPopThisClassVarInt(Proceso proceso)
		{
			proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).SetearInt(proceso.stack.PopInt());
		}

		void PrvInstStackPopThisClassVarFloat(Proceso proceso)
		{
			proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).SetearFloat(proceso.stack.PopFloat());
		}

		void PrvInstStackPopThisClassVarObj(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).SetearObjeto(pObjeto);
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPopThisClassVarString(Proceso proceso)
		{
			CObjetoCadena* pObjetoCadena = proceso.stack.PopString(false);
			proceso.m_pObjeto.ObtenerVariables().Variable(proceso.PrvObtenerParametro()).SetearString(pObjetoCadena);
			pObjetoCadena.DecrementarUso();
		}

		void PrvInstStackPopClassVarInt(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			long int_value = proceso.stack.PopInt();
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto.ObtenerVariables().Variable(nVar).SetearInt(int_value);
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPopClassVarFloat(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			float float_value = proceso.stack.PopFloat();
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto.ObtenerVariables().Variable(nVar).SetearFloat(float_value);
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPopClassVarObj(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			CObjeto* pObjetoValue = proceso.stack.PopObjeto(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto.ObtenerVariables().Variable(nVar).SetearObjeto(pObjetoValue);
			pObjetoValue.DecrementarUso();
			pObjeto.DecrementarUso();
		}

		void PrvInstStackPopClassVarString(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			CObjetoCadena* pObjetoCadenaValue = proceso.stack.PopString(false);
			int nVar = pObjeto.ObtenerDefinicion().BuscarPosicionVariable(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			ThrowErrorEjecucionSi(nVar == CDefinicionObjeto::NO_ENCONTRADO, "No se encontro la variable");
			pObjeto.ObtenerVariables().Variable(nVar).SetearString(pObjetoCadenaValue);
			pObjetoCadenaValue.DecrementarUso();
			pObjeto.DecrementarUso();
		}

		void PrvInstJump(Proceso proceso)
		{
			proceso.m_Instruccion += proceso.PrvObtenerParametro();
		}

		void PrvInstJumpIfZero(Proceso proceso)
		{
			long int_value = proceso.stack.PopInt();
			if (int_value == 0)
				proceso.m_Instruccion += proceso.PrvObtenerParametro();
			else
				proceso.PrvObtenerParametro();
		}

		void PrvInstCallThisClassFunction(Proceso proceso)
		{
			proceso.PrvEjecutarFuncion(proceso.m_pObjeto, proceso.PrvObtenerParametro());
		}

		void PrvInstCallClassFunction(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);
			CCadenaConHash& NombreFuncion = proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString();
			proceso.PrvEjecutarFuncion(pObjeto, NombreFuncion);
			pObjeto.DecrementarUso();
		}

		void PrvInstNew(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.PrvConstruirObjeto(proceso.m_pTablaConstantes.GetSimbolo(proceso.PrvObtenerParametro()).GetString());
			proceso.stack.PushObjeto(pObjeto);
		}

		void PrvInstReturn(Proceso proceso)
		{
			proceso.PrvRetornarDeFuncion();
		}

		void PrvInstThrow(Proceso proceso)
		{
			CObjeto* pObjeto = proceso.stack.PopObjeto(false);

			int n = proceso.m_pFuncion.BuscarManejadorExcepcion(proceso.m_Instruccion, pObjeto.ObtenerDefinicion().ObtenerNombre());

			while (n < 0 && proceso.stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
			{
				proceso.PrvRetornarDeFuncion();
				n = proceso.m_pFuncion.BuscarManejadorExcepcion(proceso.m_Instruccion, pObjeto.ObtenerDefinicion().ObtenerNombre());
			}

			if (proceso.stack.ObtenerElementosEnStackLlamadoFunciones() > 0)
			{
				//Se encontro un manejador
				proceso.m_Instruccion = proceso.m_pFuncion.GetManejadorExcepcion(n).EjecutarLinea;
				proceso.stack.PushObjeto(pObjeto);
				pObjeto.DecrementarUso();
			}
			else
			{
				pObjeto.DecrementarUso();
				printf("No hay manejador de excepciones por defecto!!!\n");
				//No se encontro ningun manejador.. estoy hasta las manos
			}
		}

		void PrvInstIncInt(Proceso proceso)
		{
			proceso.stack.IncrementarInt();
		}

		void PrvInstDecInt(Proceso proceso)
		{
			proceso.stack.DecrementarInt();
		}*/

		#endregion
	}
}
