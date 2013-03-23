using System;
using System.Collections.Generic;
using SimpleCompiler.VirtualMachine;

namespace SimpleCompiler.Shared
{
	public class Function
	{
		public enum FuncionTypeEnum 
		{ 
			FUNCTION_NORMAL, 
			FUNCTION_NATIVE 
		}

        public delegate bool NativeFunctionDelegate(Process process, VirtualMachine.Stack stack, out string errorDescription);

		public string Name;
		public string NameShort;
		public InstructionsList InstructionsList;
		public DataType	ReturnType;
        private List<VariableDefinition> parameters;
		public int NumberOfVariables;
		public FuncionTypeEnum FunctionType;
		public NativeFunctionDelegate NativeFunction;
		
		public Function()
		{
            parameters = new List<VariableDefinition>();
			Name = "";
			FunctionType = FuncionTypeEnum.FUNCTION_NORMAL;
			InstructionsList = new InstructionsList(8192);
		}

		public Function(string definition, NativeFunctionDelegate nativeFunction)
		{
            parameters = new List<VariableDefinition>();
            this.NativeFunction = nativeFunction;
			FunctionType = FuncionTypeEnum.FUNCTION_NATIVE;
			LoadFromDefinition(definition);
		}

		public void Clear()
		{
			parameters.Clear();
			Name = "";
            FunctionType = FuncionTypeEnum.FUNCTION_NORMAL;
			InstructionsList.Clear();
		}

		public void LoadFromDefinition(string definition)
		{
			bool ok = false;

			definition = definition.Trim();

			while(definition.IndexOf("  ") >= 0)
				definition = definition.Replace("  ", " ");

			while(definition.IndexOf(", ") >= 0)
				definition = definition.Replace(", ", ",");

			while(definition.IndexOf(" ,") >= 0)
				definition = definition.Replace(" ,", ",");

			try
			{
				string[] returnType = definition.Split( new char[] { ' ' }, 2);
				//tipoDevuelto[0] == tipo devuelto
				//tipoDevuelto[1] == resto de la declaraci�n de funcion
	            
				if (returnType.Length == 2)
				{
					this.ReturnType = new DataType(returnType[0]);

					string[] name = returnType[1].Split(new char[] { '(' }, 3);
					//nombre[0] == nombre de la funcion
					//nombre[1] == resto parametros menos el parentesis de comienzo

					if (name.Length == 2)
					{
						this.NameShort = name[0];

						string[] pars = name[1].Split(new char[] { ',' });
						//pars[0] == primer parametro o ")"
						//pars[n] == siguiente parametro
						//pars[ultimo] = parametro + ")"
	                    
						if (pars.Length >= 1)
						{
							if (pars[0] == ")")
							{
								ok = true;
							}
							else
							{
								ok = true;

								for (int i = 0; ok == true && i < pars.Length; i++)
								{
									int posPar = pars[i].IndexOf(')');

									if (posPar >= 0)
									{
										if (i != pars.Length - 1 ||
											posPar != pars[i].Length - 1)
										{
											ok = false;
										}
										else
										{
											pars[i] = pars[i].Substring(0, pars[i].Length - 1);
										}
									}

									if (ok)
									{
										string[] defs = pars[i].Split(new char[] { ' ' } , 3);
										//defs[0] == tipo de dato
										//defs[1] == nombre de parametro

										if (defs.Length == 2)
										{
											VariableDefinition defParam = new VariableDefinition();

											defParam.Type = new DataType(defs[0]);
											defParam.Name = defs[1];
											
											AddParameter(defParam);
										}
										else
											ok = false;										
									}
								}
							}
						}
					}
				}

				if (ok)
					UpdateFullName();
				else
					throw new Exception("La definici�n pasada es inv�lida [" + definition + "]");
			}
			catch(Exception)
			{
                throw new Exception("La definici�n pasada es inv�lida [" + definition + "]");
			}
		}

		public void UpdateFullName()
		{
			string nombre = "";
			string separador = ",";
			string separadorFuncion = "#";

			if (ReturnType == null)
				nombre += DataType.NAME_VOID;
			else
				nombre += ReturnType.ToString();

			nombre += separadorFuncion;
			nombre += NameShort;
			nombre += "(";

			for (int i = 0; i < GetParameters(); i++)
			{
				if (i != 0)
					nombre += separador;

				nombre += GetParameter(i).Type.ToString();
			}

			nombre += ")";

			this.Name = nombre;
		}

		public void AddParameter(VariableDefinition parametro)
		{
			parameters.Add(parametro);
		}
		
		public int GetParameters() 
		{ 
			return parameters.Count; 
		}

		public VariableDefinition GetParameter(int n) 
		{ 
			return (VariableDefinition) parameters[n];
		}

		public void Print(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			Console.Write("Function: {0}\n", Name);

			for (i = 0; i < n + 1; i++)
				Console.Write("    ");

			Console.Write("Parameters:\n");
			foreach(VariableDefinition parameter in parameters)
				parameter.Print(n + 2);
	
			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
	
			Console.Write("Number of local vars: {0}\n", NumberOfVariables);
	
			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
	
			Console.Write("Instructions:\n");
			InstructionsList.Print(n + 2);

			for (i = 0; i < n + 1; i++)
				Console.Write("    ");
		}
	}
}
