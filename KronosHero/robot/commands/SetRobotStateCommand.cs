using KronosHero.wpilib.command;
using KronosHero.wpilib.robot;

namespace KronosHero.robot.commands {
    public class SetRobotStateCommand : InstantCommand {
        public SetRobotStateCommand(Robot robot, RobotState robotState) : base(() => robot.SetRobotState(robotState)) {

        }

        public override bool RunsWhenDisabled() {
            return true;
        }
    }
}
