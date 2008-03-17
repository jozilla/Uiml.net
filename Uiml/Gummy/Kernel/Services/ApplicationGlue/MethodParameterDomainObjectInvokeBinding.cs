using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using Uiml.Peers;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class MethodParameterDomainObjectInvokeBinding : MethodParameterDomainObjectBinding
    {
        protected string m_event;

        public string Event
        {
            get { return m_event; }
        }

        public MethodParameterDomainObjectInvokeBinding(MethodParameterModel param, DomainObject dom, string e)
            : base(param, dom)
        {
            m_event = e;
        }

        /*
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
        }*/

        public override XmlNode GetUiml(XmlDocument doc)
        {
            MethodModel method = Parameter.Parent;

            // rule
            XmlElement rule = doc.CreateElement("rule");

            // <condition>
            XmlElement condition = doc.CreateElement("condition");
            rule.AppendChild(condition);
            // <event>
            XmlElement evnt = doc.CreateElement("event");
            condition.AppendChild(evnt);

            /* Invoke */
            XmlAttribute partName = doc.CreateAttribute("part-name");
            partName.Value = DomainObject.Part.Identifier;
            evnt.Attributes.Append(partName);
            XmlAttribute clss = doc.CreateAttribute("class");
            clss.Value = Event;
            evnt.Attributes.Append(clss);

            // <action>
            XmlElement action = doc.CreateElement("action");
            rule.AppendChild(action);

            // <call>
            XmlElement call = doc.CreateElement("call");
            Type t = ((ReflectionMethodModel)method).MethodInfo.ReflectedType;
            XmlAttribute callName = doc.CreateAttribute("name");
            callName.Value = t + "." + method.Name;
            call.Attributes.Append(callName);

            /* Output */

            if (method.Outputs.Count > 0)
            {
                XmlNode property = method.Outputs[0].Binding.GetUiml(doc);
                action.AppendChild(property);
                property.AppendChild(call);
            }
            else
            {
                action.AppendChild(call);
            }

            /* Inputs */
            foreach (MethodParameterModel param in method.Inputs)
            {
                XmlNode callParam = param.Binding.GetUiml(doc);
                call.AppendChild(callParam);
            }

            return rule;
        }
    }
}
