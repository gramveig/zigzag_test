using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace Alexey.ZigzagTest.Views
{
    public class Road : MonoBehaviour, IObservable<float>
    {
        [SerializeField]
        private GameObject _roadBlock;

        private Transform _transform;
        private List<IObserver<float>> _observers = new List<IObserver<float>>();
        private float _shiftTotal;

        enum RoadDirection
        {
            Forward,
            Right
        }

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

                    InstantiateRoadBlock(x, y);
                }
            }
        }

        public void GenerateRoadBeginning()
        {
            for (int i = 0; i < 10; i++)
            {
                AddBlocks();
            }
        }
        
        public void Shift(float shift)
        {
            _shiftTotal += shift;

            foreach (var observer in _observers)
            {
                observer.Notify(shift);
            }
        }

        public void Unsubscribe(IObserver<float> observer)
        {
            _observers.Remove(observer);
        }
        
        private void InstantiateRoadBlock(int x, int y)
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

        private Vector2Int GetRightmostTileCoord()
        {
            int rightmostBlockX = int.MaxValue;
            int rightmostBlockY = int.MaxValue;

            foreach (var observer in _observers)
            {
                var roadBlock = observer as RoadBlock;
                if (roadBlock == null)
                {
                    continue;
                }

                var coord = roadBlock.IntCoord;
                if (coord.x < rightmostBlockX)
                {
                    rightmostBlockX = coord.x;
                }
                
                if (coord.y < rightmostBlockY)
                {
                    rightmostBlockY = coord.y;
                }
            }

            return new Vector2Int(rightmostBlockX, rightmostBlockY);
        }

        RoadDirection GetRandomDirection()
        {
            return (RoadDirection)UnityEngine.Random.Range(0, 2);
        }

        public void AddBlocks()
        {
            var cornerTileCoord = GetRightmostTileCoord();
            Debug.Log(cornerTileCoord);
            var direction = GetRandomDirection();
            if (direction == RoadDirection.Forward)
            {
                InstantiateRoadBlock(cornerTileCoord.x, cornerTileCoord.y - 1);
            }
            else if (direction == RoadDirection.Right)
            {
                InstantiateRoadBlock(cornerTileCoord.x - 1, cornerTileCoord.y);
            }
            else
            {
                throw new Exception("Unimplemented direction");
            }
        }
    }
}