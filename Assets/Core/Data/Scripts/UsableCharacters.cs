namespace LW.Data
{
    public static class UsableCharacters
    {
        private static char[] characters =
            {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '-', 'é', 'è', 'à', 'ê', 'î', 'ô', 'û', 'ï', '\'', '_', ' '};

        public static char[] Characters { get => characters;}
    }
}