using CTRE.Gadgeteer.Module;
using CTRE.Phoenix;
using KronosHero.robot.subsystems;
using KronosHero.wpilib.command;

namespace KronosHero.robot.teleop {
    public class ShootTeleop : Command {
        private readonly Barrel barrel;
        private readonly DriverModule driverModule;
        private readonly Stopwatch stopwatch;

        private byte shotTiming = 45;

        public ShootTeleop(Barrel barrel, DriverModule driverModule) {
            this.barrel = barrel;
            this.driverModule = driverModule;
            
            stopwatch = new Stopwatch();

            this.AddRequirements(barrel);
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
            new SequentialCommandGroup(
                new WaitCommand(1),
                new InstantCommand(() => barrel.Index())
            ).Schedule();
        }
    }
}
