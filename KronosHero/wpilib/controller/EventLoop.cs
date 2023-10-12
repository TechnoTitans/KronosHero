using System.Collections;
using System.Threading;

namespace KronosHero.wpilib.controller {
    public class EventLoop {
        private readonly ArrayList bindings = new ArrayList();

        public void Bind(ThreadStart action) {
            bindings.Add(action);
        }

        public void Poll() {
            foreach (ThreadStart binding in bindings) {
                binding.Invoke();
            }
        }

        public void Clear() {
            bindings.Clear();
        }
    }
}