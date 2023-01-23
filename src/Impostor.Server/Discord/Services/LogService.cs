using System.Threading.Tasks;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Rest;
using Microsoft.Extensions.Configuration;

namespace Impostor.Server.Discord.Services;

public class LogService : DiscordBotService
{
    private readonly Snowflake _channelId;

    public LogService(IConfiguration configuration)
    {
        _channelId = ulong.Parse(configuration["LOG_CID"]);
    }

    public Task<IUserMessage> LogAsync(string message) => Bot.SendMessageAsync(_channelId, new LocalMessage().WithContent(message));

    public void LazyLog(string message) => Task.Run(async () => await LogAsync(message));
}
