#ifndef AFTERLIGHT_DIALOGUE_H
#define AFTERLIGHT_DIALOGUE_H

void dialogue_init(void);
void dialogue_interact(void);
int dialogue_is_active(void);
const char *dialogue_get_speaker(void);
const char *dialogue_get_line(void);
int dialogue_get_line_index(void);
int dialogue_get_line_count(void);

#endif
