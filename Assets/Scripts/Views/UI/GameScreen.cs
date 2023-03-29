using System;
using TMPro;
using UnityEngine;

namespace Alexey.ZigzagTest.Views.UI
{
    public class GameScreen : UIScreen, IObserver<int>
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        
        private IObservable<int> _model;

        private void OnDestroy()
        {
            _model?.Unsubscribe(this);
        }

        public IObserver<int> Subscribe(IObservable<int> observable)
        {
            _model = observable;

            return this;
        }

        public void Notify(int value)
        {
            _text.text = "Score: " + value;
        }
    }
}