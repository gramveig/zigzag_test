using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class RoadBlock : ShiftingObject
    {
        [SerializeField]
        private Crystal _crystal;

        private float _shiftTotal;

        protected override void Awake()
        {
            base.Awake();

            _crystal.Hide();
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
            Destroy(gameObject);
        }
    }
}