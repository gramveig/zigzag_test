using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class RoadBlock : ShiftingObject
    {
        [SerializeField]
        private GameObject _crystal;
        
        private float _shiftTotal;

        private const float DisappearThresholdUnits = 3;

        protected override void Awake()
        {
            base.Awake();

            _crystal.SetActive(false);
        }

        public Vector2Int IntCoord
        {
            get
            {
                var p = _transform.position;
                return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
            }
        }

        public void ShowCrystal()
        {
            _crystal.SetActive(true);
        }

        public void SetColor(Color color)
        {
            var renderer = GetComponent<Renderer>();
            renderer.material.color = color;
        }
        
        protected override void Shift(float shift)
        {
            base.Shift(shift);

            _shiftTotal += shift;
            if (_shiftTotal >= DisappearThresholdUnits)
            {
                Disappear();
            }
        }

        private void Disappear()
        {
            Destroy(gameObject);
        }
    }
}