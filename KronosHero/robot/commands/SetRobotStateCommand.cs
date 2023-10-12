using Kronos.robot;
using Kronos.wpilib.command;
using Kronos.wpilib.robot;

namespace KronosHero.robot.commands {
    public class SetRobotStateCommand : InstantCommand {
        public SetRobotStateCommand(Robot robot, RobotState robotState) : base(() => robot.SetRobotState(robotState)) {

        }

        public override bool RunsWhenDisabled() {
            return true;
        }
    }
}
