using UnityEngine;

namespace Afterlight
{
    public sealed class ClueInteractable : WorldInteractable
    {
        public int clueIndex;
        public string clueName;
        [TextArea] public string discoveryText;

        private MissionManager mission;

        public override string Prompt
        {
            get { return mission != null && mission.HasClue(clueIndex) ? "CLUE RECORDED" : "INVESTIGATE " + clueName; }
        }

        public void Configure(MissionManager manager, int index, string name, string text)
        {
            mission = manager;
            clueIndex = index;
            clueName = name;
            discoveryText = text;
        }

        public override void Interact(PlayerMotor player, GameUI ui)
        {
            if (mission == null || mission.HasClue(clueIndex))
            {
                ui.ShowMessage("Elias has already recorded this clue.");
                return;
            }

            if (mission.CollectClue(clueIndex))
            {
                ui.ShowMessage(discoveryText);
                ui.RefreshMission();
            }
        }
    }
}
