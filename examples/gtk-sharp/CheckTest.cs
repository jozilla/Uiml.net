using System;

using Uiml;
using Uiml.Executing.Binding;
using Uiml.Rendering;



public class CheckTest
{
	private System.Boolean gtk = true;
	private System.Boolean swf = false;
	private System.Boolean wxnet = false;

	public CheckTest()
	{
	}

	public System.Boolean Gtk{
		get { return gtk; }
		set { gtk = value; }
	}

	public System.Boolean Swf{
		get { return swf; }
		set { swf = value; }
	}

	public System.Boolean Wxnet{
		get { return wxnet; }
		set { wxnet = value; }
	}

	public string TellMe(bool gtk, bool swf, bool wxnet)
	{
		String str = "";
		if(gtk) str += " A good fellow ";
		if(swf) str += " A masochist ";
		if(wxnet) str += " just plain crazy ";
		return str;
	}

	
		public static int Main (string[] args)
		{
			UimlDocument uimlDoc = new UimlDocument("check.uiml");
			CheckTest ct = new CheckTest();
			uimlDoc.Connect(ct);
			IRenderer renderer = (new BackendFactory()).CreateRenderer(uimlDoc.Vocabulary);
		   IRenderedInstance ri = renderer.Render(uimlDoc);
			ri.ShowIt();
			return 0;
		}

	
}
