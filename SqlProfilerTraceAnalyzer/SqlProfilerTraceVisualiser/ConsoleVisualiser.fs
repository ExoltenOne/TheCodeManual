module SqlProfilerTraceVisualiser.ConsoleVisualiser

let printToConsole (result:seq<string*int*int*float>) =
    result
        |> Seq.map(fun (name,count,sum,avg) -> sprintf "\n%-60s - Count: %-5d - Sum: %-7d - Avg: %-5f" name count sum avg)
        |> Seq.reduce (sprintf "%s%s")
        |> printfn "%A"