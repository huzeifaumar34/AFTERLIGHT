using UnityEngine;

namespace Afterlight
{
    public sealed class DialogueNPC : WorldInteractable
    {
        private static readonly string[] FirstTalk =
        {
            "ELIAS: Mara. You knew my brother was here?",
            "MARA: Three nights ago. He came looking for a ferry manifest.",
            "ELIAS: Did he say where he was going?",
            "MARA: He asked about Ferry Six, then left this note.",
            "MARA: It says AFTERLIGHT. I do not know what it means.",
            "ELIAS: Then I will find the signal he was following."
        };

        private static readonly string[] ReturnTalk =
        {
            "MARA: The harbour office keeps the old ferry records.",
            "MARA: Be careful, Elias. Someone still watches the docks."
        };

        private bool talkedOnce;
        private GameUI gameUI;
        private PlayerMotor player;

        public override string Prompt { get { return "TALK TO MARA"; } }

        public void Configure(GameUI ui, PlayerMotor motor)
        {
            gameUI = ui;
            player = motor;
        }

        public override void Interact(PlayerMotor motor, GameUI ui)
        {
            if (gameUI == null) gameUI = ui;
            if (player == null) player = motor;
            player.SetInputLocked(true);
            gameUI.ShowDialogue("MARA", talkedOnce ? ReturnTalk : FirstTalk, () =>
            {
                talkedOnce = true;
                player.SetInputLocked(false);
            });
        }
    }
}
