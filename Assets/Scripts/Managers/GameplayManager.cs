using System;
using Alexey.ZigzagTest.Helpers;
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
        private GameScreen _gameScreen;

        [SerializeField]
        private float _speed = 0.2f;

        private bool _gameStarted;
        private GameModel _gameModel;

        private async void Start()
        {
            await StartGame();
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

        private async UniTask StartGame()
        {
            _road.GenerateHomeYard();
            _road.GenerateRoadBeginning();
            _road.OnCristalPickedEvent = OnCrystalPicked;
            _camera.SetIniPosition(_ball.CachedTransform);
            _ball.OnFallDownEvent = OnBallFallDown;
            _gameModel = new GameModel();
            _gameModel.AddScoreObserver(_gameScreen);
            _gameModel.Score = 0;
            _startScreen.Show();
            await WaitForStart();
        }
        
        private async UniTask WaitForStart()
        {
            await UniTask.WaitUntil(() => Input.anyKeyDown);

            _gameStarted = true;
            _startScreen.Hide();
            _gameScreen.Show();
        }

        private void OnCrystalPicked()
        {
            if (!_gameStarted)
            {
                return;
            }

            _gameModel.Score++;
        }

        private async void OnBallFallDown()
        {
            await EndGame();
        }

        private async UniTask EndGame()
        {
            _gameStarted = false;
            _gameScreen.Hide();
            _gameOverScreen.Show();
            await WaitForRestart();
        }

        private async UniTask WaitForRestart()
        {
            await UniTask.WaitUntil(() => Input.anyKeyDown);

            await RestartGame();
        }

        private async UniTask RestartGame()
        {
            _gameOverScreen.Hide();
            _road.Clear();
            _ball.Reset();
            _ball.Show();
            
            _road.GenerateHomeYard();
            _road.GenerateRoadBeginning();
            _gameModel = new GameModel();
            _gameModel.AddScoreObserver(_gameScreen);
            _gameModel.Score = 0;

            await WaitAndStartGame();
        }

        private async UniTask WaitAndStartGame()
        {
            _camera.Follow(_ball.CachedTransform);
            _startScreen.Show();
            await UniTask.WaitUntil(() => Input.anyKeyDown);

            _startScreen.Hide();
            _gameScreen.Show();
            _gameStarted = true;
        }
    }
}