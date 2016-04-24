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
