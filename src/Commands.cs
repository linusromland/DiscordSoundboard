
using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordSoundboard
{
	public class SlashCommands : ApplicationCommandModule
	{
		[SlashCommand("AddSound", "Add a sound to the soundboard")]
		public async Task AddSound(InteractionContext ctx,
			[Option("Name", "The name of the sound")] string name,
			[Option("File", "The file to play")] DiscordAttachment file)
		{
			await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

			Console.WriteLine($"Uploaded file {file.Url} with name {file.FileName} and type {file.MediaType}");

			if (!file.MediaType.StartsWith("audio/") && !file.MediaType.StartsWith("video/"))
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid file type"));
				return;
			}

			Console.WriteLine($"Adding sound {name} with file {file.Url}");

			// Write the file to disk
			string basePath = Directory.GetCurrentDirectory();
			string filePath = Path.Combine(basePath, "sounds", name + Path.GetExtension(file.FileName));

			// Check if the file already exists
			if (File.Exists(filePath))
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Sound already exists"));
				return;
			}

			try
			{
				byte[] fileTask = await new HttpClient().GetByteArrayAsync(file.Url);

				// Convert to mp3 with normalized volume
				using (var ffmpeg = new Process())
				{
					ffmpeg.StartInfo.FileName = "ffmpeg";
					ffmpeg.StartInfo.Arguments = $"-i pipe:0 -ss 00:00:00 -t 10 -af loudnorm -f mp3 \"{filePath}\"";
					ffmpeg.StartInfo.UseShellExecute = false;
					ffmpeg.StartInfo.RedirectStandardInput = true;
					ffmpeg.StartInfo.RedirectStandardOutput = false;
					ffmpeg.StartInfo.RedirectStandardError = false;

					ffmpeg.Start();

					await ffmpeg.StandardInput.BaseStream.WriteAsync(fileTask, 0, fileTask.Length);
					ffmpeg.StandardInput.Close();

					ffmpeg.WaitForExit();
				}



				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added sound {name}"));


				// Send the soundboard
				SendSoundboard(ctx.Channel);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to add sound"));

				// Send the soundboard
				SendSoundboard(ctx.Channel);
				return;
			}
		}

		public void SendSoundboard(DiscordChannel channel)
		{
			// Get all sounds in the sounds directory, ignore the .gitkeep file
			string basePath = Directory.GetCurrentDirectory();
			string soundsPath = Path.Combine(basePath, "sounds");
			string[] soundFiles = Directory.GetFiles(soundsPath).Where(file => !file.EndsWith(".gitkeep")).ToArray();


			List<DiscordButtonComponent> buttons = [];

			// Add each sound as a button
			foreach (string soundFile in soundFiles)
			{
				string soundName = Path.GetFileNameWithoutExtension(soundFile);
				string fileExtension = Path.GetExtension(soundFile);

				buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, soundName + fileExtension, soundName));
			}

			// Split the buttons to 5 per row and create action rows
			List<DiscordActionRowComponent> actionRows = new List<DiscordActionRowComponent>();
			for (int i = 0; i < buttons.Count; i += 5)
			{
				actionRows.Add(new DiscordActionRowComponent(buttons.GetRange(i, Math.Min(5, buttons.Count - i))));
			}


			DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder().WithContent("Soundboard").AddComponents(actionRows);

			// Send the embed
			DiscordBot.discordClient?.SendMessageAsync(channel, messageBuilder);
		}
	}
}