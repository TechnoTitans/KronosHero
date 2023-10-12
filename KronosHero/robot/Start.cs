using KronosHero.wpilib.robot;

namespace KronosHero.robot {
    public static class Start {
        public static void Main() {
            RobotBase.StartRobot(() => new Robot());
        }
    }
}
