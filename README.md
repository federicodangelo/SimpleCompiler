Simple Compiler
===============

Very simple script language with ports in C# and C++ that can be used to add scripting to any kind of application.

- "SimpleCompiler-C" contains the C++ implementation, made in Visual Studio 2005
- "SimpleCompiler-DotNet" contains the C# implementation, made in Xamarin Studio 4 (Visual Studio compatible)

Both programs can parse / execute the same scripts in the same syntax.

The C version of the code is in spanish, but I have recently translated most of the C# version to english.

Here is an example of the kind of scripts that it supports:
```C
int Test(int a) 
{
	a = a + 1;
	int b = a;

	if (b == 2)
		b = 33;

	return(b);
}

string addStrings(string a, string b)
{
	return(a+b);
}

string compareStrings(string a)
{
	string d;
	
	if (a == "Something")
		d = "Something Works!";
		
	return(d);		
}

void main()
{
	string s = addStrings("Some", "thing");
	
	compareStrings(a);
	
	Test();
}
```
