using System;
using System.Collections;
using CompiladorReducido.Compilador;
using CompiladorReducido.Compartido;
using CompiladorReducido.MaquinaVirtual;
using System.IO;

namespace CompiladorReducido
{
	class Compila
	{
		[STAThread]
		static void Main(string[] args)
		{
			Funcion[] funcionesExternas = { new Funcion("int MostrarMensaje(string mensaje)", new Funcion.FuncionNativaDelegate(FuncNativaMostrarMensaje)) };

			System.IO.StreamReader archivo = File.OpenText("Programa.txt");

			string programa = archivo.ReadToEnd();

			archivo.Close();

			Parser parser = new Parser();
			GeneradorCodigo generador = new GeneradorCodigo();
			ArrayList errores = new ArrayList();

			SyntTree tree = null;

			parser.SetProgram(programa);
			parser.SetFuncionesExternas(funcionesExternas);

			if (parser.GenSyntTree(out tree, out errores))
			{
				tree.Imprimir(0);

				Console.Write("\n\nCompilación exitosa, generando código\n\n");

				generador.GenerarCodigo(tree, out errores);		

				generador.GetDefinicionFunciones().Imprimir(0);

                Console.Write("\n\nCódigo generado, ejecutando\n\n");

                Proceso proceso = new Proceso();

				proceso.AgregarDefiniciones(generador.GetDefinicionFunciones());

				proceso.AgregarFuncionesExternas(funcionesExternas);

				proceso.Inicializar();

				proceso.EjecutarFuncion("void#main()");
			}
			else
			{
				foreach(ParserError err in errores)
				{
                    Console.WriteLine("[{0}, {1}] {2}", err.line, err.column, err.Message);	
				}
			}
		}

		static bool FuncNativaMostrarMensaje(Proceso proceso, MaquinaVirtual.Stack stack, out string descripcionError)
		{
			bool ok = true;
			descripcionError = "";

			string mensaje = stack.ObtenerVarString(0);

			Console.WriteLine(mensaje);

			stack.PushInt(123);

            return ok;
		}
	}
}
