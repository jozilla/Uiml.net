using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;
using Uiml.Peers;

using gummy_swing.Java;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingUimlSerializer : IUimlSerializer
    {
        JavaUimlRendererService m_javaUimlRenderer = null;
        Vocabulary m_voc = null;

        public SwingUimlSerializer()
		{
            m_javaUimlRenderer = new JavaUimlRendererService();
            m_voc = new Vocabulary("idtv-1.0.uiml");
		}

        public DomainObject Create()
        {
            DomainObject domObj = new DomainObject();
            domObj.PositionManipulator = new SwingPositionManipulator(domObj);
            domObj.SizeManipulator = new SwingSizeManipulator(domObj);

            return domObj;
        }
               
        public Image Serialize(DomainObject dom)
		{
            string uiml1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<uiml>"
            + "<interface>"
            + "<structure>"
            + "<part class=\"Container\" id=\"SurroundingFrame\">"
            + " <part class=\"Container\" id=\"Box\">";
            
            string uiml2 = "  <part class=\""+dom.Part.Class+"\" id=\""+dom.Part.Identifier+"\" />";

            string uiml3 = " </part>"
            + "</part>"
            + "</structure>"
            + "<style>"
            + "<property part-name=\"SurroundingFrame\" name=\"size\">800,600</property>"
            + "<property part-name=\"SurroundingFrame\" name=\"backgroundcolor\">255,0,0</property>"
            + "<property part-name=\"SurroundingFrame\" name=\"position\">0,0</property>"
            + "<property part-name=\"Box\" name=\"size\">800,600</property>";

            string uiml4 = "";

            for (int i = 0; i < dom.Properties.Count; i++)
            {
                Property prop = dom.Properties[i];
                uiml4 += "<property part-name=\""+prop.PartName+"\" name=\""+prop.Name+"\">"+prop.Value.ToString()+"</property>";
            }
            
            string uiml5 = "</style>"
            + "</interface>"
            + "<peers>"
            + "<presentation base=\"c:\\idtv-1.0.uiml\"/>"
            + "</peers>"
            + "</uiml>";         

            string uiml = uiml1 + uiml2 + uiml3 + uiml4 + uiml5;
            Console.Out.WriteLine(uiml);

            byte[] byteImage = m_javaUimlRenderer.renderPart(uiml);

            Image newImage = null;
            MemoryStream ms = new MemoryStream();

            ms.Write(byteImage, 0, byteImage.Length);
            newImage = Image.FromStream(ms, true);

            return newImage;
		}


        public Vocabulary Voc
        {
            get
            {
                return m_voc;
            }
        }

        public bool Accept(DClass dclass)
        {
            if (dclass.Identifier == "Checkbox")
            {
                return true;
            }
            if (dclass.Identifier == "NavigationIndicator")
            {
                return true;
            }
            if (dclass.Identifier == "Label")
            {
                return true;
            }
            if (dclass.Identifier == "TextField")
                return true;
            if (dclass.Identifier == "Button")
                return true;
            return false;
        }

        public bool Accept(DProperty dprop, DClass dclass)
        {
            if (dprop.Identifier == "size") { return true; }
            if (dprop.Identifier == "position") { return true; }
            if (dprop.Identifier == "visible") { return true; }

            if (dclass.Identifier == "Checkbox")
            {
                if (dprop.Identifier == "text") { return true; }
                if (dprop.Identifier == "imageChecked") { return true; }
                if (dprop.Identifier == "imageUnchecked") { return true; }
                if (dprop.Identifier == "setChecked") { return true; }                
                if (dprop.Identifier == "visible") { return true; }
                if (dprop.Identifier == "checked") { return true; }
            }
            if (dclass.Identifier == "Label" || dclass.Identifier == "TextField")
            {
                if(dprop.Identifier == "text") { return true; }
                if(dprop.Identifier == "color") { return true; }
            }
            if (dclass.Identifier == "Button")
            {
                if (dprop.Identifier == "text") { return true; }
                if (dprop.Identifier == "text2") { return true; }                
            }
            return false;
        }

        public object DefaultValue(Property p, Part part, string type)
        {
            if (p.Name == "size") { return "100,50"; }
            if (p.Name == "position") { return "0,0"; }
            if (p.Name == "visible") { return "true"; }

            if (part.Class == "Checkbox")
            {
                if (p.Name == "text") { return "check me"; }
                if (p.Name == "imageChecked") { return "c:\\images\\checked.jpg"; }
                if (p.Name == "imageUnchecked") { return "c:\\images\\unchecked.jpg"; }
                if (p.Name == "setChecked") { return "true"; }                
                if (p.Name == "visible") { return "true"; }
            }
            if (part.Class == "Label" || part.Class == "TextField")
            {
                if (p.Name == "text") { return part.Class; }
                if (p.Name == "color") { return "255,0,0"; }
            }
            if (part.Class == "Button")
            {
                if (p.Name == "text") { return "Button"; }
                if (p.Name == "text2") { return "Buttontext2"; }
            }
            return "";
        }

        public const string NAME = "idtv-1.0";
    }
}
