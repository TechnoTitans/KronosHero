using System.Threading;

namespace Kronos.wpilib.command {
    public class InstantCommand : FunctionalCommand {
        public InstantCommand(ThreadStart toRun, params Subsystem[] requirements) :
            base(toRun, () => { }, interrupted => { }, () => true, requirements) {

        }

        public InstantCommand() : this(() => { }) {

        }
    }
}