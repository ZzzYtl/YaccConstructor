﻿module Yard.Generators.GLL.Parser 
open Yard.Generators.GLL 
open System 
open System.Collections.Generic
open Yard.Generators.GLL
open Yard.Generators.Common.AST2
open Yard.Generators.Common.DataStructures
open Microsoft.FSharp.Collections

[<Measure>] type extension
[<Measure>] type key
[<Measure>] type vertex

let inline packExtension left right : int64<extension> =  LanguagePrimitives.Int64WithMeasure ((int64 left <<< 32) ||| int64 right)
let inline getRightExtension (long : int64<extension>) = int <| ((int64 long) &&& 0xffffffffL)
let inline getLeftExtension (long : int64<extension>)  = int <| ((int64 long) >>> 32)

let inline packVertex level label: int64<vertex> =  LanguagePrimitives.Int64WithMeasure ((int64 level <<< 32) ||| int64 label)
let inline getIndex1Vertex (long : int64<vertex>)       = int <| ((int64 long) &&& 0xffffffffL)
let inline getIndex2Vertex (long : int64<vertex>)       = int <| ((int64 long) >>> 32)

let inline packLabel rule position = (int rule <<< 16) ||| int position
let inline getRule packedValue = int packedValue >>> 16
let inline getPosition (packedValue : int) = int (packedValue &&& 0xffff)

let inline pack3ToInt64 p l r : int64<key>        = LanguagePrimitives.Int64WithMeasure (((int64 p) <<< 52) ||| ((int64 l) <<< 26) ||| (int64 r))
let inline getProduction (long : int64<key>)      = int (int64 long >>> 52)
let inline getLeftExtension3 (long : int64<key>)  = int((int64 long <<< 12) >>> 38)
let inline getRightExtension3 (long : int64<key>) = int((int64 long <<< 38) >>> 38)

[<Struct>]
type ExtensionTree =     
    val extension : int64<extension>
    val tree      : INode
    new(e, t) = {extension = e; tree = t}

[<AllowNullLiteral>]
type Vertex =
    val mutable OutEdges : UsualOne<Edge>
    val Level            : int
    val Label            : int
    new (level, label) = {OutEdges = Unchecked.defaultof<_>; Level = level; Label = label}

and [<Struct>] Edge =
    val Ast   : ExtensionTree
    val Dest  : int64<vertex>
    new (d, a) = {Dest = d; Ast = a}

[<Struct>]
type KeyForPreviousContext =
    val label  : int
    val vertex : int64<vertex>
    new (l, v) = {label = l; vertex = v}


[<Struct>]
type Context =
    val Index         : int
    val Label         : int
    val Vertex        : int64<vertex>
    val Ast           : ExtensionTree
    new (index, label, vertex, ast) = {Index = index; Label = label; Vertex = vertex; Ast = ast}

type ParseResult<'TokenType> =
    | Success of Tree<'TokenType>
    | Error of string

