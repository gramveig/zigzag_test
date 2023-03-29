using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class GameCamera : MonoBehaviour
    {
        private Transform _transform;
        private Vector3 _iniPosition;

        private void Awake()
        {
            _transform = transform;
        }

        public void SetIniPosition(Transform target)
        {
            _iniPosition = target.position - _transform.position;
        }

        public void Follow(Transform target)
        {
            _transform.position = target.position - _iniPosition;
        }
    }
}
