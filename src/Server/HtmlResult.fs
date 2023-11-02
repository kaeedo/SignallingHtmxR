namespace Microsoft.AspNetCore.Http

open System.Net.Mime
open System.Runtime.CompilerServices
open System.Text

type HtmlResult(html: string) =
    interface IResult with
        member _.ExecuteAsync(httpContext: HttpContext) =
            httpContext.Response.ContentType <- MediaTypeNames.Text.Html
            httpContext.Response.ContentLength <- Encoding.UTF8.GetByteCount(html)
            httpContext.Response.WriteAsync(html)

[<Extension>]
type ResultsExtensions() =
    [<Extension>]
    static member Html(resultExtensions: IResultExtensions, html: string) = HtmlResult(html)
