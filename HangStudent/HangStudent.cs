using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace HangStudent
{
    class HangStudent
    {
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
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
