using KronosHero.wpilib.netmf;
using System.Threading;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace KronosHero.wpilib.controller {
    public class BooleanEvent {
        protected readonly EventLoop EventLoop;
        private readonly BooleanEventSignalFunc signal;

        public BooleanEvent(EventLoop eventLoop, BooleanEventSignalFunc signal) {
            EventLoop = eventLoop;
            this.signal = signal;
        }

        public void IfHigh(ThreadStart action) {
            EventLoop.Bind(() => {
                if (signal.Invoke()) {
                    action.Invoke();
                }
            });
        }

        public BooleanEvent Rising() {
            bool previous = signal.Invoke();

            return new BooleanEvent(EventLoop, () => {
                bool present = signal.Invoke();
                bool ret = !previous && present;
                previous = present;
                return ret;
            });
        }

        public BooleanEvent Falling() {
            bool previous = signal.Invoke();

            return new BooleanEvent(EventLoop, () => {
                bool present = signal.Invoke();
                bool ret = previous && !present;
                previous = present;
                return ret;
            });
        }

        public BooleanEvent Negate() {
            return new BooleanEvent(EventLoop, () => !signal.Invoke());
        }

        public BooleanEvent And(BooleanEventSignalFunc other) {
            return new BooleanEvent(EventLoop, () => signal.Invoke() && other.Invoke());
        }

        public BooleanEvent Or(BooleanEventSignalFunc other) {
            return new BooleanEvent(EventLoop, () => signal.Invoke() || other.Invoke());
        }

        public object CastTo(BooleanEventCast ctor) {
            return ctor.Invoke(EventLoop, signal);
        }
    }
}