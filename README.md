# AFTERLIGHT: NAMBARA

An original open-world 3D homebrew game for the PlayStation Portable (PSP), designed to run in PPSSPP.

**Current status:** first 3D vertical-slice prototype. The CI build produces an EBOOT.PBP; PPSSPP play-testing and further gameplay work are still required.

## Premise

Nambara is a fictional coastal city. Elias returns home after his brother disappears and discovers that a powerful organization has been controlling the city from the shadows. The player explores districts, talks to characters, follows clues and gradually uncovers what happened.

## Current prototype

The first explorable coastal block now contains:
- A third-person follow camera and a low-poly player character (Elias).
- A small island district with roads, water, buildings, a ferry pier, a beacon tower, trees and street lamps.
- Basic collision against buildings and the edge of the district.
- Four points of interest. Walk close to one and press **X** to collect its clue.
- A minimap and a compact on-screen status display.

This is still a grey-box prototype. The buildings and character are made from simple coloured boxes; there are no final character models, textures, voice acting or finished story missions yet.

## Controls

| PSP control | Action |
|---|---|
| D-pad / analog stick | Move Elias |
| X (Cross) | Investigate a nearby point of interest |
| Start | Exit the prototype |

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
include/                 Public game and engine headers
src/core/                Startup, game loop and timing
src/graphics/            GU setup, 3D primitives and HUD
src/input/               Controller input
src/world/               District, player movement and interactions
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

The expected output is `EBOOT.PBP` in the project root. GitHub Actions builds the same target in CI. A successful CI build confirms compilation and packaging; it does not replace testing the EBOOT in PPSSPP or on PSP hardware.

## Development milestones

1. Buildable PSP application and CI. **Done.**
2. GU/GUM 3D scene, follow camera, movement and HUD. **In progress.**
3. A more complete explorable district with improved collision and props.
4. Interaction, dialogue and mission framework.
5. Save/load, audio, original assets and performance pass.
6. Playable story slice and PPSSPP regression testing.

## Contributing

Keep systems small and modular. Prefer original, optimized assets suitable for PSP memory and performance limits. Document controls and test changes in PPSSPP whenever possible.

## License

The project license will be selected before external contributions or asset redistribution. Until then, all rights are reserved by the repository owner.
