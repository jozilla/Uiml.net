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
    using System.Net;
    using System.IO;
    using System.Globalization;

	using System.Windows.Forms;
	using System.Drawing;
	using Uiml;
	using Uiml.Rendering;
    using Uiml.Rendering.TypeDecoding;

	public class SWFTypeDecoders
	{	    
        [TypeDecoderMethod]
        public static System.Drawing.Point DecodePoint(string val)
        {
            string[] coords = val.Split(new Char[] {','});
            return new System.Drawing.Point(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
        }

        [TypeDecoderMethod]
        public static string DecodePointInverse(Point p)
        {
            return string.Format("{0},{1}", p.X, p.Y);
        }

        [TypeDecoderMethod]
        public static System.Drawing.Size DecodeSize(string val)
        {
			string[] coords = val.Split(new Char[] {','});
			return new System.Drawing.Size(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
        }

        [TypeDecoderMethod]
        public static string DecodeSizeInverse(Size s)
        {
            return string.Format("{0},{1}", s.Width, s.Height);
        }

        [TypeDecoderMethod]
        public static System.Drawing.Image DecodeImage(string file)
        {
            try
            {
                if (file.StartsWith("http://"))
                {
                    // load image from the web
                    Stream ImageStream = new WebClient().OpenRead(file);
                    return Image.FromStream(ImageStream);
                }

                return System.Drawing.Image.FromFile((string) file);
            }
            catch
            {
                // loading the image failed
                Console.WriteLine("Could not load image from path '{0}'", file);
                return null;
            }
        }

   		[TypeDecoderMethod]
		public static System.Drawing.Font DecodeFont(string value)
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
                else if (values[0].Trim().ToLower() == "size")
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberGroupSeparator = ".";
                    size = (float)Double.Parse(values[1], nfi);
                }
                else if (values[0].Trim().ToLower() == "style")
                {
                    string[] styles = values[1].Split(new char[] { '|' });

                    for (int j = 0; j < styles.Length; j++)
                    {
                        if (styles[j].Trim().ToLower() == "bold")
                            style = style | FontStyle.Bold;
                        else if (styles[j].Trim().ToLower() == "italic")
                            style = style | FontStyle.Italic;
                        else if (styles[j].Trim().ToLower() == "regular")
                            style = style | FontStyle.Regular;
                        else if (styles[j].Trim().ToLower() == "strikeout")
                            style = style | FontStyle.Strikeout;
                        else if (styles[j].Trim().ToLower() == "underline")
                            style = style | FontStyle.Underline;
                    }
                }
                else if (values[0].Trim().ToLower() == "unit")
                {
                    if (values[1].Trim().ToLower() == "display")
                        unit = GraphicsUnit.Display;
                    else if (values[1].Trim().ToLower() == "document")
                        unit = GraphicsUnit.Document;
                    else if (values[1].Trim().ToLower() == "inch")
                        unit = GraphicsUnit.Inch;
                    else if (values[1].Trim().ToLower() == "millimeter")
                        unit = GraphicsUnit.Millimeter;
                    else if (values[1].Trim().ToLower() == "pixel")
                        unit = GraphicsUnit.Pixel;
                    else if (values[1].Trim().ToLower() == "point")
                        unit = GraphicsUnit.Point;
                    else if (values[1].Trim().ToLower() == "world")
                        unit = GraphicsUnit.World;
                }
			}
			
			return new Font(name, size, style, unit);
		}

        [TypeDecoderMethod]
        public static string DecodeFontInverse(Font f)
        {
            return string.Format("Name={0},Size={1},Style={2},Unit={3}", f.Name, f.Size.ToString().Replace(',','.'), f.Style.ToString().Replace(',', '|'), f.Unit);
        }

   		[TypeDecoderMethod]		
		public static DateTime DecodeDateTime(string value)
		{
			string[] coords = value.Split(new Char[] {'/'});
			int month = int.Parse(coords[0]);
			int day = int.Parse(coords[1]);
			int year = int.Parse(coords[2]);
			return new DateTime(year, month, day);
		}

        [TypeDecoderMethod]
        public static string DecodeDateTimeInverse(DateTime dt)
        {
            return string.Format("{0}/{1}/{2}", dt.Month, dt.Day, dt.Year);
        }
		
   		[TypeDecoderMethod]
		public static Appearance DecodeAppearance(string value)
		{
			if(value == "Button")
				return Appearance.Button;
			else
				return Appearance.Normal;
		}

        [TypeDecoderMethod]
        public static PictureBoxSizeMode DecodePictureBoxSizeMode(string value)
        {
            switch (value)
            {
                case "StretchImage":
                    return PictureBoxSizeMode.StretchImage;
                case "AutoSize":
                    return PictureBoxSizeMode.AutoSize;
                case "CenterImage":
                    return PictureBoxSizeMode.CenterImage;
                case "Zoom":
                    return PictureBoxSizeMode.Zoom;
                case "Normal":
                default:
                    return PictureBoxSizeMode.Normal;
            }
        }

        public static string DecodePictureBoxSizeModeInverse(PictureBoxSizeMode m)
        {
            return m.ToString();
        }

        [TypeDecoderMethod]
        public static string DecodeAppearanceInverse(Appearance a)
        {
            return a.ToString();
        }

   		[TypeDecoderMethod]
		public static ScrollBars DecodeScrollBars(string value)
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

        [TypeDecoderMethod]
        public static string DecodeScrollBarsInverse(ScrollBars s)
        {
            return s.ToString();
        }

   		[TypeDecoderMethod]
		public static SelectionMode DecodeSelectionMode(string value)
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

        [TypeDecoderMethod]
        public static string DecodeSelectionModeInverse(SelectionMode sel)
        {
            return sel.ToString();
        }

   		[TypeDecoderMethod]
		public static View DecodeView(string value)
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

        [TypeDecoderMethod]
        public static string DecodeViewInverse(View v)
        {
            return v.ToString();
        }

   		[TypeDecoderMethod]		
		public static Orientation DecodeOrientation(string value)
		{
			if(value == "Vertical")
				return Orientation.Vertical;
			else
				return Orientation.Horizontal;
		}

        [TypeDecoderMethod]
        public static string DecodeOrientationInverse(Orientation o)
        {
            return o.ToString();
        }

   		[TypeDecoderMethod]
		public static TickStyle DecodeTickStyle(string value)
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

        [TypeDecoderMethod]
        public static string DecodeTickStyleInverse(TickStyle t)
        {
            return t.ToString();
        }

   		[TypeDecoderMethod]
        public static TabAlignment DecodeTabAlignment(string value)
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

        [TypeDecoderMethod]
        public static string DecodeTabAlignmentInverse(TabAlignment t)
        {
            return t.ToString();
        }

        [TypeDecoderMethod]
        public static ImageLayout DecodeImageLayout(string value)
        {
            switch (value.ToLower())
            {
                case "none":
                    return ImageLayout.None;
                case "center":
                    return ImageLayout.Center;
                case "stretch":
                    return ImageLayout.Stretch;
                case "zoom":
                    return ImageLayout.Zoom;
                case "tile":
                default:
                    return ImageLayout.Tile;
            }
        }

        [TypeDecoderMethod]
        public static string DecodeImageLayoutInverse(ImageLayout il)
        {
            return il.ToString();
        }

   		[TypeDecoderMethod]
		public static ListViewItem DecodeListViewItem(string s)
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

        [TypeDecoderMethod]
        public static string DecodeListViewItemInverse(ListViewItem item)
        {
            string str = string.Empty;

            if (item.SubItems.Count > 0)
            {
                int i = 0;
                foreach (ListViewItem.ListViewSubItem child in item.SubItems)
                {
                    str += child.Text;

                    if (i != item.SubItems.Count - 1)
                        str += ";";

                    i++;
                }
            }
            else
                str = item.Text;

            return str;
        }

        // Depends on Constant <=> string[] decoder method
   		[TypeDecoderMethod(new Type[] {typeof(Constant), typeof(string[])})]
		public static ListViewItem[] DecodeListViewItemArray(Constant c)
		{
			string[] a = (string[]) TypeDecoder.Instance.GetArg(c, typeof(string[]));
			
			ListViewItem[] b = new ListViewItem[a.Length];
			for(int i = 0; i < a.Length; i++)
			{
				b[i] = (ListViewItem) DecodeListViewItem(a[i]);
			}

			return b;
		}

        [TypeDecoderMethod]
        public static ListViewItem[] DecodeSelectedListViewItemCollection(ListView.SelectedListViewItemCollection selList)
        {
            ListViewItem[] list = new ListViewItem[selList.Count];

            for (int i = 0; i < selList.Count; i++)
            {
                // a ListViewItem can only be in one list => clone
                list[i] = (ListViewItem) selList[i].Clone();
            }

            return list;
        }

   		[TypeDecoderMethod]
		public static TreeNode[] DecodeTreeNodeArray(Constant c)
		{
			TreeNode[] a = new TreeNode[c.ChildCount];

			int i = 0;
			foreach(Constant child in c.Children)
			{
				a[i] = DecodeTreeNode(child);
				i++;
			}

			return a;
		}

   		[TypeDecoderMethod]
		public static TreeNode DecodeTreeNode(Constant c)
		{
			TreeNode result = new TreeNode((string)c.Value);
			
			if(!c.HasChildren)
				return result;
						
			foreach(Constant child in c.Children)
			{
				result.Nodes.Add(DecodeTreeNode(child));
			}

			return result;
		}

   		[TypeDecoderMethod]
		public static ColumnHeader DecodeColumnHeader(string val)
		{
			ColumnHeader result = new ColumnHeader();
			result.Text = val;
			return result;
		}

        // Depends on Constant <=> string[] decoder method
   		[TypeDecoderMethod(new Type[] {typeof(Constant), typeof(string[])})]
		public static ColumnHeader[] DecodeColumnHeaderArray(Constant c)
		{
			string[] strHeaders = (string[]) TypeDecoder.Instance.GetArg(c, typeof(string[]));
			ColumnHeader[] headers = new ColumnHeader[strHeaders.Length];

			for (int i = 0; i < strHeaders.Length; i++)
			{
				headers[i] = (ColumnHeader) DecodeColumnHeader(strHeaders[i]); 
			}

			return headers;
		}
		
		///<summary>
		///Decodes color from a string
		///</summary>
		///<param name="value">String containing the specification for a color</param>
		[TypeDecoderMethod]
		public static System.Drawing.Color DecodeColor(string value)
		{
			string[] coords = value.Split(new Char[] {','});
			if(coords.Length < 2)
				return DecodeKnownColor(value);
			return System.Drawing.Color.FromArgb(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]));
		}

        [TypeDecoderMethod]
        public static string DecodeColorInverse(Color col)
        {
            if (col.IsKnownColor)
            {
                return col.Name.ToLower();
            }
            else
                return string.Format("{0},{1},{2}", col.R, col.G, col.B);
        }

		///<summary>
		///Decodes a color description into a System.Drawing.Color constant color
		/// Original source: MyXaml project, Bert Bier
		///</summary>
		///<param name="value">String containing the name of a color</param>
		private static System.Drawing.Color DecodeKnownColor(string value)
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
				default : return System.Drawing.Color.Black;
			}
		}

        public static bool Test()
        {
            bool ok = true;
            string s = string.Empty;

            // DecodePoint
            Point p1 = new Point(5, 104);
            s = DecodePointInverse(p1);
            Point p2 = DecodePoint(s);

            if (!p1.Equals(p2))
                ok = false;

            // DecodeSize
            Size s1 = new Size(1254, 67);
            s = DecodeSizeInverse(s1);
            Size s2 = DecodeSize(s);

            if (!s1.Equals(s2))
                ok = false;

            // DecodeFont
            Font f1 = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            s = DecodeFontInverse(f1);
            Font f2 = DecodeFont(s);

            if (!f1.Equals(f2))
                ok = false;

            // DecodeDateTime
            DateTime t1 = DateTime.Today;
            s = DecodeDateTimeInverse(t1);
            DateTime t2 = DecodeDateTime(s);

            if (!t1.Equals(t2))
                ok = false;

            // DecodeAppearance
            Appearance a1 = Appearance.Button;
            s = DecodeAppearanceInverse(a1);
            Appearance a2 = DecodeAppearance(s);

            if (!t1.Equals(t2))
                ok = false;

            // DecodeScrollBars
            ScrollBars sb1 = ScrollBars.Both;
            s = DecodeScrollBarsInverse(sb1);
            ScrollBars sb2 = DecodeScrollBars(s);

            if (!sb1.Equals(sb2))
                ok = false;

            // DecodeSelectionMode
            SelectionMode sm1 = SelectionMode.MultiSimple;
            s = DecodeSelectionModeInverse(sm1);
            SelectionMode sm2 = DecodeSelectionMode(s);

            if (!sm1.Equals(sm2))
                ok = false;

            // DecodeView
            View v1 = View.LargeIcon;
            s = DecodeViewInverse(v1);
            View v2 = DecodeView(s);

            if (!v1.Equals(v2))
                ok = false;

            // DecodeOrientation
            Orientation o1 = Orientation.Vertical;
            s = DecodeOrientationInverse(o1);
            Orientation o2 = DecodeOrientation(s);

            if (!o1.Equals(o2))
                ok = false;

            // DecodeTickStyle
            TickStyle ts1 = TickStyle.BottomRight;
            s = DecodeTickStyleInverse(ts1);
            TickStyle ts2 = DecodeTickStyle(s);

            if (!ts1.Equals(ts2))
                ok = false;

            // DecodeTabAlignment
            TabAlignment ta1 = TabAlignment.Right;
            s = DecodeTabAlignmentInverse(ta1);
            TabAlignment ta2 = DecodeTabAlignment(s);

            if (!ta1.Equals(ta2))
                ok = false;

            // DecodeListViewItem (single)
            ListViewItem lv1 = new ListViewItem("John Doe");
            s = DecodeListViewItemInverse(lv1);
            ListViewItem lv2 = DecodeListViewItem(s);

            /* not comparable (only by reference)
            if (!lv1.Equals(lv2))
                ok = false;
             */

            // DecodeListViewItem (subitems)
            lv1 = new ListViewItem();
            lv1.SubItems.Add("John Doe");
            lv1.SubItems.Add("Mountain View");
            lv1.SubItems.Add("43");

            s = DecodeListViewItemInverse(lv1);
            lv2 = DecodeListViewItem(s);

            /* not comparable (only by reference)
            if (!lv1.Equals(lv2))
                ok = false;
             */

            // DecodeColor
            s = DecodeColorInverse(Color.BlanchedAlmond);
            Color c = DecodeColor(s);

            if (!c.Equals(Color.BlanchedAlmond))
                ok = false;

            return ok;
        }
	}	
}

