open System.IO
open System.Collections.Generic

let parseInput(filePath) =
    let parseChar c = (c |> int) - ('0' |> int)

    File.ReadAllLines (filePath)
    |> Array.map(fun value -> (value.ToCharArray () |> Array.map parseChar))

let traverse(accessor, length) =
    let mutable vertices =  [|0..length - 1|]
                            |> Array.collect(fun x -> ([|0..length - 1|] |> Array.map (fun y -> (x, y))))
                            |> Set.ofArray

    let mutable distances = Map.empty<(int*int), int>
    let pq = new PriorityQueue<int*int, int> ();
    
    pq.Enqueue((0, 0), 0);
    distances <- distances.Add((0, 0), 0)
    while (pq.Count > 0) do 
        let _, currentNode, currentCost = pq.TryDequeue();
        if vertices.Contains (currentNode) then
            vertices <- vertices.Remove (currentNode)

            [|(0,  1) ; (0, -1); (1, 0) ; (-1, 0) |]
                |> Array.map (fun (x, y) -> (x + (currentNode |> fst), y + (currentNode |> snd)))
                |> Array.filter (vertices.Contains)
                |> Array.map(fun t -> (t, currentCost + accessor((t |> fst),(t |> snd))))
                |> Array.filter(fun (node, cost) -> cost < distances.GetValueOrDefault (node, System.Int32.MaxValue))
                |> Array.iter (fun (node, cost) -> (distances <- distances.Add (node, cost) ;
                                                    pq.Enqueue(node, cost)))

    distances[(length - 1, length - 1)]
    

let execute =
    let grid_extender(grid:int[][])(x:int, y:int) =
        (grid[x % grid.Length][y % grid.Length] + x / grid.Length + y / grid.Length - 1) % 9 + 1

    "input.txt"
    |> parseInput
    |> fun input -> [|("Q1", input, input.Length) ; ("Q2", input, input.Length * 5) |]
    |> Array.map (fun (q, grid, length) -> (q, traverse(grid_extender grid, length)))
    |> Array.map (fun (q, danger) -> printfn "%s danger: %d" q danger)

execute |> ignore