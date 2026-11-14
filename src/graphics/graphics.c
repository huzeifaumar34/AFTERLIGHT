#include <pspgu.h>
#include <pspgum.h>
#include <pspdisplay.h>
#include <pspkernel.h>
#include <stdio.h>
#include <string.h>

#include "graphics.h"

#define SCREEN_WIDTH 480
#define SCREEN_HEIGHT 272
#define BUFFER_WIDTH 512
#define HUD_VERTEX_CAPACITY 4096

typedef struct {
    unsigned int color;
    float x;
    float y;
    float z;
} CubeVertex;

typedef struct {
    unsigned int color;
    short x;
    short y;
    short z;
} HudVertex;

static unsigned int __attribute__((aligned(16))) display_list[262144];
static CubeVertex __attribute__((aligned(16))) cube_vertices[36];
static HudVertex __attribute__((aligned(16))) hud_vertices[HUD_VERTEX_CAPACITY];
static HudVertex __attribute__((aligned(16))) sprite_vertices[2];
static int hud_vertex_count = 0;
static int graphics_ready = 0;

static const float cube_positions[36][3] = {
    /* Front */
    {-0.5f,-0.5f, 0.5f}, { 0.5f,-0.5f, 0.5f}, { 0.5f, 0.5f, 0.5f},
    {-0.5f,-0.5f, 0.5f}, { 0.5f, 0.5f, 0.5f}, {-0.5f, 0.5f, 0.5f},
    /* Back */
    { 0.5f,-0.5f,-0.5f}, {-0.5f,-0.5f,-0.5f}, {-0.5f, 0.5f,-0.5f},
    { 0.5f,-0.5f,-0.5f}, {-0.5f, 0.5f,-0.5f}, { 0.5f, 0.5f,-0.5f},
    /* Left */
    {-0.5f,-0.5f,-0.5f}, {-0.5f,-0.5f, 0.5f}, {-0.5f, 0.5f, 0.5f},
    {-0.5f,-0.5f,-0.5f}, {-0.5f, 0.5f, 0.5f}, {-0.5f, 0.5f,-0.5f},
    /* Right */
    { 0.5f,-0.5f, 0.5f}, { 0.5f,-0.5f,-0.5f}, { 0.5f, 0.5f,-0.5f},
    { 0.5f,-0.5f, 0.5f}, { 0.5f, 0.5f,-0.5f}, { 0.5f, 0.5f, 0.5f},
    /* Top */
    {-0.5f, 0.5f, 0.5f}, { 0.5f, 0.5f, 0.5f}, { 0.5f, 0.5f,-0.5f},
    {-0.5f, 0.5f, 0.5f}, { 0.5f, 0.5f,-0.5f}, {-0.5f, 0.5f,-0.5f},
    /* Bottom */
    {-0.5f,-0.5f,-0.5f}, { 0.5f,-0.5f,-0.5f}, { 0.5f,-0.5f, 0.5f},
    {-0.5f,-0.5f,-0.5f}, { 0.5f,-0.5f, 0.5f}, {-0.5f,-0.5f, 0.5f}
};

static const unsigned char font_3x5[36][5] = {
    /* A-Z */
    {2,5,7,5,5}, {6,5,6,5,6}, {3,4,4,4,3}, {6,5,5,5,6},
    {7,4,6,4,7}, {7,4,6,4,4}, {3,4,5,5,3}, {5,5,7,5,5},
    {7,2,2,2,7}, {1,1,1,5,2}, {5,5,6,5,5}, {4,4,4,4,7},
    {5,7,7,5,5}, {5,7,7,7,5}, {2,5,5,5,2}, {6,5,6,4,4},
    {2,5,5,7,3}, {6,5,6,5,5}, {3,4,2,1,6}, {7,2,2,2,2},
    {5,5,5,5,7}, {5,5,5,5,2}, {5,5,7,7,5}, {5,5,2,5,5},
    {5,5,2,2,2}, {7,1,2,4,7},
    /* 0-9 */
    {7,5,5,5,7}, {2,6,2,2,7}, {6,1,2,4,7}, {6,1,3,1,6},
    {5,5,7,1,1}, {7,4,6,1,6}, {3,4,7,5,7}, {7,1,2,2,2},
    {7,5,7,5,7}, {7,5,7,1,6}
};

static int font_index(char character) {
    if (character >= 'A' && character <= 'Z') return character - 'A';
    if (character >= '0' && character <= '9') return 26 + character - '0';
    return -1;
}

static void draw_rect(int x1, int y1, int x2, int y2, unsigned int color) {
    sprite_vertices[0].color = color;
    sprite_vertices[0].x = (short)x1;
    sprite_vertices[0].y = (short)y1;
    sprite_vertices[0].z = 0;
    sprite_vertices[1].color = color;
    sprite_vertices[1].x = (short)x2;
    sprite_vertices[1].y = (short)y2;
    sprite_vertices[1].z = 0;
    sceGuDrawArray(GU_SPRITES,
                   GU_COLOR_8888 | GU_VERTEX_16BIT | GU_TRANSFORM_2D,
                   2, 0, sprite_vertices);
}

