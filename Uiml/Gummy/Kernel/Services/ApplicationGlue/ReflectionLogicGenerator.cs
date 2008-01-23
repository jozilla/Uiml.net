using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    class ReflectionLogicGenerator : LogicGenerator
    {
        public ReflectionLogicGenerator(CanvasService cs, Type t)
            : base(cs, t)
        {
        }

        public override Logic Generate()
        {
            StringWriter writer = new StringWriter();
            XmlTextWriter xmlw = new XmlTextWriter(writer);

            // <logic>
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("logic");
            xmlw.WriteStartElement("d-component");
            xmlw.WriteAttributeString("id", Type.Name);
            xmlw.WriteAttributeString("maps-to", Type.FullName);

            foreach (ReflectionMethodModel method in Methods.Values)
            {
                // <d-method>
                xmlw.WriteStartElement("d-method");
                xmlw.WriteAttributeString("id", method.Name);
                if (method.Outputs.Count > 0)
                    xmlw.WriteAttributeString("return-type", method.Outputs[0].Type.ToString());
                xmlw.WriteAttributeString("maps-to", method.Name);

                foreach (ReflectionMethodParameterModel param in method.Inputs)
                {
                    // <d-param>
                    xmlw.WriteStartElement("d-param");
                    xmlw.WriteAttributeString("id", param.Name);
                    xmlw.WriteAttributeString("type", param.Type.ToString());
                    xmlw.WriteEndElement(); // </d-param>
                }

                xmlw.WriteEndElement(); // </d-method>
            }

            xmlw.WriteEndElement(); // </d-component>
            xmlw.WriteEndElement(); // </logic>
            xmlw.WriteEndDocument();
            xmlw.Close();

            string xml = writer.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return new Logic(doc.DocumentElement);
        }
    }
}
