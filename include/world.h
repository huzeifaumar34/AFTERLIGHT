#ifndef AFTERLIGHT_WORLD_H
#define AFTERLIGHT_WORLD_H

#include "input.h"

void world_init(void);
void world_update(float delta_seconds, const InputState *input);
void world_render(void);

#endif
