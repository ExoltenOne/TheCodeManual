[<AutoOpen>]
module TraceAnalyzer

open System
open System.Text.RegularExpressions
open FSharp.Data

type Trace = XmlProvider<"trace.xml">

let analyze (traceFile:string) =
    let extractOptionValue columns id = 
        let result = Array.tryFind (fun (y:Trace.Column) -> y.Id = id) columns
        Option.map (fun (x:Trace.Column) -> x.String) result
 
    let trace = Trace.Load traceFile
 
    let getDuration (durationOption: Option<string> option) =
        match durationOption with
        | Some x ->
                    match x with
                    | Some value -> int value 
                    | None -> 0
        | None -> 0
 
    let removeSquareBraces (name:string) = 
        name.Replace("[", String.Empty).Replace("]", String.Empty)
 
    let extractSprocName execution =
        let regex = new Regex(".*exec (?<sproc>\S*)");
        let result = regex.Match execution
        let group = result.Groups.["sproc"]
        if group.Success then Some(removeSquareBraces group.Value) else None

    let getName (nameOption: Option<string> option) =
        match nameOption with
        | Some x ->
                    match x with
                    | Some value -> extractSprocName value
                    | None -> None
        | None -> None
 
    let result = trace.Events
                    |> Array.map (fun x -> x.Columns)
                    |> Array.map (fun x -> (extractOptionValue x 1, extractOptionValue x 13))
                    |> Array.map (fun (nameColumn, durationColumn) -> (getName nameColumn, getDuration durationColumn))
                    |> Array.filter (fun (name, _) -> name.IsSome)
                    |> Array.map (fun (name,duration) -> (Option.get name, duration))
                    |> Array.toSeq
                    |> Seq.groupBy (fun (name, duration) -> name)
                    |> Seq.map (fun (name, entries) -> 
                                                        let count = Seq.length entries
                                                        let sum = Seq.sumBy (fun (_,duration) -> duration) entries
                                                        let avg = Seq.averageBy (fun (_,duration) -> float duration) entries
                                                        (name, count, sum, avg))
                    |> Seq.sortBy (fun (name,_,_,_) -> name)

    result
    