namespace Minesweeper.Domain
{
    /// <summary>
    /// Immutable grid coordinate (column <see cref="X"/>, row <see cref="Y"/>).
    /// </summary>
    public readonly struct Coordinate : System.IEquatable<Coordinate>
    {
        public readonly int X;
        public readonly int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Coordinate other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Coordinate other && Equals(other);
        public override int GetHashCode() => unchecked((X * 397) ^ Y);
        public override string ToString() => $"({X}, {Y})";
    }
}
