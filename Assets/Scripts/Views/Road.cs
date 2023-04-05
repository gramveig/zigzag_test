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
        private GameObject _sampleCube;

        [SerializeField]
        private bool _colorful = true;

        public Action OnCristalPickedEvent { get; set; }

        private ObjectPool<RoadBlock> _blockPool;
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

            _blockPool = new ObjectPool<RoadBlock>(InstantiateRoadBlock);
        }

        public void GenerateHomeYard()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    InsertRoadBlockFromPool(x, y);
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
            if (_shiftTotal >= 1f)
            {
                _shiftTotal = 0;
                while (IsNewBlockNeeded())
                {
                    AddRowOfBlocks();
                }
            }

            //can destroy blocks anytime
            DestroyBlocks();
        }

        public void Clear()
        {
            foreach (var block in _blocks)
            {
                _blockPool.Return(block);
            }

            _shiftTotal = 0;
            _lastDirection = RoadDirection.None;
            _sameDirectionBlocksCount = 0;
            _blockIdxInCluster = 0;
            _crystalIdxInCluster = 0;
            _blocks.Clear();
        }

        public bool IsForwardTile(int currentBlockIdx)
        {
            var currentBlock = _blocks[currentBlockIdx];
            return IsClosestForwardBlock(currentBlock);
        }

        /// <summary>
        /// Get the index of a block under the ball.
        /// Used in autoplay
        /// </summary>
        public int GetBlockIdx(Vector3 ballPos)
        {
            const float DistanceThreshold = 0.25f;

            ballPos -= new Vector3(0, ballPos.y, 0);
            for (int i = 0; i < _blocks.Count; i++)
            {
                var block = _blocks[i];
                float m = (ballPos - block.CachedTransform.position).magnitude;
                if (m < DistanceThreshold)
                {
                    return i;
                }
            }

            //ball position is too far from the center of the block, needs aligning with the nearest block
            return -1;
        }

        /// <summary>
        /// Get the position of the block that is closest to the ball
        /// Used in autoplay
        /// </summary>
        public Vector3 GetNearestBlockPos(Vector3 ballPos)
        {
            float minMagn = float.MaxValue;
            RoadBlock nearestBlock = _blocks[0];
            foreach (var block in _blocks)
            {
                float m = (ballPos - block.CachedTransform.position).magnitude;
                if (m < minMagn)
                {
                    minMagn = m;
                    nearestBlock = block;
                }
            }

            return nearestBlock.CachedTransform.position;
        }

        private RoadBlock InstantiateRoadBlock()
        {
            var roadBlock = Instantiate(_roadBlock, _transform);
            return roadBlock;
        }
        
        private RoadBlock InsertRoadBlockFromPool(int x, int y)
        {
            var roadBlock = _blockPool.Draw();
            
            roadBlock.CachedTransform.position = new Vector3(x, 0, y);
            _blocks.Add(roadBlock);

            return roadBlock;
        }

        private void InsertRoadBlockFromPoolIntoCluster(int x, int y)
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

            var roadBlock = InsertRoadBlockFromPool(x, y);
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
                    InsertRoadBlockFromPoolIntoCluster(cornerTileCoord.x + i, cornerTileCoord.y - 1);
                }
                else if (direction == RoadDirection.Right)
                {
                    InsertRoadBlockFromPoolIntoCluster(cornerTileCoord.x - 1, cornerTileCoord.y + i);
                }
                else
                {
                    throw new Exception("Unimplemented direction");
                }
            }
        }

        private bool IsNewBlockNeeded()
        {
            var lastBlockWorldPos = _blocks[^1].CachedTransform.position;
            var lastBlockScreenPos = _cam.WorldToScreenPoint(lastBlockWorldPos + new Vector3(1, 0, 1) * CloseToScreenEdgeUnitsThreshold);
            return lastBlockScreenPos.y < Screen.height;
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
                    block.Disappear(_blockPool);
                    _blocks.RemoveAt(i);
                }
            }
        }

        private void OnCristalPicked()
        {
            OnCristalPickedEvent?.Invoke();
        }

        private bool IsClosestForwardBlock(RoadBlock block)
        {
            const float SameCoordThreshold = 0.25f;
            const float DifferentCoordThreshold = 0.5f;
            
            foreach (var b in _blocks)
            {
                if (b == block)
                {
                    continue;
                }

                var p1 = block.CachedTransform.position;
                var p2 = b.CachedTransform.position;
                if (Mathf.Abs(p1.x - p2.x) < SameCoordThreshold && p2.z - p1.z < -DifferentCoordThreshold)
                {
                    return true;
                }
            }

            return false;
        }
    }
}