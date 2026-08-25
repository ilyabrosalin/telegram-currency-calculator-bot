using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var json = File.ReadAllText("appsettings.json");
var config = JsonDocument.Parse(json);
var botToken = config.RootElement.GetProperty("BotToken").GetString() ??
               throw new InvalidOperationException("Bot token not found in configuration");

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var me = await bot.GetMe();

bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();

async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await bot.SendMessage(msg.Chat,
            "Привет! Пока я в разработке — можешь написать мне что угодно, и я повторю это в ответ \n\nСкоро научусь считать выражения и конвертировать валюты!");
    }
    else
    {
        Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
        await bot.SendMessage(msg.Chat, $"{msg.Text}");
    }
}

async Task OnUpdate(Update update)
{
}