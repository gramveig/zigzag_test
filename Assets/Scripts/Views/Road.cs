using System;
using System.Collections.Generic;
using UnityEngine;


namespace Alexey.ZigzagTest.Views
{
    public class Road : MonoBehaviour, IObservable<float>
    {
        [SerializeField]
        private GameObject _roadBlock;

        private Transform _transform;
        private List<IObserver<float>> _observers = new List<IObserver<float>>();

        private void Awake()
        {
            _transform = transform;

            //add observers for the road blocks already on scene
            var shiftingObjects = FindObjectsOfType<ShiftingObject>();
            foreach (var shiftingObject in shiftingObjects)
            {
                AddObserver(shiftingObject.gameObject);
            }
        }

        public void GenerateHomeYard()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    InitRoadBlock(x, y);
                }
            }
        }

        public void Shift(float shift)
        {
            foreach (var observer in _observers)
            {
                observer.Notify(shift);
            }
        }

        public void Unsubscribe(IObserver<float> observer)
        {
            _observers.Remove(observer);
        }
        
        private void InitRoadBlock(int x, int y)
        {
            var roadBlock = Instantiate(_roadBlock, new Vector3(x, 0, y), Quaternion.identity, _transform);
            AddObserver(roadBlock);
        }

        private void AddObserver(GameObject obj)
        {
            var observer = obj.GetComponent<IObserver<float>>();
            if (observer != null)
            {
                _observers.Add(observer.Subscribe(this));
            }
        }
    }
}