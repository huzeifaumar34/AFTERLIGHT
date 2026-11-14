TARGET = AFTERLIGHT
OBJS = src/main.o src/core/game.o src/graphics/graphics.o src/gameplay/mission.o src/gameplay/dialogue.o src/input/input.o src/world/world.o

INCDIR = include
CFLAGS = -O2 -G0 -Wall -Wextra -std=c99
CXXFLAGS = $(CFLAGS)
ASFLAGS = $(CFLAGS)

LIBS = -lpspgum -lpspgu -lpspdebug -lpspdisplay -lpspctrl -lpspkernel -lm

EXTRA_TARGETS = EBOOT.PBP
PSP_EBOOT_TITLE = AFTERLIGHT: NAMBARA

PSPSDK := $(shell psp-config --pspsdk-path)
include $(PSPSDK)/lib/build.mak
