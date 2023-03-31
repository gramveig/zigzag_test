using System.Collections.Generic;
using Alexey.ZigzagTest.Views;
using UnityEngine;

namespace Alexey.ZigzagTest.Models
{
    public class GameModel : IObservable<int>
    {
        private List<IObserver<int>> _scoreObservers = new();

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                foreach (var observer in _scoreObservers)
                {
                    observer.Notify(Score);
                }
            }
        }

        public void AddScoreObserver(IObserver<int> observer)
        {
            _scoreObservers.Add(observer.Subscribe(this));
        }

        public void Unsubscribe(IObserver<int> observer)
        {
            _scoreObservers.Remove(observer);
        }
    }
}