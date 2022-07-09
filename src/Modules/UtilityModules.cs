using System;
using System.Collections.Generic;
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

        [Command("help")]
        [Summary("Display the list of commands.")]
        public async Task Help()
        {
            List<CommandInfo> commands = Program.CommandService.Commands.ToList();
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.LightOrange;
            embed.Timestamp = DateTime.Now;

            foreach (CommandInfo command in commands)
            {
                string text = command.Summary ?? "No description available\n";
                embed.AddField(command.Name, text);
            }

            await ReplyAsync(embed: embed.Build());
        }

        //[Command("info")]
        //[Alias("server", "serverinfo")]
        //[Summary("Show server information.")]
        //[RequireBotPermission(GuildPermission.EmbedLinks)] // Require the bot the have the 'Embed Links' permissions to execute this command.
        //public async Task ServerEmbed()
        //{
        //    double botPercentage = Math.Round(Context.Guild.Users.Count(x => x.IsBot) / Context.Guild.MemberCount * 100d, 2);

        //    EmbedBuilder embed = new EmbedBuilder()
        //        .WithColor(0, 225, 225)
        //        .WithDescription(
        //            $"🏷️\n**Guild name:** {Context.Guild.Name}\n" +
        //            $"**Guild ID:** {Context.Guild.Id}\n" +
        //            $"**Created at:** {Context.Guild.CreatedAt:dd/M/yyyy}\n" +
        //            $"**Owner:** {Context.Guild.Owner}\n\n" +
        //            $"💬\n" +
        //            $"**Users:** {Context.Guild.MemberCount - Context.Guild.Users.Count(x => x.IsBot)}\n" +
        //            $"**Bots:** {Context.Guild.Users.Count(x => x.IsBot)} [ {botPercentage}% ]\n" +
        //            $"**Channels:** {Context.Guild.Channels.Count}\n" +
        //            $"**Roles:** {Context.Guild.Roles.Count}\n" +
        //            $"**Emotes: ** {Context.Guild.Emotes.Count}\n\n" +
        //            $"🌎 **Region:** {Context.Guild.VoiceRegionId}\n\n")
        //         .WithImageUrl(Context.Guild.IconUrl);

        //    await ReplyAsync($":information_source: Server info for **{Context.Guild.Name}**", embed: embed.Build());
        //}

        //[Command("role")]
        //[Alias("roleinfo")]
        //[Summary("Show information about a role.")]
        //public async Task RoleInfo([Remainder]SocketRole role)
        //{
        //    // Just in case someone tries to be funny.
        //    if (role.Id == Context.Guild.EveryoneRole.Id)
        //        return;

        //    await ReplyAsync(
        //        $":flower_playing_cards: **{role.Name}** information```ini" +
        //        $"\n[Members]             {role.Members.Count()}" +
        //        $"\n[Role ID]             {role.Id}" +
        //        $"\n[Hoisted status]      {role.IsHoisted}" +
        //        $"\n[Created at]          {role.CreatedAt:dd/M/yyyy}" +
        //        $"\n[Hierarchy position]  {role.Position}" +
        //        $"\n[Color Hex]           {role.Color}```");
        //}

        //// Please don't remove this command. I will appreciate it a lot <3
        //[Command("source")]
        //[Alias("sourcecode", "src")]
        //[Summary("Link the source code used for this bot.")]
        //public async Task Source()
        //    => await ReplyAsync($":heart: **{Context.Client.CurrentUser}** is based on this source code:\nhttps://github.com/VACEfron/Discord-Bot-Csharp");      
    }
}