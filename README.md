# Logical Expression Interpreter

## Introduction

Computer program input generally has some structure; in fact, every computer program that does input can be thought of as defining an "input language" which it accepts. An input language may be as complex as a programming language, or as simple as a sequence of numbers. Unfortunately, usual input facilities are limited, difficult to use, and often are lax about checking their inputs for validity.

`yacc` provides a general tool for describing the input to a computer program. The `yacc` user specifies the structures of his input, together with code to be invoked as each such structure is recognized. `yacc` turns such a specification into a subroutine that handles the input process; frequently, it is convenient and appropriate to have most of the flow of control in the user's application handled by this subroutine.

You will use `yacc` tool specifically designed to work with F# compiler - `fsyacc`, and the companion lexical analyzer tool - `fslex`.

In this lab, you will:

1. Become acquainted with syntax of `yacc`/`lex` tools
2. Perform compilation of the provided propositional calculus grammar
3. Use the propositional calculus interpreter to evaluate expressions
3. Learn to implement additional parsing rules for more propositional calculus operations:
    - negation (!)
    - disjunction (|)
    - material equivalence (<=>)
    - conditional expression (if ... then ... else ...)


## Grammar

For our propositional calculus implementation, we use following grammar:

```
start ::= prog

prog  ::= prog stmnt
        | stmnt

stmnt ::= NEWLINE
        | expr INFER expr NEWLINE
        | expr NEWLINE

expr  ::= ID ASSIGN expr
        | mterm

mterm ::= mterm MATEQ iterm
        | iterm

iterm ::= iterm IMPLIES oterm
        | oterm

oterm ::= oterm OR aterm
        | aterm

aterm ::= aterm AND var
        | var

var   ::= T
        | F
        | ID
        | NOT var
        | LPAR expr RPAR
        | LPAR IF var THEN expr ELSE expr RPAR
        | LPAR IF var THEN expr RPAR
```
Above grammar uses number of nonterminal and terminal symbols (in capital letters).

## FsLex / FsYacc

