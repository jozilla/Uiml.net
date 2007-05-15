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

COMPILER=gmcs
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
	@echo " building compact library"
	@make lib-cf || true
	@echo " done"
	@echo " building compact swf backend"
	@make swf-cf || true
	@echo " done"
	@echo " building xml-rpc support"
	@make xml-rpc || true
	@echo " done"

#exe :
#	mcs ${DEBUG} -out:uiml.net.exe -target:exe /r:uiml.net.dll  -main:Uiml.UimlTool UimlTool.cs 
#	cscc -out:uiml.net.exe -l System.Xml -l System.Windows.Forms *.cs Rendering/*.cs Rendering/SWF/*.cs Executing/*.cs Utils/*.cs \
#	Utils/Reflection/*.cs Peers/*.cs

exe: lib
	@echo "building executable" 
	${COMPILER} ${DEBUG} $(SOLVERLIBS) -keyfile:../uiml.net.snk -out:uiml.net.exe -target:exe -r:uiml.net-core.dll \
	FrontEnd/*.cs -main:Uiml.FrontEnd.UimlTool
	@echo "done"
	@echo "copying uiml files for frontend"
	cp FrontEnd/gtkgui.uiml .
	cp FrontEnd/compactgui.uiml .
	@echo "done"

lib: solver
	@echo "building library"
	${COMPILER} ${DEBUG} $(SOLVERLIBS) -keyfile:../uiml.net-core.snk -out:uiml.net-core.dll -target:library *.cs Rendering/*.cs Executing/*.cs \
	Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs Utils/Reflection/*.cs \
	Peers/*.cs Executing/Binding/*.cs LayoutManagement/*.cs
	@echo "done"

exe-cf:	lib-cf
	@echo "building executable" 
	${COMPILER} ${DEBUG} $(SOLVERLIBS) -d:COMPACT -keyfile:../uiml.net-cf.snk -out:uiml.net-cf.exe -target:exe -r:uiml.net-core-cf.dll \
	FrontEnd/*.cs -main:Uiml.FrontEnd.UimlTool
	@echo "done"
	@echo "copying uiml files for frontend"
	cp FrontEnd/gtkgui.uiml .
	cp FrontEnd/compactgui.uiml .
	@echo "done"

lib-cf:	solver
	@echo "building Compact .NET library"
	${COMPILER} ${DEBUG} $(SOLVERLIBS) -d:COMPACT -keyfile:../uiml.net-core-cf.snk -out:uiml.net-core-cf.dll -target:library *.cs Rendering/*.cs Executing/*.cs \
	Executing/Callers/Caller.cs Executing/Callers/LocalCaller.cs Executing/Callers/CallerFactory.cs Utils/*.cs Utils/Reflection/*.cs \
	Peers/*.cs Executing/Binding/*.cs LayoutManagement/*.cs
	@echo "done"

solver:
	@echo "building constraint solver"
	make -C ../Cassowary/ parselib lib
	cp ../Cassowary/Cassowary*dll .
	@echo "done"

gtk: 
	${COMPILER} ${DEBUG} ${GTKLIBS} -keyfile:../uiml.net-gtk.snk /r:uiml.net-core.dll -out:uiml.net-gtk.dll -target:library Rendering/GTKsharp/*.cs

wx: 
	${COMPILER} ${DEBUG} ${WXLIBS} -keyfile:../uiml.net-wx.snk /r:uiml.net-core.dll -out:uiml.net-wx.dll -target:library Rendering/WXnet/*.cs

swf: 
	${COMPILER} ${DEBUG} ${SWFLIBS} $(SOLVERLIBS) -keyfile:../uiml.net-swf.snk /r:uiml.net-core.dll -out:uiml.net-swf.dll -target:library Rendering/SWF/*.cs LayoutManagement/SWF/*.cs

swf-cf: lib-cf
	${COMPILER} ${DEBUG} ${SWFLIBS} -d:COMPACT -keyfile:../uiml.net-swf-cf.snk /r:uiml.net-core-cf.dll -out:uiml.net-swf-cf.dll -target:library Rendering/CompactSWF/*.cs

xml-rpc: 
	${COMPILER} ${DEBUG} ${XMLRPCLIBS} -keyfile:../uiml.net-xmlrpc.snk -r:uiml.net-core.dll -out:uiml.net-xmlrpc.dll -target:library Executing/Callers/XmlRpcCaller.cs

clean:
	rm -f *.exe *.dll *.mdb compactgui.uiml gtkgui.uiml
	make -C ../Cassowary/ clean


install:
	$(INSTALL) -d ${DESTDIR}${bindir}
	$(INSTALL_PROGRAM) uiml.net.exe $(DESTDIR)$(bindir)/uiml.net.exe
	$(INSTALL) -d ${DESTDIR}${monolibdir}
	$(INSTALL_PROGRAM) uiml.net-core.dll $(DESTDIR)$(monolibdir)/uiml.net-core.dll
	$(INSTALL_PROGRAM) uiml.net-gtk.dll $(DESTDIR)$(monolibdir)/uiml.net-gtk.dll
	$(INSTALL_PROGRAM) uiml.net-swf.dll $(DESTDIR)$(monolibdir)/uiml.net-swf.dll
	$(INSTALL_PROGRAM) uiml.net-swf-cf.dll $(DESTDIR)$(monolibdir)/uiml.net-swf-cf.dll
	$(INSTALL_PROGRAM) uiml.net-wx.dll $(DESTDIR)$(monolibdir)/uiml.net-wx.dll
	$(INSTALL_PROGRAM) uiml.net-xmlrpc.dll $(DESTDIR)$(monolibdir)/uiml.net-xmlrpc.dll

register: register-uiml register-gtk register-swf register-cswf register-wx register-xmlrpc

register-uiml:
	$(GACUTIL) /i uiml.net-core.dll /f $(GACUTIL_FLAGS) || exit 1;

register-gtk:
	$(GACUTIL) /i uiml.net-gtk.dll /f $(GACUTIL_FLAGS) || exit 1;

register-swf:
	$(GACUTIL) /i uiml.net-swf.dll /f $(GACUTIL_FLAGS) || exit 1;

register-cswf:
	$(GACUTIL) /i uiml.net-swf-cf.dll /f $(GACUTIL_FLAGS) || exit 1;

register-wx:
	$(GACUTIL) /i uiml.net-wx.dll /f $(GACUTIL_FLAGS) || exit 1;

register-xmlrpc:
	$(GACUTIL) /i uiml.net-xmlrpc.dll /f $(GACUTIL_FLAGS) || exit 1;

