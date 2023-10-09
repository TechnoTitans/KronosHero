using System;
using System.Collections;
using Kronos.net;
using Kronos.wpilib.controller;
using Kronos.wpilib.netmf;
using Kronos.wpilib.robot;
using Microsoft.SPOT;
// ReSharper disable MemberCanBePrivate.Global

namespace Kronos.wpilib.command {
    public static class CommandScheduler {
        private static readonly Hashtable ComposedCommands = new Hashtable();
        private static readonly Hashtable ScheduledCommands = new Hashtable();
        private static readonly Hashtable SubsystemCommandRequirements = new Hashtable();
        private static readonly Hashtable SubsystemCommandDefaults = new Hashtable();

        public static readonly EventLoop DefaultButtonLoop = new EventLoop();
        private static EventLoop _activeButtonLoop = DefaultButtonLoop;
        public static EventLoop ActiveButtonLoop {
            get { return _activeButtonLoop; }
            set { _activeButtonLoop = value; }
        }

        private static bool _disabled;
        private static RobotState _robotState = RobotState.None;
        public static RobotState RobotState {
            set { _robotState = value; }
        }

        private static readonly ArrayList InitCommandActions = new ArrayList();
        private static readonly ArrayList ExecCommandActions = new ArrayList();
        private static readonly ArrayList InterruptCommandActions = new ArrayList();
        private static readonly ArrayList FinishCommandActions = new ArrayList();

        private static bool _inRunLoop;
        private static readonly Hashtable ToScheduleCommands = new Hashtable();
        private static readonly ArrayList ToCancelCommands = new ArrayList();

        private static void InitCommand(Command command, ArrayList subsystemRequirements) {
            ScheduledCommands.Add(command, true);
            foreach (Subsystem subsystem in subsystemRequirements) {
                SubsystemCommandRequirements.Add(subsystem, command);
            }
            command.Initialize();
            foreach (InitCommandAction initCommandAction in InitCommandActions) {
                initCommandAction.Invoke(command);
            }
        }

        private static void Schedule(Command command) {
            if (command == null) {
                Debug.Print("Tried to schedule a null command!");
                return;
            }

            if (_inRunLoop) {
                ToScheduleCommands.Add(command, true);
                return;
            }

            RequireNotComposed(command);

            // Do nothing if the scheduler is disabled, the robot is disabled and the command doesn't
            // run when disabled, or the command is already scheduled.
            if (_disabled
                    || IsScheduled(command)
                    || _robotState == RobotState.Disabled && !command.RunsWhenDisabled()) {
                return;
            }

            ArrayList requirements = command.SubsystemRequirements;
            if (Collections.Disjoint(SubsystemCommandRequirements.Keys, requirements)) {
                InitCommand(command, requirements);
            } else {
                foreach (Subsystem requirement in requirements) {
                    Command requiring = Requiring(requirement);
                    if (requiring != null
                            && requiring.GetInterruptionBehavior() == Command.InterruptionBehavior.CancelIncoming) {
                        return;
                    }
                }

                foreach (Subsystem requirement in requirements) {
                    Command requiring = Requiring(requirement);
                    if (requiring != null) {
                        Cancel(requiring);
                    }
                }
                InitCommand(command, requirements);
            }
        }

        public static void Schedule(params Command[] commands) {
            if (commands == null) return;

            foreach (Command command in commands) {
                Schedule(command);
            }
        }

        public static void Run() {
            if (_disabled) {
                return;
            }

            foreach (Subsystem subsystem in SubsystemCommandDefaults.Keys) {
                subsystem.Periodic();
            }

            EventLoop loopCache = _activeButtonLoop;
            loopCache.Poll();

            _inRunLoop = true;

            ArrayList scheduledForRemoval = new ArrayList();
            foreach (DictionaryEntry scheduledCommandEntry in ScheduledCommands) {
                Command command = (Command)scheduledCommandEntry.Key;

                if (!command.RunsWhenDisabled() && _robotState == RobotState.Disabled) {
                    command.End(true);
                    foreach (InterruptCommandAction action in InterruptCommandActions) {
                        action.Invoke(command);
                    }

                    foreach (Subsystem requirement in command.SubsystemRequirements) {
                        SubsystemCommandRequirements.Remove(requirement);
                    }

                    scheduledForRemoval.Add(command);
                    continue;
                }

                command.Execute();
                foreach (ExecCommandAction action in ExecCommandActions) {
                    action.Invoke(command);
                }

                if (command.IsFinished()) {
                    command.End(false);
                    foreach (FinishCommandAction action in FinishCommandActions) {
                        action.Invoke(command);
                    }

                    scheduledForRemoval.Add(command);
                    foreach (Subsystem requirement in command.SubsystemRequirements) {
                        SubsystemCommandRequirements.Remove(requirement);
                    }
                }
            }

            foreach (Command command in scheduledForRemoval) {
                ScheduledCommands.Remove(command);
            }

            _inRunLoop = false;
            foreach (Command command in ToScheduleCommands.Keys) {
                Schedule(command);
            }

            foreach (Command command in ToCancelCommands) {
                Cancel(command);
            }

            ToScheduleCommands.Clear();
            ToCancelCommands.Clear();

            foreach (DictionaryEntry subsystemCommandEntry in SubsystemCommandDefaults) {
                if (!SubsystemCommandRequirements.Contains(subsystemCommandEntry.Key) &&
                        subsystemCommandEntry.Value != null) {
                    Schedule((Command)subsystemCommandEntry.Value);
                }
            }
        }

