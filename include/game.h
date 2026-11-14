#ifndef AFTERLIGHT_GAME_H
#define AFTERLIGHT_GAME_H

#include "input.h"

void game_init(void);
void game_update(float delta_seconds, const InputState *input);
void game_render(void);
int game_wants_exit(void);
void game_shutdown(void);

#endif
