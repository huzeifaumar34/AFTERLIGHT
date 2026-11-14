#include "game.h"
#include "graphics.h"
#include "world.h"

void game_init(void) {
    graphics_init();
    world_init();
}

void game_update(float delta_seconds, const InputState *input) {
    if (delta_seconds < 0.0f) delta_seconds = 0.0f;
    if (delta_seconds > 0.08f) delta_seconds = 0.08f;
    world_update(delta_seconds, input);
}

void game_render(void) {
    float player_x;
    float player_z;

    world_get_player_position(&player_x, &player_z);
    graphics_begin(player_x, player_z);
    world_render();
    graphics_draw_hud(player_x, player_z,
                      world_get_interaction_count(),
                      world_is_nearby(),
                      world_get_nearby_name());
    graphics_end();
}

void game_shutdown(void) {
    graphics_shutdown();
}