static void add_hud_pixel(int x, int y, int scale, unsigned int color) {
    int sx;
    int sy;
    if (hud_vertex_count + 1 >= HUD_VERTEX_CAPACITY) return;

    for (sy = 0; sy < scale; ++sy) {
        for (sx = 0; sx < scale; ++sx) {
            HudVertex *vertex = &hud_vertices[hud_vertex_count++];
            vertex->color = color;
            vertex->x = (short)(x + sx);
            vertex->y = (short)(y + sy);
            vertex->z = 0;
        }
    }
}

static void draw_text(int x, int y, const char *text, int scale,
                      unsigned int color) {
    int cursor = x;
    while (text && *text) {
        int glyph = font_index(*text);
        int row;
        int column;
        if (glyph >= 0) {
            for (row = 0; row < 5; ++row) {
                unsigned char bits = font_3x5[glyph][row];
                for (column = 0; column < 3; ++column) {
                    if (bits & (1 << (2 - column))) {
                        add_hud_pixel(cursor + column * scale,
                                      y + row * scale, scale, color);
                    }
                }
            }
        }
        cursor += 4 * scale;
        ++text;
    }
}

static void flush_hud_text(void) {
    if (hud_vertex_count > 0) {
        sceGuDrawArray(GU_POINTS,
                       GU_COLOR_8888 | GU_VERTEX_16BIT | GU_TRANSFORM_2D,
                       hud_vertex_count, 0, hud_vertices);
    }
}

