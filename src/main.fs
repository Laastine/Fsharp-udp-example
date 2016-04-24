module main

open Client
open System

[<EntryPoint>]
let main (argv :string[]) =
  [Client.messageLoop 3001]
  |> Async.Parallel
  |> Async.Ignore
  |> Async.RunSynchronously
  0
