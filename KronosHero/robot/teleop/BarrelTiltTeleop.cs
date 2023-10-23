using KronosHero.robot.subsystems;
using KronosHero.robot.utils;
using KronosHero.wpilib.command;
using KronosHero.wpilib.controller;

namespace KronosHero.robot.teleop {
    public class BarrelTiltTeleop : Command {
        private readonly Barrel barrel;
        private readonly CommandXboxController controller;

        public BarrelTiltTeleop(Barrel barrel, CommandXboxController controller) {
            this.barrel = barrel;
            this.controller = controller;

            this.AddRequirements(barrel);
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
