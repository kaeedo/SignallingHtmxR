# HTMX, WebSockets, SingalR and you

This is a very simple demo showcasing how to connect to a WebSocket endpoint using HTMX. There are two versions: The first utilizes the wonderful [Suave](https://suave.io/) to showcase how to consume a raw WebSocket endpoint, and the second is a full blown ASP.NET SignalR solution to show the difference between raw WebSocket, and SignalR specific implementation details.

Accompanying blog post [here](https://hashset.dev/article/21_htmx_web_sockets_signal_r_and_you)

## Run simple Suave version

`dotnet fsi suave.fsx`

Open browser to `localhost:8080`

## Run ASP.NET version

- `dotnet restore`
- `dotnet build`
- `dotnet run --project src/MinimalApi/MinimalApi.fsproj`

Open browser to `localhost:5003`