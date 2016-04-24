module main

open Client
open System

[<EntryPoint>]
let main (argv :string[]) =
  let client = Client()
  client.connectToServer() |> ignore
  0
