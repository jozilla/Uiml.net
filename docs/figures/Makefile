DPATTERN=dia
DPROGRAM=dia
MPATTERN=mp
MPROGRAM=mpost
OPTIONS=--nosplash
FILES = $(patsubst %.dia,%.eps,$(wildcard *.dia)) $(patsubst %.dia,%.png,$(wildcard *.dia))
	#$(patsubst %.mp,%.eps,$(wildcard *.mp)) $(patsubst %.mp,%.png,$(wildcard *.mp))


%.png: %.$(DPATTERN)
	$(DPROGRAM) $(OPTIONS) -e $@ $< || true

%.eps: %.$(DPATTERN)
	$(DPROGRAM) $(OPTIONS) -e $@ $< || true



all: $(FILES) 

clean:
	rm -rfv $(FILES)
	true
