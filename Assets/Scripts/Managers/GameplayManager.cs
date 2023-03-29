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

            /*
            if (Input.GetKeyDown(KeyCode.T))
            {
                _road.AddBlocks();
            }
            */

            _road.Shift(_speed * Time.deltaTime);
        }
    }
}