using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;

using Uiml.Executing;
using System.Reflection;

namespace Uiml.Rendering
{
	public class ConditionChecker
    {
        private Hashtable   m_eventsTriggered;
        private Stack       m_propertyValues;
        private Part        m_part;
        private IExecutable m_exer;
        private IRenderer   m_renderer;

        public ConditionChecker(Condition c, IRenderer renderer, Part p)
        {
            m_eventsTriggered = new Hashtable();
            m_propertyValues  = new Stack();
            m_part            = p;
            m_exer            = c;
            m_renderer        = renderer;
        }

        /// <summary>
        /// An event was triggered
        /// </summary>
        /// <param name="eventsTriggered">Events that were triggered during execution</param>
        /// <param name="partName">The partname where to check</param>
        /// <param name="executed">Is the condition already executed? See if it needs a reset</param>
        /// <returns>0 if no action is needed, 1 if exceeded, -1 if not exceeded</returns>
        public int CheckCondition(Hashtable eventsTriggered, string partName, bool executed)
        {
            //Console.WriteLine("==== PARTNAME: " + partName);
            int evaluated = 0;

            // save local copy
            m_eventsTriggered = eventsTriggered;
            // check condition
            bool temp = false;
            if ((temp = CheckCondition(partName)) && !executed) // condition == true && not yet executed
            {
                evaluated = 1;
                m_exer.Execute(m_renderer);
            }
            else if (temp && ((Condition)m_exer).ConditionType == Condition.EVENT) // otherwise a click on button, will only be executed once!!
            {
                evaluated = 1;
                m_exer.Execute(m_renderer);
            }
            else if (!temp)
            {
                evaluated = -1;
            }

            return evaluated;
        }

