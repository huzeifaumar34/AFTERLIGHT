using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Afterlight
{
    public sealed class GameUI : MonoBehaviour
    {
        private Canvas canvas;
        private Font font;
        private PlayerMotor player;
        private MissionManager mission;

        private GameObject titlePanel;
        private GameObject pausePanel;
        private GameObject controlsPanel;
        private GameObject dialoguePanel;
        private GameObject hudPanel;
        private Text objectiveText;
        private Text clueText;
        private Text messageText;
        private Text speakerText;
        private Text dialogueText;
        private Button dialogueNextButton;
        private OnScreenJoystick joystick;

        private string[] currentLines;
        private int dialogueIndex;
        private Action dialogueFinished;
        private float messageTimer;
        private bool playing;

        public OnScreenJoystick Joystick { get { return joystick; } }

        public void Initialize(PlayerMotor playerMotor, MissionManager manager)
        {
            player = playerMotor;
            mission = manager;
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            EnsureEventSystem();
            BuildCanvas();
            BuildTitle();
            BuildPauseMenu();
            BuildControls();
            BuildDialogue();
            BuildHud();
            RefreshMission();
            mission.OnChanged += RefreshMission;
            player.SetControlsEnabled(false);
        }

        private void Update()
        {
            if (messageTimer > 0f)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0f && messageText) messageText.gameObject.SetActive(false);
            }

            if (playing && Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            var events = new GameObject("EventSystem");
            events.AddComponent<EventSystem>();
            events.AddComponent<StandaloneInputModule>();
        }

        private void BuildCanvas()
        {
            var canvasObject = new GameObject("AFTERLIGHT / Screen UI");
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvasObject.AddComponent<GraphicRaycaster>();
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
        }

        private GameObject Panel(string name, Transform parent, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 anchoredPosition)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private Text Label(string value, Transform parent, int size, Color color,
            TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 dimensions, Vector2 position)
        {
            var go = new GameObject("Text / " + value, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = dimensions;
            rect.anchoredPosition = position;
            var text = go.GetComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private Button ActionButton(string label, Transform parent, Vector2 anchor,
            Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject("Button / " + label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            var image = go.GetComponent<Image>();
            image.color = new Color(0.13f, 0.19f, 0.20f, 0.92f);
            var button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            Label(label, go.transform, 22, new Color(0.94f, 0.87f, 0.72f),
                TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        private void BuildTitle()
        {
            titlePanel = Panel("Title screen", canvas.transform,
                new Color(0.035f, 0.055f, 0.065f, 0.94f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Panel("Title inner frame", titlePanel.transform,
                new Color(0.16f, 0.21f, 0.20f, 0.96f),
                new Vector2(0.12f, 0.13f), new Vector2(0.88f, 0.87f),
                Vector2.zero, Vector2.zero);
            Label("AFTERLIGHT", titlePanel.transform, 58, new Color(0.91f, 0.82f, 0.63f),
                TextAnchor.MiddleCenter, new Vector2(0.1f, 0.59f), new Vector2(0.9f, 0.82f),
                Vector2.zero, Vector2.zero);
            Label("N A M B A R A", titlePanel.transform, 25, new Color(0.73f, 0.79f, 0.75f),
                TextAnchor.MiddleCenter, new Vector2(0.1f, 0.50f), new Vector2(0.9f, 0.61f),
                Vector2.zero, Vector2.zero);
            Label("THE COAST REMEMBERS", titlePanel.transform, 19, new Color(0.75f, 0.62f, 0.43f),
                TextAnchor.MiddleCenter, new Vector2(0.1f, 0.42f), new Vector2(0.9f, 0.50f),
                Vector2.zero, Vector2.zero);

            ActionButton("ENTER NAMBARA", titlePanel.transform, new Vector2(0.5f, 0.32f),
                new Vector2(290, 62), StartGame);
            ActionButton("CONTROLS", titlePanel.transform, new Vector2(0.5f, 0.20f),
                new Vector2(290, 54), ShowControls);
            Label("A STORY OF SECRETS, SALT AND SHADOWS", titlePanel.transform, 14,
                new Color(0.57f, 0.64f, 0.62f), TextAnchor.MiddleCenter,
                new Vector2(0.1f, 0.06f), new Vector2(0.9f, 0.12f),
                Vector2.zero, Vector2.zero);
        }

        private void BuildPauseMenu()
        {
            pausePanel = Panel("Pause menu", canvas.transform,
                new Color(0.025f, 0.045f, 0.055f, 0.86f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Label("PAUSED", pausePanel.transform, 48, new Color(0.91f, 0.82f, 0.63f),
                TextAnchor.MiddleCenter, new Vector2(0.2f, 0.60f), new Vector2(0.8f, 0.78f),
                Vector2.zero, Vector2.zero);
            ActionButton("RESUME", pausePanel.transform, new Vector2(0.5f, 0.47f),
                new Vector2(280, 56), TogglePause);
            ActionButton("MAIN MENU", pausePanel.transform, new Vector2(0.5f, 0.34f),
                new Vector2(280, 56), ReturnToTitle);
            pausePanel.SetActive(false);
        }

        private void BuildControls()
        {
            controlsPanel = Panel("Controls screen", canvas.transform,
                new Color(0.025f, 0.045f, 0.055f, 0.96f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Label("FIELD CONTROLS", controlsPanel.transform, 44,
                new Color(0.91f, 0.82f, 0.63f), TextAnchor.MiddleCenter,
                new Vector2(0.1f, 0.68f), new Vector2(0.9f, 0.84f), Vector2.zero, Vector2.zero);
            Label("LEFT STICK  /  MOVE ELIAS\nINTERACT  /  INVESTIGATE OR TALK\nMENU  /  PAUSE THE GAME",
                controlsPanel.transform, 25, Color.white, TextAnchor.MiddleCenter,
                new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.68f),
                Vector2.zero, Vector2.zero);
            ActionButton("BACK", controlsPanel.transform, new Vector2(0.5f, 0.18f),
                new Vector2(250, 54), () => controlsPanel.SetActive(false));
            controlsPanel.SetActive(false);
        }

        private void BuildDialogue()
        {
            dialoguePanel = Panel("Dialogue panel", canvas.transform,
                new Color(0.025f, 0.045f, 0.055f, 0.95f),
                new Vector2(0.06f, 0.04f), new Vector2(0.94f, 0.34f),
                Vector2.zero, Vector2.zero);
            speakerText = Label("MARA", dialoguePanel.transform, 24,
                new Color(0.88f, 0.61f, 0.35f), TextAnchor.MiddleLeft,
                new Vector2(0.04f, 0.67f), new Vector2(0.7f, 0.95f),
                Vector2.zero, Vector2.zero);
            dialogueText = Label("", dialoguePanel.transform, 22, Color.white,
                TextAnchor.MiddleLeft, new Vector2(0.04f, 0.24f), new Vector2(0.78f, 0.68f),
                Vector2.zero, Vector2.zero);
            dialogueNextButton = ActionButton("CONTINUE", dialoguePanel.transform,
                new Vector2(0.87f, 0.40f), new Vector2(170, 56), AdvanceDialogue);
            dialoguePanel.SetActive(false);
        }

        private void BuildHud()
        {
            hudPanel = new GameObject("Gameplay HUD");
            hudPanel.transform.SetParent(canvas.transform, false);

            var objectivePanel = Panel("Objective panel", hudPanel.transform,
                new Color(0.035f, 0.055f, 0.065f, 0.84f),
                new Vector2(0.02f, 0.84f), new Vector2(0.45f, 0.98f),
                Vector2.zero, Vector2.zero);
            objectiveText = Label("FIND THE FOUR CLUES", objectivePanel.transform, 22,
                new Color(0.91f, 0.82f, 0.63f), TextAnchor.MiddleLeft,
                new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.92f),
                Vector2.zero, Vector2.zero);
            clueText = Label("CLUES  0 / 4", objectivePanel.transform, 17,
                new Color(0.72f, 0.80f, 0.76f), TextAnchor.MiddleLeft,
                new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.48f),
                Vector2.zero, Vector2.zero);

            var joystickBackground = Panel("Movement joystick", hudPanel.transform,
                new Color(0.08f, 0.13f, 0.14f, 0.52f),
                new Vector2(0.03f, 0.04f), new Vector2(0.03f, 0.04f),
                new Vector2(178, 178), new Vector2(100, 105));
            joystick = joystickBackground.AddComponent<OnScreenJoystick>();
            joystick.background = joystickBackground.GetComponent<RectTransform>();
            var handleObject = Panel("Joystick handle", joystickBackground.transform,
                new Color(0.72f, 0.78f, 0.72f, 0.62f),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(70, 70), Vector2.zero);
            joystick.handle = handleObject.GetComponent<RectTransform>();

            ActionButton("INTERACT", hudPanel.transform, new Vector2(0.89f, 0.12f),
                new Vector2(190, 88), OnInteractPressed);
            ActionButton("PAUSE", hudPanel.transform, new Vector2(0.94f, 0.93f),
                new Vector2(125, 48), TogglePause);

            var messagePanel = Panel("Message panel", hudPanel.transform,
                new Color(0.025f, 0.045f, 0.055f, 0.90f),
                new Vector2(0.18f, 0.68f), new Vector2(0.82f, 0.81f),
                Vector2.zero, Vector2.zero);
            messageText = Label("", messagePanel.transform, 20, Color.white,
                TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            messagePanel.SetActive(false);
            hudPanel.SetActive(false);
        }

        private void StartGame()
        {
            titlePanel.SetActive(false);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            hudPanel.SetActive(true);
            playing = true;
            player.SetControlsEnabled(true);
        }

        private void ShowControls()
        {
            controlsPanel.SetActive(true);
        }

        public void TogglePause()
        {
            if (!playing) return;
            bool pause = !pausePanel.activeSelf;
            pausePanel.SetActive(pause);
            player.SetControlsEnabled(!pause);
            Time.timeScale = pause ? 0f : 1f;
        }

        private void ReturnToTitle()
        {
            Time.timeScale = 1f;
            playing = false;
            player.SetControlsEnabled(false);
            pausePanel.SetActive(false);
            hudPanel.SetActive(false);
            titlePanel.SetActive(true);
        }

        private void OnInteractPressed()
        {
            if (dialoguePanel.activeSelf) AdvanceDialogue();
            else if (playing && pausePanel.activeSelf == false) player.TryInteract();
        }

        public void ShowDialogue(string speaker, string[] lines, Action onFinished)
        {
            if (lines == null || lines.Length == 0)
            {
                onFinished?.Invoke();
                return;
            }
            currentLines = lines;
            dialogueIndex = 0;
            dialogueFinished = onFinished;
            speakerText.text = speaker;
            dialoguePanel.SetActive(true);
            RenderDialogueLine();
        }

        private void AdvanceDialogue()
        {
            if (currentLines == null) return;
            dialogueIndex++;
            if (dialogueIndex >= currentLines.Length)
            {
                dialoguePanel.SetActive(false);
                currentLines = null;
                var callback = dialogueFinished;
                dialogueFinished = null;
                callback?.Invoke();
                return;
            }
            RenderDialogueLine();
        }

        private void RenderDialogueLine()
        {
            dialogueText.text = currentLines[dialogueIndex];
            dialogueNextButton.GetComponentInChildren<Text>().text =
                dialogueIndex == currentLines.Length - 1 ? "CLOSE" : "CONTINUE";
        }

        public void ShowMessage(string message)
        {
            if (!messageText) return;
            messageText.text = message;
            messageText.transform.parent.gameObject.SetActive(true);
            messageTimer = 4.2f;
        }

        public void RefreshMission()
        {
            if (!mission || !objectiveText || !clueText) return;
            objectiveText.text = mission.Objective;
            clueText.text = "CLUES   " + mission.ClueCount + " / 4";
        }

        private void OnDestroy()
        {
            if (mission != null) mission.OnChanged -= RefreshMission;
            Time.timeScale = 1f;
        }
    }
}
