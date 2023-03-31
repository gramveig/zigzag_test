using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class RoadBlock : ShiftingObject
    {
        [SerializeField]
        private Crystal _crystal;

        [SerializeField]
        private float _fallSpeed = 1;

        private float _shiftTotal;
        private bool _isFalling;

        private const float FallThreshold = -10;
        
        protected override void Awake()
        {
            base.Awake();

            _crystal.Hide();
        }

        protected void Update()
        {
            if (!_isFalling)
            {
                return;
            }

            var p = _transform.position;
            _transform.position = new Vector3(p.x, p.y - _fallSpeed * Time.deltaTime, p.z);

            if (p.y < FallThreshold)
            {
                Destroy(gameObject);
            }
        }
        
        public Vector2Int IntCoord
        {
            get
            {
                var p = _transform.position;
                return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
            }
        }

        public void ShowCrystal(Action onPick)
        {
            _crystal.Show(onPick);
        }

        public void SetColor(Color color)
        {
            var rdr = GetComponent<Renderer>();
            rdr.material.color = color;
        }

        public Transform CachedTransform => _transform;
        
        public void Disappear()
        {
            _isFalling = true;
        }

        public override void Shift(float shift)
        {
            if (!_isFalling)
            {
                base.Shift(shift);
            }
        }
    }
}