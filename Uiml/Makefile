CLASSPATH=.:../../..
OPTIONS=-g
LIBS=/r:gtk-sharp.dll /r:glib-sharp.dll /r:gdk-sharp.dll /r:pango-sharp.dll /r:wx.NET.dll /r:System.Drawing.dll /r:log4net
DEBUG=-debug+ -warn:2

Uiml2GtkSharp.exe : Part.cs Peer.cs Presentation.cs IUimlElement.cs Interface.cs Structure.cs Behavior.cs\
						Style.cs Vocabulary.cs UimlTool.cs UimlDocument.cs Property.cs Options.cs Rendering/IRenderedInstance.cs\
						Rendering/IRenderer.cs Rendering/GTKsharp/GtkRenderedInstance.cs Rendering/GTKsharp/GtkRenderer.cs\
						WrongNestingException.cs VocabularyUnavailableException.cs MappingNotFoundException.cs Param.cs\
						UimlAttributes.cs Constant.cs Rendering/ITypeDecoder.cs Rendering/GTKsharp/GtkTypeDecoder.cs\
						Executing/Action.cs Executing/Condition.cs Executing/Rule.cs Executing/Event.cs Executing/IExecutable.cs\
						Executing/Call.cs Rendering/TypeDecoder.cs Executing/ValueNotEvaluatedException.cs\
						Utils/TodoAttribute.cs Rendering/GTKsharp/GtkEventLink.cs Rendering/GTKsharp/GtkEventLinker.cs\
						Logic.cs Rendering/WXnet/WxRenderedInstance.cs Rendering/WXnet/WxRenderer.cs
	mcs ${DEBUG} ${LIBS} -o uiml.net.exe *.cs Rendering/*.cs Rendering/GTKsharp/*.cs Executing/*.cs Utils/*.cs Rendering/WXnet/*.cs

clean:
	rm uiml.net.exe
