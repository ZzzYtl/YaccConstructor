﻿//  Copyright 2011-2012 by Dmitry Avdyukhin
//
//  This file is part of YaccConctructor.
//
//  YaccConstructor is free software: you can redistribute it and/or modify
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

module Yard.Generators.GLL.Table

open Yard.Generators.RNGLR.FinalGrammar
open Yard.Generators.RNGLR.States
open Yard.Generators.RNGLR

type Table (_grammar : FinalGrammar) =
    let grammar = _grammar
    let i = grammar.indexator.nonTermCount
    let j = grammar.indexator.termCount
    let tabl = 
        let _table = Array2D.create grammar.indexator.nonTermCount grammar.indexator.termCount -1
        for i = 0 to grammar.rules.rulesCount - 1 do
            let curFirst = grammar.firstSet.[i]
            let curNonTerm = grammar.rules.leftSide i
            for j = 0 to curFirst.Count do
                let curTerm = grammar..[i].[j]
                table.[curNonTerm].[curTerm] <- i
            if chain.canInferEpsilon.[i] then 
                for j = 0 to grammar.followSet i do
                    let curTerm = grammar.followSet.[i].[j]
                    table.[curNoterm].[curTerm] <- i
                 
    table
        
    member this.table = _table
     
       
    