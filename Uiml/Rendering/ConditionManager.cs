using System;
using System.Collections;
using System.Text;

using Uiml.Executing;

namespace Uiml.Rendering
{
    public class ConditionManager
    {
        private ArrayList m_conditions;
        private Hashtable m_eventsTriggered;
        private const int TIMEOUT = 5000; // 5 seconds

        public ConditionManager()
        {
            m_conditions        = new ArrayList();
            m_eventsTriggered   = new Hashtable();
        }

        public void Add(IEventLink sel)
        {
            if (!m_conditions.Contains(sel))
            {
                m_conditions.Add(sel);
                //Console.WriteLine("//// CONDITIONS COUNT: " + m_conditions.Count);
            }
        }

        public int Count
        {
            get { return m_conditions.Count; }
        }

        public void Execute(Object sender, EventArgs e, string eventName, string partName)
        {
                CheckConditionsInterested(eventName, partName);
        }

        virtual public EventHandler CreateHandlerWrapper(string concreteEventName, string partName)
        {
            return
            delegate(object sender, EventArgs e)
            {
                Execute(sender, e, concreteEventName, partName);
            };
        }

        private void CheckConditionsInterested(string eventName, string partName)
        {
            try
            {
                AddEventTriggered(eventName, partName);
                IEnumerator en = m_conditions.GetEnumerator();
                while (en.MoveNext())
                {
                    ((IEventLink)en.Current).EventTriggered(m_eventsTriggered, partName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void AddEventTriggered(string eventName, string partName)
        {
            long now = DateTime.UtcNow.Ticks;

            Hashtable temp = new Hashtable(); // to copy stuff => otherwise some stupid exceptions
            foreach(DictionaryEntry en in m_eventsTriggered)
            {
                // delete timeout events
                foreach(DictionaryEntry en2 in ((Hashtable)en.Value))
                {
                    if (((new DateTime((long)en2.Value)).AddMilliseconds(TIMEOUT)).Ticks >= now)
                    {
                        if (!temp.ContainsKey(en.Key))
                            temp[en.Key] = new Hashtable();
                        if (!((Hashtable)temp[en.Key]).ContainsKey(en2.Key))
                            ((Hashtable)temp[en.Key]).Add(en2.Key, now);
                    }
                }
            }

            m_eventsTriggered = temp;

            if (!m_eventsTriggered.ContainsKey(eventName))
                m_eventsTriggered.Add(eventName, new Hashtable());

            if (((Hashtable)m_eventsTriggered[eventName]).ContainsKey(partName))
                ((Hashtable)m_eventsTriggered[eventName]).Remove(partName);

            ((Hashtable)m_eventsTriggered[eventName]).Add(partName, now);
        }

        static public string GetEventName(string temp)
        {
            string[] splitted = temp.Split(new char[] { '.' });
            return splitted[splitted.Length-1];
        }
    }
}
