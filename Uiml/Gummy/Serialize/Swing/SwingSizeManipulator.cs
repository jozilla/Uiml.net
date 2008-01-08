using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingSizeManipulator : SizeManipulator
    {
        public SwingSizeManipulator(DomainObject dom) : base(dom)
        {
           
        }

        public override Size Size
        {
            get
            {
                return new Size(0,0);
            }
            set
            {
                ;
            }
        }

        public override object Clone()
        {
            SwingSizeManipulator clone = new SwingSizeManipulator(null);
            return clone;
        }
    }
}
