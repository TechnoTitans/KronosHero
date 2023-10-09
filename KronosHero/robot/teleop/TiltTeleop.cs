using CTRE.Phoenix.Controller;
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

        public override void Execute() {
            double tiltPower = TitanMath.Deadband(
                controller.GetLeftTriggerAxis(),
                Constants.Controls.Deadband
            ) - TitanMath.Deadband(
                controller.GetRightTriggerAxis(),
                Constants.Controls.Deadband
            );

            barrel.Tilt(tiltPower);
        }
    }
}
