using Kronos.robot.subsystems;
using Kronos.robot.utils;
using Kronos.wpilib.command;
using Kronos.wpilib.controller;

namespace Kronos.robot.teleop {
    public class BarrelTiltTeleop : Command {
        private readonly Barrel barrel;
        private readonly CommandXboxController controller;

        public BarrelTiltTeleop(Barrel barrel, CommandXboxController controller) : base() {
            this.barrel = barrel;
            this.controller = controller;

            AddRequirements(barrel);
        }

        public override void End(bool interrupted) {
            barrel.Stop();
        }

        public override void Execute() {
            double tiltPower = (MaxMath.Deadband(
                controller.GetRightTriggerAxis(),
                Constants.Controls.Deadband
            ) - MaxMath.Deadband(
                controller.GetLeftTriggerAxis(),
                Constants.Controls.Deadband
            )) * 0.5;

            barrel.SetTiltPower(tiltPower);
        }
    }
}
