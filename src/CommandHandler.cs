﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Discord;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Discord_Bot
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {

            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Event handlers
            _client.Ready += ClientReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += SendJoinMessageAsync;
        }

        private async Task HandleCommandAsync(SocketMessage rawMessage)
        {
            if (rawMessage.Author.IsBot || !(rawMessage is SocketUserMessage message) || message.Channel is IDMChannel)
                return;

            var context = new SocketCommandContext(_client, message);

            int argPos = 0;

            JObject config = Functions.GetConfig();
            string[] prefixes = JsonConvert.DeserializeObject<string[]>(config["prefixes"].ToString());
            string[] specialCommands = JsonConvert.DeserializeObject<string[]>(config["special_commands"].ToString());

            // Check if message has any of the prefixes or mentions the bot.
            if (prefixes.Any(x => message.HasStringPrefix(x, ref argPos)) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) 
                             || specialCommands.Any(x => message.Content.ToLower().Contains(x)))
            {
                // Execute the command.
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                // If command result is unknown command, skip over - not a severe error
                if (!result.IsSuccess && result.Error.HasValue)
                    if (!(result.Error.Value is CommandError.UnknownCommand))
                        await context.Channel.SendMessageAsync($":x: {result.ErrorReason}");          
            }
            //else if (specialCommands.Any(x => message.Content.ToLower().Contains(x)))
            //{
            //    // TODO: If special commands with arguments are added later, then we can't scrap the args
            //    // TODO: For now, scrap the remainder of the message so that the function call operates

            //    foreach (string command in specialCommands)
            //    {
                    
            //    }
            //}
        }

        private async Task SendJoinMessageAsync(SocketGuild guild)
        {
            JObject config = Functions.GetConfig();
            string joinMessage = config["join_message"]?.Value<string>();

            if (string.IsNullOrEmpty(joinMessage))
                return;

            // Send the join message in the first channel where the bot can send messsages.
            foreach (var channel in guild.TextChannels.OrderBy(x => x.Position))
            {                               
                var botPerms = channel.GetPermissionOverwrite(_client.CurrentUser).GetValueOrDefault();

                if (botPerms.SendMessages == PermValue.Deny)
                    continue;

                try
                {
                    await channel.SendMessageAsync(joinMessage);
                    return;
                }
                catch 
                { 
                    continue;
                }
            }
        }

        private async Task ClientReadyAsync()
            => await Functions.SetBotStatusAsync(_client);

        public async Task InitializeAsync()
            => await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }
}