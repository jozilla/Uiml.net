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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/


namespace Uiml.Rendering.CompactSWF
{
	using System;
	using System.Collections;
	using System.Reflection;

	using System.Windows.Forms;
	using System.Drawing;
	using Uiml;
	using Uiml.Rendering;

	public class CompactSWFTypeDecoder : TypeDecoder
	{

		public CompactSWFTypeDecoder()
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
					return new System.Drawing.Bitmap((string)value);
				case "System.String":
					return (System.String)value;
				case "System.String[]":
					return DecodeStringArray(oValue);				
				case "System.DateTime":
					return DecodeDateTime(value);
				case "System.Windows.Forms.ScrollBars":
					return DecodeScrollBars(value);
				case "System.Windows.Forms.View":
					return DecodeView(value);
				case "System.Windows.Forms.Orientation":
					return DecodeOrientation(value);
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
					ColumnHeader result = new ColumnHeader();
					result.Text = (string)p.Value;
					return result;
				case "System.Windows.Forms.ListViewItem":
					return new System.Windows.Forms.ListViewItem((string)p.Value);
				case "System.Windows.Forms.ListViewItem[]":
					return DecodeListViewItemArray(p);
				case "System.Windows.Forms.TreeNode":
					return new System.Windows.Forms.TreeNode((string)p.Value);
				case "System.Windows.Forms.TreeNode[]":
					return DecodeTreeNodeArray(p);
				case "System.Drawing.Font":
					return DecodeFont((string)p.Value);
				default:
					return p.Value;
			}			
		}

		///<summary>
		///Decodes a font from a given value
		///Original code from the MyXaml project, Bert Bier
		///</summary>
		///<param name="value">Contains the font information that has to be decoded</param>
		private System.Object DecodeFont(string value)
		{
			System.Drawing.Font c = new Font("MS Sans Serif", 10, FontStyle.Regular );
			string [] Fontparts;
			value = value.Replace(" ","");

			Fontparts = value.Split(",".ToCharArray());
			FontStyle fontstyle = FontStyle.Regular;
			int fontsize = 10;

			if (Fontparts.Length == 3) 
			{
				try 
				{
					Fontparts[1] = Fontparts[1].Replace("pt", "");
					fontsize = Convert.ToInt16(Fontparts[1]);
				}
				catch (Exception e) 
				{
				}
				try 
				{
					Fontparts[2] = Fontparts[2].Replace("style", "");
					Fontparts[2] = Fontparts[2].Replace("=", "");
				}
				catch (Exception e) 
				{
				}

				switch (Fontparts[2].ToLower()) 
				{	
					case "bold": fontstyle = FontStyle.Bold; break;
					case "italic": fontstyle = FontStyle.Italic; break;
					case "regular": fontstyle = FontStyle.Regular; break;
					case "strikeout": fontstyle = FontStyle.Strikeout; break;
					case "underline": fontstyle = FontStyle.Underline; break;
				}
				return new Font(Fontparts[0], fontsize, fontstyle);
			}
			else
			{
				return null;
			}
		}

		///<summary>
		///Decodes the border style from a given string. 
		///Original code from the MyXaml project, Bert Bier
		///</summary>
		///<param name="value">The string containing a description for a font</param>
		private System.Object DecodeFormBorderStyle(string value) 
		{
			switch (value.ToLower()) 
			{
				case "fixed3d": return System.Windows.Forms.FormBorderStyle.Fixed3D;
				case "fixeddialog": return System.Windows.Forms.FormBorderStyle.FixedDialog;
				case "fixedsingle": return System.Windows.Forms.FormBorderStyle.FixedSingle; 
				case "fixedTooltindow": return System.Windows.Forms.FormBorderStyle.FixedToolWindow; 
				case "none": return System.Windows.Forms.FormBorderStyle.None; 
				case "sizable": return System.Windows.Forms.FormBorderStyle.Sizable; 
				case "sizabletooltindow": return System.Windows.Forms.FormBorderStyle.SizableToolWindow; 
				default :
					return System.Windows.Forms.FormBorderStyle.Fixed3D; 

			}
		}

		private System.Object DecodeDateTime(string value)
		{
			string[] coords = value.Split(new Char[] {'/'});
			int month = int.Parse(coords[0]);
			int day = int.Parse(coords[1]);
			int year = int.Parse(coords[2]);
			return new DateTime(year, month, day);
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

		private System.Object DecodeListViewItemArray(Property p)
		{
			TreeView x = new TreeView();
			
			string[] a = DecodeStringArray(p.Value);
			ListViewItem[] b = new ListViewItem[a.Length];
			for(int i = 0; i < a.Length; i++)
			{
				b[i] = new ListViewItem(a[i]);
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

