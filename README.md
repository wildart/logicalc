# Logical Expression Interpreter

## Introduction

Computer program input generally has some structure; in fact, every computer program that does input can be thought of as defining an "input language" which it accepts. An input language may be as complex as a programming language, or as simple as a sequence of numbers. Unfortunately, usual input facilities are limited, difficult to use, and often are lax about checking their inputs for validity.

`yacc` provides a general tool for describing the input to a computer program. The `yacc` user specifies the structures of his input, together with code to be invoked as each such structure is recognized. `yacc` turns such a specification into a subroutine that handles the input process; frequently, it is convenient and appropriate to have most of the flow of control in the user's application handled by this subroutine.

You will use `yacc` tool specifically designed to work with F# compiler - `fsyacc`, and the companion lexical analyzer tool - `fslex`.

In this lab, you will:

1. Become acquainted with syntax of `yacc`/`lex` tools
2. Perform compilation of the provided logical expression grammar
3. Create the logical expression interpreter to evaluate expressions
4. Learn to implement additional parsing rules for more logical expression operations:
    - negation (!)
    - disjunction (|)
    - material equivalence (<=>)
    - conditional expression (if ... then ... else ...)


## Logical Expression Interpreter

We'll use `fslex/fsyacc` tool to design a parser and create an interpreter of logical expressions.

A language for logical expressions has following elements:

- Values: True (`T`) and False (`F`)
- Boolean algebra operations: OR (`|`), AND (`&`), NOT (`!`), IMPLICATION (`=>`)
- Variables & assignment operation (`=`)
- Conditional expressions (`if ... then ... else ...`)
- Parentheses

Examples of logical expression:
```
!T & F

(T | F) => T

if T then (T || F) else F
```

### Grammar

We use following grammar for a logical expression language:

```
start ::= prog EOF

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
Above grammar BFN rules specify nonterminal symbols in lower case letters and terminal symbols in capital letters.

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

### Semantic Actions

A semantic action is a piece of F# code that is executed in order to assign
a semantic value to the nonterminal symbol with which this production group is associated.
A semantic action can refer to the (already computed) semantic values of
the terminal or nonterminal symbols that appear in the production via the semantic
value identifiers bound by the production.

Semantic actions can refer to unnamed semantic values via positional keywords of
the form \$1, \$2, etc. See above example of the logical AND rule, `aterm`,
where the semantic action is defined in curly brackets as `{ $1 && $3 }`.
The meaning of this semantic action is an actual logical AND operation between
values of nonterminal symbols `aterm` and `var` written in form
of the proper [F# boolean operation](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/boolean-operators).


## Files

- `logicalc.fsl`: this file contains tokenization rules for a tokenizer (lexical analyzer)
- `logicalc.fsy`: this file contains grammar rules for a parser (syntactical analyzer)
- `logicalc.fsx`: this script file contains test cases for checking correctness of a logical expression interpreter parser


## First Step

### Windows

Use to following script files while you work on your parser implementation:

- First, run `build-grammar.bat` batch file to generate and build a parser source code a parser source code for a logical expression interpreter
- Next, run `logicalc.bat` batch file to evaluate the generated parser with some test cases defined in the test script file,`logicalc.fsx`

### Linux

Use `make` command to build a parser source code for a logical expression interpreter, and run `logicalc.fsx` script with various test inputs to check correctness of the parser implementation.

Additional commands:
- `make init` install required dependencies
- `make lex` generates lexer source code from rules in `logicalc.fsl` file
- `make yacc` generates parser source code from grammar description in `logicalc.fsy` file
- `make run` runs test script `logicalc.fsx` which evaluates a generated parser


## Problems

You need to implement missing operations and expressions from the above logical expression grammar.

### Negation Operation

First time, you run the test script `logicalc.fsx` file, it stops with the following error:

```
Evaluate:

    p = F
    q = F
    (p => q) & !q |- !p

System.Exception: ParseError: no rule for lexem !
  at (wrapper managed-to-native) System.Reflection.MonoMethod.InternalInvoke(System.Reflection.MonoMethod,object,object[],System.Exception&)
  at System.Reflection.MonoMethod.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x0006a] in <6649516e5b3542319fb262b421af0adb>:0
Stopped due to error
```

The error message ` ParseError: no rule for lexem !` suggests that lexem `!` cannot be processed by the lexer. The `!` symbol corresponds to a unary boolean negation operation which is not implemented.

In order to implement the negation operation, you are required to define a negation token and corresponding lexem processing rules:

1. Add the following tokenization rule to `logicalc.fsl` file:

```
    | '!'    { Parser.NOT }
```

Similar to F# pattern matching rules, `!` is matched to the token `NOT` which is specified in curly brackets.

2. In `logicalc.fsy`, add the following token definition to the other tokens:

```
%token NOT
```

3. In `logicalc.fsy`, find the `var` grammar rule and add the following production of the negation operation to it:

```
    | NOT var          { not $2 }
