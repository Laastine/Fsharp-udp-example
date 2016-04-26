namespace Messages

open System
open System.Json
open Fleece
open Fleece.Operators

type Connect = {
  Type: string
  Address: string
}

type Connect with
  static member ToJSON (c: Connect) =
    jobj [
      "type" .= c.Type
      "address" .= c.Address
    ]

type Response = {
  Type: string
  Msg: string
}

type Response with
  static member FromJSON (_: Response) =
    function
    | JObject o ->
      let ``type`` = o .@ "type"
      let msg = o .@ "msg"
      match ``type``, msg with
      | Success ``type``, Success msg ->
        Success {
          Response.Type = ``type``
          Response.Msg = msg
        }
      | x -> Failure (sprintf "Error parsing Response: %A" x)
    | x -> Failure (sprintf "Expected Response, got %A" x)
