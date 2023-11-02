namespace Server

open Microsoft.AspNetCore.SignalR

type ChatHub() =
    inherit Hub()

    member x.BaseSendMessage(message: string) =
        base.Clients.All.SendAsync("chatMessage", $"""<li>Message received: {message}</li>""")

    member x.SendMessage(request: {| ChatMessage: string |}) =
        task {
            printfn "Received send message from frontend"
            do! x.BaseSendMessage(request.ChatMessage)
        }
