﻿module InputLoading
open Yard.Generators.GLL
open Yard.Generators.Common.ASTGLLFSA
open Yard.Generators.GLL.ParserCommon
open AbstractAnalysis.Common
open Yard.Frontends.YardFrontend
open YC.API
open AbstractParser
open System
open System.Collections.Generic
open Yard.Core
open Yard.Core.IL
//open Yard.Generators.Common.FSA.Common

open Yard.Generators.Common.AutomataCombinators

(*
ba: ASSERT
ca: ASSERT

s0: C s0 RT s0 | G s0 RL s0 | ca s0 | ca | eps

s1: C s1 RT s1 | G s0 RL s1 | eps

[<Start>]
s: ba s | s ba| s1 s | s s1 | ba | C s RT s1 | C s RT s 
*)
(*
let genGrammar calls locks asserts = 
    let assertsGrm = [|0 .. asserts - 1|] |> Array.map (fun i -> "A" + string i) |> String.concat " | "
    let mutable grmHead = 
        "ba: " + assertsGrm + " \n"
      + "ca: " + assertsGrm + " \n"
      
    let genBrs tmplt count =
        [|0..count - 1|] 
        |> Array.map (fun i -> sprintf tmplt i i)
        |> String.concat "\n    |" 

    let s1Head  = "\ns1: {} \n"
    let s1Calls = genBrs " C%i s1 RT%i s1" calls
    let s1Locks = genBrs " G%i s0 RL%i s1" locks
    
    let sHead = 
        "[<Start>]\n"
      + "s: ba s | s ba | s s1 | s1 s | ba \n"
    let sCalls1 = genBrs " C%i s RT%i s1" calls
    //let sCalls2 = genBrs " C%i s1 RT%i s" calls
    let sCalls3 = genBrs " C%i s RT%i s" calls
    
    let s0Head  = "\ns0: {} | ca s0 | ca \n"
    let s0Calls = genBrs " C%i s0 RT%i s0" calls
    let s0Locks = genBrs " G%i s0 RL%i s0" locks
    
    let alts a = a |> String.concat  "\n    |"
    
    grmHead
    + alts [|sHead; sCalls1;(* sCalls2;*) sCalls3|]
    + "\n" 
    + alts [|s0Head; s0Calls; s0Locks|]
    + "\n"
    + alts [|s1Head; s1Calls; s1Locks|] 
    + "\n"
    *)

let genParser calls locks asserts =
    let mutable time = System.DateTime.UtcNow
    
    let factory = new AutomataFactory()
    let (~%), (~&), eps, (=>), (!=>), (<~>), (<|>) = factory.Combinators

    let asserts() = ([|0..asserts - 1|] 
        |> Array.map (fun i -> %(sprintf "A%i" i)) 
        |> (fun a -> Array.fold (<|>) a.[0] a.[1..]))
    
    let brackets count left body right = ([|0..count - 1|]
        |> Array.map (fun i -> (%(sprintf left i) <~> &body <~> %(sprintf right i)))
        |> (fun a -> Array.fold (<|>) a.[0] a.[1..]))

    "ba" => asserts()
    "ca" => asserts()
    "s0" => ((    (brackets calls "C%i" "s0" "RT%i") 
              <|> (brackets calls "G%i" "s0" "RL%i") 
              <~> &"s0")
             <|> (&"ca" <~> (&"s0" <|> eps))
             <|> eps)
    "s1" => ((    (brackets calls "C%i" "s1" "RT%i") 
              <|> (brackets calls "G%i" "s0" "RL%i") 
              <~> &"s1")
             <|> eps)
    "s" !=> (((brackets calls "C%i" "s" "RT%i") <~> (&"s" <|> &"s1"))
             <|> (&"s" <~> (&"s1" <|> &"ba"))
             <|> (&"s1" <~> &"s")
             <|> (&"ba" <~> (&"s" <|> eps)))

    printfn "Automata construction time is %A" (System.DateTime.UtcNow - time)

    time <- System.DateTime.UtcNow

    let automata, tokens = factory.Produce()

    printfn "Automata generation time is %A" (System.DateTime.UtcNow - time)

    let gll = new GLL()
    gll.GenerateFromFSA automata tokens true "gll.fs" :?> ParserSourceGLL

let parseGraphFile graphFile = 
    let data = System.IO.File.ReadAllLines graphFile
    
    let data = if data.[data.Length-1].Length < 1 then data.[..data.Length-2] else data
    
    let infoLine = data.[data.Length-2]
    let startVLine = data.[data.Length-1] 
    
    let edgesLines = data.[..data.Length-3]

    let info = infoLine.Split ' '

    let calls = int <| info.[1].Trim()
    let locks = int <| info.[2].Trim()    
    let asserts = int <| info.[3].Trim()

    let tryParseInt str =
        try int str
        with e -> 0

    let startVerts = startVLine.Split ' ' |> Array.map tryParseInt 
    let edges = 
        edgesLines |> Array.map (fun s -> s.Split ' ' |> fun a -> new ParserEdge<_>(int a.[0], int a.[2], a.[1]))

    let parser = genParser calls locks asserts
    
    parser, edges, startVerts

let loadInput graphFile =
    let parserSource, edges, startVerts = parseGraphFile graphFile

    let tokenizer str = str |> parserSource.StringToToken |> int
    
    let r = new HashSet<_>()
    let ev = edges |> Array.iter (fun e ->
        r.Add e.Source |> ignore
        r.Add e.Target |> ignore)

    printfn "Start: %A" startVerts.Length
    let inputGraph = new SimpleInputGraph<_>(startVerts |> Array.filter (fun x -> r.Contains x), [||], tokenizer)
    
    //let inputGraph = new SimpleInputGraph<_>(startVerts, [||], tokenizer)
    inputGraph.AddVerticesAndEdgeRange edges |> ignore

    parserSource, inputGraph