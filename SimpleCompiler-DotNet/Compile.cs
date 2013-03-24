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
			Function[] externalFunctions = { new Function("int ShowMessage(string message)", new Function.NativeFunctionDelegate(FuncShowMessage)) };

			string program = File.ReadAllText("Program.txt");

			Parser parser = new Parser();
			CodeGenerator generator = new CodeGenerator();
            List<CodeGeneratorError> errorsCodeGenerator = new List<CodeGeneratorError>();
            List<ParserError> errorsParser = new List<ParserError>();

			SyntTree tree = null;

			parser.SetProgram(program);
			parser.SetExternalsFunctions(externalFunctions);

            if (parser.GenSyntTree(out tree, ref errorsParser))
			{
				tree.Print(0);

				Console.Write("\n\nCompilation OK, generating code\n\n");

				generator.GenerateCode(tree, ref errorsCodeGenerator);		

				generator.GetFunctionsDefinitions().Print(0);

                Console.Write("\n\nCode generation OK, running\n\n");

                Process process = new Process();

				process.AddDefinitions(generator.GetFunctionsDefinitions());

				process.AddExternalFunctions(externalFunctions);

				process.Init();

				process.CallFunction("void#main()");
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
