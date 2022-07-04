using Discord.Commands;
using Discord.WebSocket;
using System;
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
            await ReplyAsync($"testburger counter: {burgerCounter}");
        }

        [Command("pick")]
        [Alias("choose")] // Aliases that will also trigger the command.
        [Summary("Pick something.")]
        public async Task Pick([Remainder]string message = "")
        {
            string[] options = message.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string selection = options[new Random().Next(options.Length)];

            // ReplyAsync() is a shortcut for Context.Channel.SendMessageAsync()
            await ReplyAsync($"I choose **{selection.Trim()}**.");
        }

        [Command("roll")]
        [Summary("Roll the dice.")]
        public async Task Roll([Remainder]string message = "")
        {
            Match stringMatch = Regex.Match(message, @"\b(?<numDice>\d*)(?i)d(?<valueDice>\d*)$");

            try
            {
                int numDice = int.Parse(stringMatch.Groups["numDice"].Value);
                int valueDice = int.Parse(stringMatch.Groups["valueDice"].Value);

                if (numDice < 1 || valueDice < 1)
                    throw new ArgumentOutOfRangeException($"numDice: {numDice}\t valueDice: {valueDice}");

                int result = 0;
                Random random = new Random();

                for (int i = 0; i < numDice; i++)
                {
                    result += random.Next(1, valueDice + 1);
                }

                await ReplyAsync($"**{result}**");
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

        [Command("amiadmin")]
        [Summary("Check your administrator status.")]
        public async Task AmIAdmin()
        {
            if ((Context.User as SocketGuildUser).GuildPermissions.Administrator)
                await ReplyAsync($"Yes, **{Context.User.Username}**, you're an admin!");
            else
                await ReplyAsync($"No, **{Context.User.Username}**, you're **not** an admin!");
        }
    }
}
