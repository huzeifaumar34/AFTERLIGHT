# AFTERLIGHT: NAMBARA

An original open-world 3D homebrew game for the PlayStation Portable (PSP), designed to run in PPSSPP.

**Current status:** first playable-slice prototype under development. GitHub Actions builds an EBOOT.PBP. Testing the latest build in PPSSPP or on PSP hardware is still required.

## Premise

Nambara is a fictional coastal city. Elias returns home after his brother disappears and discovers that a powerful organization has been controlling the city from the shadows. The player explores districts, talks to characters, follows clues and gradually uncovers what happened.

## Current prototype

The first coastal district is a low-poly 3D test environment featuring:
- A third-person follow camera and a simple player character (Elias).
- Water, an island block, roads, buildings, a ferry pier, a signal tower, trees and street lamps.
- A title screen with Continue, Controls and Exit, plus a pause menu.
- Movement using the D-pad or analog stick, basic building/boundary collisions and a pause state.
- Four investigation locations and a fifth signal-tower destination.
- A story conversation with Mara at the fish market. Press X to advance each line; movement is locked until the conversation ends.
- A minimap, clue counter and on-screen investigation messages.
- Automatic mission-progress saving to the Memory Stick with a versioned file and checksum.

The environment and characters are currently made from coloured geometric primitives. Final character models, textures, facial animation, voice acting, music, save slots and the full story are not implemented yet.

## Controls

| PSP control | Action |
|---|---|
| D-pad / analog stick | Move Elias or navigate menus |
| X (Cross) | Select, investigate, talk or advance dialogue |
| Start | Pause or resume the game |
| Triangle | Pause during gameplay / return from Controls |
| Circle | Exit the game |

## Target and technology

- Target: Sony PSP homebrew (EBOOT.PBP), tested first in PPSSPP.
- Language: C.
- SDK: PSPSDK.
- Graphics: PSP GU/GUM.
- Input: PSP controller API.
- Build: GNU Make and the PSPSDK toolchain, automated with GitHub Actions.

This is an original project. It is not affiliated with or derived from any commercial game.

## Repository layout

```
include/                 Public game, graphics, mission and dialogue headers
src/core/                Game states, menus and main loop
src/gameplay/            Mission and dialogue state
src/graphics/            GU setup, 3D primitives and HUD
src/input/               Controller input
src/world/               District, player movement, NPC and interactions
assets/                  Reserved for original game assets
.github/workflows/       Automated build workflow
Makefile                 PSPSDK build
```

## Build locally

Install a working PSPSDK toolchain and ensure `psp-config`, `psp-gcc` and `make` are on PATH, then run:

```sh
make clean
make
```

The expected output is `EBOOT.PBP` in the project root. GitHub Actions builds the same target and stores the result as an `AFTERLIGHT-PSP` artifact. The game stores mission progress at `ms0:/PSP/SAVEDATA/AFTERLIGHT.DAT` when the emulated or physical Memory Stick is writable. A successful CI build confirms compilation and packaging; it does not replace testing the EBOOT or save file in PPSSPP or on PSP hardware.

## Development milestones

1. Buildable PSP application and continuous integration. **Done.**
2. GU/GUM 3D scene, follow camera, movement and HUD. **Prototype implemented; emulator test pending.**
3. Explorable coastal district, basic collision and props. **First block implemented.**
4. Four-clue investigation mission and signal-tower chapter finale. **Prototype implemented.**
5. Automatic mission-progress save/load using a versioned file and checksum. **Code implemented; PPSSPP storage test pending.**
6. Title menu, pause menu and first NPC dialogue. **Prototype implemented; PPSSPP control test pending.**
7. Dialogue branches, player-position saves, improved character models, animation and audio.
8. Larger districts, mission progression, performance pass and story-slice testing.

## Contributing

Keep systems small and modular. Prefer original, optimized assets suitable for PSP memory and performance limits. Test changes in PPSSPP whenever possible.

## License

The project license will be selected before external contributions or asset redistribution. Until then, all rights are reserved by the repository owner.
