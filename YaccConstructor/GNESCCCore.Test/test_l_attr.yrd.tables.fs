//this tables was generated by GNESCC
//source grammar:../../../Tests/GNESCC/test_l_attr/test_l_attr.yrd
//date:10/12/2011 16:53:50

module Yard.Generators.GNESCCGenerator.Tables_l_attr

open Yard.Generators.GNESCCGenerator
open Yard.Generators.GNESCCGenerator.CommonTypes

type symbol =
    | T_NUMBER
    | NT_e
    | NT_s
    | NT_gnesccStart
let getTag smb =
    match smb with
    | T_NUMBER -> 6
    | NT_e -> 5
    | NT_s -> 4
    | NT_gnesccStart -> 2
let getName tag =
    match tag with
    | 6 -> T_NUMBER
    | 5 -> NT_e
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
     [| false; false; false |];
     [| false; false; false |];
     [| false; false; false |]; |]
let gotoTable =
  [| [| Some 2; Some 1; None |];
     [| None; None; None |];
     [| None; None; None |];
     [| None; None; None |]; |]
let actionTable = 
  [| [| [Shift 3]; [Error]; [Error] |];
     [| [Accept]; [Accept]; [Accept] |];
     [| [Reduce 1]; [Reduce 1]; [Reduce 1] |];
     [| [Reduce 2]; [Reduce 2]; [Reduce 2] |]; |]
let tables = 
  {StartIdx=startKernelIdxs
   SymbolIdx=symbolIdx
   GotoTable=gotoTable
   ActionTable=actionTable
   IsStart=isStart
   ProdToNTerm=prodToNTerm}
