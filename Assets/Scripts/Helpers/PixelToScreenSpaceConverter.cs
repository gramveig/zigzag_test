using UnityEngine;

namespace Alexey.ZigzagTest.Helpers
{
    /// <summary>
    /// Used to convert pixel coordinates (as those returned by Screen.width/Screen.height and Screen.safeArea)
    /// into coordinates used by UI Canvas component set to Screen Space - Camera render mode
    /// </summary>
    public class PixelToScreenSpaceConverter
    {
        private Canvas _canvas;
        private Camera _camera;

        public PixelToScreenSpaceConverter(Canvas canvas, Camera camera)
        {
            _canvas = canvas;
            _camera = camera;
        }

        /// <summary>
        /// Convert pixel coordinate into screen space coordinate
        /// </summary>
        public Vector3 ToScreenSpace(Vector3 pixelPos)
        {
            pixelPos.z = -(_canvas.transform.position - _camera.transform.position).magnitude;
            Vector3 screenSpacePos = _camera.ScreenToWorldPoint(pixelPos);

            return screenSpacePos;
        }
    }
}
