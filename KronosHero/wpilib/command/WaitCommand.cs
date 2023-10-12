using CTRE.Phoenix;

namespace KronosHero.wpilib.command {
    public class WaitCommand : Command {
        protected Stopwatch stopwatch = new Stopwatch();
        private readonly double duration;
        
        public WaitCommand(double seconds) {
            duration = seconds;
        }
        
        public override void Initialize() {
            stopwatch.Start();
        }

        public override bool IsFinished() {
            return stopwatch.Duration >= duration;
        }

        public override bool RunsWhenDisabled() {
            return true;
        }
    }
}
