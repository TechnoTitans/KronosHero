using Kronos.wpilib.command;
using Kronos.wpilib.controller;

namespace Kronos.wpilib.netmf {
    public delegate void OnEndAction(bool interrupted);

    public delegate bool IsFinishedFunc();

    public delegate bool BooleanEventSignalFunc();

    public delegate object BooleanEventCast(EventLoop eventLoop, BooleanEventSignalFunc signal);

    public delegate void InitCommandAction(Command command);

    public delegate void ExecCommandAction(Command command);

    public delegate void InterruptCommandAction(Command command);

    public delegate void FinishCommandAction(Command command);

    public delegate object RobotFunc();
}