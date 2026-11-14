# AFTERLIGHT: NAMBARA

An original open-world 3D homebrew game for the PlayStation Portable (PSP), designed to run in PPSSPP.

**Current status:** foundation / prototype stage. This repository is the start of development, not a finished game. The first milestone is a reproducible PSP homebrew build and a small playable vertical slice.

## Premise

Nambara is a fictional coastal city. Elias returns home after his brother disappears and discovers that a powerful organization has been controlling the city from the shadows. The player explores districts, talks to characters, follows clues and gradually uncovers what happened.

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
include/                 Public game headers
src/core/                Startup, game loop and timing
src/graphics/            GU setup and rendering helpers
src/gameplay/            Player and gameplay systems
src/input/               Controller input
src/world/               World and scene scaffolding
assets/                  Placeholder location for original game assets
.github/workflows/       Automated build workflow
Makefile                 PSPSDK build
```

## Build locally

Install a working PSPSDK toolchain and ensure `psp-config`, `psp-gcc` and `make` are on PATH, then run:

```sh
make
```

The expected output is `EBOOT.PBP` in the project root. The GitHub Actions workflow builds the same target in CI. A successful CI build is not a substitute for testing the resulting EBOOT in PPSSPP or on hardware.

## Development milestones

1. Buildable PSP application and CI.
2. Stable frame loop, input and basic GU rendering.
3. Player movement and camera.
4. Small explorable Nambara test district.
5. Interactions, dialogue and mission framework.
6. Save/load, audio and performance pass.
7. Playable story slice and PPSSPP regression testing.

## Contributing

Keep systems small and modular. Prefer original, optimized assets suitable for PSP memory and performance limits. Document controls and test changes in PPSSPP whenever possible.

## License

The project license will be selected before external contributions or asset redistribution. Until then, all rights are reserved by the repository owner.
