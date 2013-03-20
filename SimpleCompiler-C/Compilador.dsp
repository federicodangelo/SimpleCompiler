# Microsoft Developer Studio Project File - Name="Compilador" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Console Application" 0x0103

CFG=Compilador - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "Compilador.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "Compilador.mak" CFG="Compilador - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "Compilador - Win32 Release" (based on "Win32 (x86) Console Application")
!MESSAGE "Compilador - Win32 Debug" (based on "Win32 (x86) Console Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "Compilador - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release"
# PROP Intermediate_Dir "Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_CONSOLE" /D "_MBCS" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /G6 /Gr /W3 /GX /O2 /Ob2 /D "WIN32" /D "NDEBUG" /D "_CONSOLE" /D "_MBCS" /FR /FD /c
# SUBTRACT CPP /YX /Yc /Yu
# ADD BASE RSC /l 0x2c0a /d "NDEBUG"
# ADD RSC /l 0x2c0a /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /machine:I386

!ELSEIF  "$(CFG)" == "Compilador - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug"
# PROP Intermediate_Dir "Debug"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_CONSOLE" /D "_MBCS" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_CONSOLE" /D "_MBCS" /FR /FD /GZ /c
# SUBTRACT CPP /YX /Yc /Yu
# ADD BASE RSC /l 0x2c0a /d "_DEBUG"
# ADD RSC /l 0x2c0a /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /debug /machine:I386 /pdbtype:sept

!ENDIF 

# Begin Target

# Name "Compilador - Win32 Release"
# Name "Compilador - Win32 Debug"
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# End Group
# Begin Group "Compilador"

# PROP Default_Filter ""
# Begin Group "Headers"

# PROP Default_Filter "*.h"
# Begin Source File

SOURCE=.\CGeneradorCodigo.h
# End Source File
# Begin Source File

SOURCE=.\CLexema.h
# End Source File
# Begin Source File

SOURCE=.\CLexer.h
# End Source File
# Begin Source File

SOURCE=.\COptimizador.h
# End Source File
# Begin Source File

SOURCE=.\CParser.h
# End Source File
# Begin Source File

SOURCE=.\CSimbolo.h
# End Source File
# Begin Source File

SOURCE=.\CSyntTree.h
# End Source File
# Begin Source File

SOURCE=.\CTablaSimbolos.h
# End Source File
# End Group
# Begin Group "Sources"

# PROP Default_Filter "*.cpp"
# Begin Source File

SOURCE=.\CGeneradorCodigo.cpp
# End Source File
# Begin Source File

SOURCE=.\CLexema.cpp
# End Source File
# Begin Source File

SOURCE=.\CLexer.cpp
# End Source File
# Begin Source File

SOURCE=.\COptimizador.cpp
# End Source File
# Begin Source File

SOURCE=.\CParser.cpp
# End Source File
# Begin Source File

SOURCE=.\CSimbolo.cpp
# End Source File
# Begin Source File

SOURCE=.\CSyntTree.cpp
# End Source File
# Begin Source File

SOURCE=.\CTablaSimbolos.cpp
# End Source File
# End Group
# End Group
# Begin Group "Maquina Virtual"

# PROP Default_Filter ""
# Begin Group "Headers No. 1"

# PROP Default_Filter "*.h"
# Begin Source File

SOURCE=.\CErrorEjecucion.h
# End Source File
# Begin Source File

SOURCE=.\CObjeto.h
# End Source File
# Begin Source File

SOURCE=.\CObjetoCadena.h
# End Source File
# Begin Source File

SOURCE=.\CProceso.h
# End Source File
# Begin Source File

SOURCE=.\CStack.h
# End Source File
# Begin Source File

SOURCE=.\CVariable.h
# End Source File
# Begin Source File

SOURCE=.\CVariables.h
# End Source File
# End Group
# Begin Group "Sources No. 1"

# PROP Default_Filter "*.cpp"
# Begin Source File

SOURCE=.\CErrorEjecucion.cpp
# End Source File
# Begin Source File

SOURCE=.\CObjeto.cpp
# End Source File
# Begin Source File

SOURCE=.\CProceso.cpp
# End Source File
# Begin Source File

SOURCE=.\CStack.cpp
# End Source File
# Begin Source File

SOURCE=.\CVariable.cpp
# End Source File
# Begin Source File

SOURCE=.\CVariables.cpp
# End Source File
# End Group
# End Group
# Begin Group "Compartido"

# PROP Default_Filter ""
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=.\CCadena.h
# End Source File
# Begin Source File

SOURCE=.\CDefinicionObjeto.h
# End Source File
# Begin Source File

SOURCE=.\CDefinicionVariable.h
# End Source File
# Begin Source File

SOURCE=.\CFuncion.h
# End Source File
# Begin Source File

SOURCE=.\CListaDefinicionesObjetos.h
# End Source File
# Begin Source File

SOURCE=.\CListaInstrucciones.h
# End Source File
# Begin Source File

SOURCE=.\CSimboloSimple.h
# End Source File
# Begin Source File

SOURCE=.\CTablaSimbolosSimple.h
# End Source File
# Begin Source File

SOURCE=.\CTablaVariables.h
# End Source File
# Begin Source File

SOURCE=.\CTipoDato.h
# End Source File
# Begin Source File

SOURCE=.\FuncionesUnix.h
# End Source File
# End Group
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=.\CCadena.cpp
# End Source File
# Begin Source File

SOURCE=.\CDefinicionObjeto.cpp
# End Source File
# Begin Source File

SOURCE=.\CDefinicionVariable.cpp
# End Source File
# Begin Source File

SOURCE=.\CFuncion.cpp
# End Source File
# Begin Source File

SOURCE=.\CListaDefinicionesObjetos.cpp
# End Source File
# Begin Source File

SOURCE=.\CListaInstrucciones.cpp
# End Source File
# Begin Source File

SOURCE=.\CSimboloSimple.cpp
# End Source File
# Begin Source File

SOURCE=.\CTablaSimbolosSimple.cpp
# End Source File
# Begin Source File

SOURCE=.\CTablaVariables.cpp
# End Source File
# Begin Source File

SOURCE=.\CTipoDato.cpp
# End Source File
# End Group
# End Group
# Begin Source File

SOURCE=.\Compilador.cpp
# End Source File
# Begin Source File

SOURCE=.\Programa.txt
# End Source File
# Begin Source File

SOURCE=.\Programa2.txt
# End Source File
# End Target
# End Project
