#ifndef AFTERLIGHT_INPUT_H
#define AFTERLIGHT_INPUT_H

typedef struct {
    int move_x;
    int move_y;
    int confirm;
    int cancel;
    int quit;
} InputState;

void input_init(void);
InputState input_poll(void);

#endif
