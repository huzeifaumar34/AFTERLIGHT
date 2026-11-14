#ifndef AFTERLIGHT_WORLD_H
#define AFTERLIGHT_WORLD_H

#include "input.h"

void world_init(void);
void world_update(float delta_seconds, const InputState *input);
void world_render(void);
void world_get_player_position(float *x, float *z);
int world_get_interaction_count(void);
int world_is_nearby(void);
const char *world_get_nearby_name(void);

#endif
