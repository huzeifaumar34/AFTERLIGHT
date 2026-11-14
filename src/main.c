#include <pspkernel.h>
#include <pspdebug.h>
#include <pspdisplay.h>
#include <pspctrl.h>
#include <stdio.h>

#include "game.h"
#include "input.h"

PSP_MODULE_INFO("AFTERLIGHT", 0, 0, 1);
PSP_MAIN_THREAD_ATTR(THREAD_ATTR_USER);

static int exit_callback(int arg1, int arg2, void *common) {
    (void)arg1;
    (void)arg2;
    (void)common;
    sceKernelExitGame();
    return 0;
}

static int callback_thread(SceSize args, void *argp) {
    (void)args;
    (void)argp;
    int callback_id = sceKernelCreateCallback("Exit Callback", exit_callback, NULL);
    if (callback_id >= 0) {
        sceKernelRegisterExitCallback(callback_id);
    }
    sceKernelSleepThreadCB();
    return 0;
}

static void setup_callbacks(void) {
    int thread_id = sceKernelCreateThread("Callback Thread", callback_thread,
                                          0x11, 0xFA0, 0, NULL);
    if (thread_id >= 0) {
        sceKernelStartThread(thread_id, 0, NULL);
    }
}

int main(void) {
    sceCtrlSetSamplingCycle(0);
    sceCtrlSetSamplingMode(PSP_CTRL_MODE_ANALOG);
    pspDebugScreenInit();
    setup_callbacks();
    game_init();

    unsigned int previous_tick = sceKernelGetSystemTimeLow();
    int running = 1;

    while (running) {
        unsigned int current_tick = sceKernelGetSystemTimeLow();
        unsigned int elapsed = current_tick - previous_tick;
        previous_tick = current_tick;
        float delta_seconds = (float)elapsed / 1000000.0f;

        InputState input = input_poll();
        if (input.quit) {
            running = 0;
        }

        game_update(delta_seconds);
        game_render();
        sceDisplayWaitVblankStart();
    }

    game_shutdown();
    sceKernelExitGame();
    return 0;
}
