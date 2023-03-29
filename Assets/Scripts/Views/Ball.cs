using UnityEngine;
using System;

namespace Alexey.ZigzagTest.Views
{
    public class Ball : MonoBehaviour
    {
        public Action OnFallDownEvent { get; set; }

        private enum MovementDirection
        {
            Right,
            Forward
        }
        
        private MovementDirection _movementDirection;
        private Transform _transform;

        private const float BallFallDownThreshold = -3;
        
        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            MonitorFallDown();
        }

        public void Move(float shift)
        {
            var p = _transform.position;
            if (_movementDirection == MovementDirection.Forward)
            {
                _transform.position = new Vector3(p.x + shift, p.y, p.z - shift);
            }
            else if (_movementDirection == MovementDirection.Right)
            {
                _transform.position = new Vector3(p.x - shift, p.y, p.z + shift);
            }
            else
            {
                throw new Exception("Unimplemented movement direction");
            }
        }

        public void ChangeMovementDirection()
        {
            _movementDirection = GetDifferentDirection(_movementDirection);
        }

        public Transform CachedTransform => _transform;

        private MovementDirection GetDifferentDirection(MovementDirection direction)
        {
            if (direction == MovementDirection.Forward)
            {
                return MovementDirection.Right;
            }

            return MovementDirection.Forward;
        }

        private void MonitorFallDown()
        {
            if (_transform.position.y < BallFallDownThreshold)
            {
                OnFallDownEvent?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}