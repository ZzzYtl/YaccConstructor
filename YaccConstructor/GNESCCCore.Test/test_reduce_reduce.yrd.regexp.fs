//this file was generated by GNESCC
//source grammar:../../../Tests/GNESCC/test_reduce_reduce/test_reduce_reduce.yrd
//date:10/12/2011 16:53:50

module GNESCC.Regexp_reduce_reduce

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
    let re = new Regex("(((;2;))|((;3;)))")
    let elts =
        let res = re.Match(str)
        if Seq.fold (&&) true [for g in res.Groups -> g.Success]
        then res.Groups
        else (new Regex("(((;2;))|((;3;)))",RegexOptions.RightToLeft)).Match(str).Groups
    let e0 =
        if elts.[3].Value = ""
        then
            let e2 =
                let e0 =
                    idxValMap.[elts.[5].Captures.[0].Index] |> RELeaf
                RESeq [e0]
            None, Some (e2)
        else
            let e1 =
                let e0 =
                    idxValMap.[elts.[3].Captures.[0].Index] |> RELeaf
                RESeq [e0]
            Some (e1),None
        |> REAlt

    RESeq [e0]
let a childsLst = 
    let str = buildStr childsLst
    let idxValMap = buildIndexMap childsLst
    let re = new Regex("((;7;)(;8;)(;7;))")
    let elts =
        let res = re.Match(str)
        if Seq.fold (&&) true [for g in res.Groups -> g.Success]
        then res.Groups
        else (new Regex("((;7;)(;8;)(;7;))",RegexOptions.RightToLeft)).Match(str).Groups
    let e2 =
        idxValMap.[elts.[4].Captures.[0].Index] |> RELeaf
    let e1 =
        idxValMap.[elts.[3].Captures.[0].Index] |> RELeaf
    let e0 =
        idxValMap.[elts.[2].Captures.[0].Index] |> RELeaf
    RESeq [e0; e1; e2]
let b childsLst = 
    let str = buildStr childsLst
    let idxValMap = buildIndexMap childsLst
    let re = new Regex("((;7;)(;8;)(;7;))")
    let elts =
        let res = re.Match(str)
        if Seq.fold (&&) true [for g in res.Groups -> g.Success]
        then res.Groups
        else (new Regex("((;7;)(;8;)(;7;))",RegexOptions.RightToLeft)).Match(str).Groups
    let e2 =
        idxValMap.[elts.[4].Captures.[0].Index] |> RELeaf
    let e1 =
        idxValMap.[elts.[3].Captures.[0].Index] |> RELeaf
    let e0 =
        idxValMap.[elts.[2].Captures.[0].Index] |> RELeaf
    RESeq [e0; e1; e2]

let ruleToRegex = dict [|(3,b); (2,a); (1,s)|]

