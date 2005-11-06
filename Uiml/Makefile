CLASSPATH=.:../../..
OPTIONS=-g
GTKLIBS= -pkg:gtk-sharp-2.0 
WXLIBS=/r:wx.NET.dll /r:System.Drawing
SWFLIBS=/r:System.Windows.Forms.dll /r:System.Drawing
XMLRPCLIBS=/r:XmlRpcCS.dll
#INLINESCRIPTLIBS=/r:Nemerle.Compiler /r:Boo.Lang.CodeDom
INLINESCRIPTLIBS=-pkg:nemerle -pkg:boo
#LIBS=-pkg:gtk-sharp
DEBUG=-debug+ -warn:2
#LIBS=-gtk -L/usr/local/lib/

all: lib exe
	@echo " building gtk backend"
	@make gtk || true
	@echo " done"
	@echo " building wx backend"
	@make wx || true
	@echo " done"
	@echo " building swf backend"
	@make swf || true
	@echo " done"
	@echo " building compact swf backend"
	@make cswf || true
	@echo " done"
	@echo " building xml-rpc support"
	@make xml-rpc || true
	@echo " done"

#exe :
#	mcs ${DEBUG} -out:uiml.net.exe -target:exe /r:uiml.net.dll  -main:Uiml.UimlTool UimlTool.cs 
#	cscc -out:uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs Peers/*.cs
exe:
	@echo "building executable" 
	mcs ${DEBUG} ${INLINESCRIPTLIBS} ${GTKLIBS} -keyfile:uiml.net.snk -out:uiml.net.exe -target:exe *.cs Rendering/*.cs\
 	Executing/*.cs Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs\
	Peers/*.cs Executing/Binding/*.cs FrontEnd/*.cs -main:Uiml.FrontEnd.UimlTool
	@echo "done"

cli-exe:
	@echo "building executable" 
	mcs ${DEBUG} -keyfile:uiml.net.snk -out:uiml.net.exe -target:exe *.cs Rendering/*.cs Executing/*.cs\
	Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs\
	Peers/*.cs Executing/Binding/*.cs FrontEnd/*.cs -main:Uiml.FrontEnd.CommandLine
	@echo "done"

lib :
	@echo "building library"
	mcs ${DEBUG} ${INLINESCRIPTLIBS} -keyfile:uiml.net.snk -out:uiml.net.dll -target:library *.cs Rendering/*.cs Executing/*.cs\
	Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs Peers/*.cs Executing/Binding/*.cs
	@echo "done"

gtk:
	mcs ${DEBUG} ${GTKLIBS} -keyfile:Rendering/GTKsharp/uiml-gtk-sharp.snk /r:uiml.net.dll -out:uiml-gtk-sharp.dll -target:library Rendering/GTKsharp/*.cs

wx:
	mcs ${DEBUG} ${WXLIBS} -keyfile:Rendering/WXnet/uiml-wx-net.snk /r:uiml.net.dll -out:uiml-wx-net.dll -target:library Rendering/WXnet/*.cs

swf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -out:uiml-swf.dll -target:library Rendering/SWF/*.cs

cswf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -out:uiml-compact-swf.dll -target:library Rendering/CompactSWF/*.cs

xml-rpc:
	mcs ${DEBUG} ${XMLRPCLIBS} -r:uiml.net.dll -out:uiml-xml-rpc.dll -target:library Executing/Callers/XmlRpcCaller.cs

clean:
	rm -f *.exe *.dll *.mdb
