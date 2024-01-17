using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace DiscordSoundboard
{
	class DiscordBot
	{
		public static DiscordClient? discordClient { get; private set; }

		public DiscordBot(Config.AppSettings appSettings)
		{
			discordClient = new DiscordClient(new DiscordConfiguration()
			{
				Token = appSettings.DiscordSettings.Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
			});

			SlashCommandsExtension slashCommands = discordClient.UseSlashCommands();
			slashCommands.RegisterCommands<SlashCommands>(appSettings.DiscordSettings.GuildID);
			discordClient.ComponentInteractionCreated += InteractionHandler.HandleInteraction;

			// On button press
			discordClient.Ready += async (client, args) =>
			{
				Console.WriteLine($"Connected to Discord as {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
			};

			discordClient.ConnectAsync(new DiscordActivity("with sounds", ActivityType.Playing));
		}


	}
}
