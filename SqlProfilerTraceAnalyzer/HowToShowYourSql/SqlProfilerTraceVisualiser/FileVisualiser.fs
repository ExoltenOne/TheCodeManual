module SqlProfilerTraceVisualiser.FileVisualiser

let printToFile  (result:seq<string*int*int*float>) =
    let toFile str =
        System.IO.File.WriteAllText("trace.txt", str)

    result
        |> Seq.map(fun (name,count,sum,avg) -> sprintf "\n%-60s - Count: %-5d - Sum: %-7d - Avg: %-5f" name count sum avg)
        |> Seq.reduce (sprintf "%s%s")
        |> toFile