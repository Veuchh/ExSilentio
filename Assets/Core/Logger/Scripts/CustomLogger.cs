using System;
using System.IO;
using UnityEngine;

namespace LW.Logger
{
    public static class CustomLogger
    {
        static string wordInputHistory = "WordInput,Time,Position,Rotation";
        static string consoleOpenHistory = "Time";
        static string trajectoryHistory = "Position,Rotation, Time";
        static int totalConsoleOpen = 0;
        static int totalWordInput = 0;
        static float totalDistanceTraveled = 0;

        public static void IncrementConsoleOpenValue()
        {
            totalConsoleOpen++;
            consoleOpenHistory += "\n" + Time.time.ToString("F2");
        }

        public static void IncrementTraveledDistance(float traveledDistance)
        {
            totalDistanceTraveled++;
        }

        public static void OnAddToTrajectoryHistory(Vector3 position, float rotation)
        {
            trajectoryHistory += $"\nx:{position.x} y:{position.y} z:{position.z},{rotation},{Time.time}";
        }

        public static void OnWordInput(string input, Vector3 position, float rotation)
        {
            Transform playerTransform = Camera.main.transform.parent.transform;
            position = playerTransform.position;
            rotation = playerTransform.rotation.eulerAngles.y;
            wordInputHistory += $"\n{input},{Time.time},x:{position.x} y:{position.y} z:{position.z},{rotation}";
            totalWordInput++;
        }

        public static void CompileDataToCSV()
        {
            string output = "Word inputs\n" + wordInputHistory + "\n\n\nTotal Word Amount," + totalWordInput
                +"\n\n\nConsole Open History\n"+ consoleOpenHistory+
                "\n\n\nTotal Travel Distance," + totalDistanceTraveled + "\nTotal Console Open," + totalConsoleOpen
                +"\nTotal Time In Game,"+Time.time.ToString("F2");

            TextWriter tw = new StreamWriter(Application.dataPath + "/GAME_LOG_" + DateTime.Now.Year.ToString() + "_"
                +DateTime.Now.Month.ToString() + "_"
                +DateTime.Now.Day.ToString() + "_"
                +DateTime.Now.Hour.ToString() + "_"
                +DateTime.Now.Minute.ToString()
                + ".csv", false);

            tw.Write(output);

            tw.Close();
        }
    }
}