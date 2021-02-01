//using System;
//using System.Linq;
//using ActionCalculator.Abstractions;
//using Action = ActionCalculator.Abstractions.Action;

//namespace ActionCalculator
//{
//    public class DiceRollingActionCalculator : IActionCalculator
//    {
//        private int _rerolls;
//        private Random _random;
//        private const int Retries = 100000000;

//        public CalculationResult Calculate(Calculation calculation)
//        {
//            _random = new Random(DateTime.UtcNow.Millisecond);
            
//            var failureCount = 0m;

//            for (var i = 0; i < Retries; i++)
//            {
//                _rerolls = calculation.Rerolls;

//                if (calculation.Actions.Select(ActionSucceeds).Any(success => !success))
//                {
//                    failureCount++;
//                }
//            }

//            return new CalculationResult(1m - failureCount / Retries);
//        }

//        private bool ActionSucceeds(Action action)
//        {
//            var actionSucceeds = Roll(action);

//            if (actionSucceeds)
//            {
//                return true;
//            }

//            if (_rerolls <= 0)
//            {
//                return false;
//            }

//            _rerolls--;
            
//            return Roll(action);
//        }

//        private bool Roll(Action action)
//        {
//            return _random.NextDouble() < (7.0 - action.Roll) / 6;
//        }
//    }
//}
