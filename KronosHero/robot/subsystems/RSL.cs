using CTRE.Gadgeteer.Module;
using CTRE.Phoenix;
using Kronos.robot;
using Kronos.wpilib.command;
using Kronos.wpilib.robot;

namespace KronosHero.robot.subsystems {
    public class RSL : Subsystem {
        public const uint BLINK_DURATION_MS = 500;

        private readonly Robot robot;
        private readonly DriverModule driverModule;
        private readonly int port;
        private readonly PeriodicTimeout periodicTimeout;

        private bool state = false;

        public RSL(Robot robot, DriverModule driverModule, int port) {
            this.robot = robot;
            this.driverModule = driverModule;
            this.port = port;
            this.periodicTimeout = new PeriodicTimeout(BLINK_DURATION_MS);
        }

        public override void Periodic() {
            if (robot.CurrentState == RobotState.Disabled) {
                driverModule.Set(port, state = true);
                return;
            }
            
            if (robot.CurrentState == RobotState.Teleop
                    || robot.CurrentState == RobotState.Autonomous
                    || robot.CurrentState == RobotState.Test
            ) {
                if (!periodicTimeout.Process()) {
                    return;
                }

                driverModule.Set(port, state = !state);
            }
        }
    }
}
