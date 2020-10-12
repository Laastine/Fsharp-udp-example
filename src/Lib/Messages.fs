namespace Lib

open System
open System.Json
open Fleece
open Fleece.Operators

type Connect = {
  Id: string
  State: string
}

type Connect
  with
  static member ToJSON (c: Connect) =
    jobj [
      "id" .= c.Id
      "state" .= c.State
    ]

type Connect
  with
  static member FromJSON (_: Connect) =
    function
    | JObject o ->
      let id = o .@ "id"
      let ``state`` = o .@ "state"
      match ``id``, ``state`` with
      | Success ``id``, Success ``state`` ->
        Success {
          Connect.Id = ``id``
          Connect.State = ``state``
        }
      | x -> Failure (sprintf "Error parsing Response: %A" x)
    | x -> Failure (sprintf "Expected Response, got %A" x)
