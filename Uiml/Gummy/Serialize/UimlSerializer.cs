using System;
using System.Collections.Generic;
using System.Text;

using Uiml;
using Uiml.Peers;
using Uiml.Gummy.Domain;
using System.Drawing;

namespace Uiml.Gummy.Serialize
{
    public abstract class UimlSerializer : IUimlSerializer
    {
        protected Vocabulary m_voc = null; 

        public abstract Image Serialize(DomainObject dom);

        public bool ThumbnailCallback()
        {
            return false;
        }

        protected Image SerializeToIcon(DomainObject dom, Size controlSize, Size imgSize)
        {
            DomainObject cloned = (DomainObject)dom.Clone();
            cloned.Size = controlSize;
            Image icon = Serialize(cloned);
            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            if (icon.Size.Width > imgSize.Width && icon.Size.Height > imgSize.Height)
            {
                icon = icon.GetThumbnailImage(imgSize.Width, imgSize.Height, myCallback, IntPtr.Zero);
            }
            return icon;
        }

        public virtual Image SerializeToIcon(DomainObject dom)
        {
            return SerializeToIcon(dom, new Size(30, 30), new Size(30, 30));
        }

        public abstract DomainObject Create();

        public abstract DomainObject CreateUIContainer();
        
        public abstract bool Accept(DClass dclass);
        
        public abstract bool Accept(DProperty dprop, DClass dclass);
        
        public abstract object DefaultValue(Property p, Part part, string type);
                
        public virtual Vocabulary Voc
        {
            get
            {
                return m_voc;
            }
        }
    }
}
