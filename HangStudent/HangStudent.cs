using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace HangStudent
{
    class HangStudent
    {
        //================================== VARIABLES =================================================
        private string word = "";
        private List<char> guessed;
        private bool gameStarted = false;
        private int tries = 0;
        private Random rand = new Random();
        private static string tmpPath = Directory.GetCurrentDirectory();
        private string wordlistPath = tmpPath.Substring(0, tmpPath.Length - 23) + "wordlist.txt";
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
            /*====================================== PSEUDOCODE =================================================
             *
             * if msg == !start:
             *      while word.length < 6, pick random string from wordlist and assign it to word
             *      gameStarted = true
             *      tries = 6
             *      guessed = new List<char>()
             * 
             * else if gameStarted and msg is a single char:
             *      char guess = msg
             *      guessed.Add(guess)
             *      if word contains guess:
             *          send message showing progress so far, how many tries left
             *      else:
             *          send message showing progress, piece to hangman
             *          tries--
             *      
             *      if tries == 0:
             *          show game over and display the answer
             *          gameStarted = false
             *          word = ""
             *      else if all correct letters guessed:
             *          send message "you win!"
             *          gamestarted = false
             *          word = ""
             *
             *===================================================================================================
             */



            //======================================= BOT LOGIC HERE ============================================

            string userMsg = msg.Content;

            if (userMsg.Equals("!start"))
            {
                msg.Channel.SendMessageAsync("**(This should be a game start message)**");

                while (word.Length < 6)
                {
                    StreamReader file = new StreamReader(wordlistPath);

                    int val = rand.Next() % 10000;
                    for (int i = 1; i < val; i++)
                    {
                        file.ReadLine();
                    }

                    word = file.ReadLine();
                    word.Substring(0, word.Length - 1);

                    file.Close();
                }

                gameStarted = true;
                tries = 6;
                guessed = new List<char>();

                //msg.Channel.SendMessageAsync($@"(DEBUG) Word chosen: {word}");
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
