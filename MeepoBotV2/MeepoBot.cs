using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeepoBotV2 {
    public class MeepoBot {

        private Random rand = new Random();
        private OpenDotaModule dota = new OpenDotaModule();
        private GiveawayModule giveaway = new GiveawayModule();
        private long curr = 0;

        Stopwatch stopwatch = new Stopwatch();

        static string token = "";

        public static void Main(string[] args)
            => new MeepoBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            var client = new DiscordSocketClient();

            // client.Log ...
            client.MessageReceived += MessageReceived;
            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            stopwatch.Start();

            while (true) {
                long delta = stopwatch.ElapsedMilliseconds - curr;
                curr = stopwatch.ElapsedMilliseconds;
                giveaway.updateTick(delta);
            }
            // Block this task until the program is closed.
            //await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message) {
            await dota.handleInput(message);
            await giveaway.handleInput(message);
            parseInput(message.ToString(), message);
        }

        private async void parseInput(string input, SocketMessage m) {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            if (command == "")
                return;
            else if (inputLen == 1) {
                if (command == Constants.COMMAND_HELP) {
                    await m.Channel.SendMessageAsync("Dylan's a soyboy 1 2 3");
                }
                else if (command == Constants.COMMAND_ID) {
                    await m.Channel.SendMessageAsync("" + m.Author.Id);
                }
                else if (commandRollDice(command)) {
                    string value = command.Remove(0, 2);
                    if (isValidDiceRoll(value)) {
                        int rollValue = getDiceRoll(value);
                        await m.Channel.SendMessageAsync(m.Author.Mention + " rolled a d" + value + " and got: " + rollValue);
                        return;
                    }
                    else {
                        await m.Channel.SendMessageAsync("USAGE: !d#, where # is between 1-" + Int32.MaxValue + ".");
                        return;
                    }
                }
            }
        }

        private bool commandRollDice(string command) {
            if (command[0] == '!') {
                if (Char.ToLower(command[1]) == 'd')
                    return true;
            }
            return false;
        }

        private bool isValidDiceRoll(string trimmedCommand) {
            int maxValue;
            if (Int32.TryParse(trimmedCommand, out maxValue)) {
                if (maxValue <= 0 || maxValue >= Int32.MaxValue)
                    return false;
                else
                    return true;
            }
            return false;
        }

        private int getDiceRoll(string trimmedCommand) {
            int maxValue;
            Int32.TryParse(trimmedCommand, out maxValue);
            return rand.Next(1, maxValue + 1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
