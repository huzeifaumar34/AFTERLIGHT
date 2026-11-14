TARGET = AFTERLIGHT
OBJS = src/main.o src/core/game.o src/input/input.o src/world/world.o

INCDIR = include
CFLAGS = -O2 -G0 -Wall -Wextra -std=c99
CXXFLAGS = $(CFLAGS)
ASFLAGS = $(CFLAGS)

LIBS = -lpspdebug -lpspdisplay -lpspctrl -lpspkernel

EXTRA_TARGETS = EBOOT.PBP
PSP_EBOOT_TITLE = AFTERLIGHT: NAMBARA

PSPSDK := $(shell psp-config --pspsdk-path)
include $(PSPSDK)/lib/build.mak
