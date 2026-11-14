#include <pspkernel.h>
#include <pspdisplay.h>

#include "game.h"
#include "input.h"

PSP_MODULE_INFO("AFTERLIGHT", 0, 1, 0);
PSP_MAIN_THREAD_ATTR(THREAD_ATTR_USER);

static int exit_callback(int arg1, int arg2, void *common) {
    (void)arg1;
    (void)arg2;
    (void)common;
    sceKernelExitGame();
    return 0;
}

static int callback_thread(SceSize args, void *argp) {
    int callback_id;
    (void)args;
    (void)argp;

    callback_id = sceKernelCreateCallback("Exit Callback", exit_callback, 0);
    if (callback_id >= 0) {
        sceKernelRegisterExitCallback(callback_id);
    }
    sceKernelSleepThreadCB();
    return 0;
}

static void setup_callbacks(void) {
    int thread_id = sceKernelCreateThread("Callback Thread", callback_thread,
                                          0x11, 0xFA0, 0, 0);
    if (thread_id >= 0) {
        sceKernelStartThread(thread_id, 0, 0);
    }
}

int main(void) {
    unsigned int previous_tick;
    int running = 1;

    setup_callbacks();
    input_init();
    game_init();
    previous_tick = sceKernelGetSystemTimeLow();

    while (running) {
        unsigned int current_tick = sceKernelGetSystemTimeLow();
        unsigned int elapsed = current_tick - previous_tick;
        float delta_seconds = (float)elapsed / 1000000.0f;
        InputState input;

        previous_tick = current_tick;
        input = input_poll();
        if (input.quit) running = 0;

        game_update(delta_seconds, &input);
        game_render();
        sceDisplayWaitVblankStart();
    }

    game_shutdown();
    sceKernelExitGame();
    return 0;
}
