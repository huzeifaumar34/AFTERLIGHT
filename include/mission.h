#ifndef AFTERLIGHT_MISSION_H
#define AFTERLIGHT_MISSION_H

void mission_init(void);
int mission_collect_clue(int clue_id);
void mission_interact_with_tower(void);
int mission_get_clue_count(void);
int mission_is_complete(void);
const char *mission_get_last_message(void);

#endif
