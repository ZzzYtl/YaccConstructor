//this tables was generated by GNESCC
//source grammar:../../../Tests/GNESCC/claret/braces_1/test_simple_braces.yrd
//date:10/12/2011 16:53:49

module Yard.Generators.GNESCCGenerator.Tables_simple_braces

open Yard.Generators.GNESCCGenerator
open Yard.Generators.GNESCCGenerator.CommonTypes

type symbol =
    | T_RBR
    | T_LBR
    | NT_start
    | NT_gnesccStart
let getTag smb =
    match smb with
    | T_RBR -> 6
    | T_LBR -> 5
    | NT_start -> 4
    | NT_gnesccStart -> 2
let getName tag =
    match tag with
    | 6 -> T_RBR
    | 5 -> T_LBR
    | 4 -> NT_start
    | 2 -> NT_gnesccStart
    | _ -> failwith "getName: bad tag."
let prodToNTerm = 
  [| 1; 0 |];
let symbolIdx = 
  [| 2; 3; 1; 3; 0; 1; 0 |];
let startKernelIdxs =  [0]
let isStart =
  [| [| true; true |];
     [| false; false |];
     [| false; true |];
     [| false; false |];
     [| false; false |]; |]
let gotoTable =
  [| [| Some 1; None |];
     [| None; None |];
     [| Some 3; None |];
     [| None; None |];
     [| None; None |]; |]
let actionTable = 
  [| [| [Reduce 1]; [Reduce 1; Shift 2]; [Reduce 1]; [Reduce 1] |];
     [| [Accept]; [Accept]; [Accept]; [Accept] |];
     [| [Reduce 1]; [Reduce 1; Shift 2]; [Reduce 1]; [Reduce 1] |];
     [| [Shift 4]; [Error]; [Error]; [Error] |];
     [| [Reduce 1]; [Reduce 1; Shift 2]; [Reduce 1]; [Reduce 1] |]; |]
let tables = 
  {StartIdx=startKernelIdxs
   SymbolIdx=symbolIdx
   GotoTable=gotoTable
   ActionTable=actionTable
   IsStart=isStart
   ProdToNTerm=prodToNTerm}
