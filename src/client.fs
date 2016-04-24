module Client

open System.Json
open System.Net
open System.Net.Sockets
open System.Text
open Fleece
open Messages

let serverAddr = "127.0.0.1"
let serverPort = 3000
let clientAddr = "127.0.0.1"
let serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), serverPort)

let getServerMsg(inSocket: UdpClient) = async {
  let! asyncData = inSocket.ReceiveAsync() |> Async.AwaitTask
  return asyncData.Buffer
}

let messageLoop localPort = async {
  use outSocket = new UdpClient()
  let clientEndPoint = new IPEndPoint(IPAddress.Any, localPort)
  let inSocket = new UdpClient(clientEndPoint)
  outSocket.Connect(serverEndPoint)

  let initMsg = Encoding.ASCII.GetBytes((toJSON {
                Connect.Type = "connect"
                Address = clientAddr
              }).ToString())

  let! _ = outSocket.SendAsync(initMsg, initMsg.Length) |> Async.AwaitTask
  printf "out: %s\n" (Encoding.ASCII.GetString(initMsg))
  outSocket.Close()

  let rec loop(inSocket: UdpClient) = async {
    let! msg = getServerMsg inSocket
    printf "in: %s\n" (Encoding.ASCII.GetString(msg))
    match msg with
      | ans ->
        return ()
  }

  return! async {
    try do! loop inSocket
    finally inSocket.Close()
  }
}
