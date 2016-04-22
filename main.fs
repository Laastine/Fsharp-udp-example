module main

open System

[<EntryPoint>]
let main (argv :string[]) =
  printfn "Hello World"
  0

open System.Net
open System.Net.Sockets
open System.Text

type Client() = 
  let serverAddress = new IPEndPoint(IPAddress.Any, 9999)
  
  let serverAddr = "127.0.0.1"
  let serverPort = 3000
  
  let msg = System.Text.Encoding.ASCII.GetBytes("hello world") 
  
  let listen = async {
    let ip = new IPEndPoint(IPAddress.Any, serverPort)
    let clientSock = new UdpClient()
    
    let sender = new IPEndPoint(IPAddress.Any, 0)
    printf "Listening port %d" serverPort
    let data = clientSock.Receive(ref sender)
    return data     
  }

  member client.connectToServer() =
    let task = Async.StartAsTask (listen)
    
    let connection = new UdpClient()
    let serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), serverPort)
    connection.Connect(serverEndPoint)
    connection.Send(msg, msg.Length) |> ignore
    printf "Client: %s" (Encoding.ASCII.GetString(msg))
    
    printf "Server: %s" (Encoding.ASCII.GetString(task.Result))
