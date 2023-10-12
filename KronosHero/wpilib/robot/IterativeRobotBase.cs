using System;
using CTRE.Phoenix;
using Kronos.wpilib.command;
using Microsoft.SPOT;
// ReSharper disable MemberCanBeProtected.Global

namespace Kronos.wpilib.robot {
    public abstract class IterativeRobotBase : RobotBase {
        private const RobotState INITIAL_STATE = RobotState.None;

        private RobotState lastState = INITIAL_STATE;
        private RobotState nextState = INITIAL_STATE;
        public RobotState CurrentState {
            get { return nextState; }
        }

        private readonly double period;

        protected IterativeRobotBase(double period) {
            this.period = period;
        }

        public virtual void RobotInit() { }
        public virtual void DisabledInit() { }
        public virtual void AutonomousInit() { }
        public virtual void TeleopInit() { }
        public virtual void TestInit() { }

        private bool robotPeriodicFirstRun = true;
        public virtual void RobotPeriodic() {
            if (robotPeriodicFirstRun) {
                Debug.Print("Default robotPeriodic() method... Override me!");
                robotPeriodicFirstRun = false;
            }
        }

        private bool disabledPeriodicFirstRun = true;
        public virtual void DisabledPeriodic() {
            if (disabledPeriodicFirstRun) {
                Debug.Print("Default disabledPeriodic() method... Override me!");
                disabledPeriodicFirstRun = false;
            }
        }

        private bool autonomousPeriodicFirstRun = true;
        public virtual void AutonomousPeriodic() {
            if (autonomousPeriodicFirstRun) {
                Debug.Print("Default autonomousPeriodic() method... Override me!");
                autonomousPeriodicFirstRun = false;
            }
        }

        private bool teleopPeriodicFirstRun = true;
        public virtual void TeleopPeriodic() {
            if (teleopPeriodicFirstRun) {
                Debug.Print("Default teleopPeriodic() method... Override me!");
                teleopPeriodicFirstRun = false;
            }
        }

        private bool testPeriodicFirstRun = true;
        public virtual void TestPeriodic() {
            if (testPeriodicFirstRun) {
                Debug.Print("Default testPeriodic() method... Override me!");
                testPeriodicFirstRun = false;
            }
        }

        public virtual void DisabledExit() { }
        public virtual void AutonomousExit() { }
        public virtual void TeleopExit() { }
        public virtual void TestExit() { }

        public double GetPeriod() {
            return period;
        }

        public void SetRobotState(RobotState robotState) {
            nextState = robotState;
            CommandScheduler.RobotState = robotState;
        }

        protected void LoopFunc() {
            if (lastState != nextState) {
                switch (lastState) {
                    case RobotState.Disabled:
                        DisabledExit();
                        break;
                    case RobotState.Autonomous:
                        AutonomousExit();
                        break;
                    case RobotState.Teleop:
                        TeleopExit();
                        break;
                    case RobotState.Test:
                        TestExit();
                        break;
                    case RobotState.Kill:
                    case RobotState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (nextState) {
                    case RobotState.Disabled:
                        DisabledInit();
                        break;
                    case RobotState.Autonomous:
                        AutonomousInit();
                        break;
                    case RobotState.Teleop:
                        TeleopInit();
                        break;
                    case RobotState.Test:
                        TestInit();
                        break;
                    case RobotState.Kill:
                    case RobotState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                lastState = nextState;
            }

            switch (lastState) {
                case RobotState.Disabled:
                    DisabledPeriodic();
                    break;
                case RobotState.Autonomous:
                    AutonomousPeriodic();
                    break;
                case RobotState.Teleop:
                    TeleopPeriodic();
                    break;
                case RobotState.Test:
                    TestPeriodic();
                    break;
                case RobotState.None:
                    break;
                case RobotState.Kill:
                    Kill(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RobotPeriodic();

            if (nextState.ShouldFeedWatchdog()) {
                Watchdog.Feed();
            }
        }
    }
}