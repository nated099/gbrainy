/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Memory
{
	public class MemoryIndications : Core.Main.Memory
	{
		class Indication 
		{
			public Type type;
			public object obj;

			public Indication (Type type, object obj)
			{
				this.type = type;
				this.obj = obj;
			}

			public enum Type
			{
				Start,
				Turn,
				End,
			}

			public enum TurnDirection
			{
				Right,
				Left,
				Down,
				Up
			}

			public void Draw (CairoContextEx gr, ref double x, ref double y, Indication next_prev)
			{
				const double line_length = 0.045;
				const double points = 0.045;

				if (type == Indication.Type.Start) {
					gr.Rectangle (x, y, points, points);
					gr.DrawTextCentered (x + points /2 , y + points /2, ((int)obj).ToString ());
					gr.Stroke ();

					switch ((TurnDirection) next_prev.obj) {
					case TurnDirection.Right:
						x += points;
						y += points / 2;
						break;
					case TurnDirection.Left:
						y += points / 2;
						break;
					case TurnDirection.Down:
						y += points;
						x += points / 2;
						break;
					case TurnDirection.Up:
						x += points / 2;
						break;
					}
			
				} else if (type == Indication.Type.Turn) {
					gr.MoveTo (x, y);
					switch ((TurnDirection) obj) {
					case TurnDirection.Right:
						x += line_length;
						break;
					case TurnDirection.Left:
						x -= line_length;
						break;
					case TurnDirection.Up:
						y -= line_length;
						break;
					case TurnDirection.Down:
						y += line_length;
						break;
					}

					gr.LineTo (x, y);
					gr.Stroke ();
				} else if (type == Indication.Type.End) {
					switch ((TurnDirection) next_prev.obj) {
					case TurnDirection.Right:
						y -= points / 2; 
						break;
					case TurnDirection.Left:
						x -= points;
						y -= points / 2;
						break;
					case TurnDirection.Down:
						x -= points / 2; 
						break;
					case TurnDirection.Up:
						x -= points / 2; 
						y -= points;
						break;
					}
					gr.Rectangle (x, y, points, points);
					gr.Stroke ();
					gr.DrawTextCentered (x + points /2 , y + points /2, ((int)obj).ToString ());
				}
			}	
	
			public override string ToString ()
			{
				switch (type) {
				case Indication.Type.Start:
					return String.Format (Catalog.GetString ("Start at point number {0}"), (int) obj);
				case Indication.Type.Turn: {
					switch ((TurnDirection) obj) {
					case TurnDirection.Right:
						return Catalog.GetString ("Move right");
					case TurnDirection.Left:
						return Catalog.GetString ("Move left");
					case TurnDirection.Up:
						return Catalog.GetString ("Move up");
					case TurnDirection.Down:
						return Catalog.GetString ("Move down");
					}
					break;
				}
				case Indication.Type.End:
					return String.Format (Catalog.GetString ("End at point {0}"), obj);
				}
				return null;
			}
		}

		private Indication[] indications;
		private Indication[] indications_wrongA;
		private Indication[] indications_wrongB;
		private Indication[] indications_wrongC;
		private ArrayListIndicesRandom answers;
		private int ans;

		public override string Name {
			get {return Catalog.GetString ("Memorize indications");}
		}

		public override string MemoryQuestion {
			get { 
				return String.Format (
					Catalog.GetString ("Which of the following graphics represent the indications previously given? Answer {0}, {1}, {2} or {3}."),
					GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3));}
		}

		protected override void Initialize ()
		{
			indications = new Indication [CurrentDifficulty == GameDifficulty.Easy ? 5 : 7];
			Indication.TurnDirection second_turn = (Indication.TurnDirection) 2 +  random.Next (2);
		
			indications[0] = new Indication (Indication.Type.Start, 0);
			indications[1] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left
			indications[2] = new Indication (Indication.Type.Turn, second_turn); // up or down
			indications[3] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left

			if (CurrentDifficulty==GameDifficulty.Easy) {
				indications[4] = new Indication (Indication.Type.End, 1);		
			} else {
				if (second_turn == Indication.TurnDirection.Up)
					indications[4] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Up);
				else
					indications[4] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Down);

				indications[5] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left
				indications[6] = new Indication (Indication.Type.End, 1);
			}
		
			indications_wrongA = CopyAnswer ();
			indications_wrongB = CopyAnswer ();
			indications_wrongC = CopyAnswer ();

			if ((Indication.TurnDirection) indications[3].obj == Indication.TurnDirection.Right) {
				indications_wrongA[3] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
			}
			else {
				indications_wrongA[3] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
			}

			if (CurrentDifficulty == GameDifficulty.Easy) {
				if ((Indication.TurnDirection) indications[2].obj == Indication.TurnDirection.Up) {
					indications_wrongB[2] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Down);
				}
				else {
					indications_wrongB[2] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Up);
				}
			} else {
				if ((Indication.TurnDirection) indications[5].obj == Indication.TurnDirection.Right) {
					indications_wrongB[5] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
				}
				else {
					indications_wrongB[5] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
				}
			}

			if ((Indication.TurnDirection) indications[1].obj == Indication.TurnDirection.Right) {
				indications_wrongC[1] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
			}
			else {
				indications_wrongC[1] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
			}
		
			base.Initialize ();

			answers = new ArrayListIndicesRandom (4);
			answers.Initialize ();

			for (int i = 0; i < answers.Count; i++) {
				if (answers [i] == 0) {
					right_answer = GetPossibleAnswer (i);
					ans = i;
					break;
				}
			}

			// Draw row 1
			HorizontalContainer container = new HorizontalContainer (0.05, 0.1, 0.9, 0.4);
			AddWidget (container);

			for (int i = 0; i  < 2; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.45, 0.4);
				container.AddChild (drawable_area);
				drawable_area.SelectedArea = new Rectangle (0, 0, 0.45, 0.3);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);
				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawPossibleAnswers (e.Context, 0.2, 0.1, WhichAnswer (answers[n]));
					e.Context.MoveTo (0.2, 0.12 + 0.2);
					e.Context.ShowPangoText (GetPossibleFigureAnswer (n));
				};
			}

			// Draw row 2
			container = new HorizontalContainer (0.05, 0.5, 0.9, 0.4);
			AddWidget (container);

			for (int i = 2; i  < 4; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.45, 0.4);
				container.AddChild (drawable_area);
				drawable_area.SelectedArea = new Rectangle (0, 0, 0.45, 0.3);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);
				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawPossibleAnswers (e.Context, 0.2, 0.1, WhichAnswer (answers[n]));
					e.Context.MoveTo (0.2, 0.12 + 0.2);
					e.Context.ShowPangoText (GetPossibleFigureAnswer (n));
				};
			}
		}

		private Indication[] CopyAnswer ()
		{
			Indication[] answer = new Indication [indications.Length];
			for (int i = 0; i < indications.Length; i++)
				answer[i] = new Indication (indications[i].type, indications[i].obj);

			return answer;
		}

		private static void DrawPossibleAnswers (CairoContextEx gr, double x, double y, Indication[] indications)
		{		
			for (int i = 0; i < indications.Length - 1; i++)
				indications[i].Draw (gr, ref x, ref y, indications[i + 1]);

			indications[indications.Length - 1].Draw (gr, ref x, ref y, indications[indications.Length - 2]);
		}

		private Indication[] WhichAnswer (object answer)
		{
			switch ((int) answer) {
			case 0:
				return indications;
			case 1:
				return indications_wrongA;
			case 2:
				return indications_wrongB;
			case 3:
				return indications_wrongC;
			}
			return null;
		}
	
		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);

			if (DrawAnswer == false) {
				for (int i = 0; i < indications.Length; i++)
				{
					gr.MoveTo (0.3, 0.2 + i * 0.08);
					gr.ShowPangoText (indications[i].ToString ());
					gr.Stroke ();
				}
			} else {
					for (int i = 0; i < indications.Length; i++)
					{
						gr.MoveTo (0.1, 0.2 + i * 0.08);
						gr.ShowPangoText (indications[i].ToString ());
						gr.Stroke ();
					}
					DrawPossibleAnswers (gr, 0.7, 0.3, WhichAnswer (answers[ans]));
					gr.MoveTo (0.7, 0.5);
					gr.ShowPangoText (GetPossibleFigureAnswer (ans));
					gr.Stroke ();
			}
		}
	}
}
