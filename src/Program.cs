using dotenv.net;
using DSharpPlus;

namespace DiscordSoundboard
{
	class Program
	{
		static async Task Main(string[] args)
		{
			DotEnv.Load();
			IDictionary<string, string> envVars = DotEnv.Read();

			var discord = new DiscordClient(new DiscordConfiguration()
			{
				Token = envVars["DISCORD_TOKEN"],
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
			});

			discord.MessageCreated += async (s, e) =>
			{
				if (e.Message.Content.ToLower().StartsWith("ping"))
					await e.Message.RespondAsync("pong!");
			};

			await discord.ConnectAsync();
			await Task.Delay(-1);
		}
	}
}
