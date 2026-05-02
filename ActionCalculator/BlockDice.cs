using ActionCalculator.Abstractions;
using ActionCalculator.Models;
using ActionCalculator.Utilities;

namespace ActionCalculator
{
    public class BlockDice : IBlockDice
    {
        private readonly List<List<BlockResult>>[] _rolls;
        private static readonly List<BlockResult> BlockDiceRolls = new()
        {
            BlockResult.Pow,
            BlockResult.PowPush,
            BlockResult.Push2,
            BlockResult.Push1,
            BlockResult.BothDown,
            BlockResult.Skull
        };

        public BlockDice()
        {
            _rolls = GenerateRolls().ToArray();
        }

        private static IEnumerable<List<List<BlockResult>>> GenerateRolls()
        {
            for (var i = 1; i <= 3; i++)
            {
                var rolls = new List<List<BlockResult>>();

                for (var j = 0; j < i; j++)
                {
                    rolls.Add(BlockDiceRolls);
                }

                yield return rolls.GetCombinationsOfLists().ToList();
            }
        }

        public IEnumerable<BlockResult> Rolls() => BlockDiceRolls;

        public List<List<BlockResult>> Rolls(int numberOfDice) => _rolls[numberOfDice - 1];
    }
}
