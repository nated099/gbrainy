/*
 * Copyright (C) 2008-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using Mono.Unix;
using System.Timers;
using System.ComponentModel;
using System.Xml.Serialization;

using gbrainy.Core.Views;
using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main
{
	static public class Score
	{
		/*
			How the game session is scored

			* Every game has a scoring algorithm that scores the player performance within the game.
		   	  This takes into account time used and tips (result is from 0 to 10)
			* The results are added to the games and scores arrays where we store the results for
			  the different game types (verbal, logic, etc)
			* We apply a ScoreFormula function that balances the total result with the number of
	  		  games played (is not the same 100% games won playing 2 than 10 games) and the difficulty
			
			The final result is a number from 0 to 100
		*/
		static public void UpdateSessionHistorycore (ref GameSessionHistoryExtended history, Game.Types type, Game.Difficulty difficulty, int game_score)
		{
			bool won;
			int components = 0;

			won = (game_score > 0 ? true : false);

			if (won == true) {
				history.GamesWon++;
			}

			switch (type) {
			case Game.Types.LogicPuzzle:
				history.LogicRawScore += game_score;
				history.LogicPlayed++;
				if (won) history.LogicWon++;
				history.LogicScore = ScoreFormula (ref history, type, difficulty);
				break;
			case Game.Types.MemoryTrainer:
				history.MemoryRawScore += game_score;
				history.MemoryPlayed++;
				if (won) history.MemoryWon++;
				history.MemoryScore = ScoreFormula (ref history, type, difficulty);
				break;
			case Game.Types.MathTrainer:
				history.MathRawScore += game_score;
				history.MathPlayed++;
				if (won) history.MathWon++;
				history.MathScore = ScoreFormula (ref history, type, difficulty);
				break;
			case Game.Types.VerbalAnalogy:
				history.VerbalRawScore += game_score;
				history.VerbalPlayed++;
				if (won) history.VerbalWon++;
				history.VerbalScore = ScoreFormula (ref history, type, difficulty);
				break;
			default:
				throw new InvalidOperationException ("Invalid switch value");
			}

			history.TotalScore = 0;

			// Updates total score taking only into account played game types
			if (history.LogicScore >= 0) {
				history.TotalScore += history.LogicScore;
				components++;
			}

			if (history.MemoryScore >= 0) {
				history.TotalScore += history.MemoryScore;
				components++;
			}

			if (history.MathScore >= 0) {
				history.TotalScore += history.MathScore;
				components++;
			}

			if (history.VerbalScore >= 0) {
				history.TotalScore += history.VerbalScore;
				components++;
			}

			history.TotalScore = history.TotalScore / components;
		}

		//
		// Applies scoring formula to the session
		//
		static int ScoreFormula (ref GameSessionHistoryExtended history, Game.Types type, Game.Difficulty difficulty)
		{
			int logbase, scored, played, won;
			double score, factor;

			switch (difficulty) {
			case Game.Difficulty.Easy:
				logbase = 10;
				break;
			case Game.Difficulty.Medium:
				logbase = 20;
				break;
			case Game.Difficulty.Master:
				logbase = 30;	
				break;
			default:
				throw new InvalidOperationException ("Invalid switch value");
			}

			switch (type) {
			case Game.Types.LogicPuzzle:
				scored = history.LogicRawScore; 
				played = history.LogicPlayed;
				won = history.LogicWon;
				break;
			case Game.Types.MemoryTrainer:
				scored = history.MemoryRawScore; 
				played = history.MemoryPlayed;
				won = history.MemoryWon;
				break;
			case Game.Types.MathTrainer:
				scored = history.MathRawScore; 
				played = history.MathPlayed;
				won = history.MathWon;
				break;
			case Game.Types.VerbalAnalogy:
				scored = history.VerbalRawScore; 
				played = history.VerbalPlayed;
				won = history.VerbalWon;
				break;
			default:
				throw new InvalidOperationException ("Invalid switch value");
			}

			// Simple percentage of games won vs played
			score = scored > 0 ? scored / played * 10 : 0;

			// Puts score of the game in prespective for the whole game
			factor = Math.Log (won + 2, logbase); // +2 to avoid log 0

			score = score * factor;

			if (score > 100) score = 100;

			return (int) score;
		}
	}
}
