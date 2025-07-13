# MentalCommandsLogger: Log Mental Commands from Emotiv Epoc X

MentalCommandsLogger is a C# project that extends an Emotiv Cortex SDK example to log mental command data from an Emotiv headset to a CSV file. Using WebSocket communication via the Cortex SDK, it authenticates with the Emotiv Cloud, subscribes to the mental command (`com`) data stream, and saves the data with timestamps to `MentalCommandsLogger.csv`. This project is ideal for researchers, developers, or hobbyists exploring brain-computer interfaces (BCI) or analyzing mental command patterns.


## Acknowledgements
- This project is a modified version of the [Emotiv Cortex SDK Example Project](https://github.com/Emotiv/cortex-example) (MIT License, Copyright (c) 2020 EMOTIV). Thank you to Emotiv for providing the foundation for headset integration.
- Features like CSV logging of mental command data with timestamps were developed by Giovani Florek.
- Built with the [Emotiv Cortex SDK](https://emotiv.gitbook.io/cortex-api/).
- Thanks to the Emotiv developer community for inspiration and support.

## Features
- **Mental Command Logging**: Captures mental command data (e.g., command labels like *lift*, *drop*, and their force values) from the Emotiv Epoc X headset.
- **CSV Output**: Saves data with timestamps to `MentalCommandsLogger.csv` for easy analysis in tools like Excel or Python.
- **WebSocket Communication**: Uses the Cortex SDK to authenticate with the Emotiv Cloud and stream data via WebSocket.
- **Simple Interface**: Console-based application with minimal setup, exiting on Esc key press.
- **Extensible**: Easily modified to log additional data streams (e.g., motion or EEG) or customize output formats.

## How It Works
The project uses the Emotiv Cortex SDK to:
1. **Authenticate**: Connects to the Emotiv Cloud using a user account and optional license ID.
2. **Subscribe to Mental Commands**: Requests the `com` (mental command) data stream, which includes trained commands like *neutral*, *lift*, or *drop*.
3. **Log Data**: Writes each mental command event (with timestamp, command label, and force value) to `MentalCommandsLogger.csv`.
4. **Exit Gracefully**: Stops logging and closes the session when the Esc key is pressed, ensuring data is flushed to the file.

The output CSV (`MentalCommandsLogger.csv`) includes:
- **Timestamp**: When the data was received.
- **Command Data**: Mental command label (e.g., `lift`) and associated force value.

### Example CSV Output
```csv
Timestamp,com,force
1752439918.451,lift,0.8
1752439918.652,neutral,0.0
1752439918.853,drop,0.7
```

### Customization
- **Change Output File**: Modify `OutFilePath` in `Program.cs` to save logs to a different file or location:
  ```csharp
  const string OutFilePath = @"CustomPath/MentalCommands.csv";
  ```
- **Add Data Streams**: Extend `DataStreamExample.AddStreams` to include other streams (e.g., `mot` for motion data):
  ```csharp
  dse.AddStreams("com", "mot");
  ```
- **Custom Logging**: Modify `WriteDataToFile` to change the CSV format or add additional data fields.

## Troubleshooting
| Issue                     | Solution                                                                 |
|---------------------------|--------------------------------------------------------------------------|
| Headset not detected      | Set `WantedHeadsetId` to `""` for auto-detection; ensure Cortex app is running |
| No mental command data     | Train commands in MentalCommandTrainer; verify `com` stream is active |
| Build errors (e.g., missing APIs) | Ensure `Newtonsoft.Json` and `CortexAccess` are compatible with .NET 8; add packages if needed |
| App crashes               | Check `CortexAccess.dll` compatibility with .NET 8; consider reverting to .NET Framework 4.8 |
| CSV file not created       | Verify write permissions for `OutFilePath`; ensure `FileStream` is initialized |

For detailed logs, check `MentalCommandsLogger.csv` or add debug output to `Program.cs`.

## Technical Notes
- **Upgrade to .NET 4.8**: The project was upgraded from .NET Framework 4.5.2 to .NET 8 for modern C# features and performance. 
- **WebSocket Communication**: The Cortex SDK uses WebSocket to stream data from the Emotiv Cloud, ensuring real-time mental command updates.
- **Data Handling**: Mental command data is parsed using `Newtonsoft.Json` and logged with timestamps for precise analysis.
- **Extensibility**: The `DataStreamExample` class can be extended to support additional streams or custom processing.
