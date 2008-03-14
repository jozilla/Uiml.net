using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class MethodParameterInputValueBinding : MethodParameterBinding
    {
        protected string m_value;

        public string Value
        {
            get { return m_value; }
        }

        public MethodParameterInputValueBinding(MethodParameterModel param, string value)
            : base(param)
        {
            m_value = value;
        }

        public override XmlNode GetUiml(XmlDocument doc)
        {
            // <param>
            XmlElement param = doc.CreateElement("param");
            if (Parameter.ParameterType == MethodParameterType.Input)
            {
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = Parameter.Name;
                param.Attributes.Append(id);
            }
            
            // set value
            param.InnerText = Value;

            return param;
        }
    }
}
