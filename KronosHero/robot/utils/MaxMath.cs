using System;

namespace KronosHero.robot.utils {
    public static class MaxMath {
        public static double Deadband(double value, double deadband) {
            return (Math.Abs(value) > deadband) ? value : 0;
        }

        public static double SquareInputs(double value) {
            return Math.Abs(value) * value;
        }

        public static double Clamp(double value, double low, double high) {
            return Math.Max(low, Math.Min(value, high));
        }
    }
}
