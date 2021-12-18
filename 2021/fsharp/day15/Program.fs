open System.IO


let parseInput(filePath) =
    let parseChar (c:char) = (c |> int) - ('0' |> int)

    File.ReadAllLines (filePath)
    |> Array.map(fun value -> (value.ToCharArray () |> Array.map parseChar))

let walkNodes(grid:int[][]) =
    let mutable lowestDanger = 1238

    let rec wander (x, y, score, grid:int[][])=
        let validDirections () =
            [|(1, 0); (-1, 0); (0, 1);(0, -1) |]
            |> Array.map(fun (deltaX, deltaY) -> x + deltaX, y + deltaY)
            |> Array.filter (fun (newX, newY) -> newY >= 0 && newY < grid.Length && newX >= 0 && newX < grid[newY].Length)

        let currentScore = score + grid[x][y]
        if currentScore > lowestDanger then
            currentScore
        else if y = (grid.Length - 1) && x = (grid[y].Length - 1) then
            lowestDanger <- currentScore
            currentScore
        else 
            validDirections ()
            |> Array.map (fun (newX, newY) -> wander(newX, newY, currentScore, grid))
            |> Array.min

    wander(0, 0, 0, grid)

let execute =
    "input.txt"
    |> parseInput
    |> walkNodes
    |> printf "Danger: %d"

execute