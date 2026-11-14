using UnityEngine;

namespace Afterlight
{
    public sealed class AfterlightBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 30;
            Application.runInBackground = false;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 34f;
            QualitySettings.pixelLightCount = 2;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            Physics.gravity = new Vector3(0f, -22f, 0f);
            Physics.queriesHitTriggers = true;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.47f, 0.55f, 0.58f);
            RenderSettings.ambientEquatorColor = new Color(0.33f, 0.37f, 0.34f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.16f, 0.13f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.55f, 0.60f, 0.58f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0042f;

            CreateLighting();

            var mission = gameObject.AddComponent<MissionManager>();
            var worldRoot = new GameObject("NAMBARA / COASTAL DISTRICT").transform;
            worldRoot.SetParent(transform, false);

            var elias = CharacterFactory.CreateElias(worldRoot);
            elias.transform.localPosition = new Vector3(-1.35f, 0.28f, 0.3f);
            elias.transform.localRotation = Quaternion.identity;

            var controller = elias.AddComponent<CharacterController>();
            controller.height = 1.84f;
            controller.radius = 0.25f;
            controller.center = new Vector3(0f, 0.92f, 0f);
            controller.stepOffset = 0.28f;
            controller.slopeLimit = 48f;
            controller.skinWidth = 0.04f;

            var motor = elias.AddComponent<PlayerMotor>();

            CreateCamera(elias.transform);

            var ui = gameObject.AddComponent<GameUI>();
            ui.Initialize(motor, mission);
            motor.Configure(ui.Joystick);

            HarborDistrictBuilder.Build(worldRoot, mission, ui);
        }

        private void CreateLighting()
        {
            var sunObject = new GameObject("Nambara / late afternoon sun");
            sunObject.transform.SetParent(transform, false);
            sunObject.transform.rotation = Quaternion.Euler(42f, -28f, 0f);
            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.12f;
            sun.color = new Color(1f, 0.82f, 0.65f);
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.72f;
            RenderSettings.sun = sun;
        }

        private void CreateCamera(Transform target)
        {
            var cameraObject = new GameObject("AFTERLIGHT / Third-person camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.08f;
            camera.farClipPlane = 180f;
            camera.allowHDR = false;
            camera.allowMSAA = true;
            cameraObject.AddComponent<AudioListener>();
            var follow = cameraObject.AddComponent<CameraFollow>();
            follow.target = target;
            cameraObject.transform.position = target.position + new Vector3(0, 4.2f, -6.8f);
            cameraObject.transform.LookAt(target.position + Vector3.up * 1.1f);
        }
    }
}
