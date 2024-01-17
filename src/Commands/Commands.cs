
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordSoundboard
{
	public class SlashCommands : ApplicationCommandModule
	{
		[SlashCommand("ping", "Replies with pong!")]
		public async Task Ping(InteractionContext ctx)
		{
			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong!"));
		}
	}
}