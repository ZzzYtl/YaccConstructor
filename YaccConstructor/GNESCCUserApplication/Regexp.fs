﻿//this file was generated by GNESCC
//source grammar:D:\YC\recursive-ascent\Tests\GNESCC\test_seq\test_seq.yrd
//date:10/8/2011 6:41:54 PM

module GNESCC.Regexp

open Yard.Generators.GNESCCGenerator
open System.Text.RegularExpressions

let buildIndexMap kvLst =
    let ks = List.map (fun (x:string,y) -> x.Length + 2,y) kvLst
    List.fold (fun (bl,blst) (l,v) -> bl+l,((bl,v)::blst)) (0,[]) ks
    |> snd
    |> dict

let buildStr kvLst =
    let sep = ";;"
    List.map fst kvLst 
    |> String.concat sep
    |> fun s -> ";" + s + ";"

let s childsLst = 
    let str = buildStr childsLst
    let idxValMap = buildIndexMap childsLst
    let re = new Regex("((;2;))")
    let elts =
        let res = re.Match(str)
        if Seq.fold (&&) true [for g in res.Groups -> g.Success]
        then res.Groups
        else (new Regex("((;2;))",RegexOptions.RightToLeft)).Match(str).Groups
    let e0 =
        idxValMap.[elts.[2].Captures.[0].Index] |> RELeaf
    RESeq [e0]
let e childsLst = 
    let str = buildStr childsLst
    let idxValMap = buildIndexMap childsLst
    let re = new Regex("((;6;))|((;2;)((;7;))|((;8;))|((;9;))(;2;))")
    let elts =
        let res = re.Match(str)
        if Seq.fold (&&) true [for g in res.Groups -> g.Success]
        then res.Groups
        else (new Regex("((;6;))|((;2;)((;7;))|((;8;))|((;9;))(;2;))",RegexOptions.RightToLeft)).Match(str).Groups
    if elts.[2].Value = ""
    then
        let e2 =
            let e2 =
                idxValMap.[elts.[11].Captures.[0].Index] |> RELeaf
            let e1 =
                if elts.[6].Value = ""
                then
                    let e2 =
                        if elts.[8].Value = ""
                        then
                            let e2 =
                                let e0 =
                                    idxValMap.[elts.[10].Captures.[0].Index] |> RELeaf
                                RESeq [e0]
                            None, Some (e2)
                        else
                            let e1 =
                                let e0 =
                                    idxValMap.[elts.[8].Captures.[0].Index] |> RELeaf
                                RESeq [e0]
                            Some (e1),None
                        |> REAlt

                    None, Some (e2)
                else
                    let e1 =
                        let e0 =
                            idxValMap.[elts.[6].Captures.[0].Index] |> RELeaf
                        RESeq [e0]
                    Some (e1),None
                |> REAlt

            let e0 =
                idxValMap.[elts.[4].Captures.[0].Index] |> RELeaf
            RESeq [e0; e1; e2]
        None, Some (e2)
    else
        let e1 =
            let e0 =
                idxValMap.[elts.[2].Captures.[0].Index] |> RELeaf
            RESeq [e0]
        Some (e1),None
    |> REAlt


let ruleToRegex = dict [|(2,e); (1,s)|]

