CLASSPATH=.:../../..
OPTIONS=-g
GTKLIBS= -pkg:gtk-sharp 
WXLIBS=/r:wx.NET.dll /r:System.Drawing
SWFLIBS=/r:System.Windows.Forms.dll /r:System.Drawing
#LIBS=-pkg:gtk-sharp
DEBUG=-debug+ -warn:2
#LIBS=-gtk -L/usr/local/lib/

all: lib exe
	@make gtk || true
	@make wx || true
	@make swf || true
	@make cswf || true

#exe :
#	mcs ${DEBUG} -o uiml.net.exe -target:exe /r:uiml.net.dll  -main:Uiml.UimlTool UimlTool.cs 
#	cscc -o uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs Peers/*.cs
exe:
	mcs ${DEBUG} -pkg:gtk-sharp -keyfile:uiml.net.snk -o uiml.net.exe -target:exe *.cs Rendering/*.cs Executing/*.cs Utils/*.cs\
	Peers/*.cs Executing/Binding/*.cs FrontEnd/UimlTool.cs


lib :
	mcs ${DEBUG} -keyfile:uiml.net.snk -o uiml.net.dll -target:library *.cs Rendering/*.cs Executing/*.cs Utils/*.cs\
	Peers/*.cs Executing/Binding/*.cs

gtk:
	mcs ${DEBUG} ${GTKLIBS} -keyfile:Rendering/GTKsharp/uiml-gtk-sharp.snk /r:uiml.net.dll -o uiml-gtk-sharp.dll -target:library Rendering/GTKsharp/*.cs

wx:
	mcs ${DEBUG} ${WXLIBS} -keyfile:Rendering/WXnet/uiml-wx-net.snk /r:uiml.net.dll -o uiml-wx-net.dll -target:library Rendering/WXnet/*.cs

swf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -o uiml-swf.dll -target:library Rendering/SWF/*.cs

cswf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -o uiml-compact-swf.dll -target:library Rendering/CompactSWF/*.cs



clean:
	rm *.exe *.dll *.mdb
