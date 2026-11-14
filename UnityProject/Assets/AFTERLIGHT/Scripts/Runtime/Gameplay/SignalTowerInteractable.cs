using UnityEngine;

namespace Afterlight
{
    public sealed class SignalTowerInteractable : WorldInteractable
    {
        private MissionManager mission;

        public override string Prompt { get { return "INVESTIGATE THE LIGHTHOUSE"; } }

        public void Configure(MissionManager manager)
        {
            mission = manager;
        }

        public override void Interact(PlayerMotor player, GameUI ui)
        {
            if (mission == null || mission.ClueCount < 4)
            {
                int remaining = mission == null ? 4 : 4 - mission.ClueCount;
                ui.ShowMessage("The signal is incomplete. Find " + remaining + " more clue(s).");
                return;
            }

            if (mission.TryCompleteChapter())
                ui.ShowMessage("CHAPTER ONE COMPLETE. THE LIGHTHOUSE SIGNAL IS FOUND.");
            else
                ui.ShowMessage("The signal points beyond Nambara. The search continues.");
        }
    }
}
