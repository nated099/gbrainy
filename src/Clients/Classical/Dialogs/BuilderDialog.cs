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

using gbrainy.Core.Services;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class BuilderDialog : Gtk.Dialog
	{
		ITranslations translations;

		protected ITranslations Translations { get {return translations; }}

		public BuilderDialog (ITranslations translations, string resourceName, string dialogName) : 
			this ((System.Reflection.Assembly) null, resourceName, dialogName)
		{
			this.translations = translations;
		}

		public BuilderDialog (System.Reflection.Assembly assembly, string resourceName, string dialogName) : 
			this (new GtkBeans.Builder (assembly, resourceName, null),dialogName)
		{
		}

		public BuilderDialog (GtkBeans.Builder builder, string dialogName) : base (builder.GetRawObject (dialogName))
		{
			builder.Autoconnect (this);
			IconName = "gbrainy";
		}
	}
}
