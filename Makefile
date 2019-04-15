FSHC=fsharpc
FSHI=fsharpi

OUTPUT=build
LEX=$(OUTPUT)/FsLexYacc.8.0.1/build/FsLex.exe
YACC=$(OUTPUT)/FsLexYacc.8.0.1/build/FsYacc.exe
LEXYACCLIB=$(OUTPUT)/FsLexYacc.Runtime.8.0.1/lib/net46/FsLexYacc.Runtime.dll

PROG?=logicalc
FS_LEXER=$(OUTPUT)/$(PROG)-lex.fs
FS_PARSER=$(OUTPUT)/$(PROG)-pars.fs
FSI_PARSER=$(OUTPUT)/$(PROG)-pars.fsi

all: lex yacc run

init:
	nuget restore -OutputDirectory $(OUTPUT)

lex:
	$(LEX) $(PROG).fsl -o $(FS_LEXER) --unicode

yacc:
	$(YACC) $(PROG).fsy -v -o $(FS_PARSER) --module Parser

compile:
	$(FSHC) -r $(LEXYACCLIB) --out:build/test.exe $(FS_PARSER) $(FS_LEXER) Program.fs
	cp $(LEXYACCLIB) $(OUTPUT)/

run:
	$(FSHI) --reference:build/FsLexYacc.Runtime.8.0.1/lib/net46/FsLexYacc.Runtime.dll $(PROG).fsx

pdf:
	mkdir -p docs
	pandoc -t latex --pdf-engine=xelatex -o docs/$(PROG).pdf README.md

.PHONY: clean

clean:
	rm -rf $(OUTPUT)