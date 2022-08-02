using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public static class Functions
    {
        public static async Task SetBotStatusAsync(DiscordSocketClient client)
        {
            JObject config = GetConfig();

            string currently = config["currently"]?.Value<string>().ToLower();
            string statusText = config["playing_status"]?.Value<string>();
            string onlineStatus = config["status"]?.Value<string>().ToLower();

            // Set the online status
            if (!string.IsNullOrEmpty(onlineStatus))
            {
                UserStatus userStatus = onlineStatus switch
                {
                    "dnd" => UserStatus.DoNotDisturb,
                    "idle" => UserStatus.Idle,
                    "offline" => UserStatus.Invisible,
                    _ => UserStatus.Online
                };

                await client.SetStatusAsync(userStatus);
                Console.WriteLine($"{DateTime.Now.TimeOfDay:hh\\:mm\\:ss} | Online status set | {userStatus}");
            }

            // Set the playing status
            if (!string.IsNullOrEmpty(currently) && !string.IsNullOrEmpty(statusText))
            {
                ActivityType activity = currently switch
                {
                    "listening" => ActivityType.Listening,
                    "watching" => ActivityType.Watching,
                    "streaming" => ActivityType.Streaming,
                    _ => ActivityType.Playing
                };

                await client.SetGameAsync(statusText, type: activity);
                Console.WriteLine($"{DateTime.Now.TimeOfDay:hh\\:mm\\:ss} | Playing status set | {activity}: {statusText}");
            }            
        }

        public static JObject GetConfig()
        {
            // Get the config file.
            using StreamReader configJson = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                return (JObject)JsonConvert.DeserializeObject(configJson.ReadToEnd());
        }

        public static string GetAvatarUrl(SocketUser user, ushort size = 1024)
        {
            // Get user avatar and resize it. If the user has no avatar, get the default Discord avatar.
            return user.GetAvatarUrl(size: size) ?? user.GetDefaultAvatarUrl(); 
        }

        public static EmbedBuilder HandleReminder(int time, string message, SocketCommandContext context)
        {
            // TODO: replace thread sleeping with better system eventually
            Thread.Sleep(time);

            // EmbedBuilder embed = new EmbedBuilder();
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);
            const double PSTTIMEOFFSET = -7;
            string shortDate = currentTime.AddMilliseconds(-1 * time).AddHours(PSTTIMEOFFSET).ToShortDateString();
            string shortTime = currentTime.AddMilliseconds(-1 * time).AddHours(PSTTIMEOFFSET).ToShortTimeString();
            EmbedBuilder embed = new EmbedBuilder() 
            {
                Description = message,
                Color = Color.LightOrange,
                Footer = new EmbedFooterBuilder().WithText($"Reminder from {shortDate} {shortTime} PST"),
                Url = context.Message.GetJumpUrl()
            };
            return embed;
        }
    }
}
