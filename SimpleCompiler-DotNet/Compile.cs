using System;
using System.Collections.Generic;
using SimpleCompiler.Compiler;
using SimpleCompiler.Shared;
using SimpleCompiler.VirtualMachine;
using System.IO;

namespace SimpleCompiler
{
	class Compile
	{
		[STAThread]
		static void Main(string[] args)
		{
			Function[] funcionesExternas = { new Function("int ShowMessage(string message)", new Function.NativeFunctionDelegate(FuncShowMessage)) };

			System.IO.StreamReader archivo = File.OpenText("Program.txt");

			string programa = archivo.ReadToEnd();

			archivo.Close();

			Parser parser = new Parser();
			CodeGenerator generador = new CodeGenerator();
            List<CodeGeneratorError> errorsCodeGenerator = new List<CodeGeneratorError>();
            List<ParserError> errorsParser = new List<ParserError>();

			SyntTree tree = null;

			parser.SetProgram(programa);
			parser.SetExternalsFunctions(funcionesExternas);

            if (parser.GenSyntTree(out tree, ref errorsParser))
			{
				tree.Print(0);

				Console.Write("\n\nCompilation OK, generating code\n\n");

				generador.GenerateCode(tree, ref errorsCodeGenerator);		

				generador.GetFunctionsDefinitions().Print(0);

                Console.Write("\n\nCode generation OK, running\n\n");

                Process proceso = new Process();

				proceso.AddDefinitions(generador.GetFunctionsDefinitions());

				proceso.AddExternalFunctions(funcionesExternas);

				proceso.Init();

				proceso.CallFunction("void#main()");
			}
			else
			{
                foreach(ParserError err in errorsParser)
				{
                    Console.WriteLine("[{0}, {1}] {2}", err.line, err.column, err.Message);	
				}
			}
		}

        static bool FuncShowMessage(Process process, VirtualMachine.Stack stack, out string errorDescription)
		{
			bool ok = true;
			errorDescription = "";

			string message = stack.GetVarString(0);

			Console.WriteLine(message);

			stack.PushInt(123);

            return ok;
		}
	}
}
