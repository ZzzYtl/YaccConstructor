﻿// Driver.fs contains tests of MS-SQL parser.
//
//  Copyright 2012 Semen Grigorev <rsdpisuy@gmail.com>
//
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


// id Drivers Test for memory
module ParserTests

open NUnit.Framework
open LexerHelper
open Yard.Utils.StructClass
open Yard.Utils.SourceText
open Yard.Utils.InfoClass
open Yard.Examples.MSParser
open System.IO

[<TestFixture>]
type ``MS-SQL parser tests`` () =
    let runParserTest (file:string) = 
        let p = new ProjInfo()
        let mutable counter = 1<id>
        let StreamElement = new StreamReader(file, System.Text.Encoding.UTF8)
        let map = p.GetMap StreamElement
        Lexer.id <- counter
        p.AddLine counter map
        counter <- counter + 1<id>
        match MSSqlParser.justParse file with
        | Yard.Generators.RNGLR.Parser.Error (num, tok, msg,dbg) ->
            dbg.drawGSSDot @"..\..\stack.dot"
            dbg.lastTokens 5 |> printfn "%A"
            let coordinates = 
                let x,y = tokenPos tok

                let x = p.GetCoordinates x
                let y = p.GetCoordinates y
                sprintf "(%A,%A) - (%A,%A)" x.Line x.Column y.Line y.Column
            let data =
                let d = tokenData tok
                if isLiteral tok then ""
                else (d :?> SourceText).text
            let name = tok |> tokenToNumber |> numToString
            printfn "Error in file %s at position %s on Token %s %s: %s" file coordinates name data msg
            printfn "%s" msg
           // dbg.drawGSSDot @"..\..\stack.dot"
            Assert.Fail msg
        | Yard.Generators.RNGLR.Parser.Success ast -> ()
            //Assert.Pass()

    let basePath = "../../../../../Tests/PlSqlParser"
    let filePath = "../../../../../Tests/materials/pl-sql/jrxml2pdf-release/install"
    let file name = System.IO.Path.Combine (basePath,name)
    let file_1 name = System.IO.Path.Combine (filePath,name)

    [<Test>]
    member test.``Exucute procedure with assosiation operation.`` () =
        file "exec_proc_1.sql" |> runParserTest

    [<Test>]
    member test.``Exucute procedure with two parametres.`` () =
        file "exec_proc_2.sql" |> runParserTest

    [<Test>]
    member test.``Exucute procedure with one parametr.`` () =
        file "exec_proc_3.sql" |> runParserTest

    [<Test>]
    member test.``Exucute procedure without parametres.`` () =
        file "exec_proc_4.sql" |> runParserTest

    [<Test>]
    member test.``Concat.`` () =
        file "concat.sql" |> runParserTest

    [<Test>]
    member test.``Programm with update.`` () =
        file "program_with_update.sql" |> runParserTest

    [<Test>]
    member test.``Programm with some procudure.`` () =
        file_1 "ACL_FOR_GOOGLE_MAPS.sql" |> runParserTest

    [<Test>]
    member test.``Big programm.`` () =
        file_1 "INSERT_TEMPLATES.sql" |> runParserTest

    [<Test>]
    member test.``Programm with select.`` () =
        file_1 "demodata.sql" |> runParserTest

    [<Test>]
    member test.``Programm demodata2.`` () =
        file_1 "demodata2.sql" |> runParserTest

    [<Test>]
    member test.``Programm demodata3.`` () =
        file_1 "demodata3.sql" |> runParserTest

    [<Test>]
    member test.``Programm demodata4.`` () =
        file_1 "demodata4.sql" |> runParserTest

    [<Test>]
    member test.``Programm demodata5.`` () =
        file_1 "demodata5.sql" |> runParserTest

    [<Test>]
    member test.``Programm with insert.`` () =
        file_1 "INSERT_NLS_DATA.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create and alter table.`` () =
        file_1 "JRXML_APEX2PDF_TEMPLATES.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create and alter table_2.`` () =
        file_1 "JRXML_REPORT_DEFINITIONS.sql" |> runParserTest

    [<Test>]
    member test.``Programm with alter table.`` () =
        file_1 "ALTER_JRXML_FONTS.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create and alter table_3.`` () =
        file_1 "JRXML_LOGGING.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create and alter table_4.`` () =
        file_1 "JRXML_NLS_PARAMETERS.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create and alter table_5.`` () =
        file_1 "JRXML_FONTS.sql" |> runParserTest    

    [<Test>]
    member test.``Programm with create and alter table_6.`` () =
        file_1 "JRXML_RESOURCE_ENTRIES.sql" |> runParserTest    

    [<Test>]
    member test.``Programm with create trigger_1.`` () =
        file_1 "ALTER_JRXML_REPORT_IMAGES.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create trigger_2.`` () =
        file_1 "JRXML_REPORT_IMAGES.sql" |> runParserTest

    [<Test>]
    member test.``Programm with create trigger_3.`` () =
        file_1 "JRXML_RESOURCE_FILES.sql" |> runParserTest




//[<EntryPoint>]
//let f x =
    //let tests = new ``MS-SQL parser tests`` ()
    //tests.``Programm with create trigger_1.``()
    //tests.``Programm with create trigger_1.``()
    //0 
