/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Calculation
{
	public class CalculationRatio : Game
	{
		int number_a, number_b, ratio_a, ratio_b;

		public override string Name {
			get {return Catalog.GetString ("Ratio");}
		}

		public override GameTypes Type {
			get { return GameTypes.MathTrainer;}
		}

		public override string Question {
			get {
				return String.Format (
					Catalog.GetString ("Two numbers sum {0} and have a ratio of {1} to {2}. Which are these numbers? Answer using two numbers (e.g.: 1 and 2)."), 
					number_a + number_b, ratio_a, ratio_b);
			}
		}

		public override string Rationale {
			get {
				return String.Format (Catalog.GetString ("The second number is calculated by multiplying the first by {0} and dividing it by {1}."),
					ratio_a, ratio_b);
			}
		}

		public override string Tip {
			get { return Catalog.GetString ("A ratio specifies a proportion between two numbers. A ratio a:b means that for every 'a' parts you have 'b' parts.");}
		}

		public override GameAnswerCheckAttributes CheckAttributes {
			get { return GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAll; }
		}

		public override string AnswerCheckExpression {
			get { return "[0-9]+"; }
		}

		public override string AnswerValue {
			get { return String.Format (Catalog.GetString ("{0} and {1}"), number_a, number_b); }
		}

		protected override void Initialize ()
		{
			int random_max;
		
			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				random_max = 5;
				break;
			default:
			case GameDifficulty.Medium:
				random_max = 8;
				break;
			case GameDifficulty.Master:
				random_max = 15;
				break;
			}

			number_a = 10 + random.Next (random_max);

			if (number_a % 2 !=0)
				number_a++;
		
			ratio_a = 2;
			ratio_b = 3 + random.Next (random_max);
			number_b = number_a / ratio_a * ratio_b;

			right_answer = String.Format ("{0} | {1}", number_a, number_b);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{	
			double x = DrawAreaX + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (x, DrawAreaY + 0.22);
			gr.ShowPangoText (String.Format (Catalog.GetString ("x + y = {0}"), number_a + number_b));
		
			gr.MoveTo (x, DrawAreaY + 0.44);
			gr.ShowPangoText (String.Format (Catalog.GetString ("have a ratio of {0}:{1}"), ratio_a, ratio_b));
		}
	}
}
