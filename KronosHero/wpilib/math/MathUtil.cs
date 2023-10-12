using Microsoft.SPOT;
using System;

namespace KronosHero.wpilib.math {
    public static class MathUtil {
        public static double InputModulus(double input, double minimumInput, double maximumInput) {
            double modulus = maximumInput - minimumInput;
            // Wrap input if it's above the maximum input
            int numMax = (int)((input - minimumInput) / modulus);
            input -= numMax * modulus;

            // Wrap input if it's below the minimum input
            int numMin = (int)((input - maximumInput) / modulus);
            input -= numMin * modulus;

            return input;
        }
    }
}
