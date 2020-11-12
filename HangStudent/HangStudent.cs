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
            //string token = "NzU1MTk0NTU4MDYwNTYwNDI0.X1_vvQ.KfW_8mcgZ8R1SFDGr2WG0YX2U2Q";

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
                        //msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                        game.StartGames(msg.Channel, new string[]{ word }, 7);
                    }
                    else if (userMsg[1].Equals("c"))
                    {
                        if (userMsg.Count > 2)
                        {
                            word = wordgen.getChosen(msg, userMsg[2]);
                            //msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                            game.StartGames(msg.Channel, new string[] { word }, 7);
                        }
                        else
                        {
                            msg.Channel.SendMessageAsync("ERROR: Please include chosen word, hidden with spoiler tags ( || )");
                        }
                    }
                    else if (userMsg[1].Equals("q"))
                    {
                        if (userMsg.Count > 2)
                        {
                            word = wordgen.pickLink(msg, userMsg[2]);
                            //msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
                            game.StartGames(msg.Channel, new string[]{ word }, 7);
                        }

                        // All else statements below handle exceptions
                        else { msg.Channel.SendMessageAsync("ERROR: Please include quizlet link"); }
                    }
                    else { msg.Channel.SendMessageAsync("ERROR: Unrecognized game option"); }
                }
                else { msg.Channel.SendMessageAsync("ERROR: Please include game option (r, c, q)"); }
            }



            // Handles user guess
            else if (userMsg[0].Length == 1 && userMsg.Count == 1)
            {
                game.GameTurn(userMsg[0][0], msg.Author.Username);
            }
            // print out player and game stats
            else if (userMsg[0].Equals("!score")) {
                game.Scores(msg.Channel, msg.Author.Username);
            }
            // Lists commands for user
            else if (userMsg[0].Equals("!commands"))
            {
                msg.Channel.SendMessageAsync("```- !start r : Starts game with random word\n- !start c ||(word)|| : Starts game with chosen hidden word\n- !start q (link) : Starts game with random term from quizlet link\n- !score : Show a scoreboard and stats about past games```");
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
