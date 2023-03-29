using System.Collections.Generic;
using Alexey.ZigzagTest.Views;
using UnityEngine;

namespace Alexey.ZigzagTest.Models
{
    public class GameModel : IObservable<int>
    {
        private List<IObserver<int>> _observers = new();

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                foreach (var observer in _observers)
                {
                    observer.Notify(Score);
                }
            }
        }

        public void AddObserver(GameObject obj)
        {
            var observer = obj.GetComponent<IObserver<int>>();
            if (observer != null)
            {
                _observers.Add(observer.Subscribe(this));
            }
        }

        public void Unsubscribe(IObserver<int> observer)
        {
            _observers.Remove(observer);
        }
    }
}