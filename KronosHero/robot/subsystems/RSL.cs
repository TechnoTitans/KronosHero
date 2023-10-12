using CTRE.Gadgeteer.Module;
using CTRE.Phoenix;
using KronosHero.wpilib.command;
using KronosHero.wpilib.robot;

namespace KronosHero.robot.subsystems {
    // ReSharper disable once InconsistentNaming
    public class RSL : Subsystem {
        private const uint BlinkDurationMs = 500;

        private readonly Robot robot;
        private readonly DriverModule driverModule;
        private readonly int port;
        private readonly PeriodicTimeout periodicTimeout;

        private bool state;

        public RSL(Robot robot, DriverModule driverModule, int port) {
            this.robot = robot;
            this.driverModule = driverModule;
            this.port = port;
            
            periodicTimeout = new PeriodicTimeout(BlinkDurationMs);
        }

        public override void Periodic() {
            switch (robot.CurrentState) {
                case RobotState.Disabled:
                    driverModule.Set(port, state = true);
                    return;
                case RobotState.Teleop:
                case RobotState.Autonomous:
                case RobotState.Test:
                {
                    if (!periodicTimeout.Process()) {
                        return;
                    }

                    driverModule.Set(port, state = !state);
                    break;
                }
                default:
                    driverModule.Set(port, state = false);
                    break;
            }
        }
    }
}
