using System;
using System.Collections.Generic;
using System.Text;
using Uiml.Gummy.Serialize;
using Uiml.Gummy.Domain;
using Uiml;
using System.Drawing;
using Uiml.Rendering;

namespace Uiml.Gummy.Serialize.SWF
{
    public class SWFPositionManipulator : PositionManipulator
    {
        public Property m_positionProperty = null;

        public SWFPositionManipulator(DomainObject dom) : base(dom)
        {
            checkProperties();
        }


        public override object Clone()
        {
            SWFPositionManipulator clone = new SWFPositionManipulator(null);
            return clone;
        }

        private void checkProperties()
        {
            if (DomainObject == null)
                return;
            if (m_positionProperty == null)
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
