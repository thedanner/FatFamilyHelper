using System;

namespace Left4DeadHelper.Bindings.DevILNative.Bindings
{
    internal static class Common
    {
        #region Wow64
        public static bool Wow64() => Environment.Is64BitProcess;

        #endregion

        #region Helper methods

        public static uint Limit(uint x, uint min, uint max)
        {
            return x < min ? min : x > max ? max : x;
        }
        public static uint Clamp(uint x) => Limit(x, 0, 1);

        public static ushort Limit(ushort x, ushort min, ushort max)
        {
            return x < min ? min : x > max ? max : x;
        }
        public static ushort Clamp(ushort x) => Limit(x, 0, 1);

        public static float Limit(float x, float min, float max)
        {
            return x < min ? min : x > max ? max : x;
        }
        public static float Clamp(float x) => Limit(x, 0, 1);

        public static double Limit(double x, double min, double max)
        {
            return x < min ? min : x > max ? max : x;
        }
        public static double Clamp(double x) => Limit(x, 0, 1);

        #endregion
    }
}
