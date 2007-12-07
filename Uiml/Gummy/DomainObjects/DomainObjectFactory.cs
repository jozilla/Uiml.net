using System;
using System.IO;
using Uiml;
using Uiml.Peers;
using System.Windows.Forms;
using System.Drawing;
using Uiml.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Uiml.Gummy.Serialize;

namespace Uiml.Gummy.Domain
{
    public class DomainObjectFactory
    {
        static DomainObjectFactory m_factory = null;
        Vocabulary m_vocabulary = null;
        IUimlSerializer m_serializer = null;

        static int autoId = 1;

        protected DomainObjectFactory()
        {
            m_vocabulary = ActiveSerializer.Instance.Serializer.Voc;
            m_serializer = ActiveSerializer.Instance.Serializer;
        }

        public static DomainObjectFactory Instance
        {
            get
            {
                if (m_factory == null)
                    m_factory = new DomainObjectFactory();
                return m_factory;
            }
        }

        private string AutoID()
        {
            string id = "part" + autoId;
            autoId++;
            return id;
        }

        public DomainObject Create(DClass dclass)
        {
            return Create(dclass, null);
        }

        public DomainObject Create(DClass dclass, string id)
        {
            DomainObject domObject = ActiveSerializer.Instance.Serializer.Create();
                        
            Part p = new Part();
            p.Class = dclass.Identifier;
            if (id == null)
                p.Identifier = AutoID();
            else
                p.Identifier = id;

            //Build the properties !

            List<Property> properties = new List<Property>();
            if (dclass.HasChildren)
            {
                for (int i = 0; i < dclass.Children.Count; i++)
                {                    
                    object child = dclass.Children[i];
                    if (child is DProperty)
                    {
                        //A DProperty element
                        DProperty dprop = (DProperty)child;
                        
                        if (dprop.MapsType == "setMethod" && dprop.Identifier != "daySelected"
                            && m_serializer.Accept(dprop, dclass))
                        {                            
                            Property prop = new Property();
                            prop.PartName = p.Identifier;
                            prop.Name = dprop.Identifier; //The property name
                            //Explore the parameters...
                            if (dprop.HasChildren)
                            {                                
                                for (int j = 0; j < dprop.Children.Count; j++)
                                {                                   
                                    if (dprop.Children[j] is DParam)
                                    {                                        
                                        DParam dparam = (DParam)dprop.Children[j];
                                        //Resolve the right type
                                        prop.Value = m_serializer.DefaultValue(prop, p, dparam.Type);
                                    }
                                }
                            }
                            domObject.AddAttribute(prop);
                        }
                    }
                    else if (child is Event)
                    {
                        //TODO: Implement Events
                    }
                }
            }
            domObject.Part = p;

            return domObject;

        }

        public DomainObject Create(String classtype)
        {
            return Create(classtype, null);
        }

        public DomainObject Create(String classtype, string id)
        {
            Hashtable dclasses = m_vocabulary.DClasses;

            IDictionaryEnumerator en = dclasses.GetEnumerator();
            while (en.MoveNext())
            {
                DClass dclass = (DClass)en.Value;
                if (dclass.UsedInTag == "part" && dclass.Identifier == classtype)
                {
                    DomainObject domObject = Create((DClass)en.Value, id);
                    return domObject;
                }
            }
            return null;
        }

        /*
        public WidgetModel Create(Part p, Style s)
        {
            if (WidgetTabContainerModel.GetFactory().CanHandle(p, s))
            {
                return WidgetTabContainerModel.GetFactory().DeSerialize(p, s);
            }
            else if (WidgetContainerModel.GetFactory().CanHandle(p, s))
            {
                return WidgetContainerModel.GetFactory().DeSerialize(p, s);
            }
            else if (WidgetModel.GetFactory().CanHandle(p, s))
            {
                return WidgetModel.GetFactory().DeSerialize(p, s);
            }
            return null;
        }*/

        /*
        public WidgetModel Create(Part p, Style s)
        {
            WidgetModel model = Create(p.Class,p.Identifier);
            IEnumerator prop = s.GetNamedProperties(p.Identifier);
            Console.WriteLine("Searching properties for "+p.Identifier);
            while(prop.MoveNext())
            {
                Console.WriteLine("DeSerialize the property");
                Property property = (Property)prop.Current;
                PropertyModel pmodel = model.GetPropertyByName(property.Name);
                if(pmodel != null)
                {
                    if(property.SubProp != null)
                    {
                        Console.WriteLine("Set the property subprop of "+property.Name+" from "+pmodel.Value+" to "+property.SubProp+" 9999");
                        pmodel.Value = property.SubProp;
                    }
                    if(property.Value != null)
                    {
                        Console.WriteLine("Set the property value of "+property.Name+" from "+pmodel.Value+" to "+property.Value + " 9999");
                        pmodel.Value = property.Value;
                    }
                }
                else
                    Console.WriteLine("[WARNING] Propertymodel for "+property.Name+" not found ! ");
            }
            Console.WriteLine("Created widgetmodel ....");
            model.Print();
            Console.WriteLine("End widgetmodel creation ....");
            return model;
        }*/
    }
}