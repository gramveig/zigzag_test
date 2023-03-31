using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class ShiftingObject : MonoBehaviour, IShiftable
    {
        protected Transform _transform;

        protected virtual void Awake()
        {
            _transform = transform;
        }

        public virtual void Shift(float shift)
        {
            Vector3 p = _transform.position;
            _transform.position = new Vector3(p.x + shift, 0, p.z + shift);
        }
    }
}