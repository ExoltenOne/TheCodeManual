open System
open System.IO
open FSharp.Data

type solution = XmlProvider<"ProjectFileAnalyzer.DuplicatedFilesTestCase.csproj">

[<EntryPoint>]
let main (argv:string[]) =

    let getContentFiles (items:solution.ItemGroup[]) =
                items
                    |> Array.collect ( fun x -> x.Contents)            
                    |> Array.map ( fun x -> x.Include)

    let getCompileFiles (items:solution.ItemGroup[]) =
                items
                    |> Array.collect ( fun x -> x.Compiles)            
                    |> Array.map ( fun x -> x.Include)

    let findDuplicates csprojs =

        let print (duplicates:string*string[]) =
            if((duplicates |> snd).Length > 0) then
                Console.ForegroundColor <- ConsoleColor.DarkRed
                printfn "%A" (duplicates |> fst)
                Console.ForegroundColor <- ConsoleColor.White                    
                (duplicates |> snd)|> Seq.iter ( fun x -> printfn "\t%A" x)

        let findDuplicates' (selector:solution.ItemGroup[] -> string[]) (projectFile:string) =
            let sln = solution.Load projectFile
            let duplicates = sln.ItemGroups
                                |> selector
                                |> Array.groupBy ( fun x -> x)
                                |> Array.filter ( fun (_,col) -> col.Length > 1 )
                                |> Array.map fst
            (projectFile,duplicates)

        csprojs |> Seq.map (findDuplicates' getContentFiles) |> Seq.iter print
        csprojs |> Seq.map (findDuplicates' getCompileFiles) |> Seq.iter print

    let getProjectFiles argv =
        let getArgv (argv:string[]) =
                                    match argv with
                                    | null | [||]  -> [| Directory.GetCurrentDirectory() |]
                                    | _ -> argv

        let getCsprojFileNames rootPath =
            let dir = new DirectoryInfo(rootPath)
            dir.GetFiles( "*", SearchOption.AllDirectories) |> Array.filter (fun x -> x.Extension.Contains("csproj" )) |> Array.map (fun x -> x.FullName)

        argv |> getArgv |> Array.map ( fun x -> getCsprojFileNames x)

    argv |> getProjectFiles |> Array.iter findDuplicates

    0 // return an integer exit code
