using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Random = System.Random;


namespace Alexey.ZigzagTest.Views
{
    public class Road : MonoBehaviour
    {
        [SerializeField]
        private RoadBlock _roadBlock;

        [SerializeField]
        private CrystalMode _crystalMode;

        [SerializeField]
        private int _roadWidth = 1;
        
        [SerializeField]
        private Ball _ball;

        [SerializeField]
        private GameObject _sampleCube;

        [SerializeField]
        private bool _colorful = true;

        public Action OnCristalPickedEvent { get; set; }

        private Transform _transform;
        private List<RoadBlock> _blocks = new();
        private float _shiftTotal;
        private RoadDirection _lastDirection = RoadDirection.None;
        private int _sameDirectionBlocksCount;
        private int _blockIdxInCluster;
        private int _crystalIdxInCluster;
        private Camera _cam;

        private const int MaxBlocksOfSameDirection = 5;
        private const int MaxBlocksInCluster = 5;
        private const int MaxRoadWidth = 3;
        private const int RoadBeginningLength = 20;
        private const float NewBlockPosThreshold = 15;
        private const float CloseToScreenEdgeUnitsThreshold = 2;

        private readonly Color[] _blockColors = new[] { Color.white, Color.yellow, Color.red, Color.green, Color.cyan };

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
            Destroy(_sampleCube);

            _cam = Camera.main;
            
            if (_roadWidth < 0)
            {
                _roadWidth = 0;
            }
            else if (_roadWidth > MaxRoadWidth)
            {
                _roadWidth = MaxRoadWidth;
            }
            
            _transform = transform;
        }

        public void GenerateHomeYard()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
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

            foreach (var block in _blocks)
            {
                block.Shift(shift);
            }

            //have to wait while the blocks are aligned with coordinate grid to add new blocks
            if (_shiftTotal > 1)
            {
                _shiftTotal = 0;
                if (IsNewBlockNeeded())
                {
                    AddRowOfBlocks();
                    AddRowOfBlocks();
                    AddRowOfBlocks();
                }
            }

            //can destroy blocks anytime
            DestroyBlocks();
        }

        public void Clear()
        {
            foreach (Transform t in _transform)
            {
                Destroy(t.gameObject);
            }

            _shiftTotal = 0;
            _lastDirection = RoadDirection.None;
            _sameDirectionBlocksCount = 0;
            _blockIdxInCluster = 0;
            _crystalIdxInCluster = 0;
            _blocks.Clear();
        }

        private RoadBlock InstantiateRoadBlock(int x, int y)
        {
            var roadBlock = Instantiate(_roadBlock, new Vector3(x, 0, y), Quaternion.identity, _transform);
            _blocks.Add(roadBlock);

            return roadBlock;
        }

        private void InstantiateRoadBlockInCluster(int x, int y)
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
                roadBlock.ShowCrystal(OnCristalPicked);
            }

            if (_colorful)
            {
                roadBlock.SetColor(_blockColors[_blockIdxInCluster]);
            }
            
            _blockIdxInCluster++;
        }
        
        private Vector2Int GetRightmostTileCoord()
        {
            int rightmostBlockX = int.MaxValue;
            int rightmostBlockY = int.MaxValue;

            foreach (var block in _blocks)
            {
                var roadBlock = block as RoadBlock;
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
            var direction = GetRandomDirection();
            for (int i = 0; i < _roadWidth; i++)
            {
                if (direction == RoadDirection.Forward)
                {
                    InstantiateRoadBlockInCluster(cornerTileCoord.x + i, cornerTileCoord.y - 1);
                }
                else if (direction == RoadDirection.Right)
                {
                    InstantiateRoadBlockInCluster(cornerTileCoord.x - 1, cornerTileCoord.y + i);
                }
                else
                {
                    throw new Exception("Unimplemented direction");
                }
            }
        }

        private bool IsNewBlockNeeded()
        {
            var tc = GetRightmostTileCoord();
            var p = new Vector3(tc.x, 0 ,tc.y);
            return (p - _ball.CachedTransform.position).magnitude < NewBlockPosThreshold;
        }

        private void DestroyBlocks()
        {
            for (int i = _blocks.Count - 1; i >= 0; i--)
            {
                var block = _blocks[i];
                var blockWorldPos = block.CachedTransform.position;
                var blockScreenPos = _cam.WorldToScreenPoint(blockWorldPos + new Vector3(1, 0, 1) * CloseToScreenEdgeUnitsThreshold);
                if (blockScreenPos.y < 0)
                {
                    block.Disappear();
                    _blocks.RemoveAt(i);
                }
            }
        }

        private void OnCristalPicked()
        {
            OnCristalPickedEvent?.Invoke();
        }
    }
}