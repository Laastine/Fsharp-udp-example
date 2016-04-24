namespace Client

open System.Net
open System.Net.Sockets
open System.Text
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

type Client() =
  let serverAddress = new IPEndPoint(IPAddress.Any, 9999)

  let localAddr = "127.0.0.1"
  let localPort = 3000

  let jsonMsg = {
                  Connect.Type = "connect"
                  Address = localAddr
                }
  let msg = Encoding.ASCII.GetBytes((toJSON jsonMsg).ToString())

  let listen = async {
    let ip = new IPEndPoint(IPAddress.Any, localPort)
    let clientSock = new UdpClient()

    let sender = new IPEndPoint(IPAddress.Any, 0)
    printf "Listening port %d" localPort
    let data = clientSock.Receive(ref sender)
    return data
  }

  member client.connectToServer() =
    let task = Async.StartAsTask (listen)

    let connection = new UdpClient()
    let serverEndPoint = new IPEndPoint(IPAddress.Parse(localAddr), localPort)
    connection.Connect(serverEndPoint)
    connection.Send(msg, msg.Length) |> ignore
    printf "Client: %s" (Encoding.ASCII.GetString(msg))

    printf "Server: %s" (Encoding.ASCII.GetString(task.Result))
