namespace Kronos.wpilib.command {
    public class Subsystem {
        public Subsystem() {
            CommandScheduler.RegisterSubsystem(this);
        }

        public virtual void Periodic() { }

        public virtual void SetDefaultCommand(Command defaultCommand) {
            CommandScheduler.SetDefaultCommand(this, defaultCommand);
        }

        public virtual void RemoveDefaultCommand() {
            CommandScheduler.RemoveDefaultCommand(this);
        }

        public virtual Command GetDefaultCommand() {
            return CommandScheduler.GetDefaultCommand(this);
        }

        public virtual Command GetCurrentCommand() {
            return CommandScheduler.Requiring(this);
        }

        public virtual void Register() {
            CommandScheduler.RegisterSubsystem(this);
        }
    }
}