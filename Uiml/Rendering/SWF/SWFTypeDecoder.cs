/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation; either version 2
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/


namespace Uiml.Rendering.SWF
{
	using System;
	using System.Collections;
	using System.Reflection;

	using System.Windows.Forms;
	using Uiml;
	using Uiml.Rendering;

	public class SWFTypeDecoder : TypeDecoder
	{

		public SWFTypeDecoder()
		{
		}

		public override System.Object[] GetArgs(Property p, Type[] types)
		{
		
			System.Object[] args = new System.Object[types.Length];
			
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
		/// Given an array of properties and an array of types, this method will create
		/// an array of objects by converting each property value (p[i].Value)
		/// into its appropriate type according to the Type array (types[i])
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

			string[] coords = null;
			// TODO: use reflection to create SWF types!
			switch(t.FullName)
			{
				case "System.Int32":
					return System.Int32.Parse(value);
				case "System.Int64":
					return System.Int64.Parse(value);
				case "System.Int16":
					return System.Int16.Parse(value);
				case "System.Drawing.Point":
					coords = value.Split(new Char[] {','});
					return new System.Drawing.Point(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
				case "System.Drawing.Size":
					coords = value.Split(new Char[] {','});
					return new System.Drawing.Size(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
				case "System.Drawing.Color":
					coords = value.Split(new Char[] {','});
					return System.Drawing.Color.FromArgb(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]));					
				case "System.Drawing.Image":
					return System.Drawing.Image.FromFile((string)value);					
				case "System.String":
					return (System.String)value;
				case "System.String[]":
					return DecodeStringArray(oValue);				
				case "System.DateTime":
					coords = value.Split(new Char[] {'/'});
					int month = int.Parse(coords[0]);
					int day = int.Parse(coords[1]);
					int year = int.Parse(coords[2]);
					return new DateTime(year, month, day);
				case "System.Windows.Forms.Appearance":
					if(value == "Button")
						return Appearance.Button;
					else
						return Appearance.Normal;
				case "System.Windows.Forms.ScrollBars":
					if(value == "Both")
						return ScrollBars.Both;
					else if(value == "Horizontal")
						return ScrollBars.Horizontal;
					else if(value == "Vertical")
						return ScrollBars.Vertical;
					else
						return ScrollBars.None;
				default:
					return value;
			}			
		}

		protected override System.Object ConvertComplex(Type t, Property p)
		{
			switch(t.FullName)
			{
				case "System.String[]":
					return DecodeStringArray(p.Value);
				default:
					return p.Value;
			}			
		}

		private System.Object DecodeStateType(string value)
		{
			return value;
			/*
			switch(value)
			{
				case "background" :
				case "foreground" :
				case "base-color" :
					return StateType.Normal;
				default:
					return value;
			}
			*/
		}

		/*
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
		*/

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

		/*
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
		*/
	}	
}

