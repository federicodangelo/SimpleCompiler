using System;
using System.Collections;

namespace CompiladorReducido.Compartido
{
	public class DefinicionFunciones
	{
		public const int NO_ENCONTRADO = -1;

		public TablaSimbolosSimple TablaConstantes;

		private ArrayList variables;
		private ArrayList funciones;
				
		public DefinicionFunciones()
		{
			variables = new ArrayList();
			funciones = new ArrayList();
			TablaConstantes = new TablaSimbolosSimple();
		}

		public void Limpiar()
		{
			variables.Clear();
			funciones.Clear();
			TablaConstantes.Clear();
		}

		public int ObtenerVariables() 
		{ 
			return variables.Count; 
		}
		
		public DefinicionVariable ObtenerVariable(int n) 
		{ 
			return (DefinicionVariable) variables[n]; 
		}

		public void AgregarVariable(DefinicionVariable variable)
		{
			variables.Add(variable);
		}

		public int ObtenerFunciones() 
		{ 
			return funciones.Count;
		}

		public Funcion ObtenerFuncion(int n) 
		{ 
			return (Funcion) funciones[n];
		}

		public void AgregarFuncion(Funcion funcion)
		{
			funciones.Add(funcion);
		}

		public int BuscarPosicionFuncion(string nombre)
		{
			for (int i = 0; i < funciones.Count; i++)
				if (((Funcion) funciones[i]).Nombre == nombre)
					return(i);

			return -1;
		}

		public int BuscarPosicionVariable(string nombre)
		{
			for (int i = 0; i < variables.Count; i++)
				if (((DefinicionVariable) variables[i]).Nombre == nombre)
					return(i);

			return -1;
		}

		public Funcion BuscarFuncion(string nombre)
		{
			for (int i = 0; i < funciones.Count; i++)
				if (((Funcion) funciones[i]).Nombre == nombre)
					return (Funcion) funciones[i];

			return null;
		}

		public void Imprimir(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Constantes:\n");
			TablaConstantes.Imprimir(n + 1);

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Variables:\n");
			foreach(DefinicionVariable variable in variables)
				variable.Imprimir(n + 2);

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Funciones:\n");
			foreach(Funcion funcion in funciones)
				funcion.Imprimir(n + 1);
		}

		public bool Validar()
		{
			bool valid = true;

			foreach(Funcion funcion in funciones)
				if (!PrvValidarFuncion(funcion))
				{
					valid = false;
					break;
				}

			return valid;
		}

		bool PrvValidarFuncion(Funcion funcion)
		{
			bool valid = true;

			if (funcion.TipoFuncion == Funcion.TipoFuncionEnum.FUNCION_NORMAL)
			{
				int i = 0;

				while(valid && i < funcion.ListaInstrucciones.ObtenerInstrucciones())
				{
					short param = 0;

					if (funcion.ListaInstrucciones.TieneParametro(i))
						param = funcion.ListaInstrucciones.ObtenerParametro(i);

					switch(funcion.ListaInstrucciones.ObtenerInstruccion(i))
					{
						case ListaInstrucciones.InstruccionesEnum.INST_NADA:
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_SUMAR_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_RESTAR_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_MULT_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_DIV_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_SUMAR_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_RESTAR_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_MULT_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_DIV_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_CONCAT_STRING:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_COMPARE_STRING:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_ISZERO_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_FLOAT_A_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_INT_A_FLOAT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_INT:
							if (param < 0 ||
								param >= TablaConstantes.GetSimbolos() ||
								TablaConstantes.GetSimbolo(param).Type != SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_INT)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_FLOAT:
							if (param < 0 ||
								param >= TablaConstantes.GetSimbolos() ||
								TablaConstantes.GetSimbolo(param).Type != SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_FLOAT)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_STRING:
							if (param < 0 ||
								param >= TablaConstantes.GetSimbolos() ||
								TablaConstantes.GetSimbolo(param).Type != SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_STRING)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_INT:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_FLOAT:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_VAR_STRING:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_INT:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_FLOAT:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;
				
						case ListaInstrucciones.InstruccionesEnum.INST_STACK_PUSH_GLOBAL_VAR_STRING:
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_DUP:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_INT:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_FLOAT:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_OBJ:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_VAR_STRING:
							if (param < 0 ||
								param >= funcion.CantVariables)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_INT:
							//FIX_ME Validar contra la otra definicion
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_FLOAT:
							//FIX_ME Validar contra la otra definicion
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_STACK_POP_GLOBAL_VAR_STRING:
							//FIX_ME Validar contra la otra definicion
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_JUMP_IF_ZERO:
							//FIX_ME Falta simulacion de stack..
							//El break no esta a proposito
						case ListaInstrucciones.InstruccionesEnum.INST_JUMP:
						{
							int dest = i + ListaInstrucciones.lenInstruccion + ListaInstrucciones.lenParametro + param;
							if (dest < 0 ||
								dest > funcion.ListaInstrucciones.ObtenerInstrucciones() - ListaInstrucciones.lenInstruccion) //Tiene que haber espacio para el return
								valid = false;
							//FIX_ME Falta chequear que no salte al medio de una instruccion
							//FIX_ME Falta simulacion de stack..
							break;
						}

						case ListaInstrucciones.InstruccionesEnum.INST_CALL_FUNCTION:
							if (param < 0 ||
								param >= funciones.Count)
								valid = false;
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_CALL_GLOBAL_FUNCTION:
							if (param < 0 ||
								param >= TablaConstantes.GetSimbolos() ||
								TablaConstantes.GetSimbolo(param).Type != SimboloSimple.SimboloTypeEnum.SIMBOLO_TIPO_CONSTANTE_STRING)
							{
								valid = false;
							}
							//FIX_ME Validar contra la otra definicion
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_RETURN:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_INC_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						case ListaInstrucciones.InstruccionesEnum.INST_DEC_INT:
							//FIX_ME Falta simulacion de stack..
							break;

						default:
							valid = false;
							break;
					}

					if (!funcion.ListaInstrucciones.TieneParametro(i))
						i += ListaInstrucciones.lenInstruccion;
					else
						i += ListaInstrucciones.lenInstruccion + ListaInstrucciones.lenParametro;
				}

				if (valid)
				{
					if (funcion.ListaInstrucciones.ObtenerInstrucciones() >= 1)
					{
						if (funcion.ListaInstrucciones.ObtenerInstruccion(funcion.ListaInstrucciones.ObtenerInstrucciones() - 1) != ListaInstrucciones.InstruccionesEnum.INST_RETURN)
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
