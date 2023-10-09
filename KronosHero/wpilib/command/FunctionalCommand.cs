using System.Threading;
using Kronos.wpilib.netmf;

// ReSharper disable MemberCanBePrivate.Global

namespace Kronos.wpilib.command {
    public class FunctionalCommand : Command {
        protected readonly ThreadStart OnInit;
        protected readonly ThreadStart OnExecute;
        protected readonly OnEndAction OnEnd;
        protected readonly IsFinishedFunc Finished;

        public FunctionalCommand(
                ThreadStart onInit,
                ThreadStart onExecute,
                OnEndAction onEnd,
                IsFinishedFunc finished,
                params Subsystem[] requirements
        ) {
            OnInit = onInit;
            OnExecute = onExecute;
            OnEnd = onEnd;
            Finished = finished;

            AddRequirements(requirements);
        }

        public override void Initialize() {
            OnInit.Invoke();
        }

        public override void Execute() {
            OnExecute.Invoke();
        }

        public override void End(bool interrupted) {
            OnEnd.Invoke(interrupted);
        }

        public override bool IsFinished() {
            return Finished.Invoke();
        }
    }
}