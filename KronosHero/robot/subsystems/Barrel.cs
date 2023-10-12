using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using KronosHero.robot.utils;
using KronosHero.wpilib.command;
using System;

namespace KronosHero.robot.subsystems {
    public class Barrel : Subsystem {
        private static readonly int[] BarrelIndexTickPositions = new int[Constants.Barrel.IndexPositions];
        private static int _currentBarrelIndex;

        private readonly TalonSRX barrelTiltMotor;
        private readonly TalonSRX barrelIndexMotor;

        private double tiltPower;

        static Barrel() {
            for (int i = 0; i < Constants.Barrel.IndexPositions; i++) {
                BarrelIndexTickPositions[i] = ComputeIndexTicksForPosition(i);
            }
        }

        private static int ComputeIndexTicksForPosition(int barrelIndex) {
            return (int)(MaxMath.Clamp(barrelIndex, 0, Constants.Barrel.IndexPositions - 1)
                * ((double)Constants.CTRE.TicksPerRotation / Constants.Barrel.IndexPositions))
                + Constants.Barrel.ZeroedEncoderOffsetTicks;
        }

        private static int FindClosestIndex(int currentPositionTicks) {
            return (int)Math.Round((currentPositionTicks - Constants.Barrel.ZeroedEncoderOffsetTicks)
                                   / ((double)Constants.CTRE.TicksPerRotation / Constants.Barrel.IndexPositions));
        }

        public Barrel(TalonSRX barrelTiltMotor, TalonSRX barrelIndexMotor) {
            this.barrelTiltMotor = barrelTiltMotor;
            this.barrelIndexMotor = barrelIndexMotor;

            this.tiltPower = 0;

            _currentBarrelIndex = FindClosestIndex(Math.Abs(barrelIndexMotor.GetSelectedSensorPosition())
                % Constants.CTRE.TicksPerRotation);

            Config();
        }

        private void Config() {
            barrelTiltMotor.ConfigFactoryDefault();
            barrelIndexMotor.ConfigFactoryDefault();

            barrelTiltMotor.ConfigAllSettings(new TalonSRXConfiguration {
                continuousCurrentLimit = 15,
                peakCurrentLimit = 20,
                peakCurrentDuration = 60,
                peakOutputForward = 0.6f,
                peakOutputReverse = -0.4f
            });

            barrelIndexMotor.ConfigAllSettings(new TalonSRXConfiguration {
                slot_0 = new SlotConfiguration {
                    kP = 0.004f,
                    kD = 0,
                    kF = 0
                },
                primaryPID = new BaseTalonPIDSetConfiguration {
                    selectedFeedbackSensor = FeedbackDevice.CTRE_MagEncoder_Absolute
                },
                closedloopRamp = 0.2f,
                feedbackNotContinuous = false
            });

            barrelIndexMotor.SetSensorPhase(true);

            barrelTiltMotor.SetInverted(Constants.Motors.TiltInversion);
            barrelIndexMotor.SetInverted(Constants.Motors.IndexInversion);

            barrelTiltMotor.SetNeutralMode(NeutralMode.Coast);
            barrelIndexMotor.SetNeutralMode(NeutralMode.Coast);
        }

        public override void Periodic() {
            int desiredIndexPositionTicks = BarrelIndexTickPositions[_currentBarrelIndex];

/*            double controlEffort =
                (desiredIndexPositionTicks - MathUtil.InputModulus(barrelIndexMotor.GetSelectedSensorPosition(), 0, 4096))
                * 0.004;*/

            barrelTiltMotor.Set(ControlMode.PercentOutput, tiltPower);
            barrelIndexMotor.Set(ControlMode.Position, desiredIndexPositionTicks);
            //Fallback if previous doesn't work.
            //barrelIndexMotor.Set(ControlMode.PercentOutput, controlEffort);
        }

        public void SetTiltPower(double power) {
            tiltPower = power;
        }

        public void Index() {
           _currentBarrelIndex = _currentBarrelIndex != (Constants.Barrel.IndexPositions - 1) ? _currentBarrelIndex + 1 : 0;
        }

        public void Stop() {
            SetTiltPower(0);
            barrelIndexMotor.Set(ControlMode.PercentOutput, 0);
        }
    }
}
