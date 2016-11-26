open System
open System.Text.RegularExpressions
open FSharp.Data
 
type Trace = XmlProvider<"trace.xml">
 
[<EntryPoint>]
let main argv = 
   
    let removeSquareBraces (name:string) = 
        name.Replace("[", String.Empty).Replace("]", String.Empty)
 
    let extractSprocName execution =
        let regex = new Regex(".*exec (?<sproc>\S*)");
        let result = regex.Match execution
        let group = result.Groups.["sproc"]
        if group.Success then Some(removeSquareBraces group.Value) else None
 
    // TODO Key not found in case where id column is incorrect
 
    let extractOptionValue columns id = 
        let result = Array.tryFind (fun (y:Trace.Column) -> y.Id = id) columns
        Option.map (fun (x:Trace.Column) -> x.String) result
 
    let trace = Trace.Load "trace.xml"
//    let result = trace.Events
//                    |> Array.map (fun x -> x.Columns)
//                    |> Array.map (fun x -> 
//                                            let nameColumn = Array.find (fun (y:Trace.Column) -> y.Id = 1) x
//                                            let durationColumn = Array.find (fun (y:Trace.Column) -> y.Id = 13) x
//                                            (nameColumn.String, durationColumn.String))
 
    let getDuration (durationOption: Option<string> option) =
        match durationOption with
        | Some x ->
                    match x with
                    | Some value -> int value 
                    | None -> 0
        | None -> 0
 
    let getName (nameOption: Option<string> option) =
        match nameOption with
        | Some x ->
                    match x with
                    | Some value -> extractSprocName value
                    | None -> None
        | None -> None
 
    let result2 = trace.Events
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
                    |> Seq.map(fun (name,count,sum,avg) -> sprintf "\n%-60s - Count: %-5d - Sum: %-6d - Avg: %-5f" name count sum avg)
                    |> Seq.reduce (sprintf "%s%s")
 
 
//                    |> Array.concat//                    |> Seq.where (fun x -> x.Id = 1)//                    |> Seq.map (fun x -> x.String)//                    |> Seq.where(fun x -> x.IsSome)//                    |> Seq.map(fun x -> Option.get x)//                    |> Seq.map(fun x -> extractSprocName x)//                    |> Seq.where(fun x -> x.IsSome)//                    |> Seq.map(fun x -> Option.get x)//                    |> Seq.countBy (fun x -> x)//                    |> Seq.sortBy (fun x -> snd x)//                    |> Seq.map(fun (sprocName,number) -> sprintf "%-60s - %d\n" sprocName number)//                    |> Seq.reduce (sprintf "%s%s")
   
 
    printfn "%A" result2
    0 // return an integer exit code
 
 
    //http://fsharpforfunandprofit.com/posts/the-option-type/