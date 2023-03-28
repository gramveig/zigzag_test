using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace Alexey.ZigzagTest.Views
{
    public class Road : MonoBehaviour, IObservable<float>
    {
        [SerializeField]
        private RoadBlock _roadBlock;

        [SerializeField]
        private CrystalMode _crystalMode;

        [SerializeField]
        private int _roadWidth = 1;

        private Transform _transform;
        private List<IObserver<float>> _observers = new List<IObserver<float>>();
        private float _shiftTotal;
        private RoadDirection _lastDirection = RoadDirection.None;
        private int _sameDirectionBlocksCount;
        private int _blockIdxInCluster;
        private int _crystalIdxInCluster;
        private int _currentRow;
        private int _deletedRow;

        private const int MaxBlocksOfSameDirection = 5;
        private const int MaxBlocksInCluster = 5;
        private const int MaxRoadWidth = 3;
        private const int RoadBeginningLength = 20;
        private const int RowsToSkipBeforeDeletingStarts = 3;

        private readonly Color[] _testColors = new[] { Color.white, Color.yellow, Color.red, Color.green, Color.cyan };
        private bool _debugColors = true;

        enum RoadDirection
        {
            None,
            Forward,
            Right
        }

        enum CrystalMode
        {
            Random,
            Ordered
        }

        private void Awake()
        {
            if (_roadWidth < 0)
            {
                _roadWidth = 0;
            }
            else if (_roadWidth > MaxRoadWidth)
            {
                _roadWidth = MaxRoadWidth;
            }
            
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
            for (int i = 0; i < RoadBeginningLength; i++)
            {
                AddRowOfBlocks();
            }
        }
        
        public void Shift(float shift)
        {
            _shiftTotal += shift;

            foreach (var observer in _observers)
            {
                observer.Notify(shift);
            }

            if (_shiftTotal >= 1)
            {
                _shiftTotal = 0;
                AddRowOfBlocks();

                if (_currentRow > RowsToSkipBeforeDeletingStarts)
                {
                    DeleteRowOfBlocks();
                }
            }
        }

        public void Unsubscribe(IObserver<float> observer)
        {
            _observers.Remove(observer);
        }

        private RoadBlock InstantiateRoadBlock(int x, int y)
        {
            var roadBlock = Instantiate(_roadBlock, new Vector3(x, 0, y), Quaternion.identity, _transform);
            AddObserver(roadBlock.gameObject);

            return roadBlock;
        }

        private RoadBlock InstantiateRoadBlockInCluster(int x, int y)
        {
            if (_blockIdxInCluster >= MaxBlocksInCluster)
            {
                _blockIdxInCluster = 0;
                if (_crystalMode == CrystalMode.Random)
                {
                    _crystalIdxInCluster = UnityEngine.Random.Range(0, MaxBlocksInCluster);
                }
                else if (_crystalMode == CrystalMode.Ordered)
                {
                    if (_crystalIdxInCluster >= MaxBlocksInCluster)
                    {
                        _crystalIdxInCluster = 0;
                    }
                    else
                    {
                        _crystalIdxInCluster++;
                    }
                }
                else
                {
                    throw new Exception("Unimplemented crystal mode");
                }
            }

            var roadBlock = InstantiateRoadBlock(x, y);
            if (_blockIdxInCluster == _crystalIdxInCluster)
            {
                roadBlock.ShowCrystal();
            }

            if (_debugColors)
            {
                roadBlock.SetColor(_testColors[_blockIdxInCluster]);
            }
            
            _blockIdxInCluster++;

            return roadBlock;
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

        private RoadDirection GetRandomDirection()
        {
            var direction = (RoadDirection)(UnityEngine.Random.Range(0, 2) + 1);
            if (direction == _lastDirection)
            {
                _sameDirectionBlocksCount++;
                if (_sameDirectionBlocksCount >= MaxBlocksOfSameDirection)
                {
                    direction = GetDifferentDirection(direction);
                    _sameDirectionBlocksCount = 0;
                    _lastDirection = direction;
                }
            }
            else
            {
                _sameDirectionBlocksCount = 0;
                _lastDirection = direction;
            }

            return direction;
        }

        private RoadDirection GetDifferentDirection(RoadDirection direction)
        {
            if (direction == RoadDirection.Forward)
            {
                return RoadDirection.Right;
            }

            return RoadDirection.Forward;
        }

        private void AddRowOfBlocks()
        {
            var cornerTileCoord = GetRightmostTileCoord();
            Debug.Log(cornerTileCoord);
            var direction = GetRandomDirection();
            _currentRow++;
            for (int i = 0; i < _roadWidth; i++)
            {
                RoadBlock roadBlock;
                if (direction == RoadDirection.Forward)
                {
                    roadBlock = InstantiateRoadBlockInCluster(cornerTileCoord.x + i, cornerTileCoord.y - 1);
                }
                else if (direction == RoadDirection.Right)
                {
                    roadBlock = InstantiateRoadBlockInCluster(cornerTileCoord.x - 1, cornerTileCoord.y + i);
                }
                else
                {
                    throw new Exception("Unimplemented direction");
                }

                roadBlock.Row = _currentRow;
            }
        }

        private void DeleteRowOfBlocks()
        {
            foreach (var observer in _observers)
            {
                var roadBlock = observer as RoadBlock;
                if (roadBlock != null)
                {
                    roadBlock.DeleteRow(_deletedRow);
                }
            }

            _deletedRow++;
        }
    }
}