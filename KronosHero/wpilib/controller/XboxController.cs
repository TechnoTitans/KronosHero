using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Kronos.wpilib.controller {
    public class XboxController : Xbox360Gamepad {
        public enum ButtonId : uint {
            LeftBumper = 5,
            RightBumper = 6,
            LeftStick = 9,
            RightStick = 10,
            A = 1,
            B = 2,
            X = 3,
            Y = 4,
            Back = 7,
            Start = 8
        }

        public enum AxisId : uint {
            LeftX = 0,
            RightX = 4,
            LeftY = 1,
            RightY = 5,
            LeftTrigger = 2,
            RightTrigger = 3
        }

        public XboxController(uint id) : base(UsbHostDevice.GetInstance(id), id) { }

        public double GetLeftX() {
            return GetAxis((uint)AxisId.LeftX);
        }

        public double GetRightX() {
            return GetAxis((uint)AxisId.RightX);
        }

        public double GetLeftY() {
            return GetAxis((uint)AxisId.LeftY);
        }

        public double GetRightY() {
            return GetAxis((uint)AxisId.RightY);
        }

        public double GetLeftTriggerAxis() {
            return GetAxis((uint)AxisId.LeftTrigger);
        }

        public double GetRightTriggerAxis() {
            return GetAxis((uint)AxisId.RightTrigger);
        }

        public bool GetLeftBumper() {
            return GetButton((uint)ButtonId.LeftBumper);
        }

        public bool GetRightBumper() {
            return GetButton((uint)ButtonId.RightBumper);
        }

        public BooleanEvent LeftBumper(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetLeftBumper);
        }

        public BooleanEvent RightBumper(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetRightBumper);
        }

        public bool GetLeftStickButton() {
            return GetButton((uint)ButtonId.LeftStick);
        }

        public bool GetRightStickButton() {
            return GetButton((uint)ButtonId.RightStick);
        }

        public BooleanEvent LeftStick(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetLeftStickButton);
        }

        public BooleanEvent RightStick(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetRightStickButton);
        }

        public bool GetAButton() {
            return GetButton((uint)ButtonId.A);
        }

        public BooleanEvent A(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetAButton);
        }

        public bool GetBButton() {
            return GetButton((uint)ButtonId.B);
        }

        public BooleanEvent B(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetBButton);
        }

        public bool GetXButton() {
            return GetButton((uint)ButtonId.X);
        }

        public BooleanEvent X(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetXButton);
        }

        public bool GetYButton() {
            return GetButton((uint)ButtonId.Y);
        }

        public BooleanEvent Y(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetYButton);
        }

        public bool GetBackButton() {
            return GetButton((uint)ButtonId.Back);
        }

        public BooleanEvent Back(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetBackButton);
        }

        public bool GetStartButton() {
            return GetButton((uint)ButtonId.Start);
        }

        public BooleanEvent Start(EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, GetStartButton);
        }

        public BooleanEvent LeftTrigger(double threshold, EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, () => GetLeftTriggerAxis() > threshold);
        }

        public BooleanEvent LeftTrigger(EventLoop eventLoop) {
            return LeftTrigger(0.5, eventLoop);
        }

        public BooleanEvent RightTrigger(double threshold, EventLoop eventLoop) {
            return new BooleanEvent(eventLoop, () => GetRightTriggerAxis() > threshold);
        }

        public BooleanEvent RightTrigger(EventLoop eventLoop) {
            return RightTrigger(0.5, eventLoop);
        }
    }
}