namespace Minesweeper.Domain
{
    /// <summary>Seam over randomness so mine placement is deterministic under a fixed seed.</summary>
    public interface IRandom
    {
        int Next(int maxExclusive);
    }
}
