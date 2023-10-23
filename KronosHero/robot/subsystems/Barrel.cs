using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using KronosHero.robot.utils;
using KronosHero.wpilib.command;
using KronosHero.wpilib.math;
using KronosHero.wpilib.math.controller;

namespace KronosHero.robot.subsystems {
    public class Barrel : Subsystem {
        private static readonly int[] BarrelIndexTickPositions = new int[Constants.Barrel.IndexPositions];
        private static int _currentBarrelIndex;

        private readonly TalonSRX barrelTiltMotor;
        private readonly TalonSRX barrelIndexMotor;

        private readonly PIDController pidController;

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
            return (int)((currentPositionTicks - Constants.Barrel.ZeroedEncoderOffsetTicks)
                                   / ((double)Constants.CTRE.TicksPerRotation / Constants.Barrel.IndexPositions));
        }

        public Barrel(TalonSRX barrelTiltMotor, TalonSRX barrelIndexMotor) {
            this.barrelTiltMotor = barrelTiltMotor;
            this.barrelIndexMotor = barrelIndexMotor;

            this.tiltPower = 0;

            _currentBarrelIndex = FindClosestIndex((int)MathUtil.InputModulus(
                barrelIndexMotor.GetSelectedSensorPosition(), 0, Constants.CTRE.TicksPerRotation - 1
            ));

            this.pidController = new PIDController(0.002, 0, 0);
            this.pidController.EnableContinuousInput(0, 4095);

            Config();
        }

        private void Config() {
            barrelTiltMotor.ConfigFactoryDefault();
            barrelIndexMotor.ConfigFactoryDefault();

            barrelTiltMotor.ConfigAllSettings(new TalonSRXConfiguration {
                continuousCurrentLimit = 5,
                peakCurrentLimit = 5,
                peakCurrentDuration = 60,
                peakOutputForward = 0.6f,
                peakOutputReverse = -0.4f
            });

            barrelIndexMotor.ConfigAllSettings(new TalonSRXConfiguration {
                primaryPID = new BaseTalonPIDSetConfiguration {
                    selectedFeedbackSensor = FeedbackDevice.CTRE_MagEncoder_Absolute
                },
                closedloopRamp = 0.4f,
            });
            barrelIndexMotor.SetSensorPhase(false);

            barrelTiltMotor.SetInverted(Constants.Motors.TiltInversion);
            barrelIndexMotor.SetInverted(Constants.Motors.IndexInversion);

            barrelTiltMotor.SetNeutralMode(NeutralMode.Coast);
            barrelIndexMotor.SetNeutralMode(NeutralMode.Coast);
        }

        public override void Periodic() {
            int desiredIndexPositionTicks = BarrelIndexTickPositions[_currentBarrelIndex];

            double sensorPos = MathUtil.InputModulus(barrelIndexMotor.GetSelectedSensorPosition(), 0, 4095);
            double controlEffort = pidController.Calculate(sensorPos, desiredIndexPositionTicks);

            barrelTiltMotor.Set(ControlMode.PercentOutput, tiltPower);
            barrelIndexMotor.Set(ControlMode.PercentOutput, controlEffort);
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