        public static void RegisterSubsystem(params Subsystem[] subsystems) {
            if (subsystems == null) return;

            foreach (Subsystem subsystem in subsystems) {
                if (subsystem == null) {
                    Debug.Print("Tried to register a null subsystem!");
                    continue;
                }

                if (SubsystemCommandDefaults.Contains(subsystem)) {
                    Debug.Print("Tried to register an already-registered subsystem!");
                    continue;
                }

                SubsystemCommandDefaults.Add(subsystem, null);
            }
        }

        public static void UnregisterSubsystem(params Subsystem[] subsystems) {
            if (subsystems == null) return;

            foreach (Subsystem subsystem in subsystems) {
                SubsystemCommandDefaults.Remove(subsystem);
            }
        }

        public static void SetDefaultCommand(Subsystem subsystem, Command defaultCommand) {
            if (subsystem == null) {
                Debug.Print("Tried to set a default command for a null subsystem!");
                return;
            }

            if (defaultCommand == null) {
                Debug.Print("Tried to set a null default command!");
                return;
            }

            RequireNotComposed(defaultCommand);
            if (!defaultCommand.SubsystemRequirements.Contains(subsystem)) {
                throw new ArgumentException("Default commands must require their subsystem!");
            }

            if (defaultCommand.GetInterruptionBehavior() == Command.InterruptionBehavior.CancelIncoming) {
                Debug.Print("Registering a non-interrupt-able default command!" +
                            "This will likely prevent any other commands from requiring this subsystem.");
            }

            SubsystemCommandDefaults[subsystem] = defaultCommand;
        }

        public static void RemoveDefaultCommand(Subsystem subsystem) {
            if (subsystem == null) {
                Debug.Print("Tried to remove a default command for a null subsystem!");
                return;
            }

            if (SubsystemCommandDefaults.Contains(subsystem)) {
                SubsystemCommandDefaults[subsystem] = null;
            }
        }

        public static Command GetDefaultCommand(Subsystem subsystem) {
            return (Command)SubsystemCommandDefaults[subsystem];
        }

        public static void Cancel(params Command[] commands) {
            if (commands == null) return;

            if (_inRunLoop) {
                foreach (Command command in commands) {
                    ToCancelCommands.Add(command);
                }
            }

            foreach (Command command in commands) {
                if (command == null) {
                    Debug.Print("Tried to cancel a null command!");
                    continue;
                }

                if (!IsScheduled(command)) {
                    continue;
                }

                ScheduledCommands.Remove(command);
                foreach (Subsystem requirement in command.SubsystemRequirements) {
                    SubsystemCommandRequirements.Remove(requirement);
                }
                command.End(true);
                foreach (InterruptCommandAction action in InterruptCommandActions) {
                    action.Invoke(command);
                }
            }
        }

        public static void CancelAll() {
            ICollection scheduledCommands = ScheduledCommands.Keys;
            Command[] scheduledArray = new Command[scheduledCommands.Count];
            scheduledCommands.CopyTo(scheduledArray, 0);

            Cancel(scheduledArray);
        }

        public static bool IsScheduled(params Command[] commands) {
            if (commands == null) return false;

            foreach (Command command in commands) {
                if (!ScheduledCommands.Contains(command)) {
                    return false;
                }
            }

            return true;
        }

        public static Command Requiring(Subsystem subsystem) {
            return (Command)SubsystemCommandRequirements[subsystem];
        }

        public static void Enable() {
            _disabled = false;
        }

        public static void Disable() {
            _disabled = true;
        }

        public static void OnCommandInitialize(InitCommandAction action) {
            InitCommandActions.Add(action);
        }

        public static void OnCommandExecute(ExecCommandAction action) {
            ExecCommandActions.Add(action);
        }

        public static void OnCommandInterrupt(InterruptCommandAction action) {
            InterruptCommandActions.Add(action);
        }

        public static void OnCommandFinish(FinishCommandAction action) {
            FinishCommandActions.Add(action);
        }

        public static void RegisterComposedCommands(params Command[] commands) {
            if (commands == null) return;

            RequireNotComposed(commands);
            foreach (Command command in commands) {
                ComposedCommands.Add(command, true);
            }
        }

        public static void ClearComposedCommands() {
            ComposedCommands.Clear();
        }

        public static void RemoveComposedCommand(Command command) {
            ComposedCommands.Remove(command);
        }

        public static void RequireNotComposed(Command command) {
            if (ComposedCommands.Contains(command)) {
                throw new ArgumentException("Commands that have been composed may not be added to another" +
                                            "composition or scheduled individually!");
            }
        }

        public static void RequireNotComposed(params Command[] commands) {
            if (!Collections.Disjoint(commands, ComposedCommands.Keys)) {
                throw new ArgumentException("Commands that have been composed may not be added to another" +
                                            "composition or scheduled individually!");
            }
        }

        public static bool IsComposed(Command command) {
            return ComposedCommands.Contains(command);
        }
    }
}