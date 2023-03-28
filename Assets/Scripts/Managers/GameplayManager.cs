using Alexey.ZigzagTest.Views;
using UnityEngine;


namespace Alexey.ZigzagTest.Managers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private Road _road;

        float _speed = 0.05f;

        private void Start()
        {
            _road.GenerateHomeYard();
        }

        private void Update()
        {
            _road.Shift(_speed * Time.deltaTime);
        }
    }
}