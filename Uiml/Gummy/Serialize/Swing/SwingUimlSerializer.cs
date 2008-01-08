using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;
using Uiml.Peers;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingUimlSerializer : IUimlSerializer
    {
        public SwingUimlSerializer()
		{
		}

        public DomainObject Create()
        {
            DomainObject domObj = new DomainObject();
            domObj.PositionManipulator = new SwingPositionManipulator(domObj);
            domObj.SizeManipulator = new SwingSizeManipulator(domObj);

            return domObj;
        }
               
        public Image Serialize(DomainObject dom)
		{
            //Connection with the java renderer needs to be made here
            return null;
		}


        public Vocabulary Voc
        {
            get
            {
                return null;
            }
        }

        public bool Accept(DClass dclass)
        {
            return true;
        }

        public bool Accept(DProperty dprop, DClass dclass)
        {
            return false;
        }

        public object DefaultValue(Property p, Part part, string type)
        {
            return null;
        }

        public const string NAME = "swing-1.5";
    }
}
