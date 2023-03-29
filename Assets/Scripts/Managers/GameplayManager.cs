using Alexey.ZigzagTest.Views;
using Alexey.ZigzagTest.Models;
using Alexey.ZigzagTest.Views.UI;
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
        private GameCamera _camera;
        
        [SerializeField]
        private UIScreen _startScreen;
        
        [SerializeField]
        private UIScreen _gameOverScreen;
        
        [SerializeField]
        private UIScreen _gameScreen;
        
        [SerializeField]
        private float _speed = 0.2f;

        private bool _gameStarted;
        private GameModel _gameModel;
        
        private void Start()
        {
            _road.GenerateHomeYard();
            _road.GenerateRoadBeginning();
            _road.OnCristalPickedEvent = OnCrystalPicked;
            _camera.SetIniPosition(_ball.CachedTransform);
            _ball.OnFallDownEvent = OnBallFallDown;
            _gameModel = new GameModel();
            _gameModel.AddObserver(_gameScreen.gameObject);
            _gameModel.Score = 0;
            _startScreen.Show();
            WaitForStart();
        }

        private async void WaitForStart()
        {
            await UniTask.WaitUntil(() => Input.anyKeyDown);

            _gameStarted = true;
            _startScreen.Hide();
            _gameScreen.Show();
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
            _camera.Follow(_ball.CachedTransform);
        }

        private void OnCrystalPicked()
        {
            if (!_gameStarted)
            {
                return;
            }

            _gameModel.Score++;
        }

        private void OnBallFallDown()
        {
            _gameStarted = false;
            _gameScreen.Hide();
            _gameOverScreen.Show();
        }
    }
}