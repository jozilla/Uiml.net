using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Rendering
{
	public interface IEventLink
	{
        void EventTriggered(Hashtable eventsTriggered, string partName);
	}
}
