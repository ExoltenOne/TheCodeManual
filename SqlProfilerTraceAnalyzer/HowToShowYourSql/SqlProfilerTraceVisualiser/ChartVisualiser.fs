module SqlProfilerTraceVisualiser.ChartVisualiser

open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Windows.Forms

type TypeChart =
    | Bar
    | Pie

let printToChart (result:seq<string*int*int*float>) (typeChart:TypeChart) = 
    
    let selectChart typeChart (data:seq<string*float>)  = 
                        match typeChart with
                        | TypeChart.Bar -> Chart.Bar data
                        | TypeChart.Pie -> Chart.Pie data :> GenericChart

    let createChart typeChart name (data:seq<string*float>) =
        let chart = data |> selectChart typeChart
        chart.ShowChart() |> ignore
        chart.SaveChartAs(name, ChartImageFormat.Jpeg)

    let createChart' = createChart typeChart

    let countData = result |> Seq.map (fun (name,count,_,_) -> (name,(float)count))

    let sumData = result |> Seq.map (fun (name,_,sum,_) -> (name,(float)sum/1000.))

    let avgData = result |> Seq.map (fun (name,_,_,avg) -> (name,(float)avg))

    createChart' "count.jpeg" countData
    createChart' "sum.jpeg" sumData
    createChart' "avg.jpeg" avgData
