using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace HangStudent
{
    class HangStudent
    {
        //================================== VARIABLES =================================================
        private GameLogic game = new GameLogic();
        private WordSelect wordgen = new WordSelect();
        private string word;
        private SocketUser dmUser;
        private ISocketMessageChannel originalChannel;
        //==============================================================================================

        static void Main(string[] args)
        => new HangStudent().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageHandler;
            _client.Log += Log;

            string token = "NzYyODM0MDEzMDkzNzU2OTUy.X3u6iQ.BpIPlyoNUJFEXw_FQLrqu5hzMwE";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task MessageHandler(SocketMessage msg)
        {


            //======================================= Message Handling ==========================================

            List<string> userMsg = new List<string>(msg.Content.Split(' '));

            // Handles game start (selects word and sets up game info)
            if (userMsg[0].Equals("!start"))
            {
                if (userMsg.Count > 1)
                {
                    if (userMsg[1].Equals("r"))
                    {
                        word = wordgen.pickRandom();
                        msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                        game.StartGames(msg.Channel, new string[]{ word }, 7);
                    }
                    else if (userMsg[1].Equals("d"))
                    {
                        dmUser = msg.Author;
                        originalChannel = msg.Channel;
                        msg.Channel.SendMessageAsync("Waiting for word...");
                    }
                    else if (userMsg[1].Equals("q"))
                    {
                        if (userMsg.Count > 2)
                        {
                            word = wordgen.pickLink(msg, userMsg[2]);
                            msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                            game.StartGames(msg.Channel, new string[]{ word }, 7);
                        }

                        // All statements below handle exceptions
                        else
                        {
                            msg.Channel.SendMessageAsync("ERROR: Please include quizlet link");
                        }
                    }
                    else
                    {
                        msg.Channel.SendMessageAsync("ERROR: Unrecognized game option");
                    }
                }
                else
                {
                    msg.Channel.SendMessageAsync("ERROR: Please include game option (r, d, q)");
                }
            }


            // Handles user guess
            else if (userMsg[0].Length == 1 && userMsg.Count == 1)
            {
                game.GameTurn(userMsg[0][0]);
            }


            // Handles DM word selection
            else if (msg.Author == dmUser && userMsg.Count == 1)
            {
                word = userMsg[0];
                originalChannel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                game.StartGames(originalChannel, new string[]{ word }, 7);
            }


            // Lists commands for user
            else if (userMsg[0].Equals("!commands"))
            {
                msg.Channel.SendMessageAsync("```- !start r : Starts game with random word\n- !start d : Starts game with word sent through DM to me\n- !start q (link) : Starts game with random term from quizlet link```");
            }

            //===================================================================================================

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
