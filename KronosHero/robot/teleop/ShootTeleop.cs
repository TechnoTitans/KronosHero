using CTRE.Gadgeteer.Module;
using CTRE.Phoenix;
using Kronos.robot.subsystems;
using Kronos.wpilib.command;

namespace Kronos.robot.teleop {
    public class ShootTeleop : Command {
        private readonly Barrel barrel;
        private readonly DriverModule driverModule;
        private readonly Stopwatch stopwatch;

        private byte shotTiming = 45;

        public ShootTeleop(Barrel barrel, DriverModule driverModule) : base() {
            this.barrel = barrel;
            this.driverModule = driverModule;
            this.stopwatch = new Stopwatch();

            AddRequirements(barrel);
        }

        public override void Initialize() {
            driverModule.Set(Constants.DriverModule.ShotPort, true);
            stopwatch.Start();
        }
         
        public override bool IsFinished() {
            return stopwatch.DurationMs >= shotTiming;
        }

        public override void End(bool interrupted) {
            driverModule.Set(Constants.DriverModule.ShotPort, false);
            barrel.Index();
        }
    }
}
