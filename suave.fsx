#r "nuget:Suave"

open Suave
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket

open Suave.Operators
open Suave.Filters
open Suave.RequestErrors

open System

let indexHtml =
    """
    <!DOCTYPE html>

    <meta charset="utf-8" />

    <title>Suave.IO with HTMX websocket Test</title>

    <script src="https://unpkg.com/htmx.org@1.9.6"></script>
    <script src="https://unpkg.com/htmx.org/dist/ext/ws.js"></script>
        <body>
            <h2>Suave.IO with HTMX websocket Test</h2>

            <div hx-ext="ws" ws-connect="/notifications">
                <div id="notifications"></div>
            </div>

            <div hx-ext="ws" ws-connect="/chatroom">
                <ul id="chatRoom">
                    
                </ul>
                <form id="form" ws-send>
                    <input name="chatMessage" />
                </form>
            </div>
        </body>
    </html>
    """

let chatResponse str =
    $"""
        <div id="chatRoom" hx-swap-oob="beforeend">
            <li>Message received: {str}</li>
        </div>
    """

let notificationResponse (receivedAt: DateTime) (message: string) =
    $"""
        <div id="notifications">
            Notification received at {receivedAt.ToString("hh:mm:ss.F")}: {message}
        </div>
    """

let chatroomWs (webSocket: WebSocket) (ctx: HttpContext) =
    socket {
        let mutable loop = true

        while loop do
            let! msg = webSocket.read()

            match msg with
            | (Text, data, true) ->
                let str = UTF8.toString data
                printfn "Received message: '%s'" str
                let response = chatResponse str
                let byteResponse =
                    response
                    |> System.Text.Encoding.ASCII.GetBytes
                    |> ByteSegment
                do! webSocket.send Text byteResponse true

            | (Close, _, _) ->
                let emprtyResponse = [||] |> ByteSegment
                do! webSocket.send Close emprtyResponse true
            
            | _ -> ()
    }

let notificationWs (webSocket: WebSocket) (ctx: HttpContext) =
    let notifyLoop =
        async {
            while true do
                let message =
                    seq {
                        for _ in [0..Random.Shared.Next(11, 42)] do
                            yield (char (Random.Shared.Next(32, 127))).ToString()
                    }
                    |> String.concat String.Empty

                let response = notificationResponse DateTime.UtcNow message
                let byteResponse =
                    response
                    |> System.Text.Encoding.ASCII.GetBytes
                    |> ByteSegment
                let! choice = webSocket.send Text byteResponse true
                do! Async.Sleep(Random.Shared.Next(1000,5000))
        }

    Async.Start(notifyLoop)

    socket {
        while true do
            let! message = webSocket.read()
            match message with
            | Ping, _, _ -> do! webSocket.send Pong ([||] |> ByteSegment) true
            | _ -> () 
    }

let app: WebPart =
    choose [
        path "/chatroom" >=> handShake chatroomWs
        path "/notifications" >=> handShake notificationWs
        GET >=>
            choose [ 
                path "/" >=> Successful.OK indexHtml >=> Writers.setMimeType "text/html" 
            ] 
            >=> Writers.addHeader "X-Clacks-Overhead" "GNU Terry Pratchett"
        NOT_FOUND "Nothing found"
    ]

startWebServer defaultConfig (app)