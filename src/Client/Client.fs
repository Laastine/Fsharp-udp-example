module Client

open System.Net
open System.Net.Sockets
open System.Text
open Fleece
open Lib

type RaceWinner =
| Data of UdpReceiveResult
| Sleep

let getResponseMsg (socket: UdpClient) = async {
  let! asyncData = socket.ReceiveAsync() |> Async.AwaitTask
  return Some (Data asyncData)
}

let sleepFor (milliseconds: int) = async {
  let! _ = System.Threading.Tasks.Task.Delay(milliseconds) |> Async.AwaitTask
  return Some Sleep
}

let run (remoteAddress : string) remotePort = async {
  let outSocket = new UdpClient()

  let rec loop (outSocket : UdpClient) = async {
    let connect = {
      Connect.Id = System.Guid.NewGuid().ToString()
      State = "connect"
    }
    let connectMsg = Encoding.ASCII.GetBytes((toJson connect).ToString())
    let! _ = outSocket.SendAsync(connectMsg, connectMsg.Length, IPEndPoint(IPAddress.Parse(remoteAddress), remotePort) ) |> Async.AwaitTask
    printfn "CLIENT: Message sent, awaiting response..."
    // Race the reception of a response against a 10 second sleep
    let! winner = Async.Choice [
      getResponseMsg outSocket;
      sleepFor 10000;
    ]
    match winner with
    | Some Sleep -> 
      printfn "CLIENT: No response, starting again"
    | Some (Data r) -> 
      let parseResult =
          parseJson (Encoding.ASCII.GetString(r.Buffer))
      match parseResult with
      | Error e ->
        printfn "CLIENT: Failed to parse response: %s" e
        ()
      | Ok r ->
        printfn "CLIENT: Response: id = %s, state = %s" r.Id r.State
        // Wait for a second to prevent massive spam
        do! System.Threading.Tasks.Task.Delay(1000) |> Async.AwaitTask
        ()
    | None -> 
      printfn "CLIENT: Unexpected result, starting again"

    return! loop outSocket
  }
  return! loop outSocket
}

[<EntryPoint>]
let main (argv :string[]) =
  [
    run "127.0.0.1" 3000;
  ]
  |> Async.Parallel
  |> Async.Ignore
  |> Async.RunSynchronously
  0
