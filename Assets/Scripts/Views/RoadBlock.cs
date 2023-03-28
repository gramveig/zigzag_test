using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class RoadBlock : ShiftingObject
    {
        private float _shiftTotal;

        private const float DisappearThresholdUnits = 3;

        public Vector2Int IntCoord
        {
            get
            {
                var p = _transform.position;
                return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
            }
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