#include <pspdebug.h>

#include "game.h"
#include "world.h"

static float game_time = 0.0f;
static unsigned int frame_count = 0;

void game_init(void) {
    game_time = 0.0f;
    frame_count = 0;
    world_init();
}

void game_update(float delta_seconds, const InputState *input) {
    if (delta_seconds < 0.0f) delta_seconds = 0.0f;
    if (delta_seconds > 0.1f) delta_seconds = 0.1f;

    game_time += delta_seconds;
    frame_count++;
    world_update(delta_seconds, input);
}

void game_render(void) {
    pspDebugScreenSetXY(0, 0);
    pspDebugScreenSetTextColor(0xFFFFFFFF);
    pspDebugScreenPrintf("AFTERLIGHT: NAMBARA\n");
    pspDebugScreenSetTextColor(0xFFB8D8E8);
    pspDebugScreenPrintf("A PSP homebrew project\n\n");
    pspDebugScreenSetTextColor(0xFFFFFFFF);
    pspDebugScreenPrintf("NAMBARA // PROTOTYPE BUILD\n");
    pspDebugScreenPrintf("--------------------------\n");
    pspDebugScreenPrintf("The coast remembers.\n\n");
    world_render();
    pspDebugScreenPrintf("\n\nTime: %.1f s   Frames: %u\n", game_time, frame_count);
    pspDebugScreenPrintf("\nD-pad / analog: move marker\nX: interact   O: quit\n");
}

void game_shutdown(void) {
    /* Reserved for releasing game resources. */
}
