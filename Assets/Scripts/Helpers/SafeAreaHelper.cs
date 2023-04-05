using UnityEngine;

namespace Alexey.ZigzagTest.Helpers
{
    public static class SafeAreaHelper
    {
        public static Rect GetSafeArea()
        {
#if UNITY_EDITOR && SAFE_AREA_ENABLE
            //if using Device Simulator in Editor, return the safe area of the simulated device
            if (IsDeviceSimulator)
            {
                return Screen.safeArea;
            }

            //otherwise, fake iPhoneX
            return new Rect(0f, 132f / 2436f * Screen.height, 1f * Screen.width, (2436f - 102f - 132f) / 2436f * Screen.height);
#else
            //use safe area on device
            return Screen.safeArea;
#endif
        }

        /// <summary>
        /// Returns true if running inside Unity Editor using Device Simulator
        /// </summary>
        private static bool IsDeviceSimulator
        {
            get
            {
#if UNITY_EDITOR
                return UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop;
#endif
                return false;
            }
        }
    }
}
