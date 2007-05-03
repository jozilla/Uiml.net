# De compilers:
PDF = db2pdf # for a pdf file
HTML = db2html
RTF = db2rtf
PS = db2ps

# de hoofdfile van de tekst
FILE = uiml.net.docs
TARGETS = $(FILE).rtf $(FILE).pdf $(FILE).html $(FILE).ps
XMLFILES = uiml.net.docs.xml fdl-1.2.xml uiml.xml uiml.net.xml behavior.xml
OUTDIR = uiml.net.docs

# welke bestanden mogen weg gedaan worden
DELETABLE = *.aux *.toc *.out *.ind *.lof *.idx *.ilg *.lot\
	*.blg *.bbl *.lol *.lo *.brf *.*~ *.tpt *.fo uiml.net.docs.junk/
CLEANABLE = *[^e]ps *pdf *.tex~ *.dvi *.ps.gz *.*~ *.bak\
		*.rtf *.tar.bz2 $(OUTDIR)


SUBDIR = figures


all: $(TARGETS)
	
# de hoofdregel van deze Makefile
$(FILE).pdf: $(FILE).xml $(XMLFILES)
	make pdf
	
$(FILE).ps : $(FILE).pdf
	acroread -toPostScript $(FILE).pdf

$(FILE).html: $(FILE).xml $(XMLFILES)
	make html

$(FILE).rtf: $(FILE).xml $(XMLFILES)
	make rtf
			
pdf:
	@make figs
	$(PDF) $(FILE).xml
	rm -rfv  $(DELETABLE) #verwijder intermediate files


html:
	@make figs
	$(HTML) $(FILE).xml
	mkdir $(OUTDIR)/figures || true
	cp -rfv figures/*.jpg figures/*.png $(OUTDIR)/figures/ || true

rtf:
	@make figs
	$(RTF) $(FILE).xml

php:
	@make figs
	$(HTML) $(FILE).xml
	

# ruim de boel op, alles behalve de benodigde sources	
clean: 
	@make -C figures clean
	rm -rfv $(CLEANABLE) $(DELETABLE)

figs:
	@make -C figures

.PHONY: install
install:
#	make clean
	make all
	rm -rfv $(OUTDIR)
	mv $(FILE) $(OUTDIR)/
	mkdir -p $(OUTDIR)/figures/ || true
	cp -rf figures/*.png figures/*.jpg  $(OUTDIR)/figures/
	cp -rf $(FILE).ps $(FILE).pdf $(FILE).rtf $(OUTDIR)/
	rm -rf $(OUTDIR)-dbsrc/
	mkdir $(OUTDIR)-dbsrc/ || true
	cp -rf figures/ *.xml DOMUiParser.java $(OUTDIR)-dbsrc/
	tar -c $(OUTDIR)-dbsrc | bzip2 > $(OUTDIR)-dbsrc.tar.bz2
	tar -c $(OUTDIR)-dbsrc | gzip > $(OUTDIR)-dbsrc.tar.gz
	rm -rf $(OUTDIR)-dbsrc/
	mv $(OUTDIR)-dbsrc.tar.* $(OUTDIR)/
	tar -c $(OUTDIR) | bzip2 > toship.tar.bz2
