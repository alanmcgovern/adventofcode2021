open System.IO
open System

let parseInput(filePath:string) =
    let lines = File.ReadAllLines(filePath)
    let polymer = lines[0].ToCharArray ();
    let rules = lines
                |> Array.skip 2
                |> Array.map(fun line -> line.Split(" -> "))
                |> Array.map(fun seq -> (seq[0], seq[1]))
                |> Map.ofArray
    polymer, rules

let grow_polymer (polymer:char[], transformations:Map<string,string>, iterations:int) = 

    // F# is supposed to like tail calls, right??
    let rec transform iteration (current_polymer:char[]) =
        if iteration > 0 then
            current_polymer
                |> Array.windowed 2
                |> Array.map(fun item -> $"{item[0]}{transformations.TryFind((String.Join(String.Empty, item))).Value}{item[1]}")
                |> Array.reduce(fun state value -> (state + value[1..]))
                |> Seq.toArray
                |> transform(iteration - 1)
        else
            current_polymer
    transform iterations polymer

    
let question1 =
    let (polymer, transformations) = parseInput("input.txt")
    grow_polymer(polymer, transformations, 10)
    |> Array.groupBy id
    |> Array.sortBy(fun (key, value) -> value.Length)
    |> fun final -> (final |> Array.last |> snd |> Array.length) - (final |> Array.head |> snd |> Array.length)

let execute =
    printfn "Q1 - delta is: %d" question1

execute