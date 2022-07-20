using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public class CommonModules : ModuleBase<SocketCommandContext>
    {
        
        [Command("hello")] // Command name.
        [Summary("Say hello to the bot.")] // Command summary.
        public async Task Hello()
            => await ReplyAsync($"Hello there, **{Context.User.Username}**!");

        [Command("testburger")]
        [Summary("testburger")]
        public async Task Testburger()
        {
            string burgerPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data.json";
            string json = File.ReadAllText(burgerPath);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            int burgerCounter = jsonObject["testburger"];
            jsonObject["testburger"] = ++burgerCounter;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(burgerPath, output);
            await ReplyAsync($":hamburger: testburger counter: {burgerCounter} :hamburger:");
        }

        [Command("pick")]
        [Alias("choose")] // Aliases that will also trigger the command.
        [Summary("Pick between two or more different options. `pick <option 1>, <option 2>, <option 3>, ...`")]
        public async Task Pick([Remainder]string message = "")
        {
            string[] options = message.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string selection = options[new Random().Next(options.Length)];

            // ReplyAsync() is a shortcut for Context.Channel.SendMessageAsync()
            await ReplyAsync($"I choose **{selection.Trim()}**.");
        }

        [Command("roll")]
        [Summary("Roll the dice given a number of dice and max die value. `roll <number of dice>d<max dice value>`")]
        public async Task Roll([Remainder]string message = "")
        {
            Match stringMatch = Regex.Match(message, @"\b(?<numDice>\d*)(?i)d(?<valueDice>\d*)$");

            try
            {
                int numDice = int.Parse(stringMatch.Groups["numDice"].Value);
                int valueDice = int.Parse(stringMatch.Groups["valueDice"].Value);

                if (numDice < 1 || valueDice < 1)
                    throw new ArgumentOutOfRangeException($"numDice: {numDice}\t valueDice: {valueDice}");

                // Initialize result and list of rolls
                int result = 0;
                List<int> rolls = new List<int>();
                Random random = new Random();

                // Produce rolls up to max value
                for (int i = 0; i < numDice; i++)
                {
                    int thisRoll = random.Next(1, valueDice + 1);
                    rolls.Add(thisRoll);
                    result += thisRoll;
                }

                // Construct message embed
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.LightOrange,
                    Title = $":game_die: {numDice}d{valueDice} results",
                    Timestamp = DateTime.Now,
                };

                // Based on the number of rolls, description will be a sum e.g. "1 + 2 + 3 = **6**" or just one result "**1**"
                string description;
                if (rolls.Count > 1)
                    description = string.Join(" + ", rolls) + $" = **{result}**";
                else
                    description = $"**{result}**";

                embed.WithDescription(description);

                await ReplyAsync(embed: embed.Build());
            }
            catch (FormatException ex)
            {
                await ReplyAsync($"{ex.Message} Please use the format ;roll `<num1>`d`<num2>`.");
                Console.WriteLine($"{ex.Message}");
                throw;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await ReplyAsync($"Quantity and values of dice must be positive (and not above {int.MaxValue}).");
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

        [Command("avatar")]
        [Alias("getavatar")]
        [Summary("Get a user's avatar. `avatar <@User mention>`")]
        public async Task GetAvatar([Remainder] SocketGuildUser user = null)
            => await ReplyAsync($":frame_photo: **{(user ?? Context.User as SocketGuildUser).Username}**'s avatar\n{Functions.GetAvatarUrl(user)}");

        // Supports input of format ;remindme message, #d #hr #m
        // Zero or one of any of days, hours, or minutes are accepted and the comma is required after the message
        [Command("remindme")]
        [Alias("reminder")]
        [Summary("Set a reminder (currently up to 24 days while this command is a WIP). `remindme <message>, #d #hr #m`. Combinations of days, hours, or minutes are accepted.")]
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

            foreach (Capture capture in stringMatch.Groups["remindTime"].Captures)
            {
                string remindTime = capture.Value.ToLower();
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
            await ReplyAsync(message: $"{Context.Message.Author.Mention}",
                             embed: Functions.HandleReminder(sleepTime, remindMessage, Context).Build());
        }
    }
}
