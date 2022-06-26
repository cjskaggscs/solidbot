using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot
{
    public class UtilityModules : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Show current latency.")]
        public async Task Ping()
            => await ReplyAsync($"Latency: {Context.Client.Latency} ms");

        [Command("avatar")]
        [Alias("getavatar")]
        [Summary("Get a user's avatar.")]
        public async Task GetAvatar([Remainder] SocketGuildUser user = null)
            => await ReplyAsync($":frame_photo: **{(user ?? Context.User as SocketGuildUser).Username}**'s avatar\n{Functions.GetAvatarUrl(user)}");

        // Supports input of format ;remindme message, #d #hr #m
        // Zero or one of any of days, hours, or minutes are accepted and the comma is required after the message
        [Command("remindme")]
        [Alias("reminder")]
        [Summary("Set a reminder.")]
        public async Task RemindMe([Remainder] string message = "")
        {
            Match stringMatch = Regex.Match(message, @"\b(?<remindMessage>.*)+,\s+(?<remindTime>\d\s*\w*\s*){1,3}");
            string remindMessage;

            if (stringMatch.Groups["remindMessage"].Captures.Count > 0) { remindMessage = stringMatch.Groups["remindMessage"].Captures[0].Value; }
            else 
            {
                await ReplyAsync($"Unexpected command format. Please use the format `;remindme <message>, #d #hr #m`.");
                return;
            }

            Group myVar = stringMatch.Groups["remindTime"];
            int sleepTime = 0;

            foreach (Group group in stringMatch.Groups["remindTime"].Captures)
            {
                string remindTime = group.Value.ToLower();
                int remindTimeValue;
                char remindTimeUnit;

                // Parse values and units, and convert to ms
                try
                {
                    Match singleTimeMatch = Regex.Match(remindTime, @"\b(?<timeValue>\d+)\s*(?<timeUnit>\w*)");
                    remindTimeValue = int.Parse(singleTimeMatch.Groups["timeValue"].Value);
                    remindTimeUnit = singleTimeMatch.Groups["timeUnit"].Value[0];
                    switch (remindTimeUnit)
                    {
                        case 'd':
                            remindTimeValue *= 24; 
                            goto case 'h';
                        case 'h':
                            remindTimeValue *= 60;
                            goto case 'm';
                        case 'm':
                            remindTimeValue *= 60 * 1000;
                            sleepTime += remindTimeValue;
                            if (sleepTime > Int32.MaxValue || sleepTime <= 0) throw new ArgumentOutOfRangeException($"Argument out of range: {sleepTime}.");
                            break;
                        default:
                            throw new FormatException($"Unexpected reminder time unit: {remindTime}.");
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    await ReplyAsync($"Sorry, my memory isn't that good. Please keep reminders under 24 days' time.");
                    Console.WriteLine($"{ex.Message}");
                    throw;
                }
                catch (FormatException ex)
                {
                    await ReplyAsync($"{ex.Message}. Please use the format `;remindme <message>, #d #hr #m`.");
                    Console.WriteLine($"{ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"An unexpected exception has occurred - who programmed this thing? :skull:");
                    Console.WriteLine($"{ex.Message}");
                    throw;
                };
            }

            await ReplyAsync($"Ok {Context.Message.Author.Mention}, I'll remind you then!");

            // TODO: Create a subroutine which handles reminder system in a more intelligent way - for now, just sleep a thread for some reminder
            // Call subroutine to handle the reminder
            await ReplyAsync(Functions.HandleReminder(sleepTime, remindMessage, Context));
        }

        [Command("info")]
        [Alias("server", "serverinfo")]
        [Summary("Show server information.")]
        [RequireBotPermission(GuildPermission.EmbedLinks)] // Require the bot the have the 'Embed Links' permissions to execute this command.
        public async Task ServerEmbed()
        {
            double botPercentage = Math.Round(Context.Guild.Users.Count(x => x.IsBot) / Context.Guild.MemberCount * 100d, 2);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(0, 225, 225)
                .WithDescription(
                    $"🏷️\n**Guild name:** {Context.Guild.Name}\n" +
                    $"**Guild ID:** {Context.Guild.Id}\n" +
                    $"**Created at:** {Context.Guild.CreatedAt:dd/M/yyyy}\n" +
                    $"**Owner:** {Context.Guild.Owner}\n\n" +
                    $"💬\n" +
                    $"**Users:** {Context.Guild.MemberCount - Context.Guild.Users.Count(x => x.IsBot)}\n" +
                    $"**Bots:** {Context.Guild.Users.Count(x => x.IsBot)} [ {botPercentage}% ]\n" +
                    $"**Channels:** {Context.Guild.Channels.Count}\n" +
                    $"**Roles:** {Context.Guild.Roles.Count}\n" +
                    $"**Emotes: ** {Context.Guild.Emotes.Count}\n\n" +
                    $"🌎 **Region:** {Context.Guild.VoiceRegionId}\n\n")
                 .WithImageUrl(Context.Guild.IconUrl);

            await ReplyAsync($":information_source: Server info for **{Context.Guild.Name}**", embed: embed.Build());
        }

        [Command("role")]
        [Alias("roleinfo")]
        [Summary("Show information about a role.")]
        public async Task RoleInfo([Remainder]SocketRole role)
        {
            // Just in case someone tries to be funny.
            if (role.Id == Context.Guild.EveryoneRole.Id)
                return;

            await ReplyAsync(
                $":flower_playing_cards: **{role.Name}** information```ini" +
                $"\n[Members]             {role.Members.Count()}" +
                $"\n[Role ID]             {role.Id}" +
                $"\n[Hoisted status]      {role.IsHoisted}" +
                $"\n[Created at]          {role.CreatedAt:dd/M/yyyy}" +
                $"\n[Hierarchy position]  {role.Position}" +
                $"\n[Color Hex]           {role.Color}```");
        }

        // Please don't remove this command. I will appreciate it a lot <3
        [Command("source")]
        [Alias("sourcecode", "src")]
        [Summary("Link the source code used for this bot.")]
        public async Task Source()
            => await ReplyAsync($":heart: **{Context.Client.CurrentUser}** is based on this source code:\nhttps://github.com/VACEfron/Discord-Bot-Csharp");      
    }
}