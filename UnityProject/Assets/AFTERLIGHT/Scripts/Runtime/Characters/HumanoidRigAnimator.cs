using UnityEngine;

namespace Afterlight
{
    public sealed class HumanoidRigAnimator : MonoBehaviour
    {
        public Transform leftArm;
        public Transform rightArm;
        public Transform leftLeg;
        public Transform rightLeg;
        public float stride = 28f;

        private float phase;
        private bool moving;

        public void SetMoving(bool value)
        {
            moving = value;
        }

        private void Update()
        {
            if (moving) phase += Time.deltaTime * 8.5f;
            float amount = moving ? 1f : 0f;
            float swing = Mathf.Sin(phase) * stride * amount;
            float idle = Mathf.Sin(Time.time * 1.6f) * 1.8f;

            if (leftArm) leftArm.localRotation = Quaternion.Euler(swing + idle, 0, -5f);
            if (rightArm) rightArm.localRotation = Quaternion.Euler(-swing - idle, 0, 5f);
            if (leftLeg) leftLeg.localRotation = Quaternion.Euler(-swing, 0, 0);
            if (rightLeg) rightLeg.localRotation = Quaternion.Euler(swing, 0, 0);
        }
    }
}
