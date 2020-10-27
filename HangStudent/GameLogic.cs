using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace HangStudent
{
    struct GameInfo
    {
        public ISocketMessageChannel channel;
        public string word;
        public int initTurns;
        public int turns;
        public List<char> correctLetters;
        public List<char> wrongLetters;
        public List<char> remainingLetters;

        public GameInfo(ISocketMessageChannel channel, string word, int turns, List<char> correctLetters, List<char> wrongLetters, List<char> remainingLetters)
        {
            this.channel = channel;
            this.word = word;
            this.initTurns = turns;
            this.turns = turns;
            this.correctLetters = correctLetters;
            this.wrongLetters = wrongLetters;
            this.remainingLetters = remainingLetters;
        }
    }

    public enum EngineState
    {
        Waiting, //idle, between games
        Switching, //between waiting and running, aka between rounds; in case of rapid input before the game is ready
        Running, //currently in a game
        Debug
    }

    internal class GameLogic
    {
        private Random randNum = new Random();
        private GameInfo info;
        private int turns;
        private const string alphabetStr = "abcdefghijklmnopqrstuvwxyz";
        private EngineState state = EngineState.Waiting;
        private ISocketMessageChannel channel;

        public GameLogic()
        {

        }

        public void StartGames(ISocketMessageChannel channel, string[] words, int turns)
        {
            if (this.state == EngineState.Waiting)
            {
                this.channel = channel;
                this.turns = turns;

                if (words.Length == 0)
                {
                    Console.WriteLine("Error - must have words in list!");
                }
                else
                {
                    this.state = EngineState.Switching;

                    foreach (string word in words)
                    {
                        this.Round(word);
                    }
                    this.state = EngineState.Waiting;
                }
            }
            else if (this.state == EngineState.Running)
            {
                Console.WriteLine("Error - currently running another game!");
            }
            else if (this.state == EngineState.Debug || this.state == EngineState.Switching)
            {
                Console.WriteLine("bro how");
            }
        }

        private void Round(string word)
        {
            this.info = new GameInfo(this.channel, word, this.turns, new List<char>(), new List<char>(), new List<char>(alphabetStr.ToCharArray()));
            this.state = EngineState.Running;
        }

        public GameInfo GameTurn(char guess)
        {
            GameInfo infoReturn = new GameInfo();
            if (this.state == EngineState.Running)
            {
                if (this.info.remainingLetters.Contains(guess))
                {
                    if (this.info.word.Contains(guess))
                    {
                        this.info.correctLetters.Add(guess);
                    }
                    else
                    {
                        this.info.wrongLetters.Add(guess);
                        this.info.turns -= 1;
                    }
                    this.info.remainingLetters.Remove(guess);

                    if (this.info.turns == 0)
                    {
                        this.state = EngineState.Switching;
                    }
                }
                infoReturn = this.info;
            }
            return infoReturn;
        }

        public EngineState RequestEngineState()
        {
            return this.state;
        }
    }
}
