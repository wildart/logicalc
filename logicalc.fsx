open FSharp.Text.Lexing

#load "build/logicalc-pars.fs"
open Parser

#load "build/logicalc-lex.fs"
open Lexer

let parse str =
    printfn "Evaluate:\n %s" str
    let lexbuf = LexBuffer<char>.FromString str
    Parser.start Lexer.read lexbuf

// And Operation
parse "F & T\n"

// Implication and Inference
parse "F => T |- T\n"

// Variabes
parse """
    a = T
    a & F
"""

// Modus Ponens
parse """
    p = T
    q = F
    (p => q) & p |- q
"""

// Modus Tollens
parse """
    p = F
    q = F
    (p => q) & !q |- !p
"""

// Addition
parse """
    p = T
    q = F
    p |- (p | q)
"""

// Material Equivalence
parse """
    p = T
    q = F
    p <=> q |- (p => q) & (q => p)
"""

// Conditional Expression
parse """
    (if F then F else T)
"""

// Conditional Expression
parse """
    p = T
    q = (if p then F else T)
    r = (if p then F)
    p = F
    q = (if p then F else T)
    r = (if p then F)
    p
"""

printfn "\nALL TESTS PASSED!\n\nPress any key to continue..."
System.Console.ReadLine() |> ignore
