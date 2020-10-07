using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace HangStudent
{
    class HangStudent
    {
        //================================== VARIABLES =================================================
        // string word = "";
        // List<char> guessed;
        // bool gameStarted = false;
        // int tries = 0;

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
            /*================================== BOT LOGIC HERE =================================================
             *
             * if msg == !start:
             *      while word < 6, pick random string from wordlist and assign it to word
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

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
