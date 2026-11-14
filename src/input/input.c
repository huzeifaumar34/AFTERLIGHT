#include <pspctrl.h>
#include "input.h"

void input_init(void) {
    sceCtrlSetSamplingCycle(0);
    sceCtrlSetSamplingMode(PSP_CTRL_MODE_ANALOG);
}

InputState input_poll(void) {
    SceCtrlData pad;
    InputState state = {0, 0, 0, 0, 0, 0};

    pad.Buttons = 0;
    pad.Lx = 128;
    pad.Ly = 128;
    sceCtrlReadBufferPositive(&pad, 1);

    if (pad.Buttons & PSP_CTRL_LEFT) state.move_x = -1;
    if (pad.Buttons & PSP_CTRL_RIGHT) state.move_x = 1;
    if (pad.Buttons & PSP_CTRL_UP) state.move_y = -1;
    if (pad.Buttons & PSP_CTRL_DOWN) state.move_y = 1;

    if (pad.Buttons & PSP_CTRL_CROSS) state.confirm = 1;
    if (pad.Buttons & PSP_CTRL_TRIANGLE) state.cancel = 1;
    if (pad.Buttons & PSP_CTRL_START) state.start = 1;
    if (pad.Buttons & PSP_CTRL_CIRCLE) state.quit = 1;

    /* The analog stick takes priority over the digital pad. */
    if (pad.Lx < 96) state.move_x = -1;
    else if (pad.Lx > 160) state.move_x = 1;
    if (pad.Ly < 96) state.move_y = -1;
    else if (pad.Ly > 160) state.move_y = 1;

    return state;
}
