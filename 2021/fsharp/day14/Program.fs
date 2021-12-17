open System.IO

let parseInput(filePath:string) =
    let lines = File.ReadAllLines(filePath)
    let polymer = lines[0].ToCharArray ();
    let rules = lines
                |> Array.skip 2
                |> Array.map(fun line -> line.Split(" -> "))
                |> Array.map(fun seq -> (seq[0], seq[1]))
                |> Map

    polymer, rules

let growPolymerPairs(pairs:Map<string, int64>, transformations:Map<string,string>, iterations:int) = 
    let rec transform iteration (pairs:Map<string, int64>) =
        if iteration > 0 then
            pairs
            |> Map.toArray
            |> Array.collect (fun (key, count) -> [| ($"{key[0]}{transformations[key]}", count) ; ($"{transformations[key]}{key[1]}", count) |])
            |> Array.groupBy fst
            |> Array.map(fun (key, counts) -> (key, counts |> Array.map snd |> Array.sum))
            |> Map.ofArray
            |> transform(iteration - 1)
        else
            pairs

    transform iterations pairs
    
let calculate_delta(filePath:string, iterations:int) =
    let (polymer, transformations) = parseInput(filePath)
    
    let growPolymer (pairs) =
        growPolymerPairs(pairs, transformations, iterations)

    let createPairs (elements) =
        elements
        |> Array.windowed(2)
        |> Array.map (fun pair -> $"{pair[0]}{pair[1]}")
        |> Array.groupBy id
        |> Array.map(fun (key, value) -> key, value.Length |> int64)
        |> Map

    let countElements(elements:Map<string,int64>) =
        elements
        |> Map.toArray
        |> Array.append([| ($"{polymer |> Array.last}", 1) |])
        |> Array.groupBy(fun (key, _) -> key[0])
        |> Array.map(fun (key, counts) -> (key, counts |> Array.map snd |> Array.sum))

    let printDelta(elements) =
        elements
        |> Array.sortByDescending snd
        |> fun final -> (final |> Array.head |> snd) - (final |> Array.last |> snd)

    polymer
        |> createPairs
        |> growPolymer
        |> countElements
        |> printDelta

let execute =
    let generate_polymer(iterations) =
        calculate_delta("input.txt", iterations)

    10
    |> generate_polymer
    |> printfn "Q1 - delta is: %d"

    40
    |> generate_polymer
    |> printfn "Q2 - delta is: %d"
execute