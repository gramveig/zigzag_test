using System;
using UnityEngine;

namespace Alexey.ZigzagTest.Views
{
    public class RoadBlock : ShiftingObject
    {
        [SerializeField] private GameObject _crystal;

        public int Row { get; set; }

        private float _shiftTotal;

        protected override void Awake()
        {
            base.Awake();

            _crystal.SetActive(false);
        }

        public Vector2Int IntCoord
        {
            get
            {
                var p = _transform.position;
                return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
            }
        }

        public void ShowCrystal()
        {
            _crystal.SetActive(true);
        }

        public void SetColor(Color color)
        {
            var rdr = GetComponent<Renderer>();
            rdr.material.color = color;
        }

        public void DeleteRow(int row)
        {
            if (Row == row)
            {
                Disappear();
            }
        }
        
        private void Disappear()
        {
            Destroy(gameObject);
        }
    }
}