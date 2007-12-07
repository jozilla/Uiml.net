using System;
using System.IO;
using Uiml;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Uiml.Gummy.Domain;
using Uiml.Rendering.SWF;
using Uiml.Peers;


namespace Uiml.Gummy.Serialize
{

	using System.Runtime.InteropServices;

	public class ActiveSerializer
	{
		private IUimlSerializer m_serializer = null;
		private static ActiveSerializer m_aSerializer = null;

		protected ActiveSerializer() : base()
		{
		}

		public static ActiveSerializer Instance
		{
			get
			{
				if(m_aSerializer == null)
					m_aSerializer = new ActiveSerializer();
				return m_aSerializer;
			}
		}

		public IUimlSerializer Serializer
		{
			get
			{
				return m_serializer;
			}
			set
			{
				m_serializer = value;
			}
		}
	}
}
