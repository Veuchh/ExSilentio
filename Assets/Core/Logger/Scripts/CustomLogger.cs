using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Logger
{
    public static class CustomLogger
    {
        static string wordInputHistory = "Time,WordInput,Position,Rotation";
        static string trajectoryHistory = ",Time,Position,Rotation";
        static int totalConsoleOpen = 0;
        static int totalWordInput = 0;

        public static void IncrementConsoleOpenValue()
        {
            totalConsoleOpen++;
        }

        public static void OnAddToTrajectoryHistory(Vector3 position, float rotation)
        {
            trajectoryHistory += $"\n,{Time.time},{position},{rotation}";
        }

        public static void OnWordInput(string input, Vector3 position, float rotation)
        {
            wordInputHistory += $"\n,{input},{Time.time},{position},{rotation}";
        }

        public static void CompileDataToCSV()
        {

        }
    }
}