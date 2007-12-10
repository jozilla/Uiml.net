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

		/*public object Clone()
		{			

            return null;
		}*/

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
                /*
				if(ActiveSerializer.Instance.Serializer.GetPropertyConverter().IsPositionProperty(m_property))
					return	WidgetModel.RelativePosition.X+","+WidgetModel.RelativePosition.Y;

				if(m_property.SubProp != null)
					return m_property.SubProp;
				if(m_property.Value != null)
					return	m_property.Value;
				else*/
					return "";
			}
			set
			{
                /*
				if(value == null)
					Console.WriteLine("Setting a zero value ???");
				if(ActiveSerializer.Instance.Serializer.GetPropertyConverter().IsPositionProperty(m_property)){
					int x = 0, y = 0;
					String val = value.ToString();
					String[] splitted = val.Split(new Char[]{','});
					if(splitted.Length == 2)
					{
						x = Convert.ToInt32(splitted[0]);
						y = Convert.ToInt32(splitted[1]);
						WidgetModel.RelativePosition = new Point(x,y);
						m_property.Value = "2,2";
					}
					m_property.Value = "2,2";
				}
				else{
					if(value is String){
						//Console.WriteLine("The value of the property is set 99999");
						m_property.Value = value;
					}
					else{
						//Console.WriteLine("The subprop of the property is set 99999");
						m_property.SubProp = value;
						m_property.Value = value;
					}
					WidgetModel.Dirty = true;
				}
				NotifyViews();*/
			}
		}
/*
		public override string ToString()
		{
			return "\t\t\t<property part-name=\""+m_property.PartName+"\" name=\""+m_property.Name+"\">"+Value+"</property>\n";
		}*/
	}
}
