using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class Road : MonoBehaviour
    {
        [SerializeField]
        private GameObject _roadBlock;

        public void GenerateHomeYard()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    InitRoadBlock(x, y);
                }
            }
        }

        private void InitRoadBlock(int x, int y)
        {
            Instantiate(_roadBlock, new Vector3(x, 0, y), Quaternion.identity);
        }
    }
}