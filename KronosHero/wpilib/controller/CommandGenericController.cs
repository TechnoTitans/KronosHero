using CTRE.Phoenix.Controller;
using KronosHero.wpilib.command;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace KronosHero.wpilib.controller {
    public class CommandGenericController {
        public GameController Controller {
            get;
            private set;
        }

        public CommandGenericController(GameController controller) {
            Controller = controller;
        }

        public Trigger Button(uint button) {
            return Button(button, CommandScheduler.DefaultButtonLoop);
        }

        public Trigger Button(uint button, EventLoop eventLoop) {
            return new Trigger(eventLoop, () => Controller.GetButton(button));
        }

        public Trigger AxisLessThan(uint axis, double threshold) {
            return AxisLessThan(axis, threshold, CommandScheduler.DefaultButtonLoop);
        }

        public Trigger AxisLessThan(uint axis, double threshold, EventLoop eventLoop) {
            return new Trigger(eventLoop, () => Controller.GetAxis(axis) < threshold);
        }

        public Trigger AxisGreaterThan(uint axis, double threshold) {
            return AxisGreaterThan(axis, threshold, CommandScheduler.DefaultButtonLoop);
        }

        public Trigger AxisGreaterThan(uint axis, double threshold, EventLoop eventLoop) {
            return new Trigger(eventLoop, () => Controller.GetAxis(axis) > threshold);
        }

        public double GetRawAxis(uint axis) {
            return Controller.GetAxis(axis);
        }
    }
}