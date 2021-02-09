open Funogram
open System
open Funogram.Telegram.Bot
open Funogram.Telegram.Api
open Telegram.Types
open ExtCore.Control
open System.Text.Json
open System.Net

let random = Random()

let rand() = random.Next Int32.MaxValue

let client = new WebClient()

let getClowns() =
    let request = "https://raw.githubusercontent.com/IMACULGY/pleaseclown.me/master/audio.json"
    let response = client.DownloadString request
    JsonSerializer.Deserialize<string array>(response)

let getClownAudio title =
    Voice { 
        Id = (string <| rand())
        Caption = None
        ReplyMarkup = None
        InputMessageContent = None
        Title = title
        ParseMode = None
        VoiceUrl = $"https://api.pleaseclown.me/audio/{title}?format=ogg"
        VoiceDuration = None
    }

let onUpdate (context: UpdateContext) =
    maybe {
        let! query = context.Update.InlineQuery
        query.Query |> printfn "%A"
        let requests = getClowns() |> Array.map getClownAudio
        answerInlineQueryBase (query.Id) requests None None None None None 
        |> Api.api context.Config
        |> Async.Ignore
        |> Async.Start
    } |> ignore

startBot { defaultConfig with Token = "Здесь должна быть ваша реклама" } onUpdate None
|> Async.RunSynchronously

