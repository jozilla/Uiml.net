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
using Uiml.Gummy.Kernel;
using Shape;

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

        public string AutoID()
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
                            domObject.AddAttribute(Create(dprop,p));
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

        public Property Create(DProperty dprop, Part p)
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
                        prop.Value = m_serializer.DefaultValue(prop, p, dparam.Type);
                    }
                }
            }
            return prop;
            
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

        public DomainObject Create(Part p, List<Property> propList)
        {
            DomainObject dom = Create(p.Class, p.Identifier);
            foreach(Property prop in propList)
            {
                Property foundProp = dom.FindProperty(prop.Name);
                if (foundProp != null)
                {
                    foundProp.Value = prop.Value;
                }
            }
            return dom;
        }

        public Polygon DefaultPolygon()
        {
            Polygon pol = new Polygon();            
            if (DesignerKernel.Instance.CurrentDocument.DesignSpaceData != null)
            {
                Point max = DesignerKernel.Instance.CurrentDocument.DesignSpaceData.MaximumPoint;
                Point origin = DesignerKernel.Instance.CurrentDocument.DesignSpaceData.OriginPoint;
                pol.AddPoint(new Point(0, 0));
                pol.AddPoint(new Point(max.X - origin.X, 0));
                pol.AddPoint(new Point(max.X-origin.X,max.Y - origin.Y));
                pol.AddPoint(new Point(0, max.Y-origin.Y));
            }
            return pol;
        }
    }
}