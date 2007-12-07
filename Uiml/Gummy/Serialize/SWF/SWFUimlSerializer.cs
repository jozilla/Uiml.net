using System;
using System.IO;
using Uiml;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Uiml.Gummy.Domain;
using Uiml.Rendering.SWF;
using Uiml.Peers;
using Uiml.Gummy.Visual;

namespace Uiml.Gummy.Serialize.SWF
{

	public class SWFUimlSerializer : IUimlSerializer
	{
		SWFRenderer m_renderer = null;        
	
		public SWFUimlSerializer()
		{
			m_renderer = new SWFRenderer();
			m_renderer.Voc = new Vocabulary("swf-1.1.uiml");
		}

        public DomainObject Create()
        {
            DomainObject domObj = new DomainObject();
            domObj.PositionManipulator = new SWFPositionManipulator(domObj, m_renderer);
            domObj.SizeManipulator = new SWFSizeManipulator(domObj, m_renderer);

            return domObj;
        }
               
        public Image Serialize(DomainObject dom)
		{
            Image controlImage = Serialize(dom.Part, new ArrayList(dom.Properties));
            return controlImage;
		}

        private Image Serialize(Part p, ArrayList properties)
		{
			//Control to be rendered
			Control control = null;

			//The User Interface style
			Style uiStyle = new Style();

			//Create the Style
			uiStyle.Children = properties;

			control = m_renderer.Render(p, uiStyle);

			#if MONO
            		return control;            
			#else

	            	//FIXME: Why are 'entries' and 'texts' not rendered on images?
                    /*if (p.Class.ToLower() == "entry")
                    {                        
                        return control;
                    }*/

	            	Rectangle old_bounds = control.Bounds;
        	    	bool oldVisible = control.Visible;

	            	control.SetBounds(0, 0, control.Width, control.Height);
        	    	control.Visible = true;

	            	Bitmap btmp = new Bitmap(control.Width+1, control.Height+1);
        	    	control.DrawToBitmap(btmp, control.Bounds);
                    return btmp;
			#endif
		}

        public Vocabulary Voc
        {
            get
            {
                return m_renderer.Voc;
            }
        }

        public bool Accept(DClass dclass)
        {
            if (dclass.Identifier == "TabPage")
                return false;
            return true;
        }

        public bool Accept(DProperty dprop, DClass dclass)
        {
            if (dclass.Identifier == "TabPage" && dprop.Identifier != "label")
                return false;
            if (dprop.Identifier == "bottom")
                return false;
            if (dprop.Identifier == "top")
                return false;
            if (dprop.Identifier == "right")
                return false;
            if (dprop.Identifier == "size")
                return false;
            if (dprop.Identifier == "left")
                return false;
            if (dprop.Identifier == "add")
                return false;
            if (dclass.Identifier == "Tree" && dprop.Identifier == "add")
                return false;
            if (dclass.Identifier == "Tree" && dprop.Identifier == "selectedNode")
                return false;
            return true;
        }

        public object DefaultValue(Property p, Part part, string type)
        {            
            if (type == "System.String")
            {
                if (p.Name == "text")
                    return part.Class;
                else if (p.Name == "label")
                    return part.Class;
                else
                    return " ";
            }
            else if (type == "System.Drawing.Point")
            {
                return "10,10";
            }
            else if (type == "System.Drawing.Size")
            {
                return "50,50";
            }
            else if (type == "System.Int32" || type == "System.Int64" || type == "System.Int")
            {
                if (p.Name == "selectedIndex")
                    return "-1";
                if (p.Name == "width")
                    return "50";
                if (p.Name == "height")
                    return "20";
                if (p.Name == "maximum" || p.Name == "max")
                    return "5";
                if (p.Name == "value" && part.Class == "ProgressBar")
                    return "2";
                if (p.Name == "step")
                    return "1";
                if (p.Name == "ticks")
                    return "3";
                return "0";
            }
            else if (type == "System.Boolean")
            {
                if (p.Name == "visible" || p.Name == "enabled")
                    return "true";
                if (p.Name == "multiline")
                    return "true";
                return "false";
            }
            else if (type == "System.Drawing.Color")
            {
                if (p.Name == "background" && part.Class == "Container")
                    return "khaki";
                if (p.Name == "background")
                    return "255,255,255";
                else if (p.Name == "foreground")
                    return "0,0,0";
                else
                    return "255,0,0";
            }
            else if (type == "System.Windows.Forms.ScrollBars")
            {
                return "None";
            }
            else if (type == "System.Windows.Forms.Appearance")
            {
                return "Button";
            }
            else if (type == "System.Drawing.Image")
            {
                Console.WriteLine("---- System.Drawing.Image set!");
                return "uimldotnet.png";
            }
            else if (type == "System.String[]")
            {
                Constant constant = new Constant();
                constant.Model = "list";
                Constant constant2 = new Constant();
                constant2.Value = "blaai";
                constant.Add(constant2);
                return constant;
            }
            else if (type == "System.Windows.Forms.SelectionMode")
            {
                return "MultiSimple";
            }
            else if (type == "System.Windows.Forms.ListViewItem[]")
            {
                Constant constant = new Constant();
                constant.Model = "list";
                Constant constant2 = new Constant();
                constant2.Value = "blaai";
                constant.Add(constant2);
                return constant;
            }
            else if (type == "System.Windows.Forms.ListViewItem")
            {
                return "bla1;bla2";
            }
            else if (type == "System.Windows.Forms.View")
            {
                return "Details";
            }
            else if (type == "System.Windows.Forms.ColumnHeader")
            {
                return "Title";
            }
            else if (type == "System.Windows.Forms.TreeNode[]")
            {
                Constant constant = new Constant();
                constant.Model = "tree";
                Constant constant2 = new Constant();
                constant2.Value = "treenode";
                constant.Add(constant2);
                Constant constant3 = new Constant();
                constant3.Value = "blaai";
                constant.Add(constant3);
                return constant;
            }
            else if (type == "System.Windows.Forms.TreeNode")
            {
                return "treeNode";
            }
            else if (type == "System.Windows.Forms.TickStyle")
            {
                return "Both";
            }
            else if (type == "System.Windows.Forms.Orientation")
            {
                if (part.Class == "HorizontalRange")
                    return "Horizontal";
                else if (part.Class == "VerticalRange")
                    return "Vertical";
                else
                    return "Vertical";
            }
            else if (type == "System.Windows.Forms.TabAlignment")
            {
                return "Left"; //TODO: Check if the tabPos works

            }
            else if (type == "System.Double")
            {
                return "1.2";
            }
            else if (type == "System.DateTime")
            {
                return "12/17/1982";
            }
            else
                return "";
        }

	}
}
