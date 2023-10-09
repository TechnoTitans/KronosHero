using CTRE.Gadgeteer.Module;
using CTRE.Phoenix.MotorControl.CAN;
using Kronos.robot.subsystems;
using Kronos.robot.Subsystems;
using Kronos.robot.teleop;
using Kronos.wpilib.command;
using Kronos.wpilib.controller;

namespace Kronos.robot {
    public class RobotContainer {
        private readonly TalonSRX leftMain;
        private readonly TalonSRX leftFollower;
        private readonly TalonSRX rightMain;
        private readonly TalonSRX rightFollower;

        private readonly TalonSRX barrelTiltMotor;
        private readonly TalonSRX barrelIndexMotor;

        public readonly TankDrive tankDrive;
        public readonly Barrel barrel;
        public readonly DriverModule driverModule;

        public readonly DriveTeleop driveTeleop;
        public readonly BarrelTiltTeleop barrelTeleop;
        public readonly ShootTeleop shootTeleop;

        public readonly CommandXboxController controller;

        public RobotContainer() {
            leftMain = new TalonSRX(Constants.Motors.FrontLeftMain);
            leftFollower = new TalonSRX(Constants.Motors.FrontLeftFollower);
            rightMain = new TalonSRX(Constants.Motors.FrontRightMain);
            rightFollower = new TalonSRX(Constants.Motors.FrontRightFollower);

            barrelTiltMotor = new TalonSRX(Constants.Motors.Tilt);
            barrelIndexMotor = new TalonSRX(Constants.Motors.Index);

            tankDrive = new TankDrive(leftMain, leftFollower, rightMain, rightFollower);
            barrel = new Barrel(barrelTiltMotor, barrelIndexMotor);
            driverModule = new DriverModule(Constants.DriverModule.DriveModulePort);

            controller = new CommandXboxController(Constants.Controls.ControllerPort);

            driveTeleop = new DriveTeleop(tankDrive, controller);
            barrelTeleop = new BarrelTiltTeleop(barrel, controller);
            shootTeleop = new ShootTeleop(barrel, driverModule);

            ConfigureButtonBindings();
        }

        public void ConfigureButtonBindings() {
            controller.A().OnTrue(shootTeleop);

            controller.B().OnTrue(new InstantCommand(() => {
                barrel.Index();
            }));

            controller.X().OnTrue(new InstantCommand(() => {
                driverModule.Set(
                        Constants.DriverModule.CompressorPort,
                        !driverModule.Get(Constants.DriverModule.CompressorPort)
                    );
            }));
        }
    }
}
