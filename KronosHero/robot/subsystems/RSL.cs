using CTRE.Gadgeteer.Module;
using KronosHero.wpilib.command;
using KronosHero.wpilib.robot;

namespace KronosHero.robot.subsystems {
    // ReSharper disable once InconsistentNaming
    public class RSL : Subsystem {
        private readonly Robot robot;
        private readonly DriverModule driverModule;
        private readonly int port;

        public RSL(Robot robot, DriverModule driverModule, int port) {
            this.robot = robot;
            this.driverModule = driverModule;
            this.port = port;
        }

        public override void Periodic() {
            switch (robot.CurrentState) {
                case RobotState.None:
                case RobotState.Kill:
                case RobotState.Disabled:
                    driverModule.Set(port, true);
                    return;
                case RobotState.Teleop:
                case RobotState.Autonomous:
                case RobotState.Test:
                    driverModule.Set(port, false);
                    break;                
                default:
                    driverModule.Set(port, false);
                    break;
            }
        }
    }
}
