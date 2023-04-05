using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Alexey.ZigzagTest.Views
{
    public class Ball : MonoBehaviour
    {
        public Action OnFallDownEvent { get; set; }

        public enum MovementDirection
        {
            Right,
            Forward
        }

        private bool _isActive = false;
        private MovementDirection _movementDirection;
        private Transform _transform;
        private Vector3 _iniPosition;
        private Rigidbody _rigidBody;
        private CancellationTokenSource _cancellationTokenSource;

        private const float BallFallDownThreshold = 0;

        private void Awake()
        {
            _transform = transform;
            _rigidBody = GetComponent<Rigidbody>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _iniPosition = _transform.position;
            _isActive = true;
        }

        private async void Update()
        {
            if (!_isActive)
            {
                return;
            }

            try
            {
                await MonitorFallDown();
            }
            catch (Exception e)
            {
                if (!e.IsOperationCanceledException())
                {
                    throw;
                }
            }
        }

        public void Move(float shift)
        {
            var p1 = _transform.position;
            if (_movementDirection == MovementDirection.Forward)
            {
                var p2 = new Vector3(p1.x + shift, p1.y, p1.z - shift);
                _transform.position = p2;
                transform.RotateAround(transform.position, -Vector3.right, Mathf.Sin((p2 - p1).magnitude * 0.5f * 2 * Mathf.PI) * Mathf.Rad2Deg);
            }
            else if (_movementDirection == MovementDirection.Right)
            {
                var p2 = new Vector3(p1.x - shift, p1.y, p1.z + shift);
                _transform.position = p2;
                transform.RotateAround(transform.position, Vector3.forward, Mathf.Sin((p2 - p1).magnitude * 0.5f * 2 * Mathf.PI) * Mathf.Rad2Deg);
            }
            else
            {
                throw new Exception("Unimplemented movement direction");
            }
        }

        public void ChangeMovementDirection()
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _movementDirection = GetDifferentDirection(_movementDirection);
        }

        public void SetMovementDirection(MovementDirection direction)
        {
            if (_movementDirection == direction)
            {
                return;
            }

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _movementDirection = direction;
        }

        public Transform CachedTransform => _transform;

        public MovementDirection Direction => _movementDirection;

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
            _cancellationTokenSource.Cancel(true);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _movementDirection = MovementDirection.Right;
            _transform.rotation = Quaternion.identity;
            _transform.position = _iniPosition;
            _cancellationTokenSource = new CancellationTokenSource();
            _isActive = true;
        }
        
        private MovementDirection GetDifferentDirection(MovementDirection direction)
        {
            if (direction == MovementDirection.Forward)
            {
                return MovementDirection.Right;
            }

            return MovementDirection.Forward;
        }

        private async UniTask MonitorFallDown()
        {
            if (_isActive && _transform.position.y < BallFallDownThreshold)
            {
                _isActive = false;
                OnFallDownEvent?.Invoke();
                await WaitAndHide(); 
            }
        }

        private async UniTask WaitAndHide()
        {
            await UniTask.Delay(500, cancellationToken: _cancellationTokenSource.Token);

            Hide();
        }
    }
}