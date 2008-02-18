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
        public Property m_sizeProperty = null;

        public SwingSizeManipulator(DomainObject dom) : base(dom)
        {
            checkProperties();
        }

        public override Size Size
        {
            get
            {
                checkProperties();
                string size = (string)m_sizeProperty.Value;
                string[] stringsize = size.Split(new char[] { ',' });
                Size sz = new Size(Convert.ToInt32(stringsize[0]), Convert.ToInt32(stringsize[1]));
                return sz;
            }
            set
            {
                checkProperties();
                m_sizeProperty.Value = value.Width + "," + value.Height;
            }
        }

        private void checkProperties()
        {
            if (DomainObject == null)
                return;
            if (m_sizeProperty == null || !DomainObject.Properties.Contains(m_sizeProperty))
                m_sizeProperty = DomainObject.FindProperty(IAM);
        }

        public override object Clone()
        {
            SwingSizeManipulator clone = new SwingSizeManipulator(null);
            return clone;
        }

        public static string IAM = "size";
    }
}
