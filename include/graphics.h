#ifndef AFTERLIGHT_GRAPHICS_H
#define AFTERLIGHT_GRAPHICS_H

void graphics_init(void);
void graphics_begin(float player_x, float player_z);
void graphics_draw_box(float x, float y, float z,
                       float width, float height, float depth,
                       unsigned int color);
void graphics_draw_hud(float player_x, float player_z,
                       int interactions, int nearby,
                       const char *nearby_name);
void graphics_draw_menu(int page, int selection);
void graphics_draw_dialogue(const char *speaker, const char *line,
                            int line_index, int line_count);
void graphics_end(void);
void graphics_shutdown(void);

#endif
