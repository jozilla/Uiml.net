CLASSPATH=.:../../..
OPTIONS=-g
GTKLIBS=-L /usr/local/lib/mono/gtk-sharp/ /r:gtk-sharp.dll /r:gdk-sharp.dll /r:glib-sharp.dll /r:pango-sharp.dll 
#LIBS=-pkg:gtk-sharp
DEBUG=-debug+ -warn:2
#LIBS=-gtk -L/usr/local/lib/

all: lib exe

exe :
	mcs ${DEBUG} -o uiml.net.exe -target:exe /r:uiml.net.dll  UimlTool.cs # Rendering/*.cs Executing/*.cs Utils/*.cs Peers/*.cs Executing/Binding/*.cs
#	cscc -o uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs Peers/*.cs

lib :
	mcs ${DEBUG} -o uiml.net.dll -target:library *.cs Rendering/*.cs Executing/*.cs Utils/*.cs Peers/*.cs Executing/Binding/*.cs

gtk:
	mcs ${DEBUG} ${GTKLIBS} /r:uiml.net.dll -o uiml-gtk-sharp.dll -target:library Rendering/GTKsharp/*.cs

wx:
	mcs ${DEBUG} ${WXLIBS} /r:uiml.net.dll -o uiml-gtk-sharp.dll -target:library Rendering/WXnet/*.cs

swf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -o uiml-gtk-sharp.dll -target:library Rendering/SWF/*.cs

cswf:
	mcs ${DEBUG} ${SWFLIBS} /r:uiml.net.dll -o uiml-gtk-sharp.dll -target:library Rendering/CompactSWF/*.cs



clean:
	rm uiml.net.exe
