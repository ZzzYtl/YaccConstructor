//this tables was generated by GNESCC
//source grammar:ebnf_implicit.yrd
//date:16.12.2011 11:39:53

module Yard.Generators.GNESCCGenerator.Tables_ebnf_implicit

open Yard.Generators.GNESCCGenerator
open Yard.Generators.GNESCCGenerator.CommonTypes

type symbol =
    | T_A
    | NT_b
    | NT_s
    | NT_gnesccStart
let getTag smb =
    match smb with
    | T_A -> 6
    | NT_b -> 5
    | NT_s -> 4
    | NT_gnesccStart -> 2
let getName tag =
    match tag with
    | 6 -> T_A
    | 5 -> NT_b
    | 4 -> NT_s
    | 2 -> NT_gnesccStart
    | _ -> failwith "getName: bad tag."
let prodToNTerm = 
  [| 2; 1; 0 |];
let symbolIdx = 
  [| 1; 2; 2; 3; 1; 0; 0 |];
let startKernelIdxs =  [0]
let isStart =
  [| [| true; true; true |];
     [| false; true; true |];
     [| false; false; false |];
     [| false; false; false |];
     [| false; true; true |]; |]
let gotoTable =
  [| [| Some 2; Some 1; None |];
     [| Some 2; Some 4; None |];
     [| None; None; None |];
     [| None; None; None |];
     [| Some 2; Some 4; None |]; |]
let actionTable = 
  [| [| [Reduce 2; Shift 3]; [Reduce 2]; [Reduce 2] |];
     [| [Reduce 2; Shift 3]; [Accept]; [Accept; Reduce 2] |];
     [| [Reduce 1]; [Reduce 1]; [Reduce 1] |];
     [| [Reduce 1]; [Reduce 1]; [Reduce 1] |];
     [| [Reduce 2; Shift 3]; [Reduce 2]; [Reduce 2] |]; |]
let tables = 
  {StartIdx=startKernelIdxs
   SymbolIdx=symbolIdx
   GotoTable=gotoTable
   ActionTable=actionTable
   IsStart=isStart
   ProdToNTerm=prodToNTerm}
