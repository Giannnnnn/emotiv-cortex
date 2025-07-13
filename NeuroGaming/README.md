# NeuroGaming: Mind-Controlled Mouse with Emotiv Epoc X

NeuroGaming is a C# project that extends an Emotiv Cortex SDK example project to enable hands-free mouse control using the Emotiv Epoc X headset. By processing real-time quaternion data for head motion and mental commands (e.g., *lift*, *drop*), it allows users to move the cursor and perform mouse clicks with their mind. This project is ideal for gaming, accessibility, and brain-computer interface (BCI) experimentation. Originally targeting .NET Framework, it has been upgraded to .NET 8 for modern performance and features.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

Copyright (c) 2020 EMOTIV  
Copyright (c) 2025 [Your Name]

## Acknowledgements
- This project is a modified version of the [Emotiv Cortex SDK Example Project](https://github.com/Emotiv/cortex-example) (MIT License, Copyright (c) 2020 EMOTIV). Thank you to Emotiv for providing the foundation for headset integration.
- New features, including quaternion-based cursor control, smoothing, and mental command mappings, were developed by [Your Name].
- Built with the [Emotiv Cortex SDK](https://emotiv.gitbook.io/cortex-api/).
- Thanks to the Emotiv developer community for inspiration and support.

## Features

- **Head Motion Control**: Uses quaternion data (`Q0`, `Q1`, `Q2`, `Q3`) to translate head tilts and turns into smooth cursor movements.
- **Mental Command Inputs**: Maps mental commands to mouse actions (e.g., left-click down/up) for hands-free interaction.
- **Data Logging**: Saves motion and mental command data to `MotionDataLogs.csv` for analysis or debugging.
- **Customizable Settings**: Adjustable sensitivity, deadzone, and smoothing for personalized control.
- **Extensible Framework**: Easily add new mental commands, gestures, or input mappings for custom applications.

## How It Works

### Quaternion-Based Cursor Movement
The Emotiv Epoc X headset sends quaternion data (`Q0`, `Q1`, `Q2`, `Q3`) to describe your head’s 3D orientation. A quaternion is a set of four numbers (`w`, `x`, `y`, `z`) that represents rotations in 3D space, avoiding issues like gimbal lock. The project processes quaternions as follows:
1. **Calibration**: The first quaternion received is saved as a reference point (`w_cal`, `x_cal`, `y_cal`, `z_cal`) to establish a "neutral" head position.
2. **Relative Motion**: New quaternions are compared to the calibrated reference to calculate *relative pitch* (up/down tilt) and *relative yaw* (left/right turn) using quaternion math.
3. **Deadzone Filtering**: Small movements below `MovementDeadzone` are ignored to prevent jitter.
4. **Cursor Movement**: Pitch and yaw are scaled by sensitivity settings to compute cursor displacement.
5. **Smoothing**: A buffer averages recent movements for fluid cursor motion.
6. **Screen Clamping**: The cursor is constrained to stay within the screen boundaries.

### Mental Commands
Mental commands like *lift* or *drop* are trained via the Emotiv BCI Trainer app and mapped to mouse actions. The `force` value can adjust interaction intensity.

### Additional Sensor Data
The headset provides accelerometer (`ACCX`, `ACCY`, `ACCZ`) and magnetometer (`MAGX`, `MAGY`, `MAGZ`) data, which could be used for advanced features like gesture recognition.

## Requirements

### Hardware
- Emotiv Epoc X (or compatible Emotiv headset) or a simulated device (via Emotiv Launcher)

### Software
- **.NET 8 SDK**: This project targets [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0). Install the .NET 8 SDK from Microsoft. Note: The project was upgraded from .NET Framework 4.8 (used by the original Emotiv example) to .NET 8 for improved performance and modern C# features.
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community Edition or higher recommended).
- [Emotiv Cortex SDK](https://github.com/Emotiv/cortex-example). A valid Emotiv account and license (free or paid) is required.

## Setup and Installation

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/NeuroGaming.git
cd NeuroGaming
```

### 2. Install the .NET 8 SDK
- Download and install the .NET 8 SDK from [Microsoft’s .NET download page](https://dotnet.microsoft.com/download/dotnet/8.0).
- Verify installation:
  ```bash
  dotnet --list-sdks
  ```
  Ensure an `8.0.x` version (e.g., `8.0.204`) is listed.

### 3. Configure the AppClientId and AppClientSecret
         /*
         * To get a client id and a client secret, you must connect to your Emotiv
         * account on emotiv.com and create a Cortex app.
         * https://www.emotiv.com/my-account/cortex-apps/
         */

- Access the project CortexAccess.csproj
- Paste the keys of your apps in the desired place

### 4. Open in Visual Studio
- Launch `NeuroGaming.sln` in Visual Studio 2022.

### 5. Build and Run
- In Visual Studio, click **Start** to build and run.
- Or use the terminal:
  ```bash
  dotnet build
  dotnet run
  ```

### 6. Configure the Headset
- Edit `Program.cs` to set your headset and license details:
  ```csharp
  const string WantedHeadsetId = "EPOCX-XXXXXXXX"; // Optional: your headset’s ID
  const string LicenseID = "";                     // Leave empty for auto-assigned license

  ```

## Usage

### Mental Command Controls
| Command  | Action                     |
|----------|----------------------------|
| `neutral`| No action                  |
| `lift`   | Left mouse button down     |
| `drop`   | Left mouse button up       |
| `....`   | Add your own commans here  |

To add new commands, modify the `ConvertMentalCommandToMouseAction` method:
```csharp
case "push":
    // Example: Simulate right-click
    MouseSimulator.RightButtonDown();
    break;
```

### Head Motion Controls
- **Yaw** (head turn left/right): Moves cursor horizontally.
- **Pitch** (head tilt up/down): Moves cursor vertically.
- **Calibration**: The first head position is set as the neutral point. Restart the app to recalibrate.
- **Tuning**: Adjust these constants in `Program.cs` for responsiveness:
  ```csharp
  const float HorizontalSensitivity = 70.0f;  // Horizontal cursor speed
  const float VerticalSensitivity = 50.0f;    // Vertical cursor speed
  const float MovementDeadzone = 0.03f;       // Ignore small head movements
  const int SmoothingWindow = 5;              // Number of movements to average
  ```

## Data Logging
All data is logged to `NeuroGamingDataLogs.csv` in the project directory. Each line includes:
- **Timestamp**: When the data was received.
- **Motion Data**: Quaternion values (`Q0`, `Q1`, `Q2`, `Q3`), accelerometer (`ACCX`, `ACCY`, `ACCZ`), and magnetometer (`MAGX`, `MAGY`, `MAGZ`).
- **Mental Commands**: Command name (e.g., `lift`) and force value.

**Note**: The log file is overwritten on each run. To preserve logs, update `OutFilePath` in `Program.cs`.

## Troubleshooting
| Issue                     | Solution                                                                 |
|---------------------------|--------------------------------------------------------------------------|
| Headset not detected      | Set `WantedHeadsetId` to `""` for auto-detection; ensure Cortex is running |
| No mental command response | Train commands in the Emotiv BCI Trainer app                             |
| Cursor not moving         | Verify `MOT` stream is active in Cortex; check headset signal quality      |
| Build errors (e.g., missing APIs) | Ensure `System.Drawing.Common` package is installed; verify `CortexAccess` targets .NET 8 |
| App crashes               | Check `CortexAccess.dll` compatibility with .NET 8; consider reverting to .NET Framework 4.8 |

For detailed logs, check `MotionDataLogs.csv` or enable debug output in `Program.cs`.

## Technical Notes
- **Upgrade to .NET 8**: The project was upgraded from .NET Framework 4.8 to .NET 8 for modern C# features (e.g., nullable reference types, implicit usings) and improved performance. Ensure all dependencies (e.g., `CortexAccess`) are compatible with .NET 8.
- **Quaternion Math**: Computes relative quaternions to measure head movement, reducing drift and ensuring accurate cursor control.
- **Smoothing**: A moving average filter (`SmoothingWindow`) minimizes jitter for fluid cursor motion.
- **Windows Dependency**: Uses `System.Windows.Forms` for cursor control, requiring Windows. Non-Windows platforms are not supported.