let buildAst<'TokenType> (parser : ParserSource2<'TokenType>) (tokens : seq<'TokenType>) : ParseResult<_> = 
    let tokens = Seq.toArray tokens
    let inputLength = Seq.length tokens
    let nonTermsCountLimit = 1 + (Array.max parser.LeftSide)
    let resultAST = ref None
    let getEpsilon =

         let epsilons = Array.init nonTermsCountLimit (fun i -> box (-i - 1))
         fun i -> epsilons.[i]
    if inputLength = 0 || parser.IndexEOF = parser.TokenToNumber tokens.[0] then
//        if parser.AcceptEmptyInput then
//            Success (new Tree<_>(null, getEpsilon startNonTerm, null))
//        else
        Error ("This grammar does not accept empty input.")     
    else
        let epsilon = new TerminalNode(-1)
        let setR = new Queue<Context>()   
        let setP = new System.Collections.Generic.Dictionary<int64<vertex>,ResizeArray<ExtensionTree>> ()
        let astDictionary = new System.Collections.Generic.Dictionary<int64<key>,AST> ()
        let setU = Array.init (inputLength+1) (fun _ -> new Dictionary<_*_, ResizeArray<INode>>())
            
        let currentIndex = ref 0

        let currentrule = parser.StartRule
        let dummy = null
        let dummyGSSNode = new Vertex(!currentIndex, (packLabel -1 -1))
        let currentLabel = ref <| packLabel currentrule 0
        let dummyAST = new ExtensionTree(packExtension -1 -1, dummy)
        let dummyFam = Family()
        let currentN = ref <| dummyAST
        let currentR = ref <| dummyAST
       // let edges = Array2D.init inputLength inputLength (fun _ _ -> new ResizeArray<Edge>())
 
        
        
        let gss = Array.init inputLength (fun _ -> new ResizeArray<Vertex>())
        gss.[!currentIndex].Add dummyGSSNode
        let currentGSSNode = ref <| packVertex !currentIndex (gss.[!currentIndex].Count - 1)
        let dummyGSSNode = packVertex !currentIndex (gss.[!currentIndex].Count - 1)
        let currentContext = ref <| new Context(!currentIndex, !currentLabel, !currentGSSNode, dummyAST)
        let createdFamilies = Array.init parser.rulesCount (fun _ -> new Dictionary<int64<key>, ResizeArray<Family>>())  

        let terminalNodes = new BlockResizeArray<ExtensionTree>()
        let finalExtension = pack3ToInt64 parser.StartRule 0 inputLength
   
        let findFamily prod (nodes : INode) extension : Family =
            let mutable result = None
            let res = ref <| null
            let exists = createdFamilies.[prod].TryGetValue(extension, res)
            if exists
            then 
                let fams = !res
                let contains = fams |> ResizeArray.tryFind (fun f -> f.node = nodes )
                if not contains.IsSome
                then
                    let temp = new Family(prod, nodes)
                    createdFamilies.[prod].[extension].Add temp
                    temp
                else contains.Value
            else
                let temp = new Family(prod, nodes)
                let v = new ResizeArray<Family>()
                v.Add temp
                createdFamilies.[prod].Add(extension, v)
                temp

        let handleIntermidiate (node : INode) prod extension =
            let family = findFamily prod node extension
            family

        let containsContext index label  (gssNode : int64<vertex>) (ast : INode) =
            let set = setU.[index]
            let key = (label, gssNode)
            let res = ref null
            let contains = set.TryGetValue(key, res)
            if contains
            then
                let t = ResizeArray.exists (fun e -> e = null && ast = null || (ast <> null && e <> null && e = ast)) !res
                contains, t , key
                
            else false, false, key
            

        let addContext label (index : int) (node : int64<vertex>) (ast : ExtensionTree) =
            let containsKey, containsTree, key = containsContext index label node ast.tree
            if index <= inputLength + 1 && index >= 0 && not containsTree
            then
                let cntxt = new Context(index, label, node, ast)
                if containsKey
                then 
                    setU.[index].[key].Add ast.tree
                else 
                    let t = new ResizeArray<INode>()
                    t.Add ast.tree
                    setU.[index].Add (key, t)
                setR.Enqueue(cntxt)  
        
        let getNodeP label (left : ExtensionTree) (right : ExtensionTree) =      
            let rExt = right.extension
            let condTemp = right.tree = null
            let mutable result = dummyAST
            if left.tree <> null
            then
                let lExt = left.extension
                if condTemp
                then
                    result <- new ExtensionTree(packExtension (getLeftExtension lExt) (getRightExtension lExt), (new IntermidiateNode(left.tree, right.tree, label)))
                else 
                    result <- new ExtensionTree(packExtension (getLeftExtension lExt) (getRightExtension rExt), (new IntermidiateNode(left.tree, right.tree, label)))
            else 
                result <- new ExtensionTree(packExtension (getLeftExtension rExt) (getRightExtension rExt), (new IntermidiateNode(left.tree, right.tree, label)))
            result
                
            
        let getNodeT index =
            if terminalNodes.Item index = Unchecked.defaultof<ExtensionTree>
            then
                let tree = new TerminalNode(index)
                let result = new ExtensionTree(packExtension index (index + 1), tree)
                terminalNodes.Set index result
                result
            else terminalNodes.Item index            
            
        let containsGSSNode l i =  
            let curLevel = gss.[i]
            let index = curLevel.FindIndex (fun v -> v.Label = l)
            if index = -1
            then
                   
                curLevel.Add <| new Vertex(i, l)   
                packVertex i (curLevel.Count - 1)
            else packVertex i index

        let containsEdge (b : int64<vertex>) (e : int64<vertex>) (ast : INode) =
            let edges = gss.[getIndex2Vertex b].[getIndex1Vertex b].OutEdges
            edges.first <> Unchecked.defaultof<_> && (edges.first.Dest = e || (edges.other <> null && edges.other |> Array.exists (fun edge ->  edge.Dest = e)))

        let findTree prod extension (family : Family) =            
            let result = 
                if astDictionary.ContainsKey extension
                then
                    let a = astDictionary.[extension]
                    let eq (fam : Family) =
                        fam.prod = family.prod
                        && obj.ReferenceEquals(fam.node, family.node)                    
                    
                    if not (eq a.first || (a.other <> null && a.other |> Array.exists eq))
                    then
                        if a.other <> null
                        then a.other <- Array.append a.other [|family|]
                        else a.other <- [|family|]
                    a
                else
                    let value = new AST(family, null)
                    astDictionary.Add(extension, value)
                    value
            
            result

        let create label (u : int64<vertex>) (index : int) (ast : ExtensionTree) = 
            let v = containsGSSNode label index
            if not (containsEdge v u ast.tree)
            then
                let newEdge = new Edge(u, ast)
               // edges.[gss.[getIndex2Vertex v].[getIndex1Vertex v].Level,gss.[getIndex2Vertex u].[getIndex1Vertex u].Level].Add(newEdge)
                if setP.ContainsKey v
                then
                    let trees = setP.[v]
                    let handleTree ast2 =
                        
                        let y = getNodeP label ast ast2
                        addContext label (getRightExtension ast2.extension) u y      ////ast2 or ast!!! temp
                    trees |> ResizeArray.iter handleTree
                let vertex = gss.[getIndex2Vertex v].[getIndex1Vertex v]     
                if vertex.OutEdges.first <> Unchecked.defaultof<_>
                then
                    if vertex.OutEdges.other <> null
                    then vertex.OutEdges.other <- Array.append vertex.OutEdges.other [|newEdge|]
                    else vertex.OutEdges.other <- [|newEdge|]
                else vertex.OutEdges.first <- newEdge
            v

        let pop (u : int64<vertex>) (i : int) (z : AST) extension =
            let temp = 0
            if not (u = dummyGSSNode)
            then
                let vertex = gss.[getIndex2Vertex u].[getIndex1Vertex u]
                let temp = new ExtensionTree(extension, z)
                let label = vertex.Label
                if setP.ContainsKey u           //вот тут можно попробовать какой-нибудь tryGet
                then
                    setP.[u].Add(temp)
                else 
                    let value = new ResizeArray<ExtensionTree>(5)
                    value.Add(temp)
                    setP.Add(u, value)
                let processEdge (edge : Edge) =
                    if not <| obj.ReferenceEquals(edge.Ast, dummyAST)
                    then
                        let y1 = getNodeP label edge.Ast temp
                        addContext label i edge.Dest y1
                    else addContext label i edge.Dest (new ExtensionTree(extension, z))  
                processEdge vertex.OutEdges.first
                if vertex.OutEdges.other <> null 
                then vertex.OutEdges.other |> Array.iter processEdge

        let table = parser.Table
        let condition = ref false 
        let stop = ref false

        let rec dispatcher () =
            if setR.Count <> 0
            then
                currentContext := setR.Dequeue()
                currentIndex := currentContext.Value.Index
                currentGSSNode := currentContext.Value.Vertex
                currentLabel := currentContext.Value.Label
                currentN := currentContext.Value.Ast 
                currentR := dummyAST
                condition := false
            else 
                stop := true  
                              
        and processing () =  
            condition := true
            let rule = getRule !currentLabel
            let position = getPosition !currentLabel
            if Array.length parser.rules.[rule] = 0 
            then
               // currentR := getNodeT !currentIndex
                //currentN := getNodeP !currentLabel !currentN !currentR  
                //pop !currentGSSNode !currentIndex !currentN
                printf "temp"
            else
                if Array.length parser.rules.[rule] <> position
                then
                    if !currentIndex < inputLength 
                    then
                        let curToken = parser.TokenToNumber tokens.[!currentIndex]
                        let curSymbol = parser.rules.[rule].[position]
                        if (parser.NumIsTerminal curSymbol || parser.NumIsLiteral curSymbol) //может, здесь нужен отдельный иф для проверки на совпадение текущего символа и токена
                        then
                            if curSymbol = curToken 
                            then
                                if !currentN = dummyAST 
                                then currentN := getNodeT !currentIndex
                                else currentR := getNodeT !currentIndex
                                currentIndex := !currentIndex + 1
                                currentLabel := packLabel (rule) ((position) + 1)
                                if not <| obj.ReferenceEquals(!currentR, dummyAST)
                                then currentN := getNodeP !currentLabel !currentN !currentR
                                condition := false
                        else 
                            let getIndex nTerm term = 
                                let mutable index = nTerm
                                index <- (index * (parser.IndexatorFullCount - parser.NonTermCount))
                                index <- index + term - parser.NonTermCount
                                index

                            let index = getIndex curSymbol curToken
                            currentGSSNode := create (packLabel (rule) (position + 1)) !currentGSSNode !currentIndex !currentN
                            if Array.length table.[index] <> 0 
                            then
                                let a rule = 
                                    let newLabel = packLabel rule 0
                                    addContext newLabel !currentIndex !currentGSSNode dummyAST 
                                table.[index] |> Array.iter a
                    else condition := true
                                    
                else
                    let curRight =  !currentN
                    let t1 = getLeftExtension curRight.extension
                    let t2 = getRightExtension curRight.extension
                   // let extension = curRight.extension
                    let key = pack3ToInt64 rule (getLeftExtension curRight.extension) (getRightExtension curRight.extension)
                    let resTree = handleIntermidiate curRight.tree (rule) key
                    let resTree = findTree rule key resTree
                    if  key = finalExtension
                    then resultAST := Some resTree
                    pop !currentGSSNode !currentIndex resTree currentN.Value.extension
        let control () =
             while not !stop do
                if !condition then dispatcher() else processing()
        control()
                 
        match !resultAST with
            | None -> Error ("String was not parsed")
            | Some res -> 
                    let r1 = new Tree<_> (tokens, res, parser.rules)
                    Success (r1)   
                        