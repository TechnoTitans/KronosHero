using System;
using System.Collections;

namespace KronosHero.wpilib.command {
    public class SequentialCommandGroup : Command {
        private readonly ArrayList  m_commands = new ArrayList();
        private int currentCommandIndex = -1;
        private bool runWhenDisabled = true;
        private InterruptionBehavior interruptionBehavior = InterruptionBehavior.CancelIncoming;

        public SequentialCommandGroup(params Command[] commands) {
            AddCommands(commands);
        }

        public void AddCommands(params Command[] commands) {
            if (commands == null) return;
            if (currentCommandIndex != -1) {
                throw new Exception(
                    "Commands cannot be added to a CommandGroup while the group is running");
            }
            
            CommandScheduler.RegisterComposedCommands(commands);

            foreach (Command command in commands) {
                m_commands.Add(command);
                foreach (Subsystem subsystem in command.SubsystemRequirements) {
                    SubsystemRequirements.Add(subsystem);
                }
                runWhenDisabled &= command.RunsWhenDisabled();
                if (command.GetInterruptionBehavior() == InterruptionBehavior.CancelSelf) {
                    interruptionBehavior = InterruptionBehavior.CancelSelf;
                }
            }
        }

        public override void Initialize() {
            currentCommandIndex = 0;
            
            if (m_commands.Count > 0) {
                ((Command)m_commands[0]).Initialize();
            }
        }

        public override void Execute() {
            if (m_commands.Count == 0) {
                return;
            }

            Command currentCommand = (Command)m_commands[currentCommandIndex];
            
            currentCommand.Execute();
            if (currentCommand.IsFinished()) {
                currentCommand.End(false);
                currentCommandIndex++;
                if (currentCommandIndex < m_commands.Count) {
                    ((Command)m_commands[currentCommandIndex]).Initialize();
                }
            }
        }

        public override void End(bool interrupted) {
            if (interrupted
                && m_commands.Count > 0
                && currentCommandIndex > -1
                && currentCommandIndex < m_commands.Count) {
                ((Command)m_commands[currentCommandIndex]).End(true);
            }
            currentCommandIndex = -1;
        }

        public override bool IsFinished() {
            return currentCommandIndex == m_commands.Count;
        }
        
        public override bool RunsWhenDisabled() {
            return runWhenDisabled;
        }

        public override InterruptionBehavior GetInterruptionBehavior() {
            return interruptionBehavior;
        }
    }
}
