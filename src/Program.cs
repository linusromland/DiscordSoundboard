﻿using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DiscordSoundboard
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Config.AppSettings appSettings = Config.GetConfigSettings();

			new DiscordBot(appSettings);

			await Task.Delay(-1);
		}
	}
}
