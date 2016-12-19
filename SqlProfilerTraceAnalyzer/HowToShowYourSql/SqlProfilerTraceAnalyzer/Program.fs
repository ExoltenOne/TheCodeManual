open System
open System.Text.RegularExpressions
open FSharp.Data

open Argu

open SqlProfilerTraceVisualiser.ChartVisualiser
open SqlProfilerTraceVisualiser.ConsoleVisualiser
open SqlProfilerTraceVisualiser.FileVisualiser

type Arguments =
    | TraceFile of path:string
    | Console
    | File
    | Chart of typeChart:TypeChart

with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | TraceFile _ -> "specify a trace file"
            | Console _ -> "set output to console"
            | Chart _ -> "set output to chart"
            | File _ -> "set output to file"

[<EntryPoint>]
let main argv = 
   
    let parser = ArgumentParser.Create<Arguments>()
    let arg = parser.ParseCommandLine argv

    let arguments = arg.GetAllResults()

    let file = arg.GetResult (<@ TraceFile @>, defaultValue = "trace.xml")
    let result = analyze file

    let matchResult argument result =
        match argument with
        | Arguments.TraceFile _ -> ()
        | Arguments.Console -> printToConsole result
        | Arguments.File -> printToFile result
        | Arguments.Chart typeChart -> printToChart result typeChart

    let rec matchResults arguments =
                    match arguments with
                    | head :: tail -> 
                                        matchResult head result
                                        matchResults tail
                    | [] -> ()

    matchResults arguments

    0 // return an integer exit code
