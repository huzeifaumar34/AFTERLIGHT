#include <math.h>

#include "graphics.h"
#include "world.h"

typedef struct {
    float x;
    float z;
    float width;
    float height;
    float depth;
    unsigned int wall_color;
    unsigned int roof_color;
} Building;

typedef struct {
    float x;
    float z;
    const char *name;
    int discovered;
} PointOfInterest;

static const Building buildings[] = {
    {-9.0f, -9.0f, 5.8f, 3.2f, 5.0f, 0xff706d61, 0xff424b50},
    {-9.0f,  0.0f, 5.2f, 3.1f, 4.6f, 0xffbc895d, 0xff5a5048},
    {-9.0f,  9.0f, 5.5f, 4.0f, 4.5f, 0xff7d8a85, 0xff424c4b},
    { 9.0f, -9.0f, 5.0f, 5.3f, 5.0f, 0xffa16e55, 0xff45434a},
    { 9.0f,  0.0f, 5.2f, 3.6f, 4.8f, 0xffaaa183, 0xff5b5146},
    { 9.0f,  9.0f, 4.5f, 3.0f, 4.3f, 0xff9b795e, 0xff4d4a43}
};

static PointOfInterest points[] = {
    {-9.0f, -5.6f, "PORT OFFICE", 0},
    {-9.0f,  3.4f, "FISH MARKET", 0},
    { 9.0f, -5.6f, "OLD TOWN", 0},
    {12.1f,  7.0f, "FERRY DOCK", 0}
};

static float player_x = 0.0f;
static float player_z = 6.0f;
static float walk_phase = 0.0f;
static int previous_confirm = 0;
static int interaction_count = 0;
static int nearby_point = -1;

#define BUILDING_COUNT (sizeof(buildings) / sizeof(buildings[0]))
#define POINT_COUNT (sizeof(points) / sizeof(points[0]))

static int collides_with_building(float x, float z) {
    unsigned int i;
    if (x < -13.0f || x > 13.0f || z < -13.0f || z > 13.0f) return 1;

    for (i = 0; i < BUILDING_COUNT; ++i) {
        const Building *b = &buildings[i];
        float half_w = b->width * 0.5f + 0.48f;
        float half_d = b->depth * 0.5f + 0.48f;
        if (fabsf(x - b->x) < half_w && fabsf(z - b->z) < half_d) {
            return 1;
        }
    }
    return 0;
}

static void update_nearby_point(void) {
    unsigned int i;
    float best_distance = 1000.0f;
    nearby_point = -1;

    for (i = 0; i < POINT_COUNT; ++i) {
        float dx = player_x - points[i].x;
        float dz = player_z - points[i].z;
        float distance_squared = dx * dx + dz * dz;
        if (distance_squared < best_distance) {
            best_distance = distance_squared;
            nearby_point = (int)i;
        }
    }

    if (best_distance > 3.4f * 3.4f) nearby_point = -1;
}

void world_init(void) {
    unsigned int i;
    player_x = 0.0f;
    player_z = 6.0f;
    walk_phase = 0.0f;
    previous_confirm = 0;
    interaction_count = 0;
    nearby_point = -1;
    for (i = 0; i < POINT_COUNT; ++i) points[i].discovered = 0;
}

void world_update(float delta_seconds, const InputState *input) {
    float dx;
    float dz;
    float length_squared;
    float speed = 4.2f;

    if (!input) return;
    if (delta_seconds < 0.0f) delta_seconds = 0.0f;
    if (delta_seconds > 0.08f) delta_seconds = 0.08f;

    dx = (float)input->move_x;
    dz = (float)input->move_y;
    length_squared = dx * dx + dz * dz;

    if (length_squared > 1.0f) {
        dx *= 0.70710678f;
        dz *= 0.70710678f;
    }

    if (dx != 0.0f || dz != 0.0f) {
        float next_x = player_x + dx * speed * delta_seconds;
        float next_z = player_z + dz * speed * delta_seconds;
        walk_phase += delta_seconds * 10.0f;

        if (!collides_with_building(next_x, player_z)) player_x = next_x;
        if (!collides_with_building(player_x, next_z)) player_z = next_z;
    }

    update_nearby_point();
    if (input->confirm && !previous_confirm && nearby_point >= 0) {
        if (!points[nearby_point].discovered) {
            points[nearby_point].discovered = 1;
            interaction_count++;
        }
    }
    previous_confirm = input->confirm;
}

