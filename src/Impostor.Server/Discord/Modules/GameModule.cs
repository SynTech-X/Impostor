using System;
using System.Linq;
using System.Threading.Tasks;
using Disqord.Bot.Commands.Application;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Server.Net.Manager;
using Qmmands;

namespace Impostor.Server.Discord.Modules;

public class GameModule : DiscordApplicationGuildModuleBase
{
    private readonly IGameManager _gameManager;
    private readonly IServiceProvider _serviceProvider;

    public GameModule(IGameManager gameManager, IServiceProvider serviceProvider)
    {
        _gameManager = gameManager;
        _serviceProvider = serviceProvider;
    }

    [SlashCommand("create")]
    [Description("Creates a new game.")]
    public async Task<IResult> StartAsync(string? code = null)
    {
        IGame? game;

        if (code != null)
        {
            if (code.Length != 4 && code.Length != 6)
                return Response("The game code must be 4 or 6 characters long.");

            if (!code.All(char.IsLetter))
                return Response("The game code must only contain letters.");

            if (_gameManager.Find(code) != null)
                return Response("A game with that code already exists.");

            game = await _gameManager.CreateAsync(new NormalGameOptions(), GameFilterOptions.CreateDefault(), code);
        }
        else
        {
            game = await _gameManager.CreateAsync(new NormalGameOptions(), GameFilterOptions.CreateDefault());
        }

        if (game == null)
            return Response("Failed to create game.");

        return Response($"Game created with code `{game.Code}`.");
    }

    [SlashCommand("list-games")]
    [Description("Lists all games with their codes.")]
    public IResult ListGamesAsync()
    {
        var games = _gameManager.Games.ToList();

        if (!games.Any())
            return Response("No games found.");

        return Response($"Found {games.Count} games: {string.Join("\n", games.Select(x => $"`{x.Code}`"))}");
    }

    [SlashCommand("destroy")]
    [Description("Destroys a game.")]
    public async ValueTask<IResult> DestroyAsync(string code)
    {
        var game = _gameManager.Find(code);

        if (game == null)
            return Response("No game found with that code.");

        await (_gameManager as GameManager).RemoveAsync(code);

        return Response($"Game `{game.Code}` destroyed.");
    }
}

