using UnityEngine;

namespace Afterlight
{
    public abstract class WorldInteractable : MonoBehaviour
    {
        public abstract string Prompt { get; }
        public abstract void Interact(PlayerMotor player, GameUI ui);
    }
}
