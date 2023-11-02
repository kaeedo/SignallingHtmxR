namespace Server

open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.SignalR
open System.Threading.Tasks
open System

type NotificationService(hubContext: IHubContext<ChatHub>) =
    inherit BackgroundService()

    override _.ExecuteAsync(cancellationToken: System.Threading.CancellationToken) =
        task {
            while not cancellationToken.IsCancellationRequested do
                let message =
                    seq {
                        for _ in [ 0 .. Random.Shared.Next(11, 42) ] do
                            yield (char (Random.Shared.Next(32, 127))).ToString()
                    }
                    |> String.concat String.Empty

                do!
                    hubContext.Clients.All.SendAsync(
                        "notifications",
                        $"""
                        <div id="notifications">
                            Notification received at {DateTime.UtcNow.ToString("hh:mm:ss.F")}: {message}
                        </div>
                    """

                    )

                do! Task.Delay(Random.Shared.Next(1000, 3000), cancellationToken)
        }
