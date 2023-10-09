using System;
using System.Threading;
using Kronos.wpilib.netmf;
using Microsoft.SPOT;
// ReSharper disable MemberCanBeProtected.Global

namespace Kronos.wpilib.robot {
    public abstract class RobotBase {
        private static readonly object RobotLock = new object();
        private static Thread _robotThread;
        private static RobotBase _robotCopy;
        private static bool _suppressExitWarning;

        public abstract void StartCompetition();
        public abstract void EndCompetition();

        public static void Kill(RobotBase robot) {
            if (robot != null) {
                robot.EndCompetition();
            }

            if (_robotThread == null) return;
            try {
                _robotThread.Join(200);
            } catch (Exception) {
                _robotThread.Abort();
            }
        }

        public static void RunRobot(RobotFunc robotSupplier) {
            Debug.Print("********** Robot program starting **********");

            RobotBase robot;
            try {
                robot = (RobotBase)robotSupplier.Invoke();
            } catch (Exception exception) {
                Exception cause = exception.InnerException ?? exception;
                Debug.Print("Unhandled exception instantiating robot: " + cause + "\n" + cause.StackTrace);
                Debug.Print("The robot program quit unexpectedly." +
                            "This is usually due to a code error.\n" +
                            "The above stacktrace can help determine where the error occurred.\n" +
                            "See https://wpilib.org/stacktrace for more information.\n"
                );

                Debug.Print("Could not instantiate robot!");
                return;
            }

            lock (RobotLock) {
                _robotCopy = robot;
            }

            bool errorOnExit = false;
            try {
                robot.StartCompetition();
            } catch (Exception exception) {
                Exception cause = exception.InnerException ?? exception;
                Debug.Print("Unhandled exception: " + cause + "\n" + cause.StackTrace);
                errorOnExit = true;
            } finally {
                bool suppressWarningOnExit;
                lock (RobotLock) {
                    suppressWarningOnExit = _suppressExitWarning;
                }

                if (!suppressWarningOnExit) {
                    Debug.Print("The robot program quit unexpectedly." +
                                "This is usually due to a code error.\n" +
                                "The above stacktrace can help determine where the error occurred.\n" +
                                "See https://wpilib.org/stacktrace for more information.\n");

                    if (errorOnExit) {
                        Debug.Print(
                            "The startCompetition() method (or methods called by it) should have" +
                            "handled the exception above.");
                    } else {
                        Debug.Print("Unexpected return from startCompetition() method.");
                    }
                }
            }
        }

        public static void SuppressExitWarning(bool suppress) {
            lock (RobotLock) {
                _suppressExitWarning = suppress;
            }
        }

        public static void StartRobot(RobotFunc robotSupplier) {
            lock (RobotLock) {
                _robotThread = new Thread(() => {
                    RunRobot(robotSupplier);
                });
            }

            _robotThread.Start();
            _robotThread.Join();
            SuppressExitWarning(true);

            RobotBase robot;
            lock (RobotLock) {
                robot = _robotCopy;
            }

            Kill(robot);
        }
    }
}