using System;
using System.IO;
using System.Collections.Generic;
using Discord.WebSocket;

namespace HangStudent {
	class Display {
		// the program needs to be run from the right directory
		private string IMAGES_DIRECTORY = Directory.GetCurrentDirectory()+"/img/";
		private string[] phasePaths;

		public void GameStart(GameInfo state) {
            string text = "New word! You have " + state.initTurns + " guesses.";
			SendMessage(ref state, text, true);
		}

		public void CorrectGuess(GameInfo state, char guess) {
			string text = "'"+guess+"' is correct!";
			SendMessage(ref state, text, true);
		}

		public void IncorrectGuess(GameInfo state, char guess) {
			string text = "'"+guess+"' is not in the word.";
			SendMessage(ref state, text, true);
		}

		public void Win(GameInfo state, char guess) {
			string text = "'"+guess+"' is correct! The word is \""+state.word+"\".";
		}

		public void Loss(GameInfo state, char guess) {
			string text = "'"+guess+"' is not in the word. The word was \""+state.word+"\".";
		}

		private string BuildGuessStatus(int startTurns, int turns) {
			string ret;
			if (turns == startTurns) {
				ret = "You have "+startTurns+" guesses.";
			}
			else {
				ret = turns+"/"+startTurns+" ";
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
		private string BuildWordStatus(string word, List<char> correctLetters, List<char> wrongLetters) {
			string ret = "**";

			/* blanks filled in with correct guesses */
			foreach (char c in word) {
				bool found = false;
				foreach (char guess in correctLetters) {
					if (guess == c) {
						found = true;
						break;
					}
				}
				if (found) {
					ret += c + " ";
				}
				else {
					ret += "\\_ ";
				}
			}

			/* list of incorrect guesses */
			ret += "** (";
			for (int i = 0; i < wrongLetters.Count - 1; ++i) {
				ret += wrongLetters[i] + ", ";
			}
			ret += wrongLetters[wrongLetters.Count - 1] + ")";

			return ret;
		}

		private void SendMessage(ref GameInfo state, string text, bool showGame=false) {
			if (showGame) {
				string newText = text;
				newText += "\n"+BuildWordStatus(state.word, state.correctLetters, state.wrongLetters);
				newText += "; "+BuildGuessStatus(state.initTurns, state.turns);
				state.channel.SendFileAsync(SelectImage(state.initTurns, state.turns), newText);
			}
			else {
				state.channel.SendMessageAsync(text);
			}
		}

	}
}
