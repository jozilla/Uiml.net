namespace Uiml.Gummy.Domain
{
	using System;
	using System.IO;
	using Uiml;
	using System.Collections;
	//using Uiml.Gummy.Serialize;
	using System.Windows.Forms;
	using System.Drawing;

	public class DomainObjectAttribute : Property	
	{		
		//The model which contains this property
        private DomainObject m_domainObject = null;

        public DomainObjectAttribute() : base()
		{
		}

        public DomainObjectAttribute(DomainObject dom) : this()
        {
            m_domainObject = dom;
        }
	
		public object Value
		{
			get
			{
                return "";
			}
			set
			{                
			}
		}
	}
}
