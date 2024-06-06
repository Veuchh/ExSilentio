namespace LW.Data
{
    public static class StaticData
    {
        private static int openWindowsAmount = 0;
        public static int OpenWindowsAmount { get => openWindowsAmount; set => openWindowsAmount = value; }
    }
}