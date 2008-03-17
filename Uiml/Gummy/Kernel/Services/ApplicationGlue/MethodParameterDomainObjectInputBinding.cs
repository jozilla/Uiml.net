using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class MethodParameterDomainObjectInputBinding : MethodParameterDomainObjectIOBinding
    {
        public MethodParameterDomainObjectInputBinding(MethodParameterModel param, DomainObject dom, Property prop)
            : base(param, dom, prop)
        {
        }

        public override XmlNode GetUiml(XmlDocument doc)
        {
            // <param>
            XmlElement param = doc.CreateElement("param");

            // <property>
            XmlElement prop = doc.CreateElement("property");
            param.AppendChild(prop);
            XmlAttribute partName = doc.CreateAttribute("part-name");
            partName.Value = DomainObject.Part.Identifier;
            prop.Attributes.Append(partName);
            XmlAttribute name = doc.CreateAttribute("name");
            name.Value = Property.Name;
            prop.Attributes.Append(name);

            return param;
        }
    }
}
