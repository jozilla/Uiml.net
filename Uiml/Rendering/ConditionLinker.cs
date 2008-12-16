using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
//using System.Windows.Forms;

using Uiml.Executing;

namespace Uiml.Rendering
{
	public class ConditionLinker : IEventLinker
    {
        private Structure        m_uiStruct;
        private Behavior         m_uiBehavior;
        private IRenderer        m_renderer;
        private ConditionManager m_conditions;
        private Hashtable        m_propertyLinked;

        public ConditionLinker(IRenderer renderer)
        {
            m_renderer = renderer;
            m_conditions = new ConditionManager();
            m_propertyLinked = new Hashtable();
        }

        ///<summary>
        ///Links the different rules in the behavior with the parts of the structure. 
        ///This method uses link(Rule, Part) for each rule that is available in the behavior section.
        ///</summary>
        public void Link(Structure uiStruct, Behavior uiBehavior)
        {
            if ((uiBehavior == null) || (uiStruct == null))
                return;

            m_uiStruct = uiStruct;
            m_uiBehavior = uiBehavior;

            Part topPart = uiStruct.Top;

            IEnumerator enumRules = m_uiBehavior.Rules;
            while (enumRules.MoveNext())
            {
                Rule r = (Rule)enumRules.Current;
                if (!r.IsEmpty)
                    link(r, topPart);
            }
        }

        protected IRenderer Renderer
        {
            get { return m_renderer; }
        }


        virtual protected void link(Rule r, Part p)
        {
            //Console.WriteLine("PART LINKING: " + p.Identifier);
            link(r, p, new EventLink(r.Condition, m_renderer, p));
        }

        ///<summary>
        ///Searches the parts that are used in the condition of Rule r. Events of a certain type emitted
        ///by thow parts are connected to the condition of Rule r so it can be evaluated if such an event
        ///occurs
        ///</summary>
        ///<param name="r">The rule with a condition and action</param>
        ///<param name="p">A part where p itself or one of it subparts will emit events that are of interest
        ///for the condition of Rule r</param>
        ///<param name="gel">Links the renderer with the condition so appropriate queries can be executed by the condition.
        ///It also allows the action to use the renderer afterwards to change properties in the user interface at runtime</param>

        protected void link(Rule r, Part p, IEventLink sel)
        {
            m_conditions.Add(sel);
            IEnumerator eventsEnum = r.Condition.GetEvents().GetEnumerator();
            try
            {
                while (eventsEnum.MoveNext())
                {
                    if (eventsEnum.Current is Event)
                    {
                        LinkEvent(eventsEnum.Current, p);
                    }
                    else if (eventsEnum.Current is Op)
                    {
                        LinkOp(eventsEnum.Current, p);
                    }
                    else if (eventsEnum.Current is Equal)
                    {
                        LinkEqual(eventsEnum.Current, p);
                    }
                }
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("Invalid argument: {0}", ae);
                #if !COMPACT
                Console.WriteLine("Caused by: {0}", ae.ParamName);
                #endif
            }
            catch (Exception be)
            {
                Console.WriteLine("Unexpected failure:");
                Console.WriteLine(be);
                Console.WriteLine("Please contact the uiml.net maintainer with the above output.");
            }
        }

        private void LinkEqual(Object ie, Part part)
        {
            Equal e = (Equal)ie;
            if (e.Children.Count != 2)
                throw new XmlElementMismatchException("Your input document is not in the correct format. <equal> should have only 2 elements.");

            for (int i = 0; i < e.Children.Count; i++)
            {
                if (e.Children[i] is Event)
                {
                    LinkEvent(ie, part);
                }
                else if (e.Children[i] is Constant)
                {
                    // nothing to be done
                }
                else if (e.Children[i] is Property)
                {

                }
                else if (e.Children[i] is Reference)
                {

                }
                else if (e.Children[i] is Op)
                {
                    LinkOp(ie, part);
                }
            }
        }

