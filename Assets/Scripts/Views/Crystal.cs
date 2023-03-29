using UnityEngine;
using System;

namespace Alexey.ZigzagTest.Views
{
    public class Crystal : MonoBehaviour
    {
        private Action _onPicked;

        public void Show(Action onPicked)
        {
            _onPicked = onPicked;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Ball"))
            {
                _onPicked?.Invoke();
                Hide();
            }
        }
    }
}