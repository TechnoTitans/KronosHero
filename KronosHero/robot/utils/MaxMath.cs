namespace Kronos.robot.utils {
    public static class MaxMath {
        public static double Deadband(double value, double deadband) {
            return (System.Math.Abs(value) > deadband) ? value : 0;
        }

        public static double SquareInputs(double value) {
            return System.Math.Abs(value) * value;
        }

        public static double Clamp(double value, double low, double high) {
            return System.Math.Max(low, System.Math.Min(value, high));
        }
    }
}
