using System;
using System.IO;
using Uiml;
using Uiml.Peers;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Visual;


namespace Uiml.Gummy.Serialize
{

	public interface IUimlSerializer
	{		
		//Serializes the domainobject directly
        Image Serialize(DomainObject dom);
        //Serializes the domainobject to an icon which can be displayed in the toolbox
        Image SerializeToIcon(DomainObject dom);
        //Creates an empty domain object
        DomainObject Create();
        //Accept the DClass (may appear in the toolbox)
        bool Accept(DClass dclass);
        //Accept the DProperty
        bool Accept(DProperty dprop, DClass dclass);
        //Default value for the DClass and DProperty
        object DefaultValue(Property p, Part part, string type);
		
        //Underlying vocubulary
		Vocabulary Voc
		{
			get;
		}
    }
}
