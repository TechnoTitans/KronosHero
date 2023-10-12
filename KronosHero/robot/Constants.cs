using CTRE.Gadgeteer;
using CTRE.HERO;
using CTRE.Phoenix.MotorControl;

namespace KronosHero.robot {
    public static class Constants {
        public const double LoopPeriodSeconds = 0.02;

        public static class Controls {
            public const uint ControllerPort = 0;

            public const double Deadband = 0.1;
            public const double MaxOutput = 1;
        }

        public static class Barrel {
            public const int IndexPositions = 6;
            public const int ZeroedEncoderOffsetTicks = 0;
        }

        public static class CTRE {
            public const int TicksPerRotation = 4096;
        }

        public static class DriverModule {
            public static readonly PortDefinition DriveModulePort = IO.Port3;

            public const int CompressorPort = 5;
            public const int ShotPort = 3;
            public const int RSLPort = 1;
        }

        public class ShotTimingBoard {
            public const ushort Address = 0x0001;
            public const int ClockRateKhz = 5;
        }

        public static class Motors {
            public const int FrontLeftMain = 3;
            public const InvertType FrontLeftInversion = InvertType.InvertMotorOutput;
            public const int FrontLeftFollower = 1;
            public const int FrontRightMain = 2;
            public const InvertType FrontRightInversion = InvertType.None;
            public const int FrontRightFollower = 4;

            public const int Tilt = 8;
            public const InvertType TiltInversion = InvertType.InvertMotorOutput;
            public const int Index = 7;
            public const InvertType IndexInversion = InvertType.None;
        }
    }
}