using Alexey.ZigzagTest.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Alexey.ZigzagTest.Managers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private Road _road;

        [SerializeField]
        private Ball _ball;
        
        [SerializeField]
        private float _speed = 0.2f;

        private bool _gameStarted;

        private void Start()
        {
            _road.GenerateHomeYard();
            _road.GenerateRoadBeginning();
            WaitForStart();
        }

        private async void WaitForStart()
        {
            await UniTask.WaitUntil(() => Input.anyKey);

            _gameStarted = true;
        }

        private void Update()
        {
            if (!_gameStarted)
            {
                return;
            }

            if (Input.anyKeyDown)
            {
                _ball.ChangeMovementDirection();
            }

            float shift = _speed * Time.deltaTime;
            _road.Shift(shift);
            _ball.Move(shift);
        }
    }
}