using System.Collections;

namespace Kronos.wpilib.command {
    public abstract class Command {
        public enum InterruptionBehavior {
            CancelSelf,
            CancelIncoming
        }

        public readonly ArrayList SubsystemRequirements = new ArrayList();

        protected Command() { }

        public void AddRequirements(params Subsystem[] subsystems) {
            if (subsystems == null) return;

            foreach (Subsystem subsystem in subsystems) {
                SubsystemRequirements.Add(subsystem);
            }
        }

        public virtual void Initialize() { }

        public virtual void Execute() { }

        public virtual void End(bool interrupted) { }

        public virtual bool IsFinished() {
            return true;
        }

        public virtual void Schedule() {
            CommandScheduler.Schedule(this);
        }

        public virtual void Cancel() {
            CommandScheduler.Cancel(this);
        }

        public virtual bool IsScheduled() {
            return CommandScheduler.IsScheduled(this);
        }

        public virtual bool HasRequirement(Subsystem requirement) {
            return SubsystemRequirements.Contains(requirement);
        }

        public virtual InterruptionBehavior GetInterruptionBehavior() {
            return InterruptionBehavior.CancelSelf;
        }

        public virtual bool RunsWhenDisabled() {
            return false;
        }
    }
}