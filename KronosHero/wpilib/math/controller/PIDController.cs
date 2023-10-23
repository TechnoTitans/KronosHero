using Microsoft.SPOT;
using System;

namespace KronosHero.wpilib.math.controller {
    public class PIDController {
        private double kP;
        private double kI;
        private double kD;

        private double iZone = double.PositiveInfinity;

        private readonly double period;

        private double maximumIntegral = 1;
        private double minimumIntegral = -1;

        private double maximumInput;
        private double minimumInput;

        private bool continuous;

        private double positionError;
        private double velocityError;

        private double prevError;
        private double totalError;

        private double positionTolerance = 0.05;
        private double velocityTolerance = double.PositiveInfinity;

        private double setpoint;
        private double measurement;

        private bool haveMeasurement;
        private bool haveSetpoint;

        public PIDController(double kP, double kI, double kD): this(kP, kI, kD, 0.02) {

        }

        public PIDController(double kP, double kI, double kD, double period) {
            this.kP = kP;
            this.kI = kI;
            this.kD = kD;

            if (period <= 0) {
                throw new ArgumentOutOfRangeException("Controller period must be a non-zero positive number!");
            }

            this.period = period;
        }

        public void SetPID(double kP, double kI, double kD) {
            this.kP = kP;
            this.kI = kI;
            this.kD = kD;
        }

        public void SetP(double kP) {
            this.kP = kP;
        }

        public void SetI(double kI) {
            this.kI = kI;
        }

        public void SetD(double kD) {
            this.kD = kD;
        }

        public void SetIZone(double iZone) {
            if (iZone < 0) {
                throw new ArgumentOutOfRangeException("IZone must be a non-negative number!");
            }

            this.iZone = iZone;
        }

        public double GetP() {
            return kP;
        }

        public double GetI() {
            return kI;
        }

        public double GetD() {
            return kD;
        }

        public double GetIZone() {
            return iZone;
        }

        public double GetPeriod() {
            return period;
        }

        public double GetPositionTolerance() {
            return positionTolerance;
        }

        public double GetVelocityTolerance() {
            return velocityTolerance;
        }

        public void SetSetpoint(double setpoint) {
            this.setpoint = setpoint;
            haveSetpoint = true;

            if (continuous) {
                double errorBound = (maximumInput - minimumInput) / 2d;
                positionError = MathUtil.InputModulus(setpoint - measurement, -errorBound, errorBound);
            } else {
                positionError = setpoint - measurement;
            }

            velocityError = (positionError - prevError) / period;
        }

        public double GetSetpoint() {
            return setpoint;
        }

        public bool AtSetpoint() {
            return haveMeasurement
                && haveSetpoint
                && System.Math.Abs(positionError) < positionTolerance
                && System.Math.Abs(velocityError) < velocityTolerance;
        }

        public void EnableContinuousInput(double minimumInput, double maximumInput) {
            continuous = true;
            this.minimumInput = minimumInput;
            this.maximumInput = maximumInput;
        }

        public void DisableContinousInput() {
            continuous = false;
        }

        public bool IsContinuousInputEnabled() {
            return continuous;
        }

        public void SetIntegratorRange(double minimumIntegral, double maximumIntegral) {
            this.minimumIntegral = minimumIntegral;
            this.maximumIntegral = maximumIntegral;
        }

        public void SetTolerance(double positionTolerance) {
            SetTolerance(positionTolerance, double.PositiveInfinity);
        }

        public void SetTolerance(double positionTolerance, double velocityTolerance) {
            this.positionTolerance = positionTolerance;
            this.velocityTolerance = velocityTolerance;
        }

        public double GetPositionError() {
            return positionError;
        }

        public double GetVelocityError() {
            return velocityError;
        }

        public double Calculate(double measurement, double setpoint) {
            this.setpoint = setpoint;
            haveSetpoint = true;
            return Calculate(measurement);
        }

        public double Calculate(double measurement) {
            this.measurement = measurement;
            prevError = positionError;
            haveMeasurement = true;

            if (continuous) {
                double errorBound = (maximumInput - minimumInput) / 2d;
                positionError = MathUtil.InputModulus(setpoint - measurement, -errorBound, errorBound);
            } else {
                positionError = setpoint - measurement;
            }

            velocityError = (positionError - prevError) / period;

            if (System.Math.Abs(positionError) > iZone) {
                totalError = 0;
            } else if (kI != 0) {
                totalError = MathUtil.Clamp(totalError + positionError * period, minimumIntegral / kI, maximumIntegral / kI);
            }

            return kP * positionError + kI * totalError + kD * velocityError;
        }

        public void Reset() {
            positionError = 0;
            prevError = 0;
            totalError = 0;
            velocityError = 0;
            haveMeasurement = false;
        }
    }
}