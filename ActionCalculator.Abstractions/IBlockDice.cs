using ActionCalculator.Models;

namespace ActionCalculator.Abstractions
{
    public interface IBlockDice
    {
        IEnumerable<BlockResult> Rolls();
        List<List<BlockResult>> Rolls(int numberOfDice);
    }
}
