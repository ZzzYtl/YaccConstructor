rem xcopy "%1\%2.tables.fs" %2.tables.fs -Y
fsc -o:..\Tester\bin\Release\_RACCUserApplication.dll --debug:pdbonly --noframework --define:TRACE --doc:bin\Release\RACCUserApplication.XML --optimize+ -r:"%FS%\FSharp.Core.dll" -r:"%FS_POWERPACK%\bin\FSharp.PowerPack.Compatibility.dll" -r:"%FS_POWERPACK%\bin\FSharp.PowerPack.dll" -r:"%NET_4_0%\mscorlib.dll" -r:"%NET_4_0%\System.Core.dll" -r:"%NET_4_0%\System.dll" -r:"%NET_4_0%\System.Numerics.dll" -r:D:\rec_ascent\trunk\YaccConstructor\_RACCCommon\bin\Release\_RACCCommon.dll -r:..\_RACCCore\bin\Debug\_RACCCore.dll -r:..\RACCFiniteAutomata\bin\Debug\_RACCFiniteAutomata.dll --target:dll --warn:3 --warnaserror:76 --vserrors --LCID:1033 --utf8output --fullpaths --flaterrors "C:\Users\gsv\AppData\Local\Temp\.NETFramework,Version=v4.0.AssemblyAttributes.fs" %1\%2.tables.fs Lexer.fs Driver.fs 