```

4. Run test script to verify correctness of the operation implementation.

**Note:** Grammar rules follow BNF notation. Rule LHS separated from RHS by `:` symbol. RHS parts of the grammar rule are stared with a bar `|` symbol.
Curly brackets contain a valid F# code that will be executed when expression is matched.

In above negation production of `var` rule, the curly brackets designate semantic
action associated with this production, and `$2` in it references to the second
symbol in it, a nonterminal `var`. When this production is matched during
the expression parsing, the corresponding semantic action is evaluated, all its reverences are matched to corresponding values of the symbols in the production.


### Disjunction Operation

Implement a token and a grammar rule for a disjunction (logical OR) operation.

1. Use `|` symbol to produce `OR` token in the lexer file `logicalc.fsl`.
2. Add `OR` token to the grammar file `logicalc.fsy`.
3. In the grammar file `logicalc.fsy`, write a rule for a disjunction (logical OR) operation, `oterm`, similarly to conjunction (logical AND), see the above grammar for rule definition.
4. Add appropriate semantic actions to the added rule, i.e. performing disjunction operation of two boolean values.
5. Make sure that implemented rule correctly wired with other grammar rules. See that rule symbols **exactly** match the ones in the above grammar, see `iterm` and `aterm` rules.
6. Run test script to verify correctness of the operation implementation.

### Material Equivalence Operation

Implement a token and a grammar rule for a material equivalence operation.

1. Use `<=>` symbol combination to produce `MATEQ` token in the lexer file `logicalc.fsl`.
2. Add `MATEQ` token to the grammar file `logicalc.fsy`.
3. In the grammar file `logicalc.fsy`, write a rule for a material equivalence operation, `mterm`, similarly to implication, look at the above grammar for rule definition.
4. Use following definition of a material equivalence `(p => q) & (q => p)` for implementation of the rule semantic action.
5. Make sure that implemented rule correctly wired with other grammar rules. See that rule symbols **exactly** match the ones in the above grammar, see `expr` and `iterm` rules.
6. Run test script to verify correctness of the operation implementation.

### Conditional Expression

Implement necessary tokens and a grammar rules for a conditional statement.

1. Define tokens `IF`, `THEN` and `ELSE` in the grammar file `logicalc.fsy`.
2. Create necessary tokenizer rules in the lexer file `logicalc.fsl`.
3. In the grammar file `logicalc.fsy`, write a rule for a conditional statement (see above grammar for the rule definition). Alternative branch is optional.
4. Implement rule semantic actions using the F# conditional expression evaluate for corresponding part of the production rule. If an alternative branch is not provided and a logical condition of the conditional statement is `false` then whole expression is evaluated to `false`.
5. Make sure that implemented rule correctly wired with other grammar rules.
6. Run test script to verify correctness of the expression implementation.

## Numerical Literals

An above logical expression language only works with a Boolean values. We will
introduce numerical literals to the language.

A numerical literal is an integer number of various length. For correct integration of numerical literals into the logical expressions language, the numerical literal can only appear as a part of some relational operation.

### Numerical Literal Token

1. Define a numerical literal token in the grammar file `logicalc.fsy`
    - Create a token `NUM` that has an integer type, `int`

2. Define a lexer rule to process numerical literals in the lexer file `logicalc.fsl`
    - Create a regular expression that matches arbitrary sequence of numbers
    - Generate a corresponding token that would contain a value of the matched numerical literal.
        - See `ID` token implementation for the reference.

### Numerical Equality

Numerical equality is a relational expression that has as operands numerical literals.  A corresponding grammar rule for the numerical equality must be added
to the logical expression language:

```
relop ::= NUM NUMEQ NUM
```

Now this rule has to be correctly wired with other grammar rules to ensure that
relational operation is a part of logical expression, in particular the expression rule, `expr`, must be modified.

In the grammar file `logicalc.fsy`:

1. Add the above numerical equality rule.
2. Write a semantic action for this rule that performs checks equality of numerical operands of the numerical equality operation.
3. Change `expr` rule to include relational operation production, and inclue appropriate semantic action.


### Other Relational Operators

1. Define tokens and lexer rules for other numerical operations

2. Extend relational operation `relop` rule to include more productions for other relational operations: inequality (`!=`) and ordering operations (`<`, `>`)

3. Add appropriate semantic actions to the new rules.


## Extra

- Introduce arithmetic operations on numerical literals to the logical expression language
    - Arithmetic operations can be viewed as expressions with numerical literals
    - Define new numerical expression rule, and use it everywhere where the numerical literals where used


## Links

- [FsLex](http://fsprojects.github.io/FsLexYacc/fslex.html)
- [FsYacc](http://fsprojects.github.io/FsLexYacc/fsyacc.html)
- [Menhir Reference Manual](http://gallium.inria.fr/~fpottier/menhir/manual.html)
