using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;

namespace DiscordSoundboard
{
	class InteractionHandler
	{
		private static System.Timers.Timer? timeoutTimer;

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

			VoiceNextExtension DiscordVoice = client.GetVoiceNext();

			VoiceNextConnection? vnc = DiscordVoice.GetConnection(e.Guild);

			DiscordUser user = e.User;
			DiscordMember member = e.Guild.Members[user.Id];
			DiscordChannel? voiceChannel = member.VoiceState?.Channel;

			if (voiceChannel == null)
			{
				return;
			}

			if (vnc?.TargetChannel != voiceChannel)
			{
				vnc?.Disconnect();
				vnc = null;
			}

			vnc ??= await DiscordVoice.ConnectAsync(voiceChannel);

			// Play the sound file in the voice channel
			var ffmpeg = new ProcessStartInfo
			{
				FileName = "ffmpeg",
				Arguments = $"-i \"{soundPath}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};

			Process? ffmpegProcess = Process.Start(ffmpeg);

			if (ffmpegProcess == null)
			{
				return;
			}

			Stream? ffout = ffmpegProcess.StandardOutput.BaseStream;

			VoiceTransmitSink? txStream = vnc.GetTransmitSink();


			await ffout.CopyToAsync(txStream);
			await txStream.FlushAsync();

			await vnc.WaitForPlaybackFinishAsync();

			timeoutTimer?.Dispose();
			timeoutTimer = new System.Timers.Timer(5 * 60 * 1000);
			timeoutTimer.Elapsed += (sender, args) =>
			{

				vnc?.Disconnect();
				timeoutTimer?.Dispose();
				timeoutTimer = null;
			};
			timeoutTimer.Start();
		}


	}
}