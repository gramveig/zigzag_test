using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class ShiftingObject : MonoBehaviour, IObserver<float>
    {
        protected Transform _transform;
        private IObservable<float> _road;

        protected virtual void Awake()
        {
            _transform = transform;
        }

        private void OnDestroy()
        {
            _road.Unsubscribe(this);
        }

        public IObserver<float> Subscribe(IObservable<float> road)
        {
            _road = road;

            return this;
        }

        public void Notify(float value)
        {
            Shift(value);
        }

        protected void Shift(float shift)
        {
            Vector3 p = _transform.position;
            _transform.position = new Vector3(p.x + shift, 0, p.z + shift);
        }
    }
}