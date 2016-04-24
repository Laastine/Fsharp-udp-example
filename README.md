# Fsharp-udp-example

Build and run
```
./build.sh
mono build/fsharpudpexample.exe
```

Dummy netcat UDP-listener: `nc -lu 127.0.0.1 3000`
Send UDP message to client: `echo -n '{"type": "msg", "message": "Hello World"}' | nc -u 127.0.0.1 3000`

### Environment setup
http://fsharp.org/guides/mac-linux-cross-platform/

Mono cross platform .NET framework: `brew install mono`</br>
F# Project Builder: `brew tap samritchie/forge && brew install forge`

### Build tl;dr version
- Fake (make for F#)
- Paket (package manager) `mono .paket/paket.exe add/remove nuget <package name>`
- Forge (CLI project builder) `forge reference add <path to package>`
