#include <pspdebug.h>
#include "world.h"

static int marker_x = 18;
static int marker_y = 5;

void world_init(void) {
    marker_x = 18;
    marker_y = 5;
}

void world_update(float delta_seconds) {
    (void)delta_seconds;
}

void world_render(void) {
    pspDebugScreenSetTextColor(0xFF80D8A0);
    pspDebugScreenPrintf("\n[ NAMBARA - COASTAL DISTRICT ]\n");
    pspDebugScreenSetTextColor(0xFFFFFFFF);
    pspDebugScreenPrintf("  ~ ~ ~  OCEAN  ~ ~ ~\n");
    pspDebugScreenPrintf("  [Port]     [Old Town]\n");
    pspDebugScreenPrintf("      |          |\n");
    pspDebugScreenPrintf("  [Market]---[Harbor Road]\n");
    pspDebugScreenPrintf("      |          |\n");
    pspDebugScreenPrintf("  [Station]  [ ??? ]\n");
    pspDebugScreenPrintf("\n  Exploration marker: (%d, %d)\n", marker_x, marker_y);
}
