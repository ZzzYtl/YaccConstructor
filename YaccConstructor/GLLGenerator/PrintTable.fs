﻿//  Parser.fs contains type, describing information, written to file as result of generation
//     and used by Parser and Translator.
//
//  Copyright 2011-2012 Avdyukhin Dmitry
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

module Yard.Generators.RNGLR.Printer

open Yard.Generators.RNGLR.FinalGrammar
open Yard.Generators.GLL
open Yard.Generators.GLL.Table
open System.Collections.Generic
open Yard.Generators.RNGLR
open Yard.Core.IL

let printTable (table : Table) = 
    let mutable res = "[| "
    for i = 0 to i < Table.i do
        for j = 0 to j < Table.j do
            res <- res + Table.table.[i].[j] + " "
    res <- res + "|]"
    res