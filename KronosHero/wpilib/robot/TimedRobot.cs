using CTRE.Phoenix;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace KronosHero.wpilib.robot {
    public class TimedRobot : IterativeRobotBase {
        public const double DefaultPeriodSeconds = 0.02;
        private readonly Thread loopFuncThread;

        protected TimedRobot() : this(DefaultPeriodSeconds) { }
        protected TimedRobot(double periodSeconds) : base(periodSeconds) {
            loopFuncThread = new Thread(() => {
                PeriodicTimeout loopFuncTimeout = new PeriodicTimeout((long)(periodSeconds * 1000));
                while (true) {
                    if (loopFuncTimeout.Process()) {
                        this.LoopFunc();
                    }
                }
            });

            this.SetRobotState(RobotState.Disabled);
        }


        public override void StartCompetition() {
            this.RobotInit();
            Debug.Print("********** Robot program startup complete **********");

            // start processing state changes and calling the appropriate IterativeRobotBase methods
            loopFuncThread.Start();
            // let this yield the current thread until its finished (which it will never be)
            loopFuncThread.Join();
        }

        public override void EndCompetition() {
            try {
                loopFuncThread.Join(200);
            } catch (Exception exception) {
                Debug.Print("Failed to join LoopFuncThread: " + exception + "\n" + exception.StackTrace);
                loopFuncThread.Abort();
            }
        }
    }
}