using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;
using Uiml.Peers;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingSerializer : IUimlSerializer
    {
        
        public Image Serialize(DomainObject dom)
        {
            return null;
        }

        public DomainObject Create()
        {
            return null;
        }

        public bool Accept(DClass dclass)
        {
            return false;
        }

        public bool Accept(DProperty dprop, DClass dclass)
        {
            return true;
        }

        public object DefaultValue(Property p, Part part, string type)
        {
            return null;
        }
                
        public Vocabulary Voc
        {
            get
            {
                return null;
            }
        }
    }
}
