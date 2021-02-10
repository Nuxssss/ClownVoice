open Funogram
open System
open Funogram
open System
open Funogram.Telegram.Bot
open Funogram.Telegram.Api
open Telegram.Types
open ExtCore.Control
open System.Text.Json
open System.Net

let emptyInlineVoiceQuery = {
    Id = ""
    Caption = None
    ReplyMarkup = None
    InputMessageContent = None
    Title = ""
    ParseMode = None
    VoiceUrl = ""
    VoiceDuration = None
}

let answerInlineQuery id requests = 
    answerInlineQueryBase id requests None None None None None

let random = Random()

let rand() = random.Next Int32.MaxValue

let client = new WebClient()

let getClowns() =
    "https://raw.githubusercontent.com/IMACULGY/pleaseclown.me/master/audio.json"
    |> client.DownloadString
    |> JsonSerializer.Deserialize<string array>
    
let getClownAudio title = 
    Voice { emptyInlineVoiceQuery with 
                Id = string <| rand()
                Title = title
                VoiceUrl = $"https://api.pleaseclown.me/audio/{title}?format=mp3" }

let onUpdate (context: UpdateContext) =
    maybe {
        let! query = context.Update.InlineQuery
        let requests = getClowns() |> Array.map getClownAudio
        answerInlineQuery (query.Id) requests
        |> Api.api context.Config
        |> Async.Ignore
        |> Async.Start
    } |> ignore

startBot { defaultConfig with Token = "Здесь могла быть ваша реклама" } onUpdate None
|> Async.RunSynchronously
