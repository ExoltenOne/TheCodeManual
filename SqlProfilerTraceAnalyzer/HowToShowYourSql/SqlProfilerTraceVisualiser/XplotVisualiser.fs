module SqlProfilerTraceVisualiser.XplotVisualiser

open System.IO
open System.Reflection

open XPlot.GoogleCharts

let printToXplot (result:seq<string*int*int*float>) =

    let readEmbededFile fileName =
        let assembly = Assembly.GetExecutingAssembly()

        use stream = assembly.GetManifestResourceStream(fileName)
        use reader = new StreamReader(stream)
        let file = reader.ReadToEnd()
        file

    let writeToFile fileName (content:string) =
        use stream = new FileStream(fileName, FileMode.Create)
        use writer = new StreamWriter(stream)
        writer.Write(content)

    let countData = result |> Seq.map (fun (name,count,_,_) -> (name,(float)count))

    let sumData = result |> Seq.map (fun (name,_,sum,_) -> (name,(float)sum/1000.))

    let avgData = result |> Seq.map (fun (name,_,_,avg) -> (name,(float)avg))

    let createChart title legend (data:seq<string*float>) =
        let bColor = new BackgroundColor();
        bColor.fill <- "#fcfcfc";

        let options =
            Options(
                title = title,
                orientation = "horizontal",
                width = 640,
                height = 480,
                colors = [| "#42A5F5" |],
                backgroundColor = bColor
            )

        [data]
            |> Chart.Bar
            |> Chart.WithOptions options
            |> Chart.WithLabels [legend]

    let countChart = createChart "Store procedures" "Count" countData
    let totalTimeChart = createChart "Store procedures - Total time" "Time [ms]" sumData
    let avgTimeChart = createChart "Store procedures - Average time" "Time [us]" avgData

    let replace (placeholder:string) (chart:GoogleChart) (format:string) =
        format.Replace(placeholder, chart.GetInlineHtml())

    let table = [countData; sumData; avgData]        
                    |> Chart.Table
                    |> Chart.WithOptions(Options(showRowNumber = true, width = 1280))
                    |> Chart.WithLabels [ "Store procedure name"; "Count"; "Total time [ms]"; "Average time [us]" ]

    let file = readEmbededFile "XplotChart.html"
    let newFile = file
                    |> replace "{{ table }}" table
                    |> replace "{{ chartCount }}" countChart
                    |> replace "{{ chartSum }}" totalTimeChart
                    |> replace "{{ chartAvg }}" avgTimeChart

    
    writeToFile "chartReport.html" newFile

    ()
