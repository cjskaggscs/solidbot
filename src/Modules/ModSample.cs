﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot
{
    public class ModSample : ModuleBase<SocketCommandContext>
    {
        //[Command("reloadconfig")]
        //[Alias("reload")]
        //[Summary("Reloads the config and applies changes")]
        //[RequireOwner] // Require the bot owner to execute the command successfully.
        //public async Task ReloadConfig()
        //{
        //    await Functions.SetBotStatusAsync(Context.Client);
        //    await ReplyAsync("Reloaded!");
        //}

        //[Command("kick")]
        //[Summary("Kick a user from the server.")]
        //[RequireBotPermission(GuildPermission.KickMembers)]
        //[RequireUserPermission(GuildPermission.KickMembers)]
        //public async Task Kick(SocketGuildUser targetUser, [Remainder]string reason = "No reason provided.")
        //{
        //    await targetUser.KickAsync(reason);
        //    await ReplyAsync($"**{targetUser}** has been kicked :wave:");
        //}

        //[Command("ban")]
        //[Summary("Ban a user from the server")]
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //public async Task Ban(SocketGuildUser targetUser, [Remainder]string reason = "No reason provided.")
        //{
        //    await Context.Guild.AddBanAsync(targetUser.Id, 0, reason);
        //    await ReplyAsync($"**{targetUser}** has been banned :wave:");
        //}

        //[Command("unban")]
        //[Summary("Unban a user from the server")]
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //public async Task Unban(ulong targetUser)
        //{
        //    await Context.Guild.RemoveBanAsync(targetUser);
        //    await Context.Channel.SendMessageAsync($"The user has been unbanned :clap:");
        //}

        //[Command("purge")]
        //[Summary("Bulk deletes messages in chat")]
        //[RequireOwner]
        //[RequireBotPermission(GuildPermission.ManageMessages)]
        //[RequireUserPermission(GuildPermission.ManageMessages)]
        //public async Task Purge(int delNumber)
        //{
        //    var channel = Context.Channel as SocketTextChannel;
        //    var items = await channel.GetMessagesAsync(delNumber + 1).FlattenAsync();
        //    await channel.DeleteMessagesAsync(items);
        //}
    }
}