void graphics_init(void) {
    void *framebuffer0 = guGetStaticVramBuffer(BUFFER_WIDTH, SCREEN_HEIGHT, GU_PSM_8888);
    void *framebuffer1 = guGetStaticVramBuffer(BUFFER_WIDTH, SCREEN_HEIGHT, GU_PSM_8888);
    void *depthbuffer = guGetStaticVramBuffer(BUFFER_WIDTH, SCREEN_HEIGHT, GU_PSM_4444);

    sceGuInit();
    sceGuStart(GU_DIRECT, display_list);
    sceGuDrawBuffer(GU_PSM_8888, framebuffer0, BUFFER_WIDTH);
    sceGuDispBuffer(SCREEN_WIDTH, SCREEN_HEIGHT, framebuffer1, BUFFER_WIDTH);
    sceGuDepthBuffer(depthbuffer, BUFFER_WIDTH);
    sceGuOffset(2048 - (SCREEN_WIDTH / 2), 2048 - (SCREEN_HEIGHT / 2));
    sceGuViewport(2048, 2048, SCREEN_WIDTH, SCREEN_HEIGHT);
    sceGuDepthRange(65535, 0);
    sceGuScissor(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
    sceGuEnable(GU_SCISSOR_TEST);
    sceGuEnable(GU_DEPTH_TEST);
    sceGuDepthFunc(GU_GEQUAL);
    sceGuFrontFace(GU_CW);
    sceGuDisable(GU_CULL_FACE);
    sceGuDisable(GU_TEXTURE_2D);
    sceGuShadeModel(GU_SMOOTH);
    sceGuClearColor(0xff7896a8);
    sceGuClearDepth(0);
    sceGuFinish();
    sceGuSync(GU_SYNC_FINISH, GU_SYNC_WHAT_DONE);
    sceDisplayWaitVblankStart();
    sceGuDisplay(GU_TRUE);
    graphics_ready = 1;
}

void graphics_begin(float player_x, float player_z) {
    ScePspFVector3 eye = {player_x + 10.5f, 11.0f, player_z + 14.0f};
    ScePspFVector3 target = {player_x, 0.0f, player_z};
    ScePspFVector3 up = {0.0f, 1.0f, 0.0f};

    sceGuStart(GU_DIRECT, display_list);
    sceGuClearColor(0xff7896a8);
    sceGuClearDepth(0);
    sceGuClear(GU_COLOR_BUFFER_BIT | GU_DEPTH_BUFFER_BIT);

    sceGumMatrixMode(GU_PROJECTION);
    sceGumLoadIdentity();
    sceGumPerspective(52.0f, 16.0f / 9.0f, 0.5f, 160.0f);

    sceGumMatrixMode(GU_VIEW);
    sceGumLoadIdentity();
    sceGumLookAt(&eye, &target, &up);

    sceGumMatrixMode(GU_MODEL);
    sceGumLoadIdentity();
}

static unsigned int shade_color(unsigned int color, unsigned int factor) {
    unsigned int r = color & 0xff;
    unsigned int g = (color >> 8) & 0xff;
    unsigned int b = (color >> 16) & 0xff;
    unsigned int a = (color >> 24) & 0xff;
    r = (r * factor) / 255;
    g = (g * factor) / 255;
    b = (b * factor) / 255;
    return (a << 24) | (b << 16) | (g << 8) | r;
}

void graphics_draw_box(float x, float y, float z,
                       float width, float height, float depth,
                       unsigned int color) {
    int i;
    static const unsigned int face_factors[6] = {210, 175, 150, 190, 255, 125};
    ScePspFVector3 position = {x, y, z};
    ScePspFVector3 scale = {width, height, depth};

    for (i = 0; i < 36; ++i) {
        cube_vertices[i].x = cube_positions[i][0];
        cube_vertices[i].y = cube_positions[i][1];
        cube_vertices[i].z = cube_positions[i][2];
        cube_vertices[i].color = shade_color(color, face_factors[i / 6]);
    }

    sceGumMatrixMode(GU_MODEL);
    sceGumPushMatrix();
    sceGumTranslate(&position);
    sceGumScale(&scale);
    sceGumDrawArray(GU_TRIANGLES,
                    GU_COLOR_8888 | GU_VERTEX_32BITF | GU_TRANSFORM_3D,
                    36, 0, cube_vertices);
    sceGumPopMatrix();
}

void graphics_draw_hud(float player_x, float player_z,
                       int interactions, int nearby,
                       const char *nearby_name) {
    char count_text[16];
    char prompt[64];
    int map_x;
    int map_y;

    sceGuDisable(GU_DEPTH_TEST);
    sceGuDisable(GU_CULL_FACE);
    sceGumMatrixMode(GU_PROJECTION);
    sceGumLoadIdentity();
    sceGumOrtho(0.0f, (float)SCREEN_WIDTH,
                (float)SCREEN_HEIGHT, 0.0f, -1.0f, 1.0f);
    sceGumMatrixMode(GU_VIEW);
    sceGumLoadIdentity();
    sceGumMatrixMode(GU_MODEL);
    sceGumLoadIdentity();

    /* Header panel */
    draw_rect(10, 10, 190, 62, 0xe0182028);
    hud_vertex_count = 0;
    draw_text(17, 16, "AFTERLIGHT", 2, 0xfff0dfb4);
    draw_text(17, 33, "NAMBARA COAST", 1, 0xffd2e2e8);
    draw_text(17, 45, "CLUES", 1, 0xffa9c4cc);
    snprintf(count_text, sizeof(count_text), "%02d", interactions);
    draw_text(43, 45, count_text, 1, 0xfff0dfb4);
    flush_hud_text();

    /* Minimap */
    draw_rect(386, 10, 470, 94, 0xe0182028);
    draw_rect(392, 18, 464, 88, 0xff344d48);
    draw_rect(423, 20, 431, 86, 0xff77837b);
    draw_rect(393, 51, 463, 58, 0xff77837b);
    draw_rect(405, 30, 445, 35, 0xff77837b);
    draw_rect(399, 68, 418, 72, 0xff77837b);
    draw_rect(444, 66, 458, 70, 0xff77837b);
    draw_rect(401, 26, 405, 30, 0xffe6bd72);
    draw_rect(402, 74, 406, 78, 0xffe6bd72);
    draw_rect(448, 27, 452, 31, 0xffe6bd72);
    draw_rect(451, 72, 455, 76, 0xffe6bd72);
    map_x = 427 + (int)(player_x * 2.15f);
    map_y = 54 + (int)(player_z * 2.15f);
    if (map_x < 394) map_x = 394;
    if (map_x > 460) map_x = 460;
    if (map_y < 20) map_y = 20;
    if (map_y > 85) map_y = 85;
    draw_rect(map_x - 2, map_y - 2, map_x + 3, map_y + 3, 0xff62f0d0);

    /* Bottom mission / controls strip */
    draw_rect(10, 248, 470, 268, 0xe0182028);
    hud_vertex_count = 0;
    if (nearby && nearby_name) {
        snprintf(prompt, sizeof(prompt), "X INVESTIGATE %s", nearby_name);
    } else {
        snprintf(prompt, sizeof(prompt), "EXPLORE THE COAST FIND THE SIGNAL");
    }
    draw_text(17, 254, prompt, 1, 0xfff0dfb4);
    draw_text(376, 254, "START EXIT", 1, 0xffd2e2e8);
    flush_hud_text();
    sceGuEnable(GU_DEPTH_TEST);
}

void graphics_end(void) {
    sceGuFinish();
    sceGuSync(GU_SYNC_FINISH, GU_SYNC_WHAT_DONE);
    sceGuSwapBuffers();
}

void graphics_shutdown(void) {
    if (graphics_ready) {
        sceGuTerm();
        graphics_ready = 0;
    }
}
