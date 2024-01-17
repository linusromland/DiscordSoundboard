using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DiscordSoundboard
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Config.AppSettings appSettings = Config.GetConfigSettings();

			DiscordClient? discord = new DiscordClient(new DiscordConfiguration()
			{
				Token = appSettings.DiscordSettings.Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
			});

			SlashCommandsExtension slashCommands = discord.UseSlashCommands();
			slashCommands.RegisterCommands<SlashCommands>(appSettings.DiscordSettings.GuildID);

			await discord.ConnectAsync();
			await Task.Delay(-1);
		}
	}
}
