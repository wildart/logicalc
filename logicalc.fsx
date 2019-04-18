open FSharp.Text.Lexing

#load "build/logicalc-pars.fs"
open Parser

#load "build/logicalc-lex.fs"
open Lexer

let parse str result =
    printfn "Evaluate:\n %s" str
    let lexbuf = LexBuffer<char>.FromString str
    if (Parser.start Lexer.read lexbuf) = result
    then printfn "Expression is correct."
    else failwithf "Expression interpreted incorrectly. Expression must be evaluated to `%b`." (not result)


// And Operation
parse "F & T\n" false

// Implication and Inference
parse "F => T |- T\n" true

// Variabes
parse """
    a = T
    a & F
""" false

// Modus Ponens
parse """
    p = T
    q = F
    (p => q) & p |- q
""" true

// Modus Tollens
parse """
    p = F
    q = F
    (p => q) & !q |- !p
""" true

// Addition
parse """
    p = T
    q = F
    p |- (p | q)
""" true

// Material Equivalence
parse """
    p = T
    q = F
    p <=> q |- (p => q) & (q => p)
""" true

// Conditional Expression
parse """
    (if F then F else T)
""" true

// Conditional Expression
parse """
    p = T
    q = (if p then F else T)
    r = (if p then F)
    q => r
""" true

parse """
    p = F
    q = (if p then F else T)
    r = (if p then F)
    q & r
""" false

// Numerical literals and equality
parse """
    (1 == 1)
""" true

// Other relational operations
parse """
    (1113 > 1) & (19823 != 1)
""" true

printfn "\nALL TESTS PASSED!\n\nPress any key to continue..."
System.Console.ReadLine() |> ignore
