/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections.Generic;
using System.Reflection;

using gbrainy.Core.Main;

namespace gbrainy.Clients.Classical
{
	public class CommandLine
	{
		string [] args;
		int [] play_list;
		bool cont_execution;

		public CommandLine (string [] args)
		{
			this.args = args;
			RandomOrder = true;
		}

		public bool Continue {
			get { return cont_execution; }
		}

		public int [] PlayList {
			get { return play_list; }
		}

		public bool RandomOrder { get; set; }

		public void Parse ()
		{
			cont_execution = true;

			for (int idx = 0; idx < args.Length; idx++)
			{
				switch (args [idx]) {
				case "--customgame":
					string [] names = args [idx+1].Split (',');

					for (int i = 0; i < names.Length; i++)
						names[i] = names[i].Trim ();

					BuildPlayList (names);
					break;
				case "--gamelist":
					GameList ();
					cont_execution = false;
					break;
				case "--norandom":
					RandomOrder = false;
					break;
				case "--version":
					Version ();
					cont_execution = false;
					break;
				case "--versions":
					Versions ();
					cont_execution = false;
					break;
				case "--help":
				case "--usage":
					Usage ();
					cont_execution = false;
					break;
				default:
					Console.WriteLine ("Unknown parameter {0}", args [idx]);
					break;
				}
			}
		}

		static void GameList ()
		{
			GameManager.GameLocator [] games;
			GameManager gm = new GameManager ();
			gm.GameType = GameSession.Types.AllGames;
			games = gm.AvailableGames;

			Console.WriteLine (Catalog.GetString ("List of available games"));

			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.Variant = games[i].Variant;
				Console.WriteLine (" {0}", game.Name);
			}
		}

		void BuildPlayList (string [] names)
		{
			Dictionary <string, int> dictionary;
			GameManager.GameLocator [] games;
			GameManager gm = new GameManager ();
			gm.GameType = GameSession.Types.AllGames;
			games = gm.AvailableGames;

			// Create a hash to map from game name to locator
			dictionary = new Dictionary <string, int> (games.Length);
			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.Variant = games[i].Variant;
				dictionary.Add (game.Name, i);
			}

			List <int> list = new List <int> (names.Length);

			for (int i = 0; i < names.Length; i++)
			{
				try
				{
					list.Add (dictionary [names [i]]);
				}
				catch (KeyNotFoundException e)
				{
					Console.WriteLine ("gbrainy. Game [{0}] not found", names [i]);
				}
			}

			play_list = list.ToArray ();
		}

		static void Usage ()
		{
			string usage =
			        Catalog.GetString (
			                "Usage: gbrainy [options]\n" +
			                "  --version\t\t\tPrint version information.\n" +
			                "  --help\t\t\tPrint this usage message.\n" +
			                "  --gamelist\t\t\tShows the list of available games.\n" +
			                "  --customgame [game1, gameN]\tSpecifies a list of games to play during a custom game.\n" +
					"  --norandom \t\t\tThe custom game list provided will not be randomized.\n" +
			                "  --versions \t\t\tShow dependencies.\n");

			Console.WriteLine (usage);
		}

		static void Version ()
		{
			Console.WriteLine ("gbrainy " + Defines.VERSION);
		}

		static void Versions ()
		{
			Version ();
			Console.WriteLine ("Mono .NET Version: " + Environment.Version.ToString());
			Console.WriteLine (String.Format("{0}Assembly Version Information:", Environment.NewLine));

			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName name = asm.GetName();
				Console.WriteLine ("\t" + name.Name + " (" + name.Version.ToString () + ")");
			}
		}
	}
}
