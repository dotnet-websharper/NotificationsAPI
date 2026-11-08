namespace WebSharper.Notifications.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Notifications

[<JavaScript>]
module Client =
    // The template is loaded from the DOM, so editing index.html and refreshing
    // the browser is enough; a rebuild is only needed when holes change.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let status = Var.Create "Ready to request notification permission."
    let permission = Var.Create "default"

    let requestNotification () =
        promise {
            try
                let! result = Notification.RequestPermission()
                permission := result
                if result = "granted" then
                    status := "Notification sent."
                    let _ = new Notification("Hello! Notifications are enabled.", NotificationOptions(
                        Body = "This is a test notification."
                    ))
                    ()
                else
                    status := "Notifications denied."
            with error ->
                status := $"Error: {error.Message}"
        }

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate()
            .requestNotification(fun _ -> requestNotification () |> ignore)
            .status(status.V)
            .permission(permission.V)
            .Bind()
