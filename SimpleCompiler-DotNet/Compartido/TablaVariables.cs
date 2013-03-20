using System;
using System.Collections;

namespace CompiladorReducido.Compartido
{
	public class TablaVariables
	{
		public 	enum UbicacionVariableEnum
		{
			UBICACION_GLOBAL,
			UBICACION_LOCAL			
		}

		private class DatoVariable
		{
			public string Nombre;
			public UbicacionVariableEnum Ubicacion;
			public int Posicion;
		};

		private ArrayList datosVariable;
		private int posLocal;
		private int posGlobal;

		public TablaVariables()
		{
			datosVariable = new ArrayList();
			posLocal = 0;
			posGlobal = 0;
		}

		public void Limpiar()
		{
			datosVariable = new ArrayList();
			posLocal = 0;
			posGlobal = 0;
		}
        
		public void AgregarVariableGlobal(string nombre)
		{
			DatoVariable datoVariable = new DatoVariable();

			datoVariable.Nombre = nombre;
			datoVariable.Posicion = posGlobal++;
			datoVariable.Ubicacion = UbicacionVariableEnum.UBICACION_GLOBAL;

			PrvAgregarVariable(datoVariable);
		}

		public void AgregarVariableLocal(string nombre)
		{
			DatoVariable datoVariable = new DatoVariable();

			datoVariable.Nombre = nombre;
			datoVariable.Posicion = posLocal++;
			datoVariable.Ubicacion = UbicacionVariableEnum.UBICACION_LOCAL;

			PrvAgregarVariable(datoVariable);
		}
        
		public void EliminarVariable()
		{
			DatoVariable datoVariable = (DatoVariable) datosVariable[datosVariable.Count - 1];

			if (datoVariable.Ubicacion == UbicacionVariableEnum.UBICACION_LOCAL)
				posLocal--;
			else
				posGlobal--;

			datosVariable.RemoveAt(datosVariable.Count - 1);
		}

		public int BuscarVariable(string nombre)
		{
			int i = 0;

			foreach(DatoVariable dv in datosVariable)
			{
				if (dv.Nombre == nombre)
					return i;
			
				i++;
			}

			return(-1);
		}

		public UbicacionVariableEnum UbicacionVariable(int n) 
		{ 
			return ((DatoVariable) datosVariable[n]).Ubicacion; 
		}

		public int PosicionVariable(int n) 
		{ 
			return ((DatoVariable) datosVariable[n]).Posicion; 
		}

		public string NombreVariable(int n)
		{
            return ((DatoVariable) datosVariable[n]).Nombre; 
		}

		public int CantidadVariablesLocales() 
		{ 
			return(posLocal); 
		}

		public int CantidadVariablesGlobales()
		{ 
			return(posGlobal); 
		}

		void PrvAgregarVariable(DatoVariable datoVariable)
		{
			datosVariable.Add(datoVariable);
		}
	}
}
