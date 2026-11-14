#include <string.h>
#include <pspiofilemgr.h>

#include "mission.h"

#define CLUE_COUNT 4
#define SAVE_MAGIC 0x41464c54u
#define SAVE_VERSION 1u
#define SAVE_PATH "ms0:/PSP/SAVEDATA/AFTERLIGHT.DAT"

typedef struct {
    unsigned int magic;
    unsigned int version;
    unsigned int clue_mask;
    unsigned int chapter_complete;
    unsigned int checksum;
} MissionSaveData;

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

static unsigned int save_checksum(const MissionSaveData *data) {
    return data->magic ^ data->version ^ data->clue_mask ^
           data->chapter_complete ^ 0x4e414d42u;
}

static unsigned int build_clue_mask(void) {
    unsigned int mask = 0;
    int i;
    for (i = 0; i < CLUE_COUNT; ++i) {
        if (collected[i]) mask |= (1u << i);
    }
    return mask;
}

static int save_progress(void) {
    MissionSaveData data;
    SceUID file;
    int written;

    /* PPSSPP maps ms0: to its emulated Memory Stick storage. */
    sceIoMkdir("ms0:/PSP", 0777);
    sceIoMkdir("ms0:/PSP/SAVEDATA", 0777);

    data.magic = SAVE_MAGIC;
    data.version = SAVE_VERSION;
    data.clue_mask = build_clue_mask();
    data.chapter_complete = (unsigned int)(chapter_complete != 0);
    data.checksum = save_checksum(&data);

    file = sceIoOpen(SAVE_PATH,
                     PSP_O_WRONLY | PSP_O_CREAT | PSP_O_TRUNC, 0777);
    if (file < 0) return 0;

    written = sceIoWrite(file, &data, sizeof(data));
    sceIoClose(file);
    return written == (int)sizeof(data);
}

static int load_progress(void) {
    MissionSaveData data;
    SceUID file;
    int bytes;
    int i;
    int loaded_count = 0;

    file = sceIoOpen(SAVE_PATH, PSP_O_RDONLY, 0777);
    if (file < 0) return 0;

    bytes = sceIoRead(file, &data, sizeof(data));
    sceIoClose(file);

    if (bytes != (int)sizeof(data)) return 0;
    if (data.magic != SAVE_MAGIC || data.version != SAVE_VERSION) return 0;
    if ((data.clue_mask & ~((1u << CLUE_COUNT) - 1u)) != 0u) return 0;
    if (data.chapter_complete > 1u) return 0;
    if (data.checksum != save_checksum(&data)) return 0;
    if (data.chapter_complete && data.clue_mask != ((1u << CLUE_COUNT) - 1u)) return 0;

    for (i = 0; i < CLUE_COUNT; ++i) {
        collected[i] = (unsigned char)((data.clue_mask >> i) & 1u);
        if (collected[i]) loaded_count++;
    }

    clue_count = loaded_count;
    chapter_complete = (int)data.chapter_complete;
    set_message("PROGRESS RESTORED FROM MEMORY STICK");
    return 1;
}

void mission_init(void) {
    memset(collected, 0, sizeof(collected));
    clue_count = 0;
    chapter_complete = 0;
    set_message("ELIAS SEARCHES FOR HIS MISSING BROTHER");
    load_progress();
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

    /* Saving is best-effort: gameplay continues if storage is unavailable. */
    save_progress();
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
        save_progress();
    } else {
        set_message("THE SIGNAL POINTS BEYOND NAMBARA");
    }
}

int mission_get_clue_count(void) {
    return clue_count;
}

int mission_has_clue(int clue_id) {
    if (clue_id < 0 || clue_id >= CLUE_COUNT) return 0;
    return collected[clue_id] != 0;
}

int mission_is_complete(void) {
    return chapter_complete;
}

const char *mission_get_last_message(void) {
    return last_message;
}
