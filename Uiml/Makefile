CLASSPATH=.:../../..
OPTIONS=-g
LIBS=-L /usr/local/lib/mono/gtk-sharp/ /r:gtk-sharp.dll /r:gdk-sharp.dll /r:glib-sharp.dll /r:pango-sharp.dll 
#LIBS=-pkg:gtk-sharp
DEBUG=-debug+ -warn:2
#LIBS=-gtk -L/usr/local/lib/

Uiml2GtkSharp.exe :
	mcs ${DEBUG} ${LIBS} -o uiml.net.exe *.cs Rendering/*.cs Rendering/GTKsharp/*.cs Executing/*.cs Utils/*.cs Peers/*.cs
#	cscc -o uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs Peers/*.cs


clean:
	rm uiml.net.exe
