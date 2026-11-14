# AFTERLIGHT: NAMBARA — Unity project

This is the current game project. The earlier PSP C/PSPSDK prototype remains in the repository as historical reference; it is no longer the primary target.

## Engine and target

- Unity 6.3 LTS: **6000.3.27f2**
- Target: Android APK
- Graphics API: OpenGL ES 3
- Architecture: ARMv7 (32-bit Android devices)
- Render pipeline: Built-in, for broad compatibility and a lighter mobile footprint

The editor version is pinned in `ProjectSettings/ProjectVersion.txt`. Use Unity Hub to install that exact editor with Android Build Support, Android SDK & NDK Tools, and OpenJDK.

## First Unity vertical slice

The project builds its first Nambara coastal district from custom meshes at runtime:
- Irregular island coastline, shoreline, curved streets, pavements and a market square.
- Gabled coastal buildings with separate pitched roofs, window glass, frames, shutters, cornices, doors, balconies, signs and awnings.
- Timber ferry pier, moored boat, lighthouse, palms, shade trees and harbour lanterns.
- Stylised, articulated Elias and Mara character rigs with head, facial features, clothing layers, hands, boots and walking motion.
- Third-person camera, CharacterController movement, mobile joystick and touch action button.
- Four investigation clues, Mara's opening conversation and the signal-tower finale.
- Mission progress stored with PlayerPrefs on Android.

The district is not built from stacked default Unity cubes. The buildings, roof forms, shoreline, props and characters use generated meshes. This is still an early vertical slice: final scanned/artist-authored character models, high-resolution PBR texture sets, facial animation, voice acting, audio mix and the full city are not included yet.

## Open the project

1. Install Unity Hub and Unity **6000.3.27f2** on a desktop machine.
2. Add Android Build Support, Android SDK & NDK Tools, and OpenJDK in Unity Hub.
3. Open the `UnityProject` folder.
4. The Editor setup script creates `Assets/AFTERLIGHT/Scenes/HarborDistrict.unity` on project load. You can also use **AFTERLIGHT → Create or repair Harbor District scene**.
5. Press Play. The title screen appears over the coastal scene.

The Unity Editor is not available on Android itself. Since the developer is working from a phone, the intended compile path is GitHub Actions.

## Controls

- On-screen joystick: move Elias.
- INTERACT: inspect a clue, speak to Mara or investigate the lighthouse.
- PAUSE: pause or return to the title screen.
- Keyboard in Editor: WASD/arrow keys to move, E/Space to interact, Escape to pause.

## GitHub Actions Android build

The workflow is `.github/workflows/unity-android.yml`. It builds `Builds/Android/AFTERLIGHT.apk` and uploads it as an Actions artifact.

Unity Personal requires a one-time license activation for CI:
1. Run **Acquire Unity license activation file** in GitHub Actions.
2. Download the generated `.alf` artifact.
3. Upload it at [Unity license activation](https://license.unity3d.com/) and download the `.ulf` license file.
4. In GitHub → repository Settings → Secrets and variables → Actions, create:
   - `UNITY_LICENSE`: the full contents of the `.ulf` file.
   - `UNITY_EMAIL`: your Unity account email.
   - `UNITY_PASSWORD`: your Unity account password.

Keep these values in GitHub Secrets. Do not paste them into chat or commit them to the repository. Once the secrets exist, run **Unity Android APK** from the Actions tab.

## Performance notes

The first target is a low-memory 32-bit Android phone. The build uses a 30 FPS cap, one main directional light, OpenGL ES 3, limited point lights, simple material sets and compact procedural meshes. Visual fidelity will be increased through authored models and texture atlases while profiling the actual device; effects that overwhelm the device will not be enabled by default.
