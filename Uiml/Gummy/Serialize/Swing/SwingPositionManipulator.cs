using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Serialize.Swing
{
    public class SwingPositionManipulator : PositionManipulator
    {public Property m_positionProperty = null;

        public SwingPositionManipulator(DomainObject dom)
            : base(dom)
        {
            checkProperties();
        }


        public override object Clone()
        {
            SwingPositionManipulator clone = new SwingPositionManipulator(null);
            return clone;
        }

        private void checkProperties()
        {
            if (DomainObject == null)
                return;
            if (m_positionProperty == null || !DomainObject.Properties.Contains(m_positionProperty))
                m_positionProperty = DomainObject.FindProperty(IAM);
        }

        public override System.Drawing.Point Position
        {
            get
            {
                checkProperties();
                string position = (string)m_positionProperty.Value;
                string[] stringpos = position.Split(new char[] { ',' });
                Point pnt = new Point(Convert.ToInt32(stringpos[0]), Convert.ToInt32(stringpos[1]));
                return pnt;
            }
            set
            {
                checkProperties();
                m_positionProperty.Value = value.X + "," + value.Y;
            }
        }

        public static string IAM = "position";
    }
}
