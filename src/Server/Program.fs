open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Server

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddSignalR() |> ignore

    builder.Services.AddHostedService<NotificationService>()
    |> ignore

    let app = builder.Build()

    app.MapGet(
        "/",
        Func<IResult>(fun () ->
            let html = System.IO.File.ReadAllText("wwwroot/index.html")
            Results.Extensions.Html(html))
    )
    |> ignore

    app.UseStaticFiles() |> ignore
    app.MapHub<ChatHub>("/chathub") |> ignore

    app.Run()

    0 // Exit code
