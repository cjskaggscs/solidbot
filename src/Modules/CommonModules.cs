using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
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
            string burgerPath = $"{Directory.GetCurrentDirectory()}/Data.json";
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