static void draw_building(const Building *b) {
    float base_y = -0.55f;
    float front_z = b->z + (b->depth * 0.5f) + 0.035f;
    float window_y = (b->height * 0.58f) + base_y;

    graphics_draw_box(b->x, (b->height * 0.5f) + base_y,
                      b->z, b->width, b->height, b->depth, b->wall_color);
    graphics_draw_box(b->x, b->height + base_y + 0.12f,
                      b->z, b->width + 0.25f, 0.24f,
                      b->depth + 0.25f, b->roof_color);
    graphics_draw_box(b->x - b->width * 0.23f, window_y,
                      front_z, 0.72f, 0.58f, 0.07f, 0xff526a70);
    graphics_draw_box(b->x + b->width * 0.23f, window_y,
                      front_z, 0.72f, 0.58f, 0.07f, 0xff526a70);
    graphics_draw_box(b->x, 0.24f + base_y, front_z + 0.03f,
                      0.72f, 1.05f, 0.09f, 0xff50443c);
}

static void draw_tree(float x, float z) {
    graphics_draw_box(x, 0.45f, z, 0.38f, 1.6f, 0.38f, 0xff594533);
    graphics_draw_box(x, 1.55f, z, 1.45f, 1.55f, 1.45f, 0xff3c7658);
    graphics_draw_box(x - 0.32f, 1.25f, z + 0.18f,
                      0.85f, 0.9f, 0.85f, 0xff4a8963);
}

static void draw_street_lamp(float x, float z) {
    graphics_draw_box(x, 1.45f, z, 0.16f, 2.9f, 0.16f, 0xff333d43);
    graphics_draw_box(x, 2.92f, z, 0.74f, 0.22f, 0.42f, 0xffd9c58c);
}

static void draw_player(void) {
    float bob = (walk_phase > 0.0f) ? (sinf(walk_phase) * 0.035f) : 0.0f;
    float body_y = 0.83f + bob;

    /* Elias: dark travel clothes, light shirt and a simple low-poly silhouette. */
    graphics_draw_box(player_x, body_y, player_z,
                      0.62f, 1.05f, 0.42f, 0xff35414d);
    graphics_draw_box(player_x, 1.58f + bob, player_z,
                      0.45f, 0.48f, 0.43f, 0xffbd8d6d);
    graphics_draw_box(player_x, 1.82f + bob, player_z,
                      0.48f, 0.16f, 0.45f, 0xff252a2d);
    graphics_draw_box(player_x - 0.43f, body_y, player_z,
                      0.22f, 0.82f, 0.25f, 0xff35414d);
    graphics_draw_box(player_x + 0.43f, body_y, player_z,
                      0.22f, 0.82f, 0.25f, 0xff35414d);
    graphics_draw_box(player_x - 0.17f, 0.25f + bob, player_z,
                      0.24f, 0.52f, 0.29f, 0xff252c34);
    graphics_draw_box(player_x + 0.17f, 0.25f - bob, player_z,
                      0.24f, 0.52f, 0.29f, 0xff252c34);
}

