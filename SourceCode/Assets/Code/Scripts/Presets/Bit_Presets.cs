namespace ArbazAminKhan.Presets
{
    public class Bit_Presets
    {
        public static int GetFlag(int index) => 1 << index;

        public static bool HasFlag(int mask, int index)
            => (mask & GetFlag(index)) != 0;

        public static int AddFlag(int mask, int index)
            => mask | GetFlag(index);

        public static int RemoveFlag(int mask, int index)
            => mask & ~GetFlag(index);
    }
}