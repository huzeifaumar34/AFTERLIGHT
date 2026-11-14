# AFTERLIGHT: NAMBARA

**Current engine:** Unity 6.3 LTS (6000.3.27f2)  
**Current target:** Android APK (ARMv7 / OpenGL ES 3)  
**Unity project:** `UnityProject/`

The earlier PSP C/PSPSDK build is kept as a historical prototype only. Development is now moving to Unity because the intended game needs authored 3D architecture, detailed characters, animation, materials and mobile touch controls.

## Current Unity vertical slice

The Unity project builds a coastal Nambara scene from custom meshes rather than stacking default cubes. The first scene includes gabled buildings with facades and roof detail, a shaped coastline, streets, a fish market square, ferry pier, boat, lighthouse, palms, street lanterns, stylised articulated characters, a third-person camera, mobile controls and the first investigation objectives.

This is an early vertical slice, not a finished game. The characters and environment currently use authored procedural meshes; final artist-authored models, high-resolution textures, animation polish, voice/audio and the rest of Nambara remain to be added.

## Build

Open `UnityProject/` using Unity **6000.3.27f2** with Android Build Support, Android SDK & NDK Tools and OpenJDK installed.

GitHub Actions can create the Android APK. The first-time Unity Personal activation steps are documented in [UnityProject/README.md](UnityProject/README.md). Keep Unity license and account values in GitHub Secrets; never commit them or send them in chat.

## Repository map

- `UnityProject/` — current Unity game.
- `include/`, `src/`, `Makefile` — archived PSP prototype.
- `.github/workflows/unity-android.yml` — Unity Android APK build.
- `.github/workflows/activation.yml` — one-time Unity Personal CI activation file.

## Engine pin

Unity 6000.3.27f2 is the pinned editor version. It is a Unity 6.3 LTS release and includes the security fixes listed in Unity's release notes.
