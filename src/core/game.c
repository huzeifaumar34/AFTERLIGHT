#include "game.h"
#include "graphics.h"
#include "world.h"

enum {
    MODE_TITLE = 0,
    MODE_PLAYING,
    MODE_PAUSED,
    MODE_CONTROLS
};

static int game_mode = MODE_TITLE;
static int menu_selection = 0;
static int previous_confirm = 0;
static int previous_cancel = 0;
static int previous_start = 0;
static int previous_menu_y = 0;
static int exit_requested = 0;

static int clamp_selection(int value, int maximum) {
    if (value < 0) return maximum;
    if (value > maximum) return 0;
    return value;
}

void game_init(void) {
    graphics_init();
    world_init();
    game_mode = MODE_TITLE;
    menu_selection = 0;
    previous_confirm = 0;
    previous_cancel = 0;
    previous_start = 0;
    previous_menu_y = 0;
    exit_requested = 0;
}

void game_update(float delta_seconds, const InputState *input) {
    int confirm_pressed;
    int cancel_pressed;
    int start_pressed;

    if (!input || exit_requested) return;
    if (delta_seconds < 0.0f) delta_seconds = 0.0f;
    if (delta_seconds > 0.08f) delta_seconds = 0.08f;

    confirm_pressed = input->confirm && !previous_confirm;
    cancel_pressed = input->cancel && !previous_cancel;
    start_pressed = input->start && !previous_start;

    previous_confirm = input->confirm;
    previous_cancel = input->cancel;
    previous_start = input->start;

    if (input->quit) {
        exit_requested = 1;
        return;
    }

    if (game_mode == MODE_PLAYING) {
        if (start_pressed || cancel_pressed) {
            game_mode = MODE_PAUSED;
            menu_selection = 0;
            previous_menu_y = 0;
            return;
        }
        world_update(delta_seconds, input);
        return;
    }

    if (game_mode == MODE_CONTROLS) {
        if (confirm_pressed || cancel_pressed || start_pressed) {
            game_mode = MODE_TITLE;
            menu_selection = 0;
        }
        previous_menu_y = input->move_y;
        return;
    }

    if (input->move_y != 0 && previous_menu_y == 0) {
        menu_selection = clamp_selection(menu_selection -
                          (input->move_y < 0 ? 1 : -1), 2);
    }
    previous_menu_y = input->move_y;

    if (game_mode == MODE_TITLE) {
        if (confirm_pressed) {
            if (menu_selection == 0) {
                game_mode = MODE_PLAYING;
            } else if (menu_selection == 1) {
                game_mode = MODE_CONTROLS;
            } else {
                exit_requested = 1;
            }
        }
    } else if (game_mode == MODE_PAUSED) {
        if (start_pressed || cancel_pressed) {
            game_mode = MODE_PLAYING;
        } else if (confirm_pressed) {
            if (menu_selection == 0) {
                game_mode = MODE_PLAYING;
            } else if (menu_selection == 1) {
                game_mode = MODE_TITLE;
                menu_selection = 0;
                world_init();
            } else {
                exit_requested = 1;
            }
        }
    }
}

int game_wants_exit(void) {
    return exit_requested;
}

void game_render(void) {
    float player_x;
    float player_z;

    world_get_player_position(&player_x, &player_z);
    graphics_begin(player_x, player_z);
    world_render();

    if (game_mode == MODE_PLAYING || game_mode == MODE_PAUSED) {
        graphics_draw_hud(player_x, player_z,
                          world_get_interaction_count(),
                          world_is_nearby(),
                          world_get_nearby_name());
        if (world_dialogue_is_active()) {
            graphics_draw_dialogue(world_dialogue_get_speaker(),
                                   world_dialogue_get_line(),
                                   world_dialogue_get_line_index(),
                                   world_dialogue_get_line_count());
        }
    }

    if (game_mode == MODE_TITLE) {
        graphics_draw_menu(0, menu_selection);
    } else if (game_mode == MODE_CONTROLS) {
        graphics_draw_menu(1, menu_selection);
    } else if (game_mode == MODE_PAUSED) {
        graphics_draw_menu(2, menu_selection);
    }

    graphics_end();
}

void game_shutdown(void) {
    graphics_shutdown();
}
