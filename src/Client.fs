module Client

open System.Json
open System.Net
open System.Net.Sockets
open System.Text
open Fleece
open Messages

type RaceWinner =
| Data of byte array
| Sleep

let getResponseMsg(socket: UdpClient) = async {
  let! asyncData = socket.ReceiveAsync() |> Async.AwaitTask
  return Some (Data asyncData.Buffer)
}

let sleepFor (milliseconds: int) = async {
  let! _ = System.Threading.Tasks.Task.Delay(milliseconds) |> Async.AwaitTask
  return Some Sleep
}

let run (address : string) port = async {
  let address = "127.0.0.1"
  let outEndpoint = IPEndPoint(IPAddress.Parse(address), port)
  use outSocket = new UdpClient()
  outSocket.Connect(outEndpoint)

  let inSocket = new UdpClient()
  let inEndpoint = outSocket.Client.LocalEndPoint :?> IPEndPoint
  inSocket.Connect(inEndpoint)

  let rec loop () = async {
    let connectMsg = Encoding.ASCII.GetBytes((toJson {
                  Connect.Id = System.Guid.NewGuid.ToString()
                  State = "connect"
                }).ToString())
    let! _ = outSocket.SendAsync(connectMsg, connectMsg.Length) |> Async.AwaitTask
    printf "CLIENT: Message sent, awaiting response..."
    let! winner = Async.Choice [
      getResponseMsg inSocket;
      sleepFor 10000;
    ]
    match winner with
    | Some Sleep -> 
      printf "CLIENT: No response, starting again\n"
    | Some (Data d) -> 
      let parseResult =
          parseJson (Encoding.ASCII.GetString(d))
      match parseResult with
      | Error e ->
        printf "CLIENT: Failed to parse response: %s\n" e
        ()
      | Ok r ->
        printfn "CLIENT: Response: id = %s, state = %s" r.Id r.State
        ()
    | None -> 
      printf "CLIENT: Unexpected result, starting again\n"

    return! loop ()
  }
  return! loop ()
}
