using System;
using System.Collections.Generic;

namespace SimpleCompiler.Shared
{
	public class VariablesTable
	{
		public 	enum VariableLocationEnum
		{
			LOCATION_GLOBAL,
			LOCATION_LOCAL			
		}

		private class VariableData
		{
			public string name;
			public VariableLocationEnum location;
			public int position;
		};

		private List<VariableData> variableData;
		private int posLocal;
		private int posGlobal;

		public VariablesTable()
		{
            variableData = new List<VariableData>();
			posLocal = 0;
			posGlobal = 0;
		}

		public void Clear()
		{
            variableData = new List<VariableData>();
            posLocal = 0;
			posGlobal = 0;
		}
        
		public void AddGlobalVariable(string name)
		{
			VariableData datoVariable = new VariableData();

			datoVariable.name = name;
			datoVariable.position = posGlobal++;
			datoVariable.location = VariableLocationEnum.LOCATION_GLOBAL;

			PrvAddVariable(datoVariable);
		}

		public void AddLocalVariable(string nombre)
		{
			VariableData datoVariable = new VariableData();

			datoVariable.name = nombre;
			datoVariable.position = posLocal++;
			datoVariable.location = VariableLocationEnum.LOCATION_LOCAL;

			PrvAddVariable(datoVariable);
		}
        
		public void RemoveLastVariable()
		{
			VariableData datoVariable = (VariableData) variableData[variableData.Count - 1];

			if (datoVariable.location == VariableLocationEnum.LOCATION_LOCAL)
				posLocal--;
			else
				posGlobal--;

			variableData.RemoveAt(variableData.Count - 1);
		}

		public int FindVariable(string name)
		{
			int i = 0;

			foreach(VariableData dv in variableData)
			{
				if (dv.name == name)
					return i;
			
				i++;
			}

			return(-1);
		}

		public VariableLocationEnum GetVariableLocation(int n) 
		{ 
			return ((VariableData) variableData[n]).location; 
		}

		public int GetVariablePosition(int n) 
		{ 
			return ((VariableData) variableData[n]).position; 
		}

		public string GetVariableName(int n)
		{
            return ((VariableData) variableData[n]).name; 
		}

		public int GetNumberOfLocalVariable() 
		{ 
			return(posLocal); 
		}

		public int GetNumberOfGlobalVariables()
		{ 
			return(posGlobal); 
		}

		private void PrvAddVariable(VariableData datoVariable)
		{
			variableData.Add(datoVariable);
		}
	}
}
