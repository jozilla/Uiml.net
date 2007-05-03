/*
 	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/projects/uiml.net/)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://www.edm.uhasselt.be)
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
	using System.Drawing;
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
					return DecodeColor(value);
				case "System.Drawing.Image":
					return System.Drawing.Image.FromFile((string)value);					
				case "System.String":
					return (System.String)value;
				case "System.String[]":
					return DecodeStringArray(oValue);				
				case "System.DateTime":
					return DecodeDateTime(value);
				case "System.Windows.Forms.Appearance":
					return DecodeAppearance(value);
				case "System.Windows.Forms.ScrollBars":
					return DecodeScrollBars(value);
				case "System.Windows.Forms.SelectionMode":
					return DecodeSelectionMode(value);
				case "System.Windows.Forms.View":
					return DecodeView(value);
				case "System.Windows.Forms.Orientation":
					return DecodeOrientation(value);
				case "System.Windows.Forms.TickStyle":
					return DecodeTickStyle(value);
                case "System.Windows.Forms.TabAlignment":
                    return DecodeTabAlignment(value);
                case "System.Drawing.Font":
                    return DecodeFont(value);
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
				case "System.Windows.Forms.ColumnHeader":
                    return DecodeColumnHeader(p.Value);
                case "System.Windows.Forms.ColumnHeader[]":
                    return DecodeColumnHeaderArray(p);
				case "System.Windows.Forms.ListViewItem":
					return DecodeListViewItem(p);
				case "System.Windows.Forms.ListViewItem[]":
					return DecodeListViewItemArray(p);
				case "System.Windows.Forms.TreeNode":
					return new System.Windows.Forms.TreeNode((string)p.Value);
				case "System.Windows.Forms.TreeNode[]":
					return DecodeTreeNodeArray(p);
				default:
					return p.Value;
			}			
		}

		private System.Object DecodeFont(string value)
		{
			string name = "Microsoft Sans Serif";
			float size = 8.25F;
			FontStyle style = FontStyle.Regular;
			GraphicsUnit unit = GraphicsUnit.Point;

			string[] font = value.Split(new Char[] {','});

			for(int i = 0; i < font.Length; i++)
			{
				string [] values = font[i].Split(new Char[] {'='});
			
				if( values[0].Trim().ToLower() == "name")
					name = values[1];
				else if (values[0].Trim().ToLower() == "size" )
					size = (float) Double.Parse(values[1]);
				else if(values[0].Trim().ToLower() == "style")
				{
					string [] styles = values[1].Split(new char[] {'|'});

					for (int j = 0; j < styles.Length; j++)
					{
						if(styles[j].Trim().ToLower() == "bold")
							style = style | FontStyle.Bold;
						else if(styles[j].Trim().ToLower() == "italic")
							style = style | FontStyle.Italic;
						else if(styles[j].Trim().ToLower() == "regular")
							style = style | FontStyle.Regular;
						else if(styles[j].Trim().ToLower() == "strikeout")
							style = style | FontStyle.Strikeout;
						else if(styles[j].Trim().ToLower() == "underline")
							style = style | FontStyle.Underline;
					}
				}
				else if (values[0].Trim().ToLower() == "unit")
				{
					if(values[1].Trim().ToLower() == "display")
						unit = GraphicsUnit.Display;
					else if(values[1].Trim().ToLower() == "document")
						unit = GraphicsUnit.Document;
					else if(values[1].Trim().ToLower() == "inch")
						unit = GraphicsUnit.Inch;
					else if(values[1].Trim().ToLower() == "millimeter")
						unit = GraphicsUnit.Millimeter;
					else if(values[1].Trim().ToLower() == "pixel")
						unit = GraphicsUnit.Pixel;
					else if(values[1].Trim().ToLower() == "point")
						unit = GraphicsUnit.Point;
					else if(values[1].Trim().ToLower() == "world")
						unit = GraphicsUnit.World;
				}
			}
			
			return new Font(name, size, style, unit);
		}
		private System.Object DecodeDateTime(string value)
		{
			string[] coords = value.Split(new Char[] {'/'});
			int month = int.Parse(coords[0]);
			int day = int.Parse(coords[1]);
			int year = int.Parse(coords[2]);
			return new DateTime(year, month, day);
		}

		private System.Object DecodeAppearance(string value)
		{
			if(value == "Button")
				return Appearance.Button;
			else
				return Appearance.Normal;
		}

		private System.Object DecodeScrollBars(string value)
		{
			if(value == "Both")
				return ScrollBars.Both;
			else if(value == "Horizontal")
				return ScrollBars.Horizontal;
			else if(value == "Vertical")
				return ScrollBars.Vertical;
			else
				return ScrollBars.None;
		}

		private System.Object DecodeSelectionMode(string value)
		{
			if(value == "MultiExtended")
				return SelectionMode.MultiExtended;
			else if(value == "MultiSimple")
				return SelectionMode.MultiSimple;
			else if(value == "None")
				return SelectionMode.None;
			else
				return SelectionMode.One;
		}

		private System.Object DecodeView(string value)
		{
			if(value == "LargeIcon")
				return View.LargeIcon;
			else if(value == "SmallIcon")
				return View.SmallIcon;
			else if(value == "List")
				return View.List;
			else
				return View.Details;
		}
		private System.Object DecodeOrientation(string value)
		{
			if(value == "Vertical")
				return Orientation.Vertical;
			else
				return Orientation.Horizontal;
		}

		private System.Object DecodeTickStyle(string value)
		{
			if(value == "Both")
				return TickStyle.Both;
			else if(value == "BottomRight")
				return TickStyle.BottomRight;
			else if(value == "None")
				return TickStyle.None;
			else
				return TickStyle.TopLeft;
		}

        private System.Object DecodeTabAlignment(string value)
        {
            switch (value.ToLower())
            {
                case "left":
                    return System.Windows.Forms.TabAlignment.Left;
                case "right":
                    return System.Windows.Forms.TabAlignment.Right;
                case "bottom":
                    return System.Windows.Forms.TabAlignment.Bottom;
                case "top":
                default:
                    return System.Windows.Forms.TabAlignment.Top;
            }
        }

		private System.Object DecodeListViewItem(Property p)
		{
			return DecodeListViewItem((string) p.Value);
		}

		private System.Object DecodeListViewItem(string s)
		{
			string[] vals = s.Split(new Char[] {';'});
			ListViewItem top = new ListViewItem(s);
			
			if (vals.Length > 0)
			{
				top = new ListViewItem(vals[0]);
				for (int i = 1; i < vals.Length; i++)
				{
					top.SubItems.Add(vals[i]);
				}
			}
			
			return top;
		}

		private System.Object DecodeListViewItemArray(Property p)
		{	
			string[] a = DecodeStringArray(p.Value);
			ListViewItem[] b = new ListViewItem[a.Length];
			for(int i = 0; i < a.Length; i++)
			{
				b[i] = (ListViewItem) DecodeListViewItem(a[i]);
			}

			return b;
		}

		private System.Object DecodeTreeNodeArray(Property p)
		{
			Constant top = (Constant) p.Value;
			TreeNode[] a = new TreeNode[top.ChildCount];

			int i = 0;
			foreach(Constant c in top.Children)
			{
				a[i] = (TreeNode)DecodeConstant(c);
				i++;
			}

			return a;
		}

		private System.Object DecodeConstant(Constant c)
		{
			TreeNode result = new TreeNode((string)c.Value);
			
			if(!c.HasChildren)
				return result;
						
			foreach(Constant child in c.Children)
			{
				result.Nodes.Add((TreeNode)DecodeConstant(child));
			}

			return result;
		}

		private System.Object DecodeColumnHeader(System.Object val)
		{
					ColumnHeader result = new ColumnHeader();
					result.Text = (string) val;
					return result;
		}

		private System.Object DecodeColumnHeaderArray(Property p)
		{
			string[] strHeaders = DecodeStringArray(p.Value);
			ColumnHeader[] headers = new ColumnHeader[strHeaders.Length];

			for (int i = 0; i < strHeaders.Length; i++)
			{
				headers[i] = (ColumnHeader) DecodeColumnHeader(strHeaders[i]); 
			}

			return headers;
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

		
		///<summary>
		///Decodes color from a string
		///</summary>
		///<param name="value">String containing the specification for a color</param>
		private System.Object DecodeColor(string value)
		{
			string[] coords = value.Split(new Char[] {','});
			if(coords.Length < 2)
				return DecodeKnownColor(value);
			return System.Drawing.Color.FromArgb(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]));
		}


		///<summary>
		///Decodes a color description into a System.Drawing.Color constant color
		/// Original source: MyXaml project, Bert Bier
		///</summary>
		///<param name="value">String containing the name of a color</param>
		private System.Object DecodeKnownColor(string value) 
		{
			switch (value.ToLower() )
			{
				case "aliceblue" : return System.Drawing.Color.AliceBlue ;
				case "antiquewhite" : return System.Drawing.Color.AntiqueWhite  ;
				case "aqua" : return System.Drawing.Color.Aqua  ;
				case "aquamarine" : return System.Drawing.Color.Aquamarine  ;
				case "azure" : return System.Drawing.Color.Azure ;
				case "beige" : return System.Drawing.Color.Beige  ;
				case "bisque" : return System.Drawing.Color.Bisque  ;
				case "black" : return System.Drawing.Color.Black  ;
				case "blanchedalmond" : return System.Drawing.Color.BlanchedAlmond  ;
				case "blue" : return System.Drawing.Color.Blue  ;
				case "blueviolet" : return System.Drawing.Color.BlueViolet ;
				case "brown" : return System.Drawing.Color.Brown ;
				case "burlywood" : return System.Drawing.Color.BurlyWood  ;
				case "cadetblue" : return System.Drawing.Color.CadetBlue  ;
				case "chartreuse" : return System.Drawing.Color.Chartreuse  ;
				case "chocolate" : return System.Drawing.Color.Chocolate  ;
				case "coral" : return System.Drawing.Color.Coral  ;
				case "cornflowerblue" : return System.Drawing.Color.CornflowerBlue  ;
				case "cornsilk" : return System.Drawing.Color.Cornsilk  ;
				case "crimson" : return System.Drawing.Color.Crimson  ;
				case "cyan" : return System.Drawing.Color.Cyan  ;
				case "darkblue" : return System.Drawing.Color.DarkBlue  ;
				case "darkcyan" : return System.Drawing.Color.DarkCyan  ;
				case "darkgoldenrod" : return System.Drawing.Color.DarkGoldenrod  ;
				case "darkgray" : return System.Drawing.Color.DarkGray  ;
				case "darkgreen" : return System.Drawing.Color.DarkGreen  ;
				case "darkkhaki" : return System.Drawing.Color.DarkKhaki  ;
				case "darkmagenta" : return System.Drawing.Color.DarkMagenta  ;
				case "darkolivegreen" : return System.Drawing.Color.DarkOliveGreen;
				case "darkorange" : return System.Drawing.Color.DarkOrange;
				case "darkorchid" : return System.Drawing.Color.DarkOrchid;
				case "darkred" : return System.Drawing.Color.DarkRed;
				case "darksalmon" : return System.Drawing.Color.DarkSalmon;
				case "darkseagreen" : return System.Drawing.Color.DarkSeaGreen;
				case "darkslateblue" : return System.Drawing.Color.DarkSlateBlue;
				case "darkslategray" : return System.Drawing.Color.DarkSlateGray;
				case "darkturquoise" : return System.Drawing.Color.DarkTurquoise;
				case "darkviolet" : return System.Drawing.Color.DarkViolet;
				case "deeppink" : return System.Drawing.Color.DeepPink;
				case "deepskyblue" : return System.Drawing.Color.DeepSkyBlue;
				case "dimgray" : return System.Drawing.Color.DimGray;
				case "dodgerblue" : return System.Drawing.Color.DodgerBlue;
				case "firebrick" : return System.Drawing.Color.Firebrick;
				case "floralwhite" : return System.Drawing.Color.FloralWhite;
				case "forestgreen" : return System.Drawing.Color.ForestGreen;
				case "fuchsia" : return System.Drawing.Color.Fuchsia;
				case "gainsboro" : return System.Drawing.Color.Gainsboro;
				case "ghostwhite" : return System.Drawing.Color.GhostWhite;
				case "gold" : return System.Drawing.Color.Gold;
				case "goldenrod" : return System.Drawing.Color.Goldenrod;
				case "gray" : return System.Drawing.Color.Gray;
				case "green" : return System.Drawing.Color.Green;
				case "greenyellow" : return System.Drawing.Color.GreenYellow;
				case "honeydew" : return System.Drawing.Color.Honeydew;
				case "hotpink" : return System.Drawing.Color.HotPink;
				case "indianred" : return System.Drawing.Color.IndianRed;
				case "indigo" : return System.Drawing.Color.Indigo;
				case "ivory" : return System.Drawing.Color.Ivory;
				case "khaki" : return System.Drawing.Color.Khaki;
				case "lavender" : return System.Drawing.Color.Lavender;
				case "lavenderblush" : return System.Drawing.Color.LavenderBlush;
				case "lawngreen" : return System.Drawing.Color.LawnGreen;
				case "lemonchiffon" : return System.Drawing.Color.LemonChiffon;
				case "lightblue" : return System.Drawing.Color.LightBlue;
				case "lightcoral" : return System.Drawing.Color.LightCoral;
				case "lightcyan" : return System.Drawing.Color.LightCyan;
				case "oldenrodyellow" : return System.Drawing.Color.LightGoldenrodYellow;
				case "lightgray" : return System.Drawing.Color.LightGray;
				case "lightgreen" : return System.Drawing.Color.LightGreen;
				case "lightpink" : return System.Drawing.Color.LightPink;
				case "lightsalmon" : return System.Drawing.Color.LightSalmon;
				case "lightseagreen" : return System.Drawing.Color.LightSeaGreen;
				case "lightskyblue" : return System.Drawing.Color.LightSkyBlue;
				case "lightslategray" : return System.Drawing.Color.LightSlateGray;
				case "lightsteelblue" : return System.Drawing.Color.LightSteelBlue;
				case "lightyellow" : return System.Drawing.Color.LightYellow;
				case "lime" : return System.Drawing.Color.Lime;
				case "limegreen" : return System.Drawing.Color.LimeGreen;
				case "linen" : return System.Drawing.Color.Linen;
				case "magentanta" : return System.Drawing.Color.Magenta;
				case "maroon" : return System.Drawing.Color.Maroon;
				case "mediumaquamarine" : return System.Drawing.Color.MediumAquamarine;
				case "mediumblue" : return System.Drawing.Color.MediumBlue;
				case "mediumorchid" : return System.Drawing.Color.MediumOrchid;
				case "mediumpurple" : return System.Drawing.Color.MediumPurple;
				case "mediumseagreen" : return System.Drawing.Color.MediumSeaGreen;
				case "mediumslateblue" : return System.Drawing.Color.MediumSlateBlue;
				case "miumspringgreen" : return System.Drawing.Color.MediumSpringGreen;
				case "mediumturquoise" : return System.Drawing.Color.MediumTurquoise;
				case "mediumvioletred" : return System.Drawing.Color.MediumVioletRed;
				case "midnightblue" : return System.Drawing.Color.MidnightBlue;
				case "mintcream" : return System.Drawing.Color.MintCream;
				case "mistyrose" : return System.Drawing.Color.MistyRose;
				case "moccasin" : return System.Drawing.Color.Moccasin;
				case "navajowhite" : return System.Drawing.Color.NavajoWhite;
				case "navy" : return System.Drawing.Color.Navy;
				case "oldlace" : return System.Drawing.Color.OldLace;
				case "olive" : return System.Drawing.Color.Olive;
				case "olivedrab" : return System.Drawing.Color.OliveDrab;
				case "orange" : return System.Drawing.Color.Orange;
				case "orangered" : return System.Drawing.Color.OrangeRed;
				case "orchid" : return System.Drawing.Color.Orchid;
				case "palegoldenrod" : return System.Drawing.Color.PaleGoldenrod;
				case "palegreen" : return System.Drawing.Color.PaleGreen;
				case "paleturquoise" : return System.Drawing.Color.PaleTurquoise;
				case "palevioletred" : return System.Drawing.Color.PaleVioletRed;
				case "papayawhip" : return System.Drawing.Color.PapayaWhip;
				case "peachpuff" : return System.Drawing.Color.PeachPuff;
				case "peru" : return System.Drawing.Color.Peru;
				case "pink" : return System.Drawing.Color.Pink;
				case "plum" : return System.Drawing.Color.Plum;
				case "powderblue" : return System.Drawing.Color.PowderBlue;
				case "purple" : return System.Drawing.Color.Purple;
				case "red" : return System.Drawing.Color.Red;
				case "rosybrown" : return System.Drawing.Color.RosyBrown;
				case "royalblue" : return System.Drawing.Color.RoyalBlue;
				case "saddlebrown" : return System.Drawing.Color.SaddleBrown;
				case "salmon" : return System.Drawing.Color.Salmon;
				case "sandybrown" : return System.Drawing.Color.SandyBrown;
				case "seagreen" : return System.Drawing.Color.SeaGreen;
				case "seashell" : return System.Drawing.Color.SeaShell;
				case "sienna" : return System.Drawing.Color.Sienna;
				case "silver" : return System.Drawing.Color.Silver;
				case "skyblue" : return System.Drawing.Color.SkyBlue;
				case "slateblue" : return System.Drawing.Color.SlateBlue;
				case "slategray" : return System.Drawing.Color.SlateGray;
				case "snow" : return System.Drawing.Color.Snow;
				case "springgreen" : return System.Drawing.Color.SpringGreen;
				case "steelblue" : return System.Drawing.Color.SteelBlue;
				case "tan" : return System.Drawing.Color.Tan;
				case "teal" : return System.Drawing.Color.Teal;
				case "thistle" : return System.Drawing.Color.Thistle;
				case "tomato" : return System.Drawing.Color.Tomato;
				case "transparent" : return System.Drawing.Color.Transparent;
				case "turquoise" : return System.Drawing.Color.Turquoise;
				case "violet" : return System.Drawing.Color.Violet;
				case "wheat" : return System.Drawing.Color.Wheat;
				case "white" : return System.Drawing.Color.White;
				case "whitesmoke" : return System.Drawing.Color.WhiteSmoke;
				case "yellow" : return System.Drawing.Color.Yellow;
				case "yellowgreen" : return System.Drawing.Color.YellowGreen;
				default : return System.Drawing.Color.Black ;
			}
		}

	}	
}

