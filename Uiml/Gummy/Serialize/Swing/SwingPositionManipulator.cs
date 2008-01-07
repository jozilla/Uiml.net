using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingPositionManipulator : PositionManipulator
    {
        public SwingPositionManipulator(DomainObject obj) : base(obj)
        {
        }

        public override Point Position
        {
            get
            {
                return new Point(2,2);    
            }
            set
            {
                ;
            }
        }

        public override object Clone()
        {
            SwingPositionManipulator clone = new SwingPositionManipulator(null);
            return clone;
        }
    }
}
