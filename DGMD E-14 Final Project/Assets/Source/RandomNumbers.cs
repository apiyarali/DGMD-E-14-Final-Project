
namespace Assets.Source
{
    public static class RandomNumbers
    {
        private static System.Random random = new System.Random();

        public static int GetRandomIntegerInclusive(int minimum, int maximum)
        {
            return random.Next(minimum, maximum + 1);
        }

        public static decimal GetRandomDecimalInclusive(decimal minimum, decimal maximum)
        {
            return GetRandomDecimalInclusive(maximum - minimum) + minimum;
        }
        public static decimal GetRandomDecimalInclusive(decimal maximum)
        {
            return (decimal)random.NextDouble() * maximum;
        }
    }
}
