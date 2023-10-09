namespace Kronos.wpilib.robot {
    public enum RobotState {
        None,
        Disabled,
        Autonomous,
        Teleop,
        Test,
        Kill
    }

    public static class RobotStateMethods {
        public static bool ShouldFeedWatchdog(this RobotState state) {
            return state == RobotState.Autonomous || state == RobotState.Teleop || state == RobotState.Test;
        }
    }
}