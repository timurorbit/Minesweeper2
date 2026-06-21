namespace Minesweeper.Domain
{
    public sealed class SystemRandom : IRandom
    {
        private readonly System.Random random;

        public SystemRandom() : this(System.Environment.TickCount) { }
        public SystemRandom(int seed) => random = new System.Random(seed);

        public int Next(int maxExclusive) => random.Next(maxExclusive);
    }
}
