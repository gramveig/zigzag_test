using Alexey.ZigzagTest.Views;
using UnityEngine;


namespace Alexey.ZigzagTest.Managers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private Road _road;

        float _speed = 0.2f;

        private void Start()
        {
            _road.GenerateHomeYard();
            _road.GenerateRoadBeginning();
        }

        private void Update()
        {
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