void world_render(void) {
    unsigned int i;

    /* Water, island platform, streets and concrete walkways. */
    graphics_draw_box(0.0f, -1.18f, 0.0f, 70.0f, 0.35f, 70.0f, 0xff416f82);
    graphics_draw_box(0.0f, -0.84f, 0.0f, 27.0f, 0.5f, 27.0f, 0xff9b927a);
    graphics_draw_box(0.0f, -0.54f, 0.0f, 3.5f, 0.08f, 27.0f, 0xff464a4b);
    graphics_draw_box(0.0f, -0.53f, 3.0f, 27.0f, 0.08f, 3.5f, 0xff464a4b);
    graphics_draw_box(0.0f, -0.53f, -5.0f, 27.0f, 0.08f, 2.2f, 0xff464a4b);

    /* Sidewalks and a few painted road markings. */
    graphics_draw_box(-2.25f, -0.49f, 0.0f, 0.55f, 0.1f, 27.0f, 0xffc1b69b);
    graphics_draw_box( 2.25f, -0.49f, 0.0f, 0.55f, 0.1f, 27.0f, 0xffc1b69b);
    graphics_draw_box(-8.8f, -0.47f, 3.0f, 0.16f, 0.035f, 1.1f, 0xffc9b98d);
    graphics_draw_box(-5.8f, -0.47f, 3.0f, 0.16f, 0.035f, 1.1f, 0xffc9b98d);
    graphics_draw_box( 5.8f, -0.47f, 3.0f, 0.16f, 0.035f, 1.1f, 0xffc9b98d);
    graphics_draw_box( 8.8f, -0.47f, 3.0f, 0.16f, 0.035f, 1.1f, 0xffc9b98d);

    for (i = 0; i < BUILDING_COUNT; ++i) draw_building(&buildings[i]);

    /* Ferry pier extending from the eastern edge of the island. */
    graphics_draw_box(14.2f, -0.36f, 7.0f, 6.2f, 0.42f, 5.2f, 0xff725b43);
    graphics_draw_box(14.2f, -0.08f, 7.0f, 5.8f, 0.12f, 4.8f, 0xff9c7952);
    graphics_draw_box(11.25f, 0.02f, 7.0f, 0.16f, 0.7f, 0.16f, 0xff574736);
    graphics_draw_box(17.1f, 0.02f, 7.0f, 0.16f, 0.7f, 0.16f, 0xff574736);
    graphics_draw_box(14.2f, 0.02f, 4.65f, 6.0f, 0.7f, 0.16f, 0xff574736);
    graphics_draw_box(14.2f, 0.02f, 9.35f, 6.0f, 0.7f, 0.16f, 0xff574736);

    /* A small beacon tower on the north-east point. */
    graphics_draw_box(11.7f, 1.15f, -11.1f, 1.35f, 3.4f, 1.35f, 0xffd1c9ae);
    graphics_draw_box(11.7f, 2.98f, -11.1f, 1.65f, 0.32f, 1.65f, 0xffa34f44);
    graphics_draw_box(11.7f, 3.38f, -11.1f, 0.62f, 0.5f, 0.62f, 0xffe7d49b);

    /* Trees and lamps add scale to the first test district. */
    draw_tree(-11.7f, 5.7f);
    draw_tree(-11.6f, -3.1f);
    draw_tree(11.8f, 4.3f);
    draw_tree(11.8f, -3.0f);
    draw_tree(5.7f, 11.5f);
    draw_tree(-5.8f, 11.5f);
    draw_street_lamp(-3.2f, 1.0f);
    draw_street_lamp(3.2f, 1.0f);
    draw_street_lamp(-3.2f, -7.0f);
    draw_street_lamp(3.2f, -7.0f);

    draw_player();
}

void world_get_player_position(float *x, float *z) {
    if (x) *x = player_x;
    if (z) *z = player_z;
}

int world_get_interaction_count(void) {
    return interaction_count;
}

int world_is_nearby(void) {
    return nearby_point >= 0;
}

const char *world_get_nearby_name(void) {
    if (nearby_point < 0) return 0;
    if (points[nearby_point].discovered) return "CLUE FOUND";
    return points[nearby_point].name;
}
