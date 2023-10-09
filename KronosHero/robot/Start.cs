using Kronos.wpilib.robot;

namespace Kronos.robot {
    public static class Start {
        public static void Main() {
            RobotBase.StartRobot(() => new Robot());
        }
    }
}
