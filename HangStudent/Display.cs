using System;
using System.IO;
using Discord.WebSocket;

namespace HangStudent {
	class Display {
		// the program needs to be run from the right directory
		private string IMAGES_DIRECTORY = Directory.GetCurrentDirectory()+"/img/";
		private string[] phasePaths;

		public Display() {
			/* initialize images */
			phasePaths = new string[10];
			for (int i = 0; i < 10; ++i) {
				phasePaths[i] = IMAGES_DIRECTORY+"phase"+(i+1)+".png";
			}
		}

		public void GameStart(ISocketMessageChannel channel) {
			SendMessage(channel, "Game start! You get 6 guesses.\n\\_ \\_ \\_", phasePaths[0]);
		}

		public void CorrectGuess(ISocketMessageChannel channel) {
			SendMessage(channel, "'E' is correct! 6 guesses remain.\n\\_ E \\_", phasePaths[0]);
		}

		public void IncorrectGuess(ISocketMessageChannel channel) {
			SendMessage(channel, "'A' is incorrect. 5 guesses remain.\n\\_ \\_ \\_", phasePaths[1]);
		}

		public void Win(ISocketMessageChannel channel) {
			SendMessage(channel, "'E' is correct! The word is BET. Play again?");
		}

		public void Loss(ISocketMessageChannel channel) {
			SendMessage(channel, "'A' is incorrect. The word was BET. Play again?");
		}

		private void SendMessage(ISocketMessageChannel channel, string text, string imageUri=null) {
			if (imageUri != null) {
				channel.SendFileAsync(imageUri, text);
			}
			else {
				channel.SendMessageAsync(text);
			}
		}

	}
}
