open System
open System.Globalization
open System.IO
open System.Text.RegularExpressions

open Newtonsoft.Json

open FSharp.Data
open FSharp.ExcelProvider

// The file you get from https://www.foreignlaborcert.doleta.gov/docs/py2015q4/H-1B_Disclosure_Data_FY15_Q4.xlsx
type Visa = ExcelFile<"H-1B_Disclosure_Data_FY15_Q4.xlsx">
type Cities = CsvProvider<"worldcitiespopussample.txt">

type VisaGroupRequest = { CityName:String; Region:String; Salary:decimal; OffersNumber:int  }

type City = { Name:String; Region:String; Latitude:decimal; Longitude:decimal }

type VisaGroupRequestLocalization = { Name:String; Region:String; Salary:decimal; OffersNumber:int; Latitude:decimal; Longitude:decimal }

[<EntryPoint>]
let main argv = 
    
    let visa = new Visa()

    let starts date limitDate = 
        match DateTime.TryParse(date) with
        | (true, parsedDate) -> parsedDate > limitDate
        | _ -> false

    let startsIn2015 date = starts date (new DateTime(2015,1,1))

    let isComputerScieneOrMath (code:String) = 
        code.StartsWith "15"

    let getNumber text =
        let regex = new Regex("^\d*(\.\d*)?")
        let result = regex.Match(text)
        match result.Success with
        | true -> Some((decimal)result.Value)
        | false -> None

    let calculateSalary (paymentRate:String) paymentUnit =
        match paymentUnit with
        | "Year" -> getNumber paymentRate
        | "Hour" -> match getNumber paymentRate with
                    | Some value -> Some(value * 2087m)
                    | None -> None
        | _ -> None 

    let excludedRegions = [| "PR";"GU";"VI";"MP" |]

    let visaCities = visa.Data
                    |> Seq.filter (fun item -> item.CASE_STATUS = "CERTIFIED")
                    |> Seq.filter (fun item -> startsIn2015 item.EMPLOYMENT_START_DATE)
                    |> Seq.filter (fun item -> isComputerScieneOrMath item.SOC_CODE)
                    |> Seq.filter (fun item -> not (Array.contains item.WORKSITE_STATE excludedRegions))
                    |> Seq.map (fun item -> ( item.WORKSITE_CITY, item.WORKSITE_STATE, calculateSalary item.WAGE_RATE_OF_PAY item.WAGE_UNIT_OF_PAY))
                    |> Seq.filter (fun (_,_,salary) -> salary.IsSome)
                    |> Seq.map (fun (name, state, salary) -> (name.ToLower(), state, salary.Value))
                    |> Seq.groupBy (fun (name,state,_)  -> (name,state))
                    |> Seq.map (fun (cityAndState, groups) -> { 
                                                                   CityName = fst cityAndState; 
                                                                   Region = snd cityAndState; 
                                                                   Salary = Seq.averageBy (fun (_,_,salary) -> salary) groups; 
                                                                   OffersNumber = Seq.length groups })

    let cities = Cities.Load("worldcitiespopus.txt").Rows
                    |> Seq.map (fun item -> ({ City.Name = item.City; Region = item.Region; Latitude = item.Latitude; Longitude = item.Longitude}))

    let findCity (visaRequest:VisaGroupRequest) =
        let result = cities |> Seq.tryFind (fun item -> item.Name = visaRequest.CityName && item.Region = visaRequest.Region)
        match result with
            | Some value -> Some(value)
            | _ -> None

    let visaCitiesLocalization = visaCities 
                                    |> Seq.map (fun city -> (city, city |> findCity))
                                    |> Seq.filter (fun city -> (snd city).IsSome)
                                    |> Seq.map (fun (request,city) -> {
                                                                           Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase request.CityName;
                                                                           Region = request.Region;
                                                                           Salary = request.Salary;
                                                                           OffersNumber = request.OffersNumber;
                                                                           Longitude = city.Value.Longitude;
                                                                           Latitude = city.Value.Latitude})

    printfn "%A" (Seq.length visaCities)

    let writeToFile fileName (content:string) =
        use stream = new FileStream(fileName, FileMode.Create)
        use writer = new StreamWriter(stream)
        writer.Write(content)

    JsonConvert.SerializeObject visaCitiesLocalization |> writeToFile "us-cities.json" 

    printfn "%A" (Seq.length visaCitiesLocalization)

    0 // return an integer exit code
