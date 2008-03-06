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

	public class SWFUimlSerializer : UimlSerializer
	{
		SWFRenderer m_renderer = null;        
	
		public SWFUimlSerializer()
		{
			m_renderer = new SWFRenderer();
			m_renderer.Voc = new Vocabulary("swf-1.1.uiml");
		}

        public override DomainObject Create()
        {
            DomainObject domObj = new DomainObject();
            domObj.PositionManipulator = new SWFPositionManipulator(domObj);
            domObj.SizeManipulator = new SWFSizeManipulator(domObj);

            return domObj;
        }

        public override DomainObject CreateUIContainer()
        {
            return DomainObjectFactory.Instance.Create("Container");
        }
               
        public override Image Serialize(DomainObject dom)
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
	            	Rectangle old_bounds = control.Bounds;
        	    	bool oldVisible = control.Visible;

	            	control.SetBounds(0, 0, control.Width, control.Height);
        	    	control.Visible = true;

	            	Bitmap btmp = new Bitmap(control.Width, control.Height);
        	    	control.DrawToBitmap(btmp, control.Bounds);
                    return btmp;
			#endif
		}

        public override Image SerializeToIcon(DomainObject dom)
        {
            if (dom.Part.Class == "List" || dom.Part.Class == "Tree")
            {
                return SerializeToIcon(dom, new Size(90, 90), new Size(30, 30));
            }
            else if (dom.Part.Class == "Label")
            {
                return SerializeToIcon(dom, new Size(30, 20), new Size(30, 20));
            }
            else if (dom.Part.Class == "Button" || dom.Part.Class == "ToggleButton")
            {
                return SerializeToIcon(dom, new Size(45, 20), new Size(30, 25));
            }
            else
                return base.SerializeToIcon(dom);
        }

        public override Vocabulary Voc
        {
            get
            {
                return m_renderer.Voc;
            }
        }

        public override bool Accept(DClass dclass)
        {
            if (dclass.Identifier == "TabPage" || dclass.Identifier == "Tabs" || dclass.Identifier == "Entry")
                return false;
            return true;
        }

        public override bool Accept(DProperty dprop, DClass dclass)
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

        public override object DefaultValue(Property p, Part part, string type)
        {    
            if (p.Name == "maximum" || p.Name == "max")
                return "5";
            else if (p.Name == "value" && part.Class == "ProgressBar")
                return "2";
            else if (p.Name == "step")
                return "1";
            else if (p.Name == "ticks")
                return "3";
            else if (p.Name == "multiline")
                return "true";
            else if (type == "System.Windows.Forms.ListViewItem[]")
            {
                Constant constant = new Constant();
                constant.Model = "list";
                Constant constant2 = new Constant();
                constant2.Value = "blaai";
                constant.Add(constant2);
                return constant;
            }
            else if (type == "System.Drawing.Image")
            {
                return "images/empty_image.png";
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
            else if (part.Class == "Label" && p.Name == "text")
            {
                return "label";
            }
            else if (part.Class == "Button" && p.Name == "label")
            {
                return "button";
            }
            else if (part.Class == "ToggleButton" && p.Name == "label")
            {
                return "toggle";
            }
            else
            {
                m_renderer.Render(part, new Style());
                object value = m_renderer.PropertySetter.GetValue(part, p);
                if (value is Color)
                {
                    Color color = (System.Drawing.Color)value;
                    return String.Format("{0},{1},{2}", color.R, color.G, color.B);
                }
                else if (value is Point)
                {
                    Point pnt = (Point)value;
                    return String.Format("{0},{1}", pnt.X, pnt.Y);
                }
                else if (value is Size)
                {
                    Size size = (Size)value;
                    return String.Format("{0},{1}", size.Width, size.Height);
                }
                else if (value is DateTime)
                {
                    DateTime time = (DateTime)value;
                    return String.Format("{0}/{1}/{2}", time.Month, time.Day, time.Year);
                }
                if (value != null)
                    return value.ToString();
                else
                    return "";
            }
        }

        public const string NAME = "swf-1.1";

	}
}
