#include <pspdebug.h>
#include "world.h"

static int marker_x = 18;
static int marker_y = 5;
static unsigned int interaction_count = 0;

void world_init(void) {
    marker_x = 18;
    marker_y = 5;
    interaction_count = 0;
}

void world_update(float delta_seconds, const InputState *input) {
    (void)delta_seconds;
    if (input == 0) return;

    marker_x += input->move_x;
    marker_y += input->move_y;

    if (marker_x < 0) marker_x = 0;
    if (marker_x > 40) marker_x = 40;
    if (marker_y < 0) marker_y = 0;
    if (marker_y > 12) marker_y = 12;

    if (input->confirm) interaction_count++;
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
    pspDebugScreenPrintf("  Interactions: %u\n", interaction_count);
}
