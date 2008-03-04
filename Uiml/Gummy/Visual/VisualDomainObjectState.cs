using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Visual
{
    public abstract class VisualDomainObjectState
    {
        protected VisualDomainObject m_visDom = null;

        public VisualDomainObjectState()
        {            
        }

        ~VisualDomainObjectState()
        {
            finalize();
        }

        private void finalize()
        {
            if (m_visDom == null)
                return;
        }

        public virtual void Attach(VisualDomainObject visDom)
        {
            Detach();
            m_visDom = visDom;           
        }

        public virtual void Detach()
        {
            finalize();
        }       
    }
}
