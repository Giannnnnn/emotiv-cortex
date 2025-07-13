using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Drawing;
using CortexAccess;
using System.Text;

namespace NeuroGaming
{
    class Program
    {
        // init constants before running
        const string OutFilePath = @"NeuroGamingDataLogs.csv";
        const string LicenseID = ""; // Should be put empty string. Emotiv Cloud will choose the license automatically
        const string WantedHeadsetId = ""; // if you want to connect to specific headset, put headset id here. For example: "EPOCX-71D833AC"
        const string motionDataCommandType = "mot";
        const string mentalCommandDataType = "com";
        const float mentalCommandPushThreshhold = 0;
        const float mentalCommandLiftThreshhold = 0;

        // Sensitivity Configuration
        const float BaseSensitivity = 0.24f;
        const float HorizontalSensitivity = 70.0f;
        const float VerticalSensitivity = 50.0f;
        const float MovementDeadzone = 0.03f;
        const int SmoothingWindow = 6;

        // Calibration quaternion
        private static double w_cal = 0.0;
        private static double x_cal = 0.0;
        private static double y_cal = 0.0;
        private static double z_cal = 0.0;
        private static bool isCalibrated = false;
        private static Queue<Point> movementBuffer = new Queue<Point>();

        // P/Invoke for mouse_event to simulate mouse clicks
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private static FileStream OutFileStream;

        static void Main(string[] args)
        {
            Console.WriteLine("Enjoy the experience of playing games using your mind!");
            Console.WriteLine("Please wear Headset with good signal!!!");
            Console.WriteLine("Read the readme file to start using this!!!");


            // Center the cursor on the screen
            var screenBounds = Screen.PrimaryScreen.Bounds;
            Cursor.Position = new Point(screenBounds.Width / 2, screenBounds.Height / 2);

            // Delete Output file if existed
            if (File.Exists(OutFilePath))
            {
                File.Delete(OutFilePath);
            }
            OutFileStream = new FileStream(OutFilePath, FileMode.Append, FileAccess.Write);


            DataStreamExample dse = new DataStreamExample();
            dse.AddStreams(motionDataCommandType);
            dse.AddStreams(mentalCommandDataType);
            dse.OnSubscribed += SubscribedOK;
            dse.OnMotionDataReceived += OnMotionDataReceived;
            dse.OnMentalComandDataReceived += OnMentalCommandDataReceived;

            dse.Start(LicenseID, false, WantedHeadsetId);

            Console.WriteLine("Press Esc to flush data to file and exit");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }

            // Unsubcribe stream
            dse.UnSubscribe();
            Thread.Sleep(5000);

            // Close Session
            dse.CloseSession();
            Thread.Sleep(5000);
            // Close Out Stream
            OutFileStream.Dispose();
        }

        private static void SubscribedOK(object sender, Dictionary<string, JArray> e)
        {
            foreach (string key in e.Keys)
            {
                if (key == motionDataCommandType)
                {
                    // print header
                    ArrayList header = e[key].ToObject<ArrayList>();
                    //add timeStamp to header
                    header.Insert(0, "Timestamp");
                    WriteDataToFile(header);
                }
                else if (key == mentalCommandDataType)
                {
                    // print header
                    ArrayList header = e[key].ToObject<ArrayList>();
                    //add timeStamp to header
                    header.Insert(0, "Timestamp");
                    WriteDataToFile(header);
                }
            }
        }

        // Write Header and Data to File
        private static void WriteDataToFile(ArrayList data)
        {
            int i = 0;
            for (; i < data.Count - 1; i++)
            {
                byte[] val = Encoding.UTF8.GetBytes(data[i].ToString() + ", ");

                if (OutFileStream != null)
                    OutFileStream.Write(val, 0, val.Length);
                else
                    break;
            }
            // Last element
            byte[] lastVal = Encoding.UTF8.GetBytes(data[i].ToString() + "\n");
            if (OutFileStream != null)
                OutFileStream.Write(lastVal, 0, lastVal.Length);
        }

