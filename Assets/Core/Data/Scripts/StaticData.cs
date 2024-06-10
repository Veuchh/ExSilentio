using UnityEngine;

namespace LW.Data
{
    public static class StaticData
    {
        private static int openWindowsAmount = 0;
        private static Transform playerTransform;
        public static int OpenWindowsAmount { get => openWindowsAmount; set => openWindowsAmount = value; }
        public static Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    }
}