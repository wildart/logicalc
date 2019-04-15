@echo off

SET FSHC=bin\FSharp\fsc.exe
SET LEX=bin\FsLexYacc.8.0.1\build\FsLex.exe
SET YACC=bin\FsLexYacc.8.0.1\build\FsYacc.exe
SET LEXYACCLIB=bin\FsLexYacc.Runtime.8.0.1\lib\net46\FsLexYacc.Runtime.dll

SET PROG=logicalc
SET OUTPUT=build
SET FS_LEXER=%OUTPUT%\%PROG%-lex.fs
SET FS_PARSER=%OUTPUT%\%PROG%-pars.fs

mkdir %OUTPUT%
del /q %OUTPUT%\*

%LEX% %PROG%.fsl -o %FS_LEXER% --unicode

%YACC% %PROG%.fsy -v -o %FS_PARSER% --module Parser

rem %FSHC% -r %LEXYACCLIB) --out:build/test.exe %FS_PARSER% %FS_LEXER% Program.fs
rem copy %LEXYACCLIB% %OUTPUT%\

pause