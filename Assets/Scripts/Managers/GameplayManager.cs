using Alexey.ZigzagTest.Views;
using UnityEngine;


namespace Alexey.ZigzagTest.Managers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private Road _road;

        private void Start()
        {
            _road.GenerateHomeYard();
        }
    }
}