using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using Kronos.robot.utils;
using Kronos.wpilib.command;
using KronosHero.wpilib.math;
using Microsoft.SPOT;
using System;

namespace Kronos.robot.subsystems {
    public class Barrel : Subsystem {
        private static int[] BarrelIndexTickPositions = new int[Constants.Barrel.IndexPositions];
        private static int CurrentBarrelIndex;

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
                * (Constants.CTRE.TicksPerRotation / Constants.Barrel.IndexPositions))
                + Constants.Barrel.ZeroedEncoderOffsetTicks;
        }

        private static int FindClosestIndex(int currentPositionTicks) {
            return (int)(System.Math.Round((currentPositionTicks - Constants.Barrel.ZeroedEncoderOffsetTicks)
                / (Constants.CTRE.TicksPerRotation / Constants.Barrel.IndexPositions)));
        }

        public Barrel(TalonSRX barrelTiltMotor, TalonSRX barrelIndexMotor) : base() {
            this.barrelTiltMotor = barrelTiltMotor;
            this.barrelIndexMotor = barrelIndexMotor;

            this.tiltPower = 0;

            CurrentBarrelIndex = FindClosestIndex(System.Math.Abs(barrelIndexMotor.GetSelectedSensorPosition()) % Constants.CTRE.TicksPerRotation);

            Config();
        }

        private void Config() {
            barrelTiltMotor.ConfigFactoryDefault();
            barrelIndexMotor.ConfigFactoryDefault();

            barrelTiltMotor.ConfigAllSettings(new TalonSRXConfiguration {
                continuousCurrentLimit = 15,
                peakCurrentLimit = 20,
                peakCurrentDuration = 60,
                peakOutputForward = 0.5f,
                peakOutputReverse = -0.5f
            });

            barrelIndexMotor.ConfigAllSettings(new TalonSRXConfiguration {
                slot_0 = new SlotConfiguration {
                    kP = 2,
                    kD = 0,
                    kF = 0,
                    closedLoopPeakOutput = 0.5f,
                },
                primaryPID = new BaseTalonPIDSetConfiguration {
                    selectedFeedbackSensor = FeedbackDevice.CTRE_MagEncoder_Absolute
                },
            });

            barrelIndexMotor.SetSensorPhase(true);

            barrelTiltMotor.SetInverted(Constants.Motors.TiltInversion);
            barrelIndexMotor.SetInverted(Constants.Motors.IndexInversion);

            barrelTiltMotor.SetNeutralMode(NeutralMode.Coast);
            barrelIndexMotor.SetNeutralMode(NeutralMode.Coast);
        }

        public override void Periodic() {
            int desiredIndexPositionTicks = BarrelIndexTickPositions[CurrentBarrelIndex];
            Debug.Print("I: " + CurrentBarrelIndex + " T: " + desiredIndexPositionTicks);
            Debug.Print("" + MathUtil.InputModulus(barrelIndexMotor.GetSelectedSensorPosition(), 0, 4096));

            double controlEffort = (desiredIndexPositionTicks - MathUtil.InputModulus(barrelIndexMotor.GetSelectedSensorPosition(), 0, 4096)) * 0.001;

            Debug.Print("E" + controlEffort);

            barrelTiltMotor.Set(ControlMode.PercentOutput, tiltPower);
            barrelIndexMotor.Set(ControlMode.PercentOutput, controlEffort);
        }

        public void SetTiltPower(double power) {
            tiltPower = power;
        }

        public void Index() {
           // CurrentBarrelIndex = CurrentBarrelIndex != (Constants.Barrel.IndexPositions - 1) ? CurrentBarrelIndex + 1 : 0;
        }

        public void Stop() {
            SetTiltPower(0);
            barrelIndexMotor.Set(ControlMode.PercentOutput, 0);
        }
    }
}
