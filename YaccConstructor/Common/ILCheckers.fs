﻿//  ILCheckers.fs contains checkers for IL
//
//  Copyright 2011, 2012 Semen Grigorev <rsdpisuy@gmail.com>
//            2011, 2012 Ilia Shenbin <ilya.shenbin@gmail.com>
//  This file is part of YaccConctructor.
//
//  YaccConstructor is free software:you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

module Yard.Core.Checkers

open Yard.Core.IL.Production
open Yard.Core.IL
open System.Collections.Generic
open System.Linq

let private startRulesCount (def:Yard.Core.IL.Definition.t<_,_>) =
    def.grammar.Count(fun r -> r._public)

let IsStartRuleExists def =
    startRulesCount def > 0

let IsSingleStartRule def =
    startRulesCount def = 1

let IsChomskyNormalForm (def:Yard.Core.IL.Definition.t<_,_>) =
    def.grammar.All 
        (fun r ->
            match r.body with
            | PSeq([{rule = PToken(_);omit =_ ;binding =_;checker = _}],_,_)
            | PSeq([{rule = PRef(_);omit =_ ;binding =_;checker = _}
                   ;{rule = PRef(_);omit =_ ;binding =_;checker = _}],_,_) -> true 
            | _ -> false)

let GetUndeclaredNonterminalsList (def:Yard.Core.IL.Definition.t<Source.t, Source.t>) =
    let declaredRules = def.grammar |> List.map (fun r -> r.name.text)
    let undeclaredRules = new HashSet<_>()
    let addUndeclaredRule (name : Source.t) additionRules = 
        let name = name.text
        if not (List.exists ((=) name) declaredRules
                || Seq.exists ((=) name) additionRules)
        then
            undeclaredRules.Add name |> ignore

    let rec getUndeclaredRules additionRules body =
        let getUndeclaredRulesCurried body = getUndeclaredRules additionRules body
        match body with
        | PRef (name,_) -> addUndeclaredRule name additionRules
        | PMetaRef (name,_,exprList) ->  
            addUndeclaredRule name additionRules
            exprList |> List.iter getUndeclaredRulesCurried 
        | PSeq (exprList,_,_) -> exprList |> List.iter (fun r -> getUndeclaredRulesCurried r.rule)
        | PPerm exprList    -> exprList |> List.iter getUndeclaredRulesCurried 
        | PRepet (expr,_,_)
        | PMany expr
        | PSome expr
        | POpt  expr -> getUndeclaredRulesCurried expr
        | PAlt (lExpr,rExpr) -> 
            getUndeclaredRulesCurried lExpr
            getUndeclaredRulesCurried rExpr
        | PLiteral _ 
        | PToken _  -> ()

    def.grammar 
    |> List.iter 
        (fun r -> 
            let additionRules = new HashSet<_>()
            r.metaArgs |> List.iter (fun i -> additionRules.Add i.text |> ignore)
            getUndeclaredRules additionRules r.body)

    List.ofSeq undeclaredRules

// returns a list of rule's names which are reachead from start rule in the grammar
let reachableRulesInfo_of_list (rules: IL.Rule.t<_,_> list) =
    let declaredRules = rules |> List.map (fun r -> r.name)
    let reachedRules = new HashSet<_>()
    
    let getAdditionRules (rule:Yard.Core.IL.Rule.t<Source.t,Source.t>) =
        let newAdditionRules = new HashSet<_>()
        rule.metaArgs |> List.iter (fun i -> newAdditionRules.Add i.text |> ignore)
        newAdditionRules

    let rec getReachableRules additionRules body =
        let getReachableRulesCurried body = getReachableRules additionRules body
        match body with
        | PRef (name,_) -> addReachedRule name additionRules
        | PMetaRef (name,_,exprList) ->  
            addReachedRule name additionRules
            exprList |> List.iter getReachableRulesCurried 
        | PSeq (exprList,_,_) -> exprList |> List.iter (fun r -> getReachableRulesCurried r.rule)
        | PPerm exprList    -> exprList |> List.iter getReachableRulesCurried
        | PRepet (expr,_,_)
        | PMany expr
        | PSome expr
        | POpt  expr -> getReachableRulesCurried expr
        | PAlt (lExpr,rExpr) -> 
            getReachableRulesCurried lExpr
            getReachableRulesCurried rExpr
        | PLiteral _ 
        | PToken _  -> ()

    and addReachedRule (name : Source.t) additionRules =  
        let name = name.text
        if not (Seq.exists ((=) name) additionRules
           || Seq.exists ((=) name) reachedRules)
        then
            reachedRules.Add name |> ignore
            let rule = rules |> List.find (fun r -> r.name.text = name)
            let newAdditionRules = getAdditionRules rule
            getReachableRules newAdditionRules rule.body

    let start_rule = rules |> List.find (fun r -> r._public)
    start_rule.name.text |> reachedRules.Add |> ignore
    getReachableRules (getAdditionRules start_rule) start_rule.body
    reachedRules |> Seq.toList

let reachableRulesInfo (def: Yard.Core.IL.Definition.t<_,_>) =
  reachableRulesInfo_of_list def.grammar

let IsUnusedRulesExists(def:Yard.Core.IL.Definition.t<_,_>) =
  let reachedRules = reachableRulesInfo def
  def.grammar |> List.exists (fun r -> reachedRules |> Seq.forall (fun n -> n <> r.name.text))
