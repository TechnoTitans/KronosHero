using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using Kronos.robot.utils;
using Kronos.wpilib.command;
using Microsoft.SPOT;

namespace Kronos.robot.Subsystems {
    public class TankDrive : Subsystem {
        private readonly TalonSRX leftMain;
        private readonly TalonSRX leftFollower;
        private readonly TalonSRX rightMain;
        private readonly TalonSRX rightFollower;

        private WheelSpeeds speeds;

        public TankDrive(TalonSRX leftMain, TalonSRX leftFollower, TalonSRX rightMain, TalonSRX rightFollower) : base() {
            this.leftMain = leftMain;
            this.leftFollower = leftFollower;
            this.rightMain = rightMain;
            this.rightFollower = rightFollower;

            this.speeds = new WheelSpeeds(0, 0);

            Config();
        }

        private void Config() {
            leftMain.ConfigFactoryDefault();
            leftFollower.ConfigFactoryDefault();
            rightMain.ConfigFactoryDefault();
            rightFollower.ConfigFactoryDefault();

            TalonFXConfiguration motorConfiguration = new TalonFXConfiguration {
                statorCurrLimit = new StatorCurrentLimitConfiguration {
                    currentLimit = 15,
                    triggerThresholdCurrent = 10,
                    triggerThresholdTime = 0.1f,
                    enable = true
                },
                openloopRamp = 0.4f
            };

            leftMain.ConfigAllSettings(motorConfiguration);
            leftFollower.ConfigAllSettings(motorConfiguration);
            rightMain.ConfigAllSettings(motorConfiguration);
            rightFollower.ConfigAllSettings(motorConfiguration);

            leftMain.SetInverted(Constants.Motors.FrontLeftInversion);
            leftFollower.SetInverted(InvertType.FollowMaster);
            rightMain.SetInverted(Constants.Motors.FrontRightInversion);
            rightFollower.SetInverted(InvertType.FollowMaster);

            leftMain.SetNeutralMode(NeutralMode.Brake);
            leftFollower.SetNeutralMode(NeutralMode.Brake);
            rightMain.SetNeutralMode(NeutralMode.Brake);
            rightFollower.SetNeutralMode(NeutralMode.Brake);

            leftFollower.Set(ControlMode.Follower, leftMain.GetDeviceID());
            rightFollower.Set(ControlMode.Follower, rightMain.GetDeviceID());
        }

        public override void Periodic() {
            leftMain.Set(ControlMode.PercentOutput, speeds.Left * Constants.Controls.MaxOutput);
            rightMain.Set(ControlMode.PercentOutput, speeds.Right * Constants.Controls.MaxOutput);
        }

        public void SetInputs(double xSpeed, double zRotation, bool squareInputs = true) {
            xSpeed = MaxMath.Deadband(xSpeed, Constants.Controls.Deadband);
            zRotation = MaxMath.Deadband(zRotation, Constants.Controls.Deadband);

            speeds = ArcadeDrive(xSpeed, zRotation, squareInputs);
        }

        private static WheelSpeeds ArcadeDrive(double xSpeed, double zRotation, bool squareInputs) {
            xSpeed = MaxMath.Clamp(xSpeed, -1.0, 1.0);
            zRotation = MaxMath.Clamp(zRotation, -1.0, 1.0);

            if (squareInputs) {
                xSpeed = MaxMath.SquareInputs(xSpeed);
                zRotation = MaxMath.SquareInputs(zRotation);
            }

            double leftSpeed = xSpeed - zRotation;
            double rightSpeed = xSpeed + zRotation;

            double greaterInput = System.Math.Max(System.Math.Abs(xSpeed), System.Math.Abs(zRotation));
            double lesserInput = System.Math.Min(System.Math.Abs(xSpeed), System.Math.Abs(zRotation));
            if (greaterInput == 0.0) {
                return new WheelSpeeds(0, 0);
            }

            double saturatedInput = (greaterInput + lesserInput) / greaterInput;
            leftSpeed /= saturatedInput;
            rightSpeed /= saturatedInput;

            return new WheelSpeeds(leftSpeed, rightSpeed);
        }
    }
}
