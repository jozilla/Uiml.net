using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class CutDomainObject : ACommand
    {
        CopyDomainObject m_copyCommand = null;
        DeleteDomainObject m_deleteCommand = null;

        public CutDomainObject()
            : base()
        {
            m_copyCommand = new CopyDomainObject();
            m_deleteCommand = new DeleteDomainObject();
            Label = "cut";
        }

        public CutDomainObject(DomainObject dom) : this()
        {
            m_copyCommand.DomainObject = dom;
            m_deleteCommand.DomainObject = dom;
        }

        public override void Execute()
        {
            m_copyCommand.Execute();
            m_deleteCommand.Execute();
        }

        public override void Undo()
        {            
        }

        public override bool Enabled
        {
            get
            {
                return m_copyCommand.Enabled && m_deleteCommand.Enabled;
            }
            set
            {
                ;
            }
        }
    }
}
