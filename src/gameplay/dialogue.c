#include "dialogue.h"

static const char *first_conversation[] = {
    "ELIAS I THOUGHT YOU LEFT NAMBARA",
    "YOUR BROTHER CAME HERE THREE NIGHTS AGO",
    "HE ASKED ABOUT FERRY NUMBER SIX",
    "THEN HE GAVE ME THIS NOTE",
    "IT SAYS AFTERLIGHT IS A WARNING"
};

static const char *second_conversation[] = {
    "IF YOU FIND THE SIGNAL DO NOT ANSWER IT",
    "PROMISE ME YOU WILL BE CAREFUL ELIAS"
};

static int active = 0;
static int talked_once = 0;
static int line_index = 0;
static int conversation = 0;

void dialogue_init(void) {
    active = 0;
    talked_once = 0;
    line_index = 0;
    conversation = 0;
}

void dialogue_interact(void) {
    if (!active) {
        active = 1;
        conversation = talked_once ? 1 : 0;
        line_index = 0;
        return;
    }

    line_index++;
    if (line_index >= dialogue_get_line_count()) {
        active = 0;
        talked_once = 1;
        line_index = 0;
    }
}

int dialogue_is_active(void) {
    return active;
}

const char *dialogue_get_speaker(void) {
    return "MARA";
}

const char *dialogue_get_line(void) {
    if (!active) return "";
    if (conversation == 0) return first_conversation[line_index];
    return second_conversation[line_index];
}

int dialogue_get_line_index(void) {
    return line_index;
}

int dialogue_get_line_count(void) {
    return conversation == 0 ?
           (int)(sizeof(first_conversation) / sizeof(first_conversation[0])) :
           (int)(sizeof(second_conversation) / sizeof(second_conversation[0]));
}