For implementing an interpreter based on the above grammar, [fslex](http://fsprojects.github.io/FsLexYacc/fslex.html) and [fsyacc](http://fsprojects.github.io/FsLexYacc/fsyacc.html) will be used.

A lexical analyzer tool, `fslex`, performs a lexem generation for a character input. The lexem generation is guided by a collection of rules defined in `logicalc.fsl` file. These rules match character input to tokes used in grammar through direct character, string or regular expression match.

Structure of the lexical analyzer tokenization rule closely follows the F# pattern matching expression. Each pattern is a regular expression which matches character or string to a token or some F# expression.

All terminal symbols used in the above grammar needed to be correctly tokenized by `fslex` program, in order to insure the parser's correct work.
See an example of tokenization rules defined in `logicalc.fsl` file:

```
rule read =
    parse
    | whitespace     { read lexbuf }
    | newline	     { Parser.NEWLINE }
    | ['T' 't']      { Parser.T }
    | ['F' 'f']      { Parser.F }
    | '='            { Parser.ASSIGN }
    | '&'            { Parser.AND }
    | ['a'-'z']+     { ID( lexem lexbuf ) }
    | _              { failwith ("ParseError: lexem " + lexem lexbuf) }
    | eof   	     { Parser.EOF }
```

Syntax analysis and a parser code generation is performed by `fsyacc` tool.
`fsyacc` requires a file with a description of grammar terminal symbols (tokens) and rules.

You can find definition of tokens (grammar terminal symbols) in `logicalc.fsy` file:

```
%token T
%token F
%token <string> ID
%token NEWLINE ASSIGN AND EOF
```

The file `logicalc.fsy` also contains the implementation of some of above grammar rules. The LHS of the rule (a nonterminal symbol) is separated from rule RHS by `:` symbol. Multiple RHS productions separated by `|` symbol.

So the following BNF rule

```
aterm ::= aterm AND var
        | var
```

is written as

```
aterm:
     | aterm AND var    { $1 && $3 }
     | var              { $1 }
```

## Files

- `logicalc.fsl`: contains tokenization rules for a tokenizer (lexical analyzer)
- `logicalc.fsy`: contains grammar rules for a parser (syntactical analyzer)
- `logicalc.fsx`: contains test cases for checking correctness of a logical expression interpreter


## First Step

### Windows

Use to following script files while you work on your parser implementation:

- First, run `build-grammar.bat` script to generate and build a parser source code a parser source code for a logical expression interpreter
- Next, run `logicalc.bat` script to evaluate the generated parser with some test cases defined in the test script file,`logicalc.fsx`

### Linux

Use `make` command to build a parser source code for a logical expression interpreter, and run `logicalc.fsx` script with various test inputs to check correctness of the parser implementation.

Additional commands:
- `make init` install required dependencies
- `make lex` generates lexer source code from rules in `logicalc.fsl` file
- `make yacc` generates parser source code from grammar description in `logicalc.fsy` file
- `make run` runs test script `logicalc.fsx` which evaluates a generated parser

## Problems

Now you implement missing operations and expressions from the above propositional calculus grammar.

### Negation Operation

For negation operation, first, you need to implement a negation token and associated lexem processing

1. Add following tokenization rule to `logicalc.fsl` file:

```
    | '!'    { Parser.NOT }
```

Similar to pattern matching rules, `!` is matched to the token `NOT` which listed in curly brackets.

2. In `logicalc.fsy`, add token definition to the other tokens:

```
%token NOT
```

3. In `logicalc.fsy`, find the `var` grammar rule and add a production for the negation operation to it:

```
    | NOT var          { not $2 }
```

Grammar rules definition follow BNF notation. Rule LHS separated from RHS by `:` symbol. RHS parts of the grammar rule are stared with a bar `|` symbol.
Curly brackets contain a valid F# code that will be executed when expression is matched.
In above negation production of `var` rule, `$2` in curly braces references to the second symbol in rule which is a nonterminal `var` and is evaluated to a current state of this symbol during parsing.


### Disjunction Operation

Implement a token and a grammar rule for a disjunction (logical OR) operation.

1. Use `|` symbol to produce `OR` token in the lexer file `logicalc.fsl`
2. Add `OR` token to the grammar file `logicalc.fsy`
3. Write a grammar rule for a disjunction operation, `oterm`, similarly to conjunction (logical AND), see the above grammar for rule definition.
4. Make sure that implemented rule correctly wired with other grammar rule. See that rule symbols **exactly** match the ones in the above grammar, see `iterm` and `aterm` rules.

### Material Equivalence Operation

Implement a token and a grammar rule for a material equivalence operation.

1. Use `<=>` symbol combination to produce `MATEQ` token in the lexer file `logicalc.fsl`
2. Add `MATEQ` token to the grammar file `logicalc.fsy`
3. Write a grammar rule for a material equivalence operation, `mterm`, similarly to implication, look at the above grammar for rule definition.
4. Use following definition of a material equivalence `(p => q) & (q => p)` for implementation of the rule code segment.
5. Make sure that implemented rule correctly wired with other grammar rule. See that rule symbols **exactly** match the ones in the above grammar, see `expr` and `iterm` rules.

### Conditional Expression

Implement necessary tokens and a grammar rules for a conditional statement.

1. Difine tokens `IF`, `THEN` and `ELSE` in the grammar file `logicalc.fsy`
2. Create necessary tokenizer rules in the lexer file `logicalc.fsl`
3. Write a grammar rule for a conditional statement (see above grammar for the rule definition). Alternative branch is optional.
4. Use F# conditional expression evaluate corresponding part of the production rule. If an alternative branch is not provided and a logical condition of the conditional statement is `false` then whole expression is evaluated to `false`.
5. Make sure that implemented rule correctly wired with other grammar rule.


## Extra

- Introduce integer numbers to your interpreter
    1. Create appropriate regular expression for tokenize numerical literals
    2. Add necessary productions to the grammar to handle numerical values
    3. Introduce arithmetic operations to the grammar

## Links

- [FsLex](http://fsprojects.github.io/FsLexYacc/fslex.html)
- [FsYacc](http://fsprojects.github.io/FsLexYacc/fsyacc.html)
- [Menhir Reference Manual](http://gallium.inria.fr/~fpottier/menhir/manual.html)
