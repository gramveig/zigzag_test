using UnityEngine;
using System;

namespace Alexey.ZigzagTest.Views
{
    public enum BallMovementDirection
    {
        Forward,
        Right
    }
    
    public class Ball : MonoBehaviour
    {
        private BallMovementDirection _movementDirection = BallMovementDirection.Forward;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }
        
        public void Move(float shift)
        {
            var p = _transform.position;
            if (_movementDirection == BallMovementDirection.Forward)
            {
                _transform.position = new Vector3(p.x + shift, p.y, p.z);
            }
            else if (_movementDirection == BallMovementDirection.Right)
            {
                _transform.position = new Vector3(p.x, p.y, p.z + shift);
            }
            else
            {
                throw new Exception("Unimplemented movement direction");
            }
        }

        public void ChangeMovementDirection()
        {
            _movementDirection = GetDifferentDirection(_movementDirection);
            Debug.Log(_movementDirection);
        }

        private BallMovementDirection GetDifferentDirection(BallMovementDirection direction)
        {
            if (direction == BallMovementDirection.Forward)
            {
                return BallMovementDirection.Right;
            }

            return BallMovementDirection.Forward;
        }
    }
}