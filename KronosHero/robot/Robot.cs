using CTRE.Phoenix;
using Kronos.wpilib.command;
using Kronos.wpilib.robot;
using KronosHero.robot.commands;
using Microsoft.SPOT;

namespace Kronos.robot {
    public class Robot : TimedRobot {
        private RobotContainer robotContainer;

        public override void RobotInit() {
            robotContainer = new RobotContainer(this);

            robotContainer.controller.Start().OnTrue(new SetRobotStateCommand(this, RobotState.Teleop));

            robotContainer.controller.Back().OnTrue(new SetRobotStateCommand(this, RobotState.Disabled));
        }

        public override void RobotPeriodic() {
            UsbDeviceConnection controllerConnectionStatus =
                robotContainer.controller.Controller.GetConnectionStatus();

            if (controllerConnectionStatus != UsbDeviceConnection.Connected && CurrentState != RobotState.Disabled) {
                SetRobotState(RobotState.Disabled);
            }

            CommandScheduler.Run();
        }

        public override void DisabledInit() {
            //robotContainer.tankDrive.RemoveDefaultCommand();
            //robotContainer.barrel.RemoveDefaultCommand();
        }

        public override void TeleopInit() {
            robotContainer.tankDrive.SetDefaultCommand(robotContainer.driveTeleop);
            robotContainer.barrel.SetDefaultCommand(robotContainer.barrelTeleop);
        }
    }
}
