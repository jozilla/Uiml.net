using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Peers;
using System.Collections;

using Uiml.Rendering;
using Uiml.Gummy.Serialize;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class VocabularyMetadata
    {
        private Vocabulary m_voc;
        private Dictionary<string, string> m_events = new Dictionary<string,string>();
        private Dictionary<string, string> m_inputs = new Dictionary<string,string>();
        private Dictionary<string, string> m_outputs = new Dictionary<string, string>();
        private Dictionary<string, DClassType> m_types = new Dictionary<string,DClassType>();

        public VocabularyMetadata(Vocabulary voc)
        {
            m_voc = voc;

            if (voc.Identifier == "swf-1.1")
            {
                // FIXME: hard-coded for now

                /*m_types.Add("Button", DClassType.Action);
                m_types.Add("Text", DClassType.Input);
                m_types.Add("Text", DClassType.Output);
                m_types.Add("Entry", DClassType.Input);
                m_types.Add("Entry", DClassType.Output);*/

                m_events.Add("Button", "ButtonPressed");
                m_inputs.Add("Entry", "text");
                m_outputs.Add("Entry", "text");
                m_inputs.Add("Text", "text");
                m_outputs.Add("Text", "text");
                m_outputs.Add("List", "content");
                m_outputs.Add("Label", "background");// FIXME: just for testing
            }
        }

        public string GetEvent(string dclass)
        {
            return m_events[dclass];
        }

        public string GetInputProperty(string dclass, Type propertyType)
        {
            List<DProperty> props = new List<DProperty>();
            props.AddRange((DProperty[]) (((DClass)m_voc.DClasses[dclass]).Search(typeof(DProperty)).ToArray(typeof(DProperty))));
            
            // check return type
            List<DProperty> results = new List<DProperty>();
            foreach (DProperty prop in props)
            {
                if (prop.Identifier == m_inputs[dclass])
                {
                    if (prop.MapsType == DProperty.GET_METHOD)
                    {
                        if (Type.GetType(prop.ReturnType) == propertyType)
                        {
                            return m_inputs[dclass];
                        }
                        else
                        {
                            // TODO: check for typedecoder
                        }
                    }
                }
            }

            return null;
        }

        public string GetOutputProperty(string dclass, Type propertyType)
        {
            // TODO: check for typedecoder
            List<DProperty> props = new List<DProperty>();
            props.AddRange((DProperty[])(((DClass)m_voc.DClasses[dclass]).Search(typeof(DProperty)).ToArray(typeof(DProperty))));

            // check all params 
            List<DProperty> results = new List<DProperty>();
            foreach (DProperty prop in props)
            {
                if (prop.Identifier == m_inputs[dclass])
                {
                    if (prop.MapsType == DProperty.SET_METHOD)
                    {
                        if (prop.Children.Count == 1) // only handle single parameter children for now
                        {
                            if (Type.GetType(((DParam)prop.Children[0]).Type) == propertyType)
                            {
                                return m_outputs[dclass];
                            }
                            else
                            {
                                try
                                {
                                    // get current renderer, and check for type decoder
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public enum DClassType 
        {
            Action,
            Input,
            Output
        }
    }
}
