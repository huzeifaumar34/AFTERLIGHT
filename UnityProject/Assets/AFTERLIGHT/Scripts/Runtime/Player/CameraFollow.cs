using UnityEngine;

namespace Afterlight
{
    public sealed class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 4.2f, -6.8f);
        public float positionSmooth = 7f;
        public float lookHeight = 1.15f;

        private void LateUpdate()
        {
            if (!target) return;
            Vector3 desired = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, desired,
                1f - Mathf.Exp(-positionSmooth * Time.deltaTime));
            Vector3 focus = target.position + Vector3.up * lookHeight;
            Quaternion look = Quaternion.LookRotation(focus - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look,
                1f - Mathf.Exp(-12f * Time.deltaTime));
        }
    }
}
