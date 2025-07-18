﻿using CortexAccess;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;

namespace MentalCommandsLogger
{
    class Program
    {
        // init constants before running
        const string OutFilePath = @"MentalCommandsLogger.csv";
        const string LicenseID = ""; // Should be put empty string. Emotiv Cloud will choose the license automatically
        const string WantedHeadsetId = ""; // if you want to connect to specific headset, put headset id here. For example: "EPOCX-71D833AC"

        private static FileStream OutFileStream;

        static void Main(string[] args)
        {
            Console.WriteLine("Mental Commands LOGGER");
            Console.WriteLine("Please wear Headset with good signal!!!");

            // Delete Output file if existed
            if (File.Exists(OutFilePath))
            {
                File.Delete(OutFilePath);
            }
            OutFileStream = new FileStream(OutFilePath, FileMode.Append, FileAccess.Write);


            DataStreamExample dse = new DataStreamExample();
            dse.AddStreams("com");
            dse.OnSubscribed += SubscribedOK;
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
                if (key == "com")
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

        private static void OnMentalCommandDataReceived(object sender, ArrayList mentalCommandData)
        {
            Console.WriteLine(mentalCommandData.ToString());
            WriteDataToFile(mentalCommandData);
        }

    }
}
