using System;
using System.Collections;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.Compilador
{
	public class SyntTree
	{
		public enum SyntTypeEnum
		{
			SYNT_INSTRUCCION_NULA,
			SYNT_INSTRUCCION,
			SYNT_RETURN,
			SYNT_LISTA_FUNCIONES,
			SYNT_LISTA_INSTRUCCIONES,
			SYNT_DECLARACION_FUNCION,
			SYNT_DECLARACION_VARIABLE,
			SYNT_LLAMADO_FUNCION,
			SYNT_TABLA_SIMBOLOS,
			SYNT_VARIABLE,
			SYNT_IDENTIFICADOR,
			SYNT_CONSTANTE,
			SYNT_CONVERSION,
			SYNT_OP_COMPARACION_IGUAL,
			SYNT_OP_COMPARACION_DISTINTO,
			SYNT_OP_ASIGNACION,
			SYNT_OP_MAS,
			SYNT_OP_MENOS,
			SYNT_OP_POR,
			SYNT_OP_DIV,
			SYNT_IF,
			SYNT_FOR,
			SYNT_WHILE,
			SYNT_NONE
		};

		public SyntTypeEnum	Type;
		public Simbolo	ReturnType;
		public Simbolo	SimboloDefinicionFuncion;

		private ArrayList childs;

		public long Integer;
		public float Float;
		public string String;
		public TablaSimbolos TablaSimbolos;

		public SyntTree()
		{
			String = "";
		}
		
		void Clear()
		{
			TablaSimbolos = null;
			ReturnType = null;
			SimboloDefinicionFuncion = null;
			childs = null;
			Integer = 0;
			Float = 0.0f;
			String = "";
		}

		public void AddChild(SyntTree node)
		{
			if (childs == null)
				childs = new ArrayList();

			childs.Add(node);
        }

		public int GetChilds() 
		{ 
			if (childs == null)
				return 0;

			return(childs.Count); 
		}

		public SyntTree GetChild(int n) 
		{ 
			return (SyntTree) childs[n]; 
		}

		public void Imprimir(int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write("    ");

			int hijo = 0;

			switch(Type)
			{
				case SyntTypeEnum.SYNT_INSTRUCCION:
					Console.Write("Instrucción: \n");
					break;

				case SyntTypeEnum.SYNT_RETURN:
					Console.Write("Return: \n");
					break;

				case SyntTypeEnum.SYNT_FOR:
					Console.Write("Instrucción For:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Inicialización:\n");
					GetChild(hijo++).Imprimir(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condicion:\n");
					GetChild(hijo++).Imprimir(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Incremento:\n");
					GetChild(hijo++).Imprimir(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Ciclo:\n");
					GetChild(hijo++).Imprimir(n + 2);
					break;

				case SyntTypeEnum.SYNT_WHILE:
					Console.Write("Instrucción While:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condición:\n");
					GetChild(hijo++).Imprimir(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Ciclo:\n");
					GetChild(hijo++).Imprimir(n + 2);
					break;

				case SyntTypeEnum.SYNT_IF:
					Console.Write("Instrucción If:\n");
			
					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Condición:\n");
					GetChild(hijo++).Imprimir(n + 2);

					for (i = 0; i < n + 1; i++)
						Console.Write("    ");
					Console.Write("Si es verdad:\n");
					GetChild(hijo++).Imprimir(n + 2);

					if (GetChilds() == 3)
					{
						for (i = 0; i < n + 1; i++)
							Console.Write("    ");
						Console.Write("Si es falso:\n");
						GetChild(hijo++).Imprimir(n + 2);
					}
					break;

				case SyntTypeEnum.SYNT_INSTRUCCION_NULA:
					Console.Write("Instrucción nula\n");
					break;

				case SyntTypeEnum.SYNT_LISTA_FUNCIONES:
					Console.Write("Lista de funciones:\n");
					break;

				case SyntTypeEnum.SYNT_LISTA_INSTRUCCIONES:
					Console.Write("Lista de instrucciones:\n");
					break;

				case SyntTypeEnum.SYNT_IDENTIFICADOR:
					Console.Write("Identificador: {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_VARIABLE:
					Console.Write("Variable: {0}\n", String);
					break;
		
				case SyntTypeEnum.SYNT_CONSTANTE:
					Console.Write("Valor constante ");

					if (ReturnType.Nombre == TipoDato.NOMBRE_INT)
						Console.Write("(entero) {0}\n", Integer);
					else
						if (ReturnType.Nombre == TipoDato.NOMBRE_FLOAT)
							Console.Write("(float) {0}\n", Float);
					else
						if (ReturnType.Nombre == TipoDato.NOMBRE_STRING)
							Console.Write("(cadena) {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_OP_ASIGNACION:
					Console.Write ("Operador =\n");
					break;

				case SyntTypeEnum.SYNT_OP_MAS:
					Console.Write ("Operador +\n");
					break;

				case SyntTypeEnum.SYNT_OP_MENOS:
					Console.Write ("Operador -\n");
					break;

				case SyntTypeEnum.SYNT_OP_POR:
					Console.Write ("Operador *\n");
					break;

				case SyntTypeEnum.SYNT_OP_DIV:
					Console.Write ("Operador /\n");
					break;

				case SyntTypeEnum.SYNT_LLAMADO_FUNCION:
					Console.Write("LLamado a función: {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_DECLARACION_VARIABLE:
					Console.Write("Declaración de variable del tipo {0}\n", String);
					break;

				case SyntTypeEnum.SYNT_DECLARACION_FUNCION:
					Console.Write("Declaración de la función: {0}\n", String);
					break;
		
				case SyntTypeEnum.SYNT_CONVERSION:
					Console.Write("Conversion desde {0} a {1}\n", GetChild(0).ReturnType.Nombre, ReturnType.Nombre);
					break;

				case SyntTypeEnum.SYNT_TABLA_SIMBOLOS:
					Console.Write("Tabla de simbolos\n");
					TablaSimbolos.Imprimir(n + 1);
					break;
			}

			n++;

			for (; hijo < GetChilds(); hijo++)
				GetChild(hijo).Imprimir(n);
		}
	}
}
