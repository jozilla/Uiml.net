/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

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
	

	public class GtkTypeDecoder : TypeDecoder
	{

		public GtkTypeDecoder()
		{
		}

		public override System.Object[] GetArgs(Property p, Type[] types)
		{
			System.Object[] args= new System.Object[types.Length];
			int i = 0;
			foreach(Type t in types)
			{
				if(t.IsPrimitive)
					args[i] = ConvertPrimitive(t, p);
				else
					args[i] = ConvertComplex(t, p);
				i++;
			}
			return args;
		}
	
		///<summary>
		///Given an array of properties and an array of types, this method will create
		/// an array of objects by converting each property value (p[i].Value)
		/// into its appropriate type accoriding to the Type array (types[i])
		///</summary>
		public override System.Object[] GetMultipleArgs(Property[] p, Type[] types)
		{
			System.Object[] args= new System.Object[types.Length];
			int i = 0;
			foreach(Type t in types)
			{
				if(t.IsPrimitive)
					args[i] = ConvertPrimitive(t, p[i]);
				else
					args[i] = ConvertComplex(t, p[i]);
				i++;
			}
			return args;

		}


		public override System.Object GetArg(System.Object o, Type t)
		{
			if(t.IsPrimitive)
			{
				return ConvertPrimitive(t, o);
			}
			else
			{
				return ConvertComplex(t, o);
			}
		}

		///<summary>
		/// Converts the object oValue to the type given by t
		///</summary>
		protected override System.Object ConvertComplex(Type t, System.Object oValue)
		{
			string value = "";
			if(oValue is string)
				value = (string)oValue;
			else if(t.FullName == "System.String")
				return oValue.ToString();
		

			switch(t.FullName)
			{
				case "System.Int32":
					return System.Int32.Parse(value);
				case "System.Int64":
					return System.Int64.Parse(value);
				case "System.Int16":
					return System.Int16.Parse(value);
				case "Gdk.Color":
					if(oValue.GetType().FullName == "Gdk.Color")
						return oValue;
					return DecodeColor(value);
				case "Gtk.StateType":
					return DecodeStateType(value);
				case "GLib.List":
					return DecodeList(oValue);
				case "Gtk.TreeModel":
					return DecodeTree(oValue);
				case "System.String":
					return (System.String)value;
				case "System.String[]":
					return DecodeStringArray(oValue);
				default:
					return value;
			}
		}

		protected override System.Object ConvertComplex(Type t, Property p)
		{
			switch(t.FullName)
			{
				case "Gdk.Color":
					if(p.Value.GetType().FullName == "Gdk.Color"	)
						return p.Value;
					return DecodeColor((System.String)p.Value);
				case "Gtk.StateType":
					return DecodeStateType(p.Name);
				case "Pango.FontDescription":
					return DecodeFont((System.String)p.Value);
				case "GLib.List":
					return DecodeList(p.Value);
				case "System.String[]":
					return DecodeStringArray(p.Value);
				default:
					return p.Value;
			}
		}


		private System.Object DecodeFont(string value)
		{
			return Pango.FontDescription.FromString(value);
		}

		///<summary>
		/// This method decodes a color from a string value. Is this the right way for implementing
		/// Gtk Type conversions?
		///</summary>
		private Gdk.Color DecodeColor(string value)
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
		

		private System.Object DecodeStateType(string value)
		{
			switch(value)
			{
				case "background" :
				case "foreground" :
				case "base-color" :
					return StateType.Normal;
				default:
					return value;
			}
		}

		private GLib.List DecodeList(System.Object value)
		{
			List list = new GLib.List((IntPtr) 0, typeof (System.String));
			IEnumerator enumConstants = (((Constant)value).Children).GetEnumerator();	
			while(enumConstants.MoveNext())
			{
				Constant c = (Constant)enumConstants.Current;
				list.Append((String)c.Value);
			}
			return list;
		}

		private System.String[] DecodeStringArray(System.Object value)
		{
			ArrayList strArrayList = new ArrayList();
			IEnumerator enumConstants = (((Constant)value).Children).GetEnumerator();	
			while(enumConstants.MoveNext())
			{
				Constant c = (Constant)enumConstants.Current;
				strArrayList.Add(c.Value);
			}
			return (System.String[])(strArrayList.ToArray(Type.GetType("System.String")));
		}


		private Gtk.TreeModel DecodeTree(System.Object value)
		{
			Constant c = (Constant)value;
			if(c.Model == Constant.LIST)
				return DecodeListStore(c);
			else
				return DecodeTreeStore(c);
		}

		private Gtk.TreeModel DecodeTreeStore(Constant c)
		{
			TreeStore ts = new TreeStore(typeof(string), typeof(string));
			TreeIter parent = ts.AppendValues(c.Value);
			IEnumerator enumConst = c.Children.GetEnumerator();
			while(enumConst.MoveNext())
				FillTree(parent, (Constant)enumConst.Current, ref ts);
			return ts;
		}

		private void FillTree(TreeIter it, Constant c, ref TreeStore ts)
		{
			TreeIter it2 = ts.AppendValues(it, c.Value);
			if(c.Children != null)
			{
				IEnumerator enumConst = c.Children.GetEnumerator();
				while(enumConst.MoveNext())
					FillTree(it2, (Constant)enumConst.Current, ref ts);
			}
		}

		private Gtk.TreeModel DecodeListStore(Constant c)
		{
			ListStore ls = new ListStore(typeof(string));			
			IEnumerator enumConst = (c.Children).GetEnumerator();
			while(enumConst.MoveNext())
			{
				ls.AppendValues(((Constant)enumConst.Current).Value);
			}
			return ls;
		}


		public static string COLOR = "Gdk.Color";
	}
}
