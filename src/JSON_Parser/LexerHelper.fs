module LexerHelper

open AbstractLexer.Core
open Microsoft.FSharp.Text
open Microsoft.FSharp.Reflection
open JSON.Parser
open Yard.Utils.SourceText
open Yard.Utils.StructClass
open AbstractAnalysis.Common
open YC.AbstractAnalysis.CommonInterfaces

open System

//let appendBuf (str:string) = str_buf.Append(str) |> ignore

let getLiteral brs value =
    match genLiteral value (value,brs) with
    | Some x -> x
    | None -> raise (LexerError(value,box brs)) //failwithf "Fail to get token with name %s " value