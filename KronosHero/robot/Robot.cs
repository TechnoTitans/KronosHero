using CTRE.Phoenix;
using Kronos.wpilib.command;
using Kronos.wpilib.robot;

namespace Kronos.robot {
    public class Robot : TimedRobot {
        private RobotContainer robotContainer;

        public override void RobotInit() {
            robotContainer = new RobotContainer();
        }

        public override void RobotPeriodic() {
            UsbDeviceConnection controllerConnectionStatus =
                robotContainer.controller.Controller.GetConnectionStatus();

            if (controllerConnectionStatus == UsbDeviceConnection.Connected && CurrentState != RobotState.Teleop) {
                SetRobotState(RobotState.Teleop);
            } else if (controllerConnectionStatus == UsbDeviceConnection.NotConnected) {
                SetRobotState(RobotState.Disabled);
            }

            CommandScheduler.Run();
        }

        public override void DisabledInit() {
            robotContainer.tankDrive.RemoveDefaultCommand();
            robotContainer.barrel.RemoveDefaultCommand();
        }

        public override void TeleopInit() {
            robotContainer.tankDrive.SetDefaultCommand(robotContainer.driveTeleop);
            robotContainer.barrel.SetDefaultCommand(robotContainer.barrelTeleop);
        }
    }
}
