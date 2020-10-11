# FSharp UDP Example

## Environment

1. [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Build

```
dotnet build
```

## Run

### Client & Server

On two separate shells, run the client & server independently:

1. Start server

```
# Shell 1
cd src/Server
dotnet run
```

2. Start client
```
# Shell 2
cd src/Client
dotnet run
```

You'll then see messages being sent from the client and responses being sent from the server


### Client only

To test the UDP client, start a _netcat_ server before running the program:

```sh
cd src/Client
nc -lu 127.0.0.1 3000 &
dotnet run
```
Obviously the server will never respond as it's just a dumb listening socket so the client will just loop timing out.

### Server only

To test the UDP server, start the server and then use a _netcat_ udp client to send valid JSON:

```sh
cd src/Server
dotnet run &
echo -n '{"state": "connect", "id": "'$(uuidgen)'"}' | nc -u 127.0.0.1 3000
```

The server should respond to each valid client message issued by echo/netcat.