        private static void OnMotionDataReceived(object sender, ArrayList motionData)
        {
            Console.WriteLine(motionData.ToString());
            WriteDataToFile(motionData);

            ConvertQuaternionToCursorMove(sender, motionData);
        }

        private static void OnMentalCommandDataReceived(object sender, ArrayList mentalCommandData)
        {
            Console.WriteLine(mentalCommandData.ToString());
            WriteDataToFile(mentalCommandData);
            var timestamp = mentalCommandData[0]; // Optional, not used
            var command = mentalCommandData[1].ToString(); // This is the mental command (e.g., "push", "pull")
            var force = Convert.ToSingle(mentalCommandData[2]); // This is the strength of the command, as a float

            // Log to console for debugging
            Console.WriteLine($"Mental Command: {command}, Force: {force}");
            ConvertMentalCommandToMouseAction(command, force);
        }


        private static void ConvertQuaternionToCursorMove(object sender, ArrayList motData)
        {
            if (motData.Count < 7) return;

            double w = Convert.ToDouble(motData[3]); // q0
            double x = Convert.ToDouble(motData[4]); // q1
            double y = Convert.ToDouble(motData[5]); // q2
            double z = Convert.ToDouble(motData[6]); // q3

            if (!isCalibrated)
            {
                w_cal = w; x_cal = x; y_cal = y; z_cal = z;
                isCalibrated = true;
                return;
            }

            // Compute relative quaternion
            double w_rel = w * w_cal + x * x_cal + y * y_cal + z * z_cal;
            double x_rel = -w * x_cal + x * w_cal - y * z_cal + z * y_cal;
            double y_rel = -w * y_cal + x * z_cal + y * w_cal - z * x_cal;
            double z_rel = -w * z_cal - x * y_cal + y * x_cal + z * w_cal;

            double relativePitch = 2 * x_rel;
            double relativeYaw = 2 * z_rel;

            if (Math.Abs(relativeYaw) < MovementDeadzone) relativeYaw = 0;
            if (Math.Abs(relativePitch) < MovementDeadzone) relativePitch = 0;

            int rawX = (int)(-relativeYaw * HorizontalSensitivity * BaseSensitivity);
            int rawY = (int)(-relativePitch * VerticalSensitivity * BaseSensitivity);

            movementBuffer.Enqueue(new Point(rawX, rawY));
            if (movementBuffer.Count > SmoothingWindow)
                movementBuffer.Dequeue();

            int smoothX = (int)movementBuffer.Average(p => p.X);
            int smoothY = (int)movementBuffer.Average(p => p.Y);

            int targetX = Cursor.Position.X + smoothX;
            int targetY = Cursor.Position.Y + smoothY;

            int clampedX = Math.Max(0, Math.Min(targetX, Screen.PrimaryScreen.Bounds.Width));
            int clampedY = Math.Max(0, Math.Min(targetY, Screen.PrimaryScreen.Bounds.Height));

            Cursor.Position = new Point(clampedX, clampedY);
        }

        private static void SimulateMouseHold(float force)
        {
            if (force <= mentalCommandPushThreshhold) return;
            if (force >= mentalCommandPushThreshhold)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                Console.WriteLine("Mouse left button being hold.");
            }
        }

        private static void SimulateMouseDrop(float force)
        {
            if (force <= mentalCommandLiftThreshhold) return;
            if (force >= mentalCommandLiftThreshhold)
            {
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                Console.WriteLine("Mouse left button dropped.");
            }
        }

        private static void ConvertMentalCommandToMouseAction(string command, float force)
        {
            switch (command)
            {
                case "neutral":
                    //Add your method to neutral state action here
                    Console.WriteLine("Neutral Command Received");
                    break;

                case "lift":
                    SimulateMouseHold(force);
                    break;

                case "drop":
                    SimulateMouseDrop(force);
                    break;

                default:
                    break;
            }

        }

    }
}