        /// <summary>
        /// Check if the condition evaluates to true
        /// </summary>
        /// <returns>Condition is true or false</returns>
        private bool CheckCondition(string partName)
        {
            try
            {
                Condition c = (Condition)m_exer;

                if (c.ConditionType == Condition.EVENT)
                {
                    //Console.WriteLine(" CHECKING EVENT");
                    return CheckEvent(partName);
                }
                else if (c.ConditionType == Condition.OPERATOR)
                {
                    //Console.WriteLine(" CHECKING OPERATOR");
                    Op op = (Op)((Condition)m_exer).ConditionObject;
                    return CheckOp(op, partName);
                }
                else if (c.ConditionType == Condition.EQUAL)
                {
                    //Console.WriteLine(" CHECKING EQUAL");
                    return CheckEqual((Equal)((Condition)m_exer).ConditionObject, partName);
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// Check if an event is triggered
        /// </summary>
        /// <returns>Event triggered?</returns>
        private bool CheckEvent(string partName)
        {
            // check if event is triggered
            string tempPartName = ((Event)((Condition)m_exer).ConditionObject).PartName;
            string tempClass = ((Event)((Condition)m_exer).ConditionObject).Class;
            string tempMapsTo = Renderer.Voc.MapsOnCls(((Event)((Condition)m_exer).ConditionObject).Class);
            string name = ConditionManager.GetEventName(tempMapsTo);

            // FIXME: clean this up so it fits better in the entire code
            if (tempClass == "init")
            {
                // special case: part name doesn't matter!
                return true;
            }

            if (tempPartName == partName && m_eventsTriggered.ContainsKey(name))
                if (((Hashtable)m_eventsTriggered[name]).ContainsKey(partName))
                    return true;
                else
                    return false;
            else
                return false;
        }

        /// <summary>
        /// Check an operator
        /// </summary>
        /// <returns>Operator evaluates to true?</returns>
        private bool CheckOp(Op op, string partName)
        {
            // constant, property, reference, call, op, event
            // get type
            string type = op.Type;

            if (type == Op.AND)
            {
                bool valid = true;
                foreach (object o in op.Children) // check foreach object if true
                {
                    valid = valid && CheckIfTriggered(o, partName);
                }

                return valid;
            }
            else if (type == Op.OR)
            {
                bool valid = false;
                foreach (object o in op.Children) // check foreach object if true
                {
                    valid = valid || CheckIfTriggered(o, partName);
                }

                return valid;
            }
            else if (type == Op.EQUAL)
            {
                bool valid = false;
                // all children must come to the same value
                object value = "";
                bool first = true;
                foreach (object o in op.Children)
                {
                    if (CheckIfTriggered(o, partName))
                    {
                        if (first) // first child
                        {
                            value = GetValue(o, partName);
                            if (value != null && value.GetType().Name == "Boolean")
                                value = value.ToString().ToLower();
                            first = false;
                        }
                        else
                        {
                            /*Console.WriteLine("Value: " + value);
                            Console.WriteLine("Value: " + GetValue(o, partName));
                            Console.WriteLine("Equal? " + value.Equals(GetValue(o, partName)));
                            Console.WriteLine("");*/

                            object temp = GetValue(o, partName);
                            if (temp != null && temp.GetType().Name == "Boolean")
                                temp = temp.ToString().ToLower();

                            if (value == null)
                                valid = false;
                            else
                                valid = (value.Equals(temp));
                        }
                    }
                    else
                        valid = false;
                }

                return valid;
            }
            else if (type == Op.NOTEQUAL)
            {
                bool valid = false;
                // all children must come to the same value
                object value = "";
                bool first = true;
                foreach (object o in op.Children)
                {
                    if (CheckIfTriggered(o, partName))
                    {
                        if (first) // first child
                        {
                            value = GetValue(o, partName);
                            if (value != null && value.GetType().Name == "Boolean")
                                value = value.ToString().ToLower();
                            first = false;
                        }
                        else
                        {
                            /*Console.WriteLine("Value: " + value);
                            Console.WriteLine("Value: " + GetValue(o, partName));
                            Console.WriteLine("Equal? " + value.Equals(GetValue(o, partName)));
                            Console.WriteLine("");*/

                            object temp = GetValue(o, partName);
                            if (temp != null && temp.GetType().Name == "Boolean")
                                temp = temp.ToString().ToLower();

                            if (value == null)
                                valid = false;
                            else
                                valid = !(value.Equals(temp));
                        }
                    }
                    else
                        valid = false;
                }

                return valid;
            }
            else if (type == Op.LESSTHAN)
            {
                bool lessThan = false;
                object value = "";
                bool first = true;

                foreach (object o in op.Children)
                {
                    if (CheckIfTriggered(o, partName))
                    {
                        if (first)
                        {
                            value = GetValue(o, partName);
                            if (value != null && value.GetType().Name == "Boolean")
                                value = value.ToString().ToLower();
                            first = false;
                        }
                        else
                        {
                            object temp = GetValue(o, partName);
                            if (temp != null && temp.GetType().Name == "Boolean")
                                temp = temp.ToString().ToLower();

                            if (value == null)
                                lessThan = false;
                            else
                            {
                                int result = temp.ToString().CompareTo(value.ToString()); // 1 if 'temp' is greater than 'value'
                                if (result == 1)
                                    lessThan = true;
                                else
                                    lessThan = false;
                            }
                        }
                    }
                    else
                        lessThan = false;
                }

                return lessThan;
            }
            else if (type == Op.GREATERTHAN)
            {
                bool greaterThan = false;
                object value = "";
                bool first = true;

                foreach (object o in op.Children)
                {
                    if (CheckIfTriggered(o, partName))
                    {
                        if (first)
                        {
                            value = GetValue(o, partName);
                            if (value != null && value.GetType().Name == "Boolean")
                                value = value.ToString().ToLower();
                            first = false;
                        }
                        else
                        {
                            object temp = GetValue(o, partName);
                            if (temp != null && temp.GetType().Name == "Boolean")
                                temp = temp.ToString().ToLower();

                            if (value == null)
                                greaterThan = false;
                            else
                            {
                                int result = value.ToString().CompareTo(temp.ToString()); // 1 if 'value' is greater than 'temp'
                                if (result == 1)
                                    greaterThan = true;
                                else
                                    greaterThan = false;
                            }
                        }
                    }
                    else
                        greaterThan = false;
                }

                return greaterThan;
            }

            return false;
        }

        /// <summary>
        /// Check an equal element
        /// </summary>
        /// <param name="e">The element to check</param>
        /// <param name="partName">The part name</param>
        /// <returns>True if 2 child elements are equal</returns>
        private bool CheckEqual(Equal e, string partName)
        {
            bool valid = false;
            // all children must come to the same value
            object value = "";
            bool first = true;
            foreach (object o in e.Children)
            {
                if (CheckIfTriggered(o, partName))
                {
                    valid = true;
                    if (first) // first child
                        value = GetValue(o, partName);
                    else
                        valid = (value.Equals(GetValue(o, partName)));
                }
                else
                    valid = false;
            }

            return valid;
        }

        private object GetValue(object o, string partName)
        {
            if (o is Property)
            {
                // get property value
                // check if property is like it needs to be
                Property temp = (Property)o;
                Part p = GetPart(temp.PartName);
                object value = null;
                if (p != null && ((Property)o).PartName == p.Identifier)
                {
                    Object w = (Object)p.UiObject;

                    try
                    {
                        string property = Renderer.Voc.GetPropertyGetter(temp.Name, p.Class);

                        PropertyInfo pi = w.GetType().GetProperty(property);
                        if (pi != null)
                            value = pi.GetValue(w, null); ;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while getting property [{0}] from type [{1}]", temp.Name, p.Class);
                        Console.WriteLine(e.StackTrace);
                    }
                }

                return value;
            }
            else if (o is Constant)
            {
                return ((Constant)o).Value;
            }
            else if (o is Op)
            {
                return false;
            }
            else if (o is Event)
            {
                return false;
            }
            else if (o is Reference)
            {
                return false;
            }
            else if (o is Call)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if an object o is triggered in the interface
        /// </summary>
        /// <param name="o">The object to check</param>
        /// <returns>Triggered or not</returns>
        private bool CheckIfTriggered(object o, string partName)
        {
            //return m_eventsTriggered.Contains(o);
            // check if in table => just check key => otherwise we are not in this class
            if (o is Property)
            {
                return true; // property needs only to be checked with his/her value
            }
            else if (o is Constant)
            {
                return true; // constant is always triggered
            }
            else if (o is Event)
            {
                string temp = Renderer.Voc.MapsOnCls(((Event)o).Class);

                return m_eventsTriggered.ContainsKey(ConditionManager.GetEventName(temp));
            }
            else if (o is Op)
            {
                return CheckOp((Op)o, partName);
            }

            return false;
        }

        private Part GetPart(string partName)
        {
            return GetPart(partName, m_part);
        }

        private Part GetPart(string partName, Part sibling)
        {
            if (sibling.Identifier == partName)
                return sibling;
            else if (sibling.Children.Count == 0)
                return null;
            else
            {
                for (int i = 0; i < sibling.Children.Count; i++)
                {
                    Part curr = GetPart(partName, (Part)sibling.Children[i]);
                    if (curr != null)
                        return curr;
                }

                return null;
            }
        }

        /// <summary>
        /// Get the condition
        /// </summary>
        public Condition Condition
        {
            get { return (Condition)m_exer; }
        }

        /// <summary>
        /// Get the renderer
        /// </summary>
        public IRenderer Renderer
        {
            get { return m_renderer; }
        }
	}
}
