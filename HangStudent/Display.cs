using System;
using System.IO;
using Discord.WebSocket;

namespace HangStudent {
	class Display {
		// the program needs to be run from the right directory
		private string IMAGES_DIRECTORY = Directory.GetCurrentDirectory()+"/img/";
		private string[] phasePaths;

		public void GameStart(GameInfo state) {
			SendMessage(ref state, "Some text", true);
		}

		public void CorrectGuess(ISocketMessageChannel channel) {
		}

		public void IncorrectGuess(ISocketMessageChannel channel) {
		}

		public void Win(ISocketMessageChannel channel) {
		}

		public void Loss(ISocketMessageChannel channel) {
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
		private string BuildWordLine(string word, char[] correctLetters, char[] wrongLetters) {
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
			for (int i = 0; i < wrongLetters.Length - 1; ++i) {
				ret += wrongLetters[i] + ", ";
			}
			ret += wrongLetters[wrongLetters.Length - 1] + ")";

			return ret;
		}

		private void SendMessage(ref GameInfo state, string text, bool showGame=false) {
			if (showGame) {
				state.channel.SendFileAsync(SelectImage(state.startTurns, state.turns),
						text+"\n"+BuildWordLine(state.word, state.correctLetters, state.wrongLetters));
			}
			else {
				state.channel.SendMessageAsync(text);
			}
		}

	}
}
