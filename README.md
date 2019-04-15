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

For implementing an interpreter based on the above grammar. We'll use `fslex` and `fsyacc` tools.

`fslex` requires a collection of rule

See file `logicalc.fsy` for implementation of some of above rules. The LHS of the rule (a nonterminal symbol is separated from rule RHS by `:` symbol. Multiple RHS productions separated by `|` symbol.

You can find definition of  terminal symbols (tokens) in `logicalc.fsy` file:

```
%token T
%token F
%token <string> ID
%token NEWLINE ASSIGN AND IMPLIES INFER LPAR RPAR
```

All terminal symbols needed to be correctly tokenized by `fslex` program.
Corresponding tokenizer matching rules are located in `logicalc.fsl` file:

```
rule read =
    parse
    | whitespace     { read lexbuf }
    | newline	     { Parser.NEWLINE }
    | ['T' 't']      { Parser.T }
    | ['F' 'f']      { Parser.F }
    | '='            { Parser.ASSIGN }
    | '&'            { Parser.AND }
    | "=>"           { Parser.IMPLIES }
    | "|-"           { Parser.INFER }
    | '('            { Parser.LPAR }
    | ')'            { Parser.RPAR }
    | ['a'-'z']+     { ID( lexem lexbuf ) }
    | _              { failwith ("ParseError: lexem " + lexem lexbuf) }
    | eof   	     { Parser.EOF }

```

Structure of the lexical analyzer tokenization rule closely follows the F# mattern matching expression. Each pattern is a regular expression which matches character or string to a token or some F# expression.

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