        private void LinkEvent(Object ie, Part part)
        {
            Event e = (Event)ie;
            Part thePart = part.SearchPart(e.PartName);

            if (e.Class == "init")
            {
                // special case: init event
                string fullEventName = m_renderer.Voc.MapsOnCls(e.Class);
                string typeName = fullEventName.Substring(0, fullEventName.LastIndexOf('.'));
                string eventName = fullEventName.Substring(fullEventName.LastIndexOf('.') + 1);

                // get the top part
                Part top = m_renderer.Top;

                // get the current rendered instances
                IRenderedInstance instance = m_renderer.TopWindow;
                Type type = instance.GetType();

                /*Type type = null;
                foreach (Assembly a in ExternalLibraries.Instance.Assemblies)
                {
                    try
                    {
                        type = a.GetType(typeName, true);
                        break;
                    }
                    catch
                    {
                    }
                }

                if (type == null)
                {
                    Console.WriteLine("Error in behavior specification: init event cannot be linked. Please check your vocabulary (e.g. the maps-to attribute of the 'init' d-class).");
                    return;
                }*/

                EventInfo init = type.GetEvent(eventName);
                // now create a correct handler
                EventHandler handler = delegate(object sender, EventArgs args)
                {
                    // add the top part as the part-name
                    m_conditions.Execute(instance, args, "init", top.Identifier);
                };

                // add the handler to the event
                init.AddEventHandler(instance, handler);

                // tada!
                return;

                /*// get the connected object                
                IEnumerator enumObjects = ExternalObjects.Instance.LoadedObjects;
                while (enumObjects.MoveNext())
                {
                    object obj = enumObjects.Current;
                    Type objType = obj.GetType();
                    if (objType.Equals(type) || objType.IsSubclassOf(type))
                    {
                        // found it
                        // now create a correct handler
                        EventHandler handler = delegate(object sender, EventArgs args) 
                        { 
                            // add the top part as the part-name
                            m_conditions.Execute(obj, args, "init", "top");
                        };

                        // add the handler to the event
                        init.AddEventHandler(obj, handler);

                        // tada!
                        return;
                    }
                }*/
            }

            if (thePart == null)
            {
                if (e.PartName == "")
                    Console.WriteLine("Error in behavior specification: no part name given for {0}", e.Class);
                else
                    Console.WriteLine("Error in behavior specification: {0} does not exist for event {0}", e.PartName, e.Class);
                return;
            }
            Object w = (Object)thePart.UiObject;

            string concreteEventName = m_renderer.Voc.MapsOnCls(e.Class);
            /* TODO: eventhandling is done with an ugly hack: 
             * see the comments in the GTK# event handling classes for details.
             */

            string eventId = m_renderer.Voc.GetEventFor(thePart.Class, concreteEventName);

            EventHandler wrapper = m_conditions.CreateHandlerWrapper(ConditionManager.GetEventName(concreteEventName), e.PartName);

            //Sometimes eventId is a composed event:
            //it is an event of a property of widget w
            //so first load the property of w, and then link with
            //the event of this property
            //<property>.<event>

            Type theType = w.GetType();
            int j = eventId.IndexOf('.');
            System.Object targetObject = thePart.UiObject;
            while (j != -1)
            {
                String parentType = eventId.Substring(0, j);
                eventId = eventId.Substring(j + 1, eventId.Length - j - 1);
                PropertyInfo pInfo = theType.GetProperty(parentType);
                theType = pInfo.PropertyType;
                targetObject = pInfo.GetValue(targetObject, null);
                j = eventId.IndexOf('.');
            }
            EventInfo eInfo = theType.GetEvent(eventId);
            //load the event info as provided by the mappings of Widget w
            eInfo.AddEventHandler(targetObject, wrapper);
        }

        private void LinkOp(Object ie, Part part)
        {
            ArrayList al = new ArrayList();
            ((Op)ie).GetEvents(al);

            foreach (object o in al)
            { // constant, property, reference, call, op, event
                if (o is Constant)
                {
                    // does not need to be link to anything?
                }
                else if (o is Property)
                {
                    LinkProperty((Property)o, part);
                }
                else if (o is Reference)
                {
                    //LinkReference(o, part);
                }
                else if (o is Call)
                {
                    //LinkCall(o, part);
                }
                else if (o is Op)
                {
                    LinkOp(o, part);
                }
                else if (o is Event)
                {
                    LinkEvent(o, part);
                }
            }
        }

        private void LinkProperty(Property p, Part part)
        {
            // check if not already linked
            if (!PropertyLinked(p.Name, p.PartName))
            {
                m_propertyLinked[p.Name] = p.PartName;
                //Console.WriteLine("////////////// NOT LINKED: " + p.Name + " // " + p.PartName);

                Part thePart = part.SearchPart(p.PartName);
                if (thePart == null)
                {
                    if (p.PartName == "")
                        Console.WriteLine("Error in behavior specification: no part name given for {0}", p.PartClass);
                    else
                        Console.WriteLine("Error in behavior specification: {0} does not exist for event {0}", p.PartName, p.PartClass);
                    return;
                }
                Object w = (Object)thePart.UiObject;

                // register all events of this part => lots of overhead
                foreach (EventInfo ei in w.GetType().GetEvents())
                {
                    //Console.WriteLine("Adding event {0} to {1}", ei.Name, thePart.UiObject.GetType());
                    try
                    {
                        ei.AddEventHandler(thePart.UiObject, m_conditions.CreateHandlerWrapper(ei.Name, p.PartName));
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private bool PropertyLinked(string name, string theClass)
        {
            if (m_propertyLinked.ContainsKey(name))
            {
                return ((string)m_propertyLinked[name]) == theClass;
            }

            return false;
        }

        private const string EXECUTE_METHOD = "Execute";
	}
}
