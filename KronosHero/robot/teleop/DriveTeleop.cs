using Kronos.robot.Subsystems;
using Kronos.wpilib.command;
using Kronos.wpilib.controller;
using Microsoft.SPOT;

namespace Kronos.robot.teleop {
    public class DriveTeleop : Command {
        private readonly TankDrive tankDrive;
        private readonly CommandXboxController controller;

        public DriveTeleop(TankDrive tankDrive, CommandXboxController controller) : base() {
            this.tankDrive = tankDrive;
            this.controller = controller;

            AddRequirements(tankDrive);
        }

        public override void Execute() {
            double xSpeed = -controller.GetLeftY();
            double zRotation = controller.GetRightX();

            tankDrive.SetInputs(xSpeed, zRotation);
        }

        public override bool IsFinished() {
            return false;
        }
    }
}
