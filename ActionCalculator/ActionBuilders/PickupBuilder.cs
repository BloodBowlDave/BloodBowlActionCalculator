﻿using ActionCalculator.Abstractions;
using ActionCalculator.Models.Actions;
using ActionCalculator.Utilities;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.ActionBuilders;

public class PickupBuilder : IActionBuilder
{
    private readonly ID6 _d6;

    public PickupBuilder(ID6 d6)
    {
        _d6 = d6;
    }

    public Action Build(string input)
    {
        var usePro = input.Contains("*");

        input = input.Replace("*", "");

        var roll = int.Parse(input.Length == 2 ? input[1..] : input);
        var success = _d6.Success(1, roll);

        return new PickUp(success, 1 - success, usePro, roll);
    }
}