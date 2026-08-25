using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var json = File.ReadAllText("appsettings.json");
var config = JsonDocument.Parse(json);
var botToken = config.RootElement.GetProperty("BotToken").GetString() ??
               throw new InvalidOperationException("Bot token not found in configuration");

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var me = await bot.GetMe();

bot.OnMessage += OnMessage;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text is null) return;
    Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
    await bot.SendMessage(msg.Chat, $"{msg.Text}");
}