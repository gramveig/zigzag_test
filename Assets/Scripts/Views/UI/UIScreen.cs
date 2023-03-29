using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views.UI
{
    public class UIScreen : MonoBehaviour
    {
        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}