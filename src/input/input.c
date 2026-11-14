#include <pspctrl.h>
#include "input.h"

void input_init(void) {
    sceCtrlSetSamplingCycle(0);
    sceCtrlSetSamplingMode(PSP_CTRL_MODE_ANALOG);
}

InputState input_poll(void) {
    SceCtrlData pad;
    InputState state = {0, 0, 0, 0, 0};

    sceCtrlReadBufferPositive(&pad, 1);

    if (pad.Buttons & PSP_CTRL_LEFT) state.move_x = -1;
    if (pad.Buttons & PSP_CTRL_RIGHT) state.move_x = 1;
    if (pad.Buttons & PSP_CTRL_UP) state.move_y = -1;
    if (pad.Buttons & PSP_CTRL_DOWN) state.move_y = 1;

    if (pad.Buttons & PSP_CTRL_CROSS) state.confirm = 1;
    if (pad.Buttons & PSP_CTRL_CIRCLE) state.cancel = 1;
    if (pad.Buttons & PSP_CTRL_START) state.quit = 1;

    if (pad.Buttons & PSP_CTRL_ANALUP) state.move_y = -1;
    if (pad.Buttons & PSP_CTRL_ANALDOWN) state.move_y = 1;
    if (pad.Buttons & PSP_CTRL_ANALLEFT) state.move_x = -1;
    if (pad.Buttons & PSP_CTRL_ANALRIGHT) state.move_x = 1;

    return state;
}
