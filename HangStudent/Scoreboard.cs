using System;
using System.Collections.Generic;

namespace HangStudent {
	// these classes are basically c(++) structs because i don't know the right
	// way to pass around related mutable data in c#
	public class PlayerStats {
		public string name;
		public int correct;
		public int incorrect;
		public int score;
	}

	public class Stats {
		public List<PlayerStats> players;
		public int wins;
		public int losses;
		public int rounds;
		public string bestWord;
	}

	public class Scoreboard {
		private Stats stats;

		public Scoreboard() {
			stats = new Stats();
			stats.players = new List<PlayerStats>();
			stats.wins = 0;
			stats.losses = 0;
			stats.rounds = 0;
			string bestWord = "";
		}

		public void AddCorrect(string playerName) {
			PlayerStats p = GetPlayer(playerName);
			p.correct += 1;
			p.score += 1;
		}

		public void AddIncorrect(string playerName) {
			PlayerStats p = GetPlayer(playerName);
			p.incorrect += 1;
		}
		public void AddWin(string word) // update counters and best word
		{
			stats.wins += 1;
			stats.rounds += 1;
			if (stats.bestWord == null)
            {
				stats.bestWord = word;
            }
			else if (word.Length >= stats.bestWord.Length)
            {
				stats.bestWord = word;
            }
		}
		public void AddLoss() {
			stats.losses += 1;
			stats.rounds += 1;
		}
		public ref Stats GetStats() {
			return ref stats;
		}

		private PlayerStats GetPlayer(string name) {
			foreach (PlayerStats player in stats.players) {
				if (player.name == name) {
					return player;
				}
			}
			// add the new player
			PlayerStats p = new PlayerStats();
			p.name = name;
			p.correct = 0;
			p.incorrect = 0;
			p.score = 0;
			stats.players.Add(p);
			return p;
		}
	}
}
