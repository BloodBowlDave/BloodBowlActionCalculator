using ActionCalculator.Models;
using Microsoft.AspNetCore.Components;
using Block = ActionCalculator.Models.Actions.Block;
using Player = ActionCalculator.Models.Player;
using Dodge = ActionCalculator.Models.Actions.Dodge;
using Action = ActionCalculator.Models.Actions.Action;

namespace ActionCalculator.Web.Client.Pages.Components.Calculation
{
    public partial class ActionSummary
    {
        private int Successes { get; set; }

        [CascadingParameter(Name = "Season")]
        public string Season { get; set; } = "Season 3";

        [Parameter]
        public PlayerAction PlayerAction { get; set; } = null!;

        [Parameter]
        public int Index { get; set; }

        [Parameter]
        public EventCallback<int> RemoveAction { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleBreakTackle { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleDivingTackle { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleBrawler { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleHatred { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnTogglePro { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleRerollInaccurate { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, bool>> OnToggleRerollFailure { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, int>> OnSuccessesChanged { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, int>> OnPushesChanged { get; set; }

        [Parameter]
        public EventCallback<Tuple<int, int>> OnFrenzyDiceChanged { get; set; }

        [Parameter]
        public ActionType? LastActionType { get; set; }

        [Parameter]
        public ActionType? PenultimateActionType { get; set; }

        private Player Player => PlayerAction.Player;
        private Action Action => PlayerAction.Action;

        private void RemoveActionByIndex(int i)
        {
            RemoveAction.InvokeAsync(i);
        }

        private bool RerollFailure() => ((Models.Actions.Dauntless)Action).RerollFailure;

        private bool BreakTackle() => ((Dodge)Action).UseBreakTackle;

        private bool DivingTackle() => ((Dodge)Action).UseDivingTackle;

        private bool UseBrawler() => ((Block)Action).UseBrawler;

        private bool UseHatred() => ((Block)Action).UseHatred;

        private bool UsePro() => Action.UsePro;

        private bool RerollInaccurate() => Action.ActionType switch
        {
            ActionType.Pass => ((Models.Actions.Pass)Action).RerollInaccuratePass,
            ActionType.ThrowTeammate => ((Models.Actions.ThrowTeammate)Action).RerollInaccurateThrow,
            _ => false
        };

        private void ToggleBreakTackle()
        {
            OnToggleBreakTackle.InvokeAsync(new Tuple<int, bool>(Index, !BreakTackle()));
        }

        private void ToggleDivingTackle()
        {
            OnToggleDivingTackle.InvokeAsync(new Tuple<int, bool>(Index, !DivingTackle()));
        }

        private void ToggleBrawler()
        {
            OnToggleBrawler.InvokeAsync(new Tuple<int, bool>(Index, !UseBrawler()));
        }

        private void ToggleHatred()
        {
            OnToggleHatred.InvokeAsync(new Tuple<int, bool>(Index, !UseHatred()));
        }

        private void TogglePro()
        {
            OnTogglePro.InvokeAsync(new Tuple<int, bool>(Index, !UsePro()));
        }

        private void ToggleRerollInaccurate()
        {
            OnToggleRerollInaccurate.InvokeAsync(new Tuple<int, bool>(Index, !RerollInaccurate()));
        }

        private void ToggleRerollFailure()
        {
            OnToggleRerollFailure.InvokeAsync(new Tuple<int, bool>(Index, !RerollFailure()));
        }

        private void SuccessesChanged(int successes)
        {
            OnSuccessesChanged.InvokeAsync(new Tuple<int, int>(Index, successes));
        }

        private void PushesChanged(int pushes)
        {
            OnPushesChanged.InvokeAsync(new Tuple<int, int>(Index, pushes));
        }

        private void FrenzyDiceChanged(int dice)
        {
            OnFrenzyDiceChanged.InvokeAsync(new Tuple<int, int>(Index, dice));
        }

        private int BlockSuccessesMax() =>
            PlayerAction.RequiresNonCriticalFailure
                ? 5
                : 5 - ((Block)Action).NumberOfNonCriticalFailures;

        private IEnumerable<string> GetActionOptions()
        {
            var options = new List<string>();

            if (Action.ActionType == ActionType.Block)
            {
                if (Player.CanUseSkill(CalculatorSkills.Brawler, CalculatorSkills.None)) options.Add("Use Brawler");
                if (Season == "Season 3" && Player.CanUseSkill(CalculatorSkills.Hatred, CalculatorSkills.None)) options.Add("Use Hatred");
            }

            if (Action.ActionType == ActionType.Dodge)
            {
                if (Player.CanUseSkill(CalculatorSkills.BreakTackle, CalculatorSkills.None)) options.Add("Use Break Tackle");
                options.Add("Affected by Diving Tackle");
            }

            if (Action.ActionType is ActionType.Pass or ActionType.ThrowTeammate)
            {
                options.Add("Reroll Inaccurate");
            }

            if (Action.ActionType == ActionType.Dauntless)
            {
                options.Add("Reroll Failure");
            }

            if (Action.IsRerollable() && Player.CanUseSkill(CalculatorSkills.Pro, CalculatorSkills.None))
            {
                options.Add("Use Pro");
            }

            return options;
        }

        private IReadOnlyCollection<string> GetSelectedOptions()
        {
            var selected = new List<string>();

            if (Action.ActionType == ActionType.Block)
            {
                if (Player.CanUseSkill(CalculatorSkills.Brawler, CalculatorSkills.None) && UseBrawler()) selected.Add("Use Brawler");
                if (Season == "Season 3" && Player.CanUseSkill(CalculatorSkills.Hatred, CalculatorSkills.None) && UseHatred()) selected.Add("Use Hatred");
            }

            if (Action.ActionType == ActionType.Dodge)
            {
                if (Player.CanUseSkill(CalculatorSkills.BreakTackle, CalculatorSkills.None) && BreakTackle()) selected.Add("Use Break Tackle");
                if (DivingTackle()) selected.Add("Affected by Diving Tackle");
            }

            if (Action.ActionType is ActionType.Pass or ActionType.ThrowTeammate)
            {
                if (RerollInaccurate()) selected.Add("Reroll Inaccurate");
            }

            if (Action.ActionType == ActionType.Dauntless)
            {
                if (RerollFailure()) selected.Add("Reroll Failure");
            }

            if (Action.IsRerollable() && Player.CanUseSkill(CalculatorSkills.Pro, CalculatorSkills.None) && UsePro())
            {
                selected.Add("Use Pro");
            }

            return selected;
        }

        private void OptionsChanged(IReadOnlyCollection<string> newSelected)
        {
            var current = GetSelectedOptions().ToHashSet();
            var newSet = newSelected.ToHashSet();

            foreach (var added in newSet.Except(current))
            {
                ApplyOption(added, true);
            }

            foreach (var removed in current.Except(newSet))
            {
                ApplyOption(removed, false);
            }
        }

        private void ApplyOption(string option, bool value)
        {
            switch (option)
            {
                case "Use Brawler": OnToggleBrawler.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Use Hatred": OnToggleHatred.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Use Break Tackle": OnToggleBreakTackle.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Affected by Diving Tackle": OnToggleDivingTackle.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Reroll Inaccurate": OnToggleRerollInaccurate.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Reroll Failure": OnToggleRerollFailure.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
                case "Use Pro": OnTogglePro.InvokeAsync(new Tuple<int, bool>(Index, value)); break;
            }
        }
    }
}

