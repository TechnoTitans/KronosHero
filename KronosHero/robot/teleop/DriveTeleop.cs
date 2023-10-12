using KronosHero.robot.subsystems;
using KronosHero.wpilib.command;
using KronosHero.wpilib.controller;

namespace KronosHero.robot.teleop {
    public class DriveTeleop : Command {
        private readonly TankDrive tankDrive;
        private readonly CommandXboxController controller;

        public DriveTeleop(TankDrive tankDrive, CommandXboxController controller) {
            this.tankDrive = tankDrive;
            this.controller = controller;

            this.AddRequirements(tankDrive);
        }

        public override void Execute() {
            double xSpeed = -controller.GetLeftY();
            double zRotation = controller.GetRightX();

            tankDrive.SetInputs(xSpeed, zRotation);
        }
    }
}
