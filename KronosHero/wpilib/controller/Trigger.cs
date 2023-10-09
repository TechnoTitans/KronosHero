using Kronos.wpilib.command;
using Kronos.wpilib.netmf;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Kronos.wpilib.controller {
    public class Trigger {
        private readonly EventLoop eventLoop;
        private readonly BooleanEventSignalFunc condition;

        public Trigger(EventLoop eventLoop, BooleanEventSignalFunc condition) {
            this.eventLoop = eventLoop;
            this.condition = condition;
        }

        public static Trigger Create(EventLoop eventLoop, BooleanEventSignalFunc condition) {
            return new Trigger(eventLoop, condition);
        }

        public Trigger(BooleanEventSignalFunc condition) : this(CommandScheduler.DefaultButtonLoop, condition) { }

        public Trigger OnTrue(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();
                if (!pressedLast && pressed) {
                    command.Schedule();
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger OnFalse(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();
                if (pressedLast && !pressed) {
                    command.Schedule();
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger WhileTrue(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();

                if (!pressedLast && pressed) {
                    command.Schedule();
                } else if (pressedLast && !pressed) {
                    command.Cancel();
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger WhileFalse(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();

                if (pressedLast && !pressed) {
                    command.Schedule();
                } else if (!pressedLast && pressed) {
                    command.Cancel();
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger ToggleOnTrue(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();

                if (!pressedLast && pressed) {
                    if (command.IsScheduled()) {
                        command.Cancel();
                    } else {
                        command.Schedule();
                    }
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger ToggleOnFalse(Command command) {
            bool pressedLast = condition.Invoke();
            eventLoop.Bind(() => {
                bool pressed = condition.Invoke();

                if (pressedLast && !pressed) {
                    if (command.IsScheduled()) {
                        command.Cancel();
                    } else {
                        command.Schedule();
                    }
                }

                pressedLast = pressed;
            });

            return this;
        }

        public Trigger And(Trigger trigger) {
            return new Trigger(() => condition.Invoke() && trigger.condition.Invoke());
        }

        public Trigger Or(Trigger trigger) {
            return new Trigger(() => condition.Invoke() || trigger.condition.Invoke());
        }

        public Trigger Or() {
            return new Trigger(() => !condition.Invoke());
        }
    }
}