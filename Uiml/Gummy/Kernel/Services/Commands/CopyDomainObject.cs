using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class CopyDomainObject : ACommand
    {   
        private DomainObject m_dom = null;

        public CopyDomainObject()
            : base()
        {            
            Label = "copy";
        }

        public CopyDomainObject(DomainObject dom) : this()
        {
            DomainObject = dom;
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_dom;
            }
            set
            {
                m_dom = value;
            }
        }

        public override void Execute()
        {
            if (m_dom != null)
            {
                Selected.SelectedDomainObject.Instance.ClipBoardDomainObject = (DomainObject)m_dom.Clone();
            }
            else if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Selected.SelectedDomainObject.Instance.ClipBoardDomainObject = (DomainObject)Selected.SelectedDomainObject.Instance.Selected.Clone();
            }            
        }

        public override void Undo()
        {
            //Nothing to do
        }

        public override bool Enabled
        {
            get
            {
                return m_dom != null || Selected.SelectedDomainObject.Instance.Selected != null; 
            }
            set
            {                
            }
        }
    }
}
