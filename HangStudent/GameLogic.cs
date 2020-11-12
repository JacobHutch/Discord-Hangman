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
        public string alphabet;

        public GameInfo(ISocketMessageChannel channel, string word, int initTurns, int turns, List<char> correctLetters, List<char> wrongLetters, List<char> remainingLetters, string alphabet)
        {
            this.channel = channel;
            this.word = word;
            this.initTurns = turns;
            this.turns = turns;
            this.correctLetters = correctLetters;
            this.wrongLetters = wrongLetters;
            this.remainingLetters = remainingLetters;
            this.alphabet = alphabet;
        }
    }

    public enum EngineState
    {
        Waiting, //idle, between games
        Switching, //between waiting and running, aka between rounds; in case of rapid input before the game is ready
        Running, //currently in a game
        Debug
    }

    class GameLogic
    {
        Display display = new Display();
        private Random randNum = new Random();
        private GameInfo info;
        private int turns;
        private const string alphabetStr = "abcdefghijklmnopqrstuvwxyz";
        private EngineState state = EngineState.Waiting;
        private ISocketMessageChannel channel;

        public GameLogic()
        {

        }

        public void StartGames(ISocketMessageChannel channel, string[] words, int turns, string[] wordDefs=null)
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

                    for (int i = 0; i < words.Length; ++i)
                    {
                        if (wordDefs != null) {
                            // flashcard hint
                            this.Round(words[i], wordDefs[i]);
                        }
                        else {
                            this.Round(words[i]);
                        }
                    }
                    //this.state = EngineState.Waiting;
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

        private void Round(string word, string wordDef=null)
        {
            this.info = new GameInfo(this.channel, word, this.turns, this.turns, new List<char>(), new List<char>(), new List<char>(alphabetStr.ToCharArray()), alphabetStr);
            this.state = EngineState.Running;
            display.GameStart(this.info, wordDef);
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

                        bool wordComplete = true;
                        foreach (char c in info.word) {
                            if (alphabetStr.Contains(c)
                                    && !info.correctLetters.Contains(c)) {
                                wordComplete = false;
                                break;
                            }
                        }
                        if (!wordComplete)
                        {
                            display.CorrectGuess(this.info, guess);
                        }
                        else
                        {
                            display.Win(this.info, guess);
                            this.state = EngineState.Waiting;
                        }
                    }
                    else
                    {
                        this.info.wrongLetters.Add(guess);
                        this.info.turns -= 1;

                        if (info.turns > 0) {
                            display.IncorrectGuess(this.info, guess);
                        }
                        else
                        {
                            display.Loss(this.info, guess);
                            this.state = EngineState.Waiting;
                        }
                    }
                    this.info.remainingLetters.Remove(guess);
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
