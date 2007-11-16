/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://edm.uhasselt.be)
								Hasselt University

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/


namespace Uiml.Rendering.GTKsharp
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Gtk;
	using GtkSharp;
	using Gdk;

	using GLib;

	using Type = System.Type;
		
	using Uiml;
	using Uiml.Rendering;
    using Uiml.Rendering.TypeDecoding;

	using Property = Uiml.Property;
	

	public class GtkTypeDecoders
	{
        [TypeDecoderMethod]
		public static Pango.FontDescription DecodeFont(string value)
		{
			return Pango.FontDescription.FromString(value);
		}

		///<summary>
		/// This method decodes a color from a string value. 
		///</summary>
		[TypeDecoderMethod]
		public static Gdk.Color DecodeColor(string value)
		{
			//try whether it is a color name
			Gdk.Color c = new Gdk.Color();
			if(Gdk.Color.Parse(value, ref c))
				return c;
			else
			{
				try
				{	
					byte red=0,green=0,blue=0;
					String[] splitted = value.Split(",".ToCharArray());
					red = Byte.Parse(splitted[0]);
					green = Byte.Parse(splitted[1]);
					blue = Byte.Parse(splitted[2]);
					return new Gdk.Color(red,green,blue);
				}
				catch(Exception e)
				{
					throw new InvalidTypeValueException(COLOR, value);
				}			
			}
		}

		///<summary>
		///Decodes the StateType value. This method only returns StateType.Normal; it should
		///be fixed for more complex behavior of widgets.
		///</summary>
		[TypeDecoderMethod]
		public static StateType DecodeStateType(string value)
		{
			switch(value)
			{
				case "background":
				case "foreground":
				case "base-color":
				default:
					return StateType.Normal;
			}
		}

        [TypeDecoderMethod]
        public static TreeViewColumn DecodeColumn(string s)
        {
            TreeViewColumn column = new TreeViewColumn();
            column.Title = s;

            return column;
        }

        [TypeDecoderMethod]
		public static GLib.List DecodeList(Constant c)
		{
			List list = new GLib.List((IntPtr) 0, typeof (System.String));
			IEnumerator enumConstants = c.Children.GetEnumerator();
			while(enumConstants.MoveNext())
			{
				Constant child = (Constant)enumConstants.Current;
				list.Append((string)child.Value);
			}
			return list;
		}

        [TypeDecoderMethod]
		public static Gtk.TreeModel DecodeTree(Constant c)
		{
			if(c.Model == Constant.LIST)
				return DecodeListStore(c);
			else
				return DecodeTreeStore(c);
		}

		private static Gtk.TreeModel DecodeTreeStore(Constant c)
		{
			TreeStore ts = new TreeStore(typeof(string), typeof(string));
			TreeIter parent = ts.AppendValues(c.Value);
			IEnumerator enumConst = c.Children.GetEnumerator();
			while(enumConst.MoveNext())
				FillTree(parent, (Constant)enumConst.Current, ref ts);
			return ts;
		}

		private static void FillTree(TreeIter it, Constant c, ref TreeStore ts)
		{
			TreeIter it2 = ts.AppendValues(it, c.Value);
			if(c.Children != null)
			{
				IEnumerator enumConst = c.Children.GetEnumerator();
				while(enumConst.MoveNext())
					FillTree(it2, (Constant)enumConst.Current, ref ts);
			}
		}

		private static Gtk.TreeModel DecodeListStore(Constant c)
		{
            ListStore ls = null;
            bool created = false;

			IEnumerator enumConst = (c.Children).GetEnumerator();
			while(enumConst.MoveNext())
			{
                Constant child = ((Constant)enumConst.Current);
                string[] values = ((string)child.Value).Split(';');

                if (!created) // if we didn't create the ListStore yet
                {
                    // create a new ListStore with the number of
                    // columns the first value provides 
                    Type[] columnTypes = new Type[values.Length];
                    for (int i = 0; i < values.Length; i++)
                        columnTypes[i] = typeof(string);
                    ls = new ListStore(columnTypes);

                    created = true; // don't do this again
                }

                ls.AppendValues(values);
            }

			return ls;
		}

		public static string COLOR = "Gdk.Color";
	}
}
