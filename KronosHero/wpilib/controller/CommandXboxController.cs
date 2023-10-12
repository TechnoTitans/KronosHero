using KronosHero.wpilib.command;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace KronosHero.wpilib.controller {
    public class CommandXboxController : CommandGenericController {
        private readonly XboxController xboxController;

        public CommandXboxController(XboxController xboxController) : base(xboxController) {
            this.xboxController = xboxController;
        }

        public CommandXboxController(uint port) : this(new XboxController(port)) { }

        public Trigger LeftBumper() {
            return LeftBumper(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger LeftBumper(EventLoop eventLoop) {
            return (Trigger)xboxController.LeftBumper(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger RightBumper() {
            return RightBumper(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger RightBumper(EventLoop eventLoop) {
            return (Trigger)xboxController.RightBumper(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger LeftStick() {
            return LeftStick(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger LeftStick(EventLoop eventLoop) {
            return (Trigger)xboxController.LeftStick(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger RightStick() {
            return RightStick(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger RightStick(EventLoop eventLoop) {
            return (Trigger)xboxController.RightStick(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger A() {
            return A(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger A(EventLoop eventLoop) {
            return (Trigger)xboxController.A(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger B() {
            return B(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger B(EventLoop eventLoop) {
            return (Trigger)xboxController.B(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger X() {
            return X(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger X(EventLoop eventLoop) {
            return (Trigger)xboxController.X(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger Y() {
            return Y(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger Y(EventLoop eventLoop) {
            return (Trigger)xboxController.Y(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger Start() {
            return Start(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger Start(EventLoop eventLoop) {
            return (Trigger)xboxController.Start(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger Back() {
            return Back(CommandScheduler.DefaultButtonLoop);
        }

        public Trigger Back(EventLoop eventLoop) {
            return (Trigger)xboxController.Back(eventLoop).CastTo(Trigger.Create);
        }

        public Trigger LeftTrigger(EventLoop eventLoop, double threshold) {
            return (Trigger)xboxController.LeftTrigger(threshold, eventLoop).CastTo(Trigger.Create);
        }

        public Trigger LeftTrigger(double threshold = 0.5) {
            return LeftTrigger(CommandScheduler.DefaultButtonLoop, threshold);
        }

        public Trigger RightTrigger(EventLoop eventLoop, double threshold) {
            return (Trigger)xboxController.RightTrigger(threshold, eventLoop).CastTo(Trigger.Create);
        }

        public Trigger RightTrigger(double threshold = 0.5) {
            return RightTrigger(CommandScheduler.DefaultButtonLoop, threshold);
        }

        public Trigger Pov(int angle) {
            return Pov(angle, CommandScheduler.DefaultButtonLoop);
        }

        public Trigger Pov(int angle, EventLoop eventLoop) {
            return new Trigger(eventLoop, () => xboxController.GetPov() == angle);
        }

        public Trigger PovUp() {
            return Pov(0);
        }

        public Trigger PovUpRight() {
            return Pov(45);
        }

        public Trigger PovRight() {
            return Pov(90);
        }

        public Trigger PovDownRight() {
            return Pov(135);
        }

        public Trigger PovDown() {
            return Pov(180);
        }

        public Trigger PovDownLeft() {
            return Pov(225);
        }

        public Trigger PovLeft() {
            return Pov(270);
        }

        public Trigger PovUpLeft() {
            return Pov(315);
        }

        public Trigger PovCenter() {
            return Pov(-1);
        }

        public double GetLeftX() {
            return xboxController.GetLeftX();
        }

        public double GetRightX() {
            return xboxController.GetRightX();
        }

        public double GetLeftY() {
            return xboxController.GetLeftY();
        }

        public double GetRightY() {
            return xboxController.GetRightY();
        }

        public double GetLeftTriggerAxis() {
            return xboxController.GetLeftTriggerAxis();
        }

        public double GetRightTriggerAxis() {
            return xboxController.GetRightTriggerAxis();
        }
    }
}