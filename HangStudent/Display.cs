using System;
using System.IO;
using System.Collections.Generic;
using Discord.WebSocket;

namespace HangStudent {
	class Display {
		//private string IMAGES_DIRECTORY = tmpPath.Substring(0, tmpPath.Length - 23) + "img/"; 
		private string IMAGES_DIRECTORY = Directory.GetCurrentDirectory()+"/img/";
		private string[] phasePaths;

		public Display() {
			// init hangman phase paths
			phasePaths = new string[10];
			for (int i = 0; i < 10; ++i) {
				phasePaths[i] = IMAGES_DIRECTORY + "phase"+(i+1)+".png";
			}
		}

		public void GameStart(GameInfo state, string wordDef=null) {
			string text = "New word!";
			if (wordDef != null) {
				// flashcard definition
				text += " The hint is:\n> "+wordDef;
			}
			SendMessage(ref state, text, true);
		}

		public void CorrectGuess(GameInfo state, char guess) {
			string text = "'**"+guess+"**' is **correct**!";
			SendMessage(ref state, text, true);
		}

		public void IncorrectGuess(GameInfo state, char guess) {
			string text = "'**"+guess+"**' is not in the word.";
			SendMessage(ref state, text, true);
		}

		public void Win(GameInfo state, char guess) {
			string text = "'**"+guess+"**' is **correct**! The word is \"**"+state.word+"**\".";
			SendMessage(ref state, text, false);
		}

		public void Loss(GameInfo state, char guess) {
			string text = "'**"+guess+"**' is not in the word. The word was \"**"+state.word+"**\".";
			SendMessage(ref state, text, false);
		}

		public void Scores(ISocketMessageChannel channel, Scoreboard scores, string username) {
			SendScores(channel, ref scores.GetStats(), username);
		}

		private string BuildGuessStatus(int startTurns, int turns) {
			string ret;
			if (turns == startTurns) {
				ret = "You have "+startTurns+" guesses.";
			}
			else {
				ret = turns+"/"+startTurns;
				if (turns == 1) {
					ret += " guess remains.";
				}
				else {
					ret += " guesses remain.";
				}
			}
			return ret;
		}

		private string SelectImage(int startTurns, int turns) {
			if (startTurns <= 7) {
				return phasePaths[7-turns];
			}
			// if 7 isn't enough, use the first 10 images instead
			else if (startTurns <= 10) {
				return phasePaths[10-turns];
			}
			// if there aren't enough images, repeat the first one until there are
			else {
				if (turns > 10)
					return phasePaths[0];
				else
					return phasePaths[10-turns];
			}
		}

		/* build the string of blanks and guessed letters */
		private string BuildWordStatus(string word, List<char> correctLetters, List<char> wrongLetters, string alphabet) {
			string ret = "**";

			/* blanks filled in with correct guesses */
			foreach (char c in word) {
				if (alphabet.Contains(c)) {
					bool found = false;
					foreach (char guess in correctLetters) {
						if (guess == c) {
							found = true;
							break;
						}
					}
					if (found) {
						ret += c + " "; // guessed letter
					}
					else {
						ret += "\\_ "; // unguessed letter
					}
				}
				else {
					ret += "**"+c+"** "; // unguessable character
				}
			}
			ret += "**";

			/* list of incorrect guesses */
			if (wrongLetters.Count != 0) {
				ret += " (";
				for (int i = 0; i < wrongLetters.Count - 1; ++i) {
					ret += wrongLetters[i] + ", ";
				}
				ret += wrongLetters[wrongLetters.Count - 1] + ")";
			}

			return ret;
		}

		private void SendMessage(ref GameInfo state, string text, bool showGame=false) {
			if (showGame) {
				string newText = text;
				newText += "\n"+BuildWordStatus(state.word, state.correctLetters, state.wrongLetters, state.alphabet);
				newText += "; "+BuildGuessStatus(state.initTurns, state.turns);
				state.channel.SendFileAsync(SelectImage(state.initTurns, state.turns), newText);
			}
			else {
				state.channel.SendMessageAsync(text);
			}
		}

		private void SendScores(ISocketMessageChannel channel, ref Stats stats, string username) {
			//Check for the player's name, if player exists then index will be >= 0, otherwise -1
			int userIndex = -1;
			foreach (PlayerStats user in stats.players) {
				if (user.name == username)
                {
					userIndex = stats.players.IndexOf(user);
                }
            }

            //If player exists, print player stats and total, if not just print total stats
            if (userIndex >= 0)
			{
				channel.SendMessageAsync("+++++++++++++++\n" + 
                    "**" + stats.players[userIndex].name + " Stats**\n" + "Correct: " + stats.players[userIndex].correct +
					"\nIncorrect: " + stats.players[userIndex].incorrect + "\nScore: " + stats.players[userIndex].score + "\n\n" +
					"**Total Stats**\n" + "Wins: " + stats.wins + "\nLosses: " + stats.losses +
					"\nRounds: " + stats.rounds + "\nBest Word: " + stats.bestWord +
                    "\n+++++++++++++++");
			}
			else
			{
				channel.SendMessageAsync("+++++++++++++++\n" + 
                    "**Total Stats**\n" + "Wins: " + stats.wins + "\nLosses: " + stats.losses +
					"\nRounds: " + stats.rounds + "\nBest Word: " + stats.bestWord +
                    "\n+++++++++++++++");
			}
        }
	}
}
