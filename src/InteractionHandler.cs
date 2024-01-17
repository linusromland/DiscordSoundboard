using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordSoundboard
{
	class InteractionHandler
	{
		public static async Task HandleInteraction(DiscordClient client, ComponentInteractionCreateEventArgs e)
		{
			await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

			string fileName = e.Id;
			string basePath = Directory.GetCurrentDirectory();
			string soundsPath = Path.Combine(basePath, "sounds");
			string soundPath = Path.Combine(soundsPath, fileName);

			if (!File.Exists(soundPath))
			{
				return;
			}


		}


	}
}