namespace MRRC.Guacamole
{
    public static class Maths
    {
        /// <summary>
        /// Modulus operator
        /// </summary>
        public static int Mod(this int x, int m) => (x % m + m) % m;
    }
}