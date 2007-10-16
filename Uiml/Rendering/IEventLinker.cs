using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Rendering
{
	public interface IEventLinker
	{
        void Link(Structure uiStruct, Behavior uiBehavior);
	}
}
