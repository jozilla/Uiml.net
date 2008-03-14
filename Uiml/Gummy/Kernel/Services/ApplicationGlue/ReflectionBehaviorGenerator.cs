using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

using Uiml.Peers;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ReflectionBehaviorGenerator : IBehaviorGenerator
    {
        private ConnectedMethodsModel m_model;
        private Dictionary<Type, List<ReflectionMethodModel>> m_logicTree = new Dictionary<Type, List<ReflectionMethodModel>>();
        private VocabularyMetadata m_vocMeta;

        public ConnectedMethodsModel Model
        {
            get { return m_model; }
        }

        public ReflectionBehaviorGenerator(ConnectedMethodsModel model)
        {
            Vocabulary voc = ActiveSerializer.Instance.Serializer.Voc;
            m_vocMeta = new VocabularyMetadata(voc);
            m_model = model;
        }

        public void Update(MethodModel m)
        {
            ReflectionMethodModel method = (ReflectionMethodModel) m;
            Type type = method.MethodInfo.ReflectedType;

            if (!m_logicTree.ContainsKey(type))
                m_logicTree[type] = new List<ReflectionMethodModel>();

            m_logicTree[type].Add(method);
        }

        public XmlNode GenerateLogic(XmlDocument doc)
        {
            // <logic>
            XmlElement logic = doc.CreateElement("logic");

            foreach (Type t in m_logicTree.Keys)
            {
                // <d-component>
                XmlElement comp = doc.CreateElement("d-component");
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = t.Name;
                comp.Attributes.Append(id);
                XmlAttribute mapsTo = doc.CreateAttribute("maps-to");
                mapsTo.Value = t.FullName;
                comp.Attributes.Append(mapsTo);
                logic.AppendChild(comp);

                foreach (ReflectionMethodModel method in m_logicTree[t])
                {
                    // <d-method>
                    XmlElement dmethod = doc.CreateElement("d-method");
                    XmlAttribute methId = doc.CreateAttribute("id");
                    methId.Value = method.Name;
                    dmethod.Attributes.Append(methId);
                    if (method.Outputs.Count > 0)
                    {
                        XmlAttribute returnType = doc.CreateAttribute("return-type");
                        returnType.Value = method.Outputs[0].Type.ToString();
                        dmethod.Attributes.Append(returnType);
                    }
                    XmlAttribute methMapsTo = doc.CreateAttribute("maps-to");
                    methMapsTo.Value = method.Name;
                    dmethod.Attributes.Append(methMapsTo);
                    comp.AppendChild(dmethod);

                    foreach (ReflectionMethodParameterModel param in method.Inputs)
                    {
                        // <d-param>
                        XmlElement dparam = doc.CreateElement("d-param");
                        XmlAttribute paramId = doc.CreateAttribute("id");
                        paramId.Value = param.Name;
                        dparam.Attributes.Append(paramId);
                        XmlAttribute type = doc.CreateAttribute("type");
                        type.Value = param.Type.ToString();
                        dparam.Attributes.Append(type);
                        dmethod.AppendChild(dparam);
                    }
                }
            }

            return logic;
        }

        public XmlNode GenerateBehavior(XmlDocument doc)
        {
            // <behavior>
            XmlElement behavior = doc.CreateElement("behavior");

            foreach (ConnectedMethod method in Model.Methods)
            {
                if (method.IsComplete())
                {
                    try
                    {
                        // rule
                        XmlElement rule = doc.CreateElement("rule");
                        behavior.AppendChild(rule);

                        // <condition>
                        XmlElement condition = doc.CreateElement("condition");
                        rule.AppendChild(condition);
                        // <event>
                        XmlElement evnt = doc.CreateElement("event");
                        condition.AppendChild(evnt);

                        /* Invoke */
                        XmlAttribute partName = doc.CreateAttribute("part-name");
                        partName.Value = method.Invoke.Part.Identifier;
                        evnt.Attributes.Append(partName);
                        XmlAttribute clss = doc.CreateAttribute("class");
                        clss.Value = m_vocMeta.GetEvent(method.Invoke.Part.Class);
                        evnt.Attributes.Append(clss);

                        // <action>
                        XmlElement action = doc.CreateElement("action");
                        rule.AppendChild(action);
                        /* Output */
                        // <property>
                        XmlElement prop = doc.CreateElement("property");
                        XmlAttribute propPartName = doc.CreateAttribute("part-name");
                        propPartName.Value = method.Output.Part.Identifier;
                        prop.Attributes.Append(propPartName);
                        XmlAttribute propName = doc.CreateAttribute("name");
                        propName.Value = m_vocMeta.GetOutputProperty(method.Output.Part.Class, method.Method.Outputs[0].Type);
                        prop.Attributes.Append(propName);
                        action.AppendChild(prop);
                        // <call>
                        XmlElement call = doc.CreateElement("call");
                        Type t = ((ReflectionMethodModel)method.Method).MethodInfo.ReflectedType;
                        XmlAttribute callName = doc.CreateAttribute("name");
                        callName.Value = t + "." + method.Method.Name;
                        call.Attributes.Append(callName);
                        prop.AppendChild(call);
                        /* Inputs */
                        foreach (MethodParameterModel param in method.Inputs)
                        {
                            DomainObject dom = param.Link;

                            if (param.Bound)
                            {
                                XmlNode callParam = param.Binding.GetUiml(doc);
                                call.AppendChild(callParam);
                            }
                            else
                            {
                                // <param>
                                XmlElement callParam = doc.CreateElement("param");
                                XmlAttribute paramId = doc.CreateAttribute("id");
                                paramId.Value = param.Name;
                                callParam.Attributes.Append(paramId);
                                call.AppendChild(callParam);
                                // <property>
                                XmlElement paramProp = doc.CreateElement("property");
                                callParam.AppendChild(paramProp);
                                XmlAttribute paramPropPartName = doc.CreateAttribute("part-name");
                                paramPropPartName.Value = dom.Part.Identifier;
                                paramProp.Attributes.Append(paramPropPartName);
                                XmlAttribute paramPropName = doc.CreateAttribute("name");
                                paramPropName.Value = m_vocMeta.GetInputProperty(dom.Part.Class, param.Type);
                                paramProp.Attributes.Append(paramPropName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return behavior;
        }
    }
}
