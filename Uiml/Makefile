PACKAGE=uimlnet

GACUTIL=gacutil
GACUTIL_FLAGS=/package $(PACKAGE) /gacdir $(DESTDIR)$(prefix)/lib

INSTALL=install
INSTALL_PROGRAM=$(INSTALL)
INSTALL_DATA=$(INSTALL) -m 644

prefix=/usr
exec_prefix=${prefix}
bindir=${exec_prefix}/bin
libdir=$(exec_prefix)/lib
monolibdir=${libdir}/mono/1.0/

CLASSPATH=.:../../..
OPTIONS=-g
GTKLIBS= -pkg:gtk-sharp-2.0 
WXLIBS=-lib:${monolibdir} /r:wx.NET.dll /r:System.Drawing
SWFLIBS=/r:System.Windows.Forms.dll /r:System.Drawing
XMLRPCLIBS=-lib:${monolibdir} /r:XmlRpcCS.dll
#INLINESCRIPTLIBS=/r:Nemerle.Compiler /r:Boo.Lang.CodeDom
#INLINESCRIPTLIBS=#-pkg:nemerle -pkg:boo
#LIBS=-pkg:gtk-sharp
DEBUG=-debug+ -warn:2
#LIBS=-gtk -L/usr/local/lib/
SOLVERLIBS = -lib:${monolibdir} -r:Cassowary -r:Cassowary.Parsing

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
#	cscc -out:uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs \
#	Utils/Reflection/*.cs Peers/*.cs

exe:	
	@echo "building executable" 
	mcs ${DEBUG} ${GTKLIBS} $(SOLVERLIBS) -keyfile:uiml.net.snk -out:uiml.net.exe -target:exe *.cs Rendering/*.cs \
	Executing/*.cs Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs \
	Utils/Reflection/*.cs Peers/*.cs Executing/Binding/*.cs LayoutManagement/*.cs FrontEnd/*.cs -main:Uiml.FrontEnd.UimlTool
	@echo "done"
	@echo "copying uiml files for frontend"
	cp FrontEnd/gtkgui.uiml .
	cp FrontEnd/compactgui.uiml .
	@echo "done"

lib:	
	@echo "building library"
	mcs ${DEBUG} $(SOLVERLIBS) -keyfile:uiml.net.snk -out:uiml.net.dll -target:library *.cs Rendering/*.cs Executing/*.cs \
	Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs Utils/Reflection/*.cs \
	Peers/*.cs Executing/Binding/*.cs LayoutManagement/*.cs
	@echo "done"

solver:
	@echo "building constraint solver"
	make -C ../Cassowary/ parselib lib
	cp ../Cassowary/Cassowary*dll .
	@echo "done"

gtk:
	mcs ${DEBUG} ${GTKLIBS} -keyfile:Rendering/GTKsharp/uiml-gtk-sharp.snk /r:uiml.net.dll -out:uiml-gtk-sharp.dll -target:library Rendering/GTKsharp/*.cs

wx:
	mcs ${DEBUG} ${WXLIBS} -keyfile:Rendering/WXnet/uiml-wx-net.snk /r:uiml.net.dll -out:uiml-wx-net.dll -target:library Rendering/WXnet/*.cs

swf:	
	mcs ${DEBUG} ${SWFLIBS} $(SOLVERLIBS) -keyfile:Rendering/SWF/uiml-swf.snk /r:uiml.net.dll -out:uiml-swf.dll -target:library Rendering/SWF/*.cs LayoutManagement/SWF/*.cs

cswf:
	mcs ${DEBUG} ${SWFLIBS} -keyfile:Rendering/CompactSWF/uiml-compactswf.snk /r:uiml.net.dll -out:uiml-compact-swf.dll -target:library Rendering/CompactSWF/*.cs

xml-rpc:
	mcs ${DEBUG} ${XMLRPCLIBS} -keyfile:uiml-xml-rpc.snk -r:uiml.net.dll -out:uiml-xml-rpc.dll -target:library Executing/Callers/XmlRpcCaller.cs

clean:
	rm -f *.exe *.dll *.mdb compactgui.uiml gtkgui.uiml
	make -C ../Cassowary/ clean


install:
	$(INSTALL) -d ${DESTDIR}${bindir}
	$(INSTALL_PROGRAM) uiml.net.exe $(DESTDIR)$(bindir)/uiml.net.exe
	$(INSTALL) -d ${DESTDIR}${monolibdir}
	$(INSTALL_PROGRAM) uiml.net.dll $(DESTDIR)$(monolibdir)/uiml.net.dll
	$(INSTALL_PROGRAM) uiml-gtk-sharp.dll $(DESTDIR)$(monolibdir)/uiml-gtk-sharp.dll
	$(INSTALL_PROGRAM) uiml-swf.dll $(DESTDIR)$(monolibdir)/uiml-swf.dll
	$(INSTALL_PROGRAM) uiml-compact-swf.dll $(DESTDIR)$(monolibdir)/uiml-compact-swf.dll
	$(INSTALL_PROGRAM) uiml-wx-net.dll $(DESTDIR)$(monolibdir)/uiml-wx-net.dll
	$(INSTALL_PROGRAM) uiml-xml-rpc.dll $(DESTDIR)$(monolibdir)/uiml-xml-rpc.dll

register: register-uiml register-gtk register-swf register-cswf register-wx register-xmlrpc

register-uiml:
	$(GACUTIL) /i uiml.net.dll /f $(GACUTIL_FLAGS) || exit 1;

register-gtk:
	$(GACUTIL) /i uiml-gtk-sharp.dll /f $(GACUTIL_FLAGS) || exit 1;

register-swf:
	$(GACUTIL) /i uiml-swf.dll /f $(GACUTIL_FLAGS) || exit 1;

register-cswf:
	$(GACUTIL) /i uiml-compact-swf.dll /f $(GACUTIL_FLAGS) || exit 1;

register-wx:
	$(GACUTIL) /i uiml-wx-net.dll /f $(GACUTIL_FLAGS) || exit 1;

register-xmlrpc:
	$(GACUTIL) /i uiml-xml-rpc.dll /f $(GACUTIL_FLAGS) || exit 1;

