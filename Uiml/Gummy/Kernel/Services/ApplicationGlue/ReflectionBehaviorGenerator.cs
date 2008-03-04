using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

using Uiml.Peers;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    class ReflectionBehaviorGenerator : IBehaviorGenerator
    {
        private Dictionary<Type, List<ReflectionMethodModel>> m_logicTree = new Dictionary<Type, List<ReflectionMethodModel>>();
        private VocabularyMetadata m_vocMeta;

        public ReflectionBehaviorGenerator()
        {
            Vocabulary voc = ActiveSerializer.Instance.Serializer.Voc;
            m_vocMeta = new VocabularyMetadata(voc);
        }

        private void Init()
        {
            m_logicTree.Clear();

            foreach (ConnectedMethod cm in ApplicationGlueRegistry.Instance.Methods.Values)
            {
                ReflectionMethodModel method = (ReflectionMethodModel)cm.Method;
                Type type = method.MethodInfo.ReflectedType;

                if (!m_logicTree.ContainsKey(type))
                    m_logicTree[type] = new List<ReflectionMethodModel>();

                m_logicTree[type].Add(method);
            }
        }

        public Logic GenerateLogic(out string logicXml)
        {
            Init(); //FIXME: efficiency

            StringWriter writer = new StringWriter();
            XmlTextWriter xmlw = new XmlTextWriter(writer);

            xmlw.WriteStartDocument();

            // <logic>
            xmlw.WriteStartElement("logic"); 

            foreach (Type t in m_logicTree.Keys)
            {
                // <d-component>
                xmlw.WriteStartElement("d-component");
                xmlw.WriteAttributeString("id", t.Name);
                xmlw.WriteAttributeString("maps-to", t.FullName);

                foreach (ReflectionMethodModel method in m_logicTree[t])
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
            }

            xmlw.WriteEndElement(); // </logic>
            xmlw.WriteEndDocument();
            xmlw.Close();

            string xml = writer.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            logicXml = doc.DocumentElement.OuterXml;
            return new Logic(doc.DocumentElement);
        }

        public Behavior GenerateBehavior(out string behaviorXml)
        {
            Init(); //FIXME: efficiency

            StringWriter writer = new StringWriter();
            XmlTextWriter xmlw = new XmlTextWriter(writer);

            xmlw.WriteStartDocument();
            
            // <behavior>
            xmlw.WriteStartElement("behavior");

            foreach (ConnectedMethod method in ApplicationGlueRegistry.Instance.Methods.Values)
            {
                try
                {
                    xmlw.WriteStartElement("rule");

                    xmlw.WriteStartElement("condition"); // <condition>
                    xmlw.WriteStartElement("event"); // <event>
                    /* Invoke */
                    xmlw.WriteAttributeString("part-name", method.Invoke.Part.Identifier);
                    xmlw.WriteAttributeString("class", m_vocMeta.GetEvent(method.Invoke.Part.Class));
                    xmlw.WriteEndElement(); // </event>
                    xmlw.WriteEndElement(); // </condition>

                    xmlw.WriteStartElement("action"); // <action>
                    /* Output */
                    xmlw.WriteStartElement("property"); // <property>
                    xmlw.WriteAttributeString("part-name", method.Output.Part.Identifier);
                    xmlw.WriteAttributeString("name", m_vocMeta.GetOutputProperty(method.Output.Part.Class, method.Method.Outputs[0].Type));
                    xmlw.WriteStartElement("call"); // <call>
                    Type t = ((ReflectionMethodModel)method.Method).MethodInfo.ReflectedType;
                    xmlw.WriteAttributeString("name", t + "." + method.Method.Name);
                    /* Inputs */
                    foreach (KeyValuePair<MethodParameterModel, DomainObject> item in method.Inputs)
                    {
                        MethodParameterModel param = item.Key;
                        DomainObject dom = item.Value;

                        xmlw.WriteStartElement("param"); // <param>
                        xmlw.WriteStartElement("property"); // <property>
                        xmlw.WriteAttributeString("part-name", dom.Part.Identifier);
                        xmlw.WriteAttributeString("name", m_vocMeta.GetInputProperty(dom.Part.Class, param.Type));
                        xmlw.WriteEndElement(); // </property>
                        xmlw.WriteEndElement(); // </param>
                    }
                    xmlw.WriteEndElement(); // </call>
                    xmlw.WriteEndElement(); // </property>
                    xmlw.WriteEndElement(); // </action>

                    xmlw.WriteEndElement(); // </rule>
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            xmlw.WriteEndElement(); // </behavior>
            xmlw.WriteEndDocument();
            xmlw.Close();

            string xml = writer.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            behaviorXml = doc.DocumentElement.OuterXml;
            return new Behavior(doc); // todo
        }
    }
}
