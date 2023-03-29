using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

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

        private const float BallFallDownThreshold = 0;
        
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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            _movementDirection = MovementDirection.Right;
            _transform.rotation = Quaternion.identity;
        }
        
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
                WaitHide();
            }
        }

        private async void WaitHide()
        {
            await UniTask.Delay(500);
            
            Hide();
        }
    }
}