namespace MRRC.Guacamole
{
    public static class Maths
    {
        public static int Mod(this int x, int m) => (x % m + m) % m;
    }
}