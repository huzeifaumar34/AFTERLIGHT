#include <string.h>

#include "mission.h"

#define CLUE_COUNT 4

static unsigned char collected[CLUE_COUNT];
static int clue_count = 0;
static int chapter_complete = 0;
static char last_message[72];

static const char *clue_messages[CLUE_COUNT] = {
    "DOCK LOG SAYS HE LEFT ON FERRY SIX",
    "A FISHER SAW A BLACK VAN AT THE OLD PIER",
    "THE SHOP RADIO REPEATED AFTERLIGHT",
    "THE FERRY ROUTE ENDED AT THE SIGNAL TOWER"
};

static void set_message(const char *message) {
    if (!message) message = "";
    strncpy(last_message, message, sizeof(last_message) - 1);
    last_message[sizeof(last_message) - 1] = '\0';
}

void mission_init(void) {
    memset(collected, 0, sizeof(collected));
    clue_count = 0;
    chapter_complete = 0;
    set_message("ELIAS SEARCHES FOR HIS MISSING BROTHER");
}

int mission_collect_clue(int clue_id) {
    if (clue_id < 0 || clue_id >= CLUE_COUNT) return 0;
    if (collected[clue_id]) return 0;

    collected[clue_id] = 1;
    clue_count++;
    set_message(clue_messages[clue_id]);

    if (clue_count == CLUE_COUNT) {
        set_message("FOUR CLUES POINT TO THE SIGNAL TOWER");
    }
    return 1;
}

void mission_interact_with_tower(void) {
    if (clue_count < CLUE_COUNT) {
        set_message("THE SIGNAL IS STILL UNCLEAR FIND MORE CLUES");
        return;
    }

    if (!chapter_complete) {
        chapter_complete = 1;
        set_message("CHAPTER ONE COMPLETE THE SIGNAL IS FOUND");
    } else {
        set_message("THE SIGNAL POINTS BEYOND NAMBARA");
    }
}

int mission_get_clue_count(void) {
    return clue_count;
}

int mission_is_complete(void) {
    return chapter_complete;
}

const char *mission_get_last_message(void) {
    return last_message;
}
