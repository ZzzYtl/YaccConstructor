//this file was generated by GNESCC
//source grammar:../../../Tests/GNESCC/test_opt/test_opt.yrd
//date:10/12/2011 16:53:50

module GNESCC.Actions_opt

open Yard.Generators.GNESCCGenerator

let getUnmatched x expectedType =
    "Unexpected type of node\nType " + x.ToString() + " is not expected in this position\n" + expectedType + " was expected." |> failwith

let value x = (x:>Lexer_opt.MyLexeme).MValue

let getStr x y =
    let s = 
        if Option.isSome y
        then " " + y.Value
        else ""
    value x + s 

let s0 expr = 
    let inner  = 
        match expr with
        | RESeq [x0; x1] -> 
            let (n) =
                let yardElemAction expr = 
                    match expr with
                    | RELeaf tNUMBER -> tNUMBER :?> 'a
                    | x -> getUnmatched x "RELeaf"

                yardElemAction(x0)
            let (p) =
                let yardElemAction expr = 
                    match expr with
                    | REOpt(opt) -> 
                        let yardOptAction expr = 
                            match expr with
                            | RESeq [x0] -> 
                                let (p) =
                                    let yardElemAction expr = 
                                        match expr with
                                        | RELeaf tPLUS -> tPLUS :?> 'a
                                        | x -> getUnmatched x "RELeaf"

                                    yardElemAction(x0)
                                (value p)
                            | x -> getUnmatched x "RESeq"

                        if opt.IsSome then Some (yardOptAction opt.Value) else None 
                    | x -> getUnmatched x "REOpt"

                yardElemAction(x1)
            (getStr n p)
        | x -> getUnmatched x "RESeq"
    box (inner)

let ruleToAction = dict [|(1,s0)|]

