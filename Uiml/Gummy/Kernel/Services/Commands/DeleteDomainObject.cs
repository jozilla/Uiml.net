using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class DeleteDomainObject : ACommand
    {
        private DomainObject m_domObject = null;        

        public DeleteDomainObject()
            : base()
        {
            Label = "delete";
        }

        public DeleteDomainObject(DomainObject dom) : this()
        {
            DomainObject = dom;
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_domObject;
            }
            set
            {
                m_domObject = value;
            }
        }

        public override void Execute()
        {            
            if (m_domObject != null)
                DesignerKernel.Instance.CurrentDocument.DomainObjects.Remove(m_domObject);            
            else if(Selected.SelectedDomainObject.Instance.Selected != null)
                DesignerKernel.Instance.CurrentDocument.DomainObjects.Remove(Selected.SelectedDomainObject.Instance.Selected);
        }

        public override void Undo()
        {   
            //Nothing yet...
        }

        public override bool Enabled
        {
            get
            {
                return m_domObject != null || Selected.SelectedDomainObject.Instance.Selected != null;
            }
            set
            {               
            }
        }
    }
}
