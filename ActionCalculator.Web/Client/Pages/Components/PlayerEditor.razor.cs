using ActionCalculator.Models;
using ActionCalculator.Utilities;
using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Pages.Components
{
    public partial class PlayerEditor
    {
        [CascadingParameter(Name = "Season")]
        public string Season { get; set; } = "Season 3";

        [Parameter]
        public Player CurrentPlayer { get; set; } = null!;

        [Parameter]
        public int PlayerNumber { get; set; }

        [Parameter] 
        public EventCallback<Player> OnPlayerChanged { get; set; }

        [Parameter]
        public EventCallback OnNewPlayer { get; set; }
        
        private IEnumerable<CalculatorSkills> GetSkills() =>
            typeof(CalculatorSkills).ToEnumerable<CalculatorSkills>()
                .Where(x => x > 0 && (!x.HasAttribute<HideFromPlayerEditorAttribute>() || (Season == "Season 3" && (x == CalculatorSkills.Hatred || x == CalculatorSkills.LoneFouler))))
                .OrderBy(x => x.ToString());

        private IReadOnlyCollection<CalculatorSkills> SelectedSkills =>
            GetSkills().Where(s => CurrentPlayer.CanUseSkill(s, CalculatorSkills.None)).ToList();

        private void SkillsChanged(IEnumerable<CalculatorSkills> selected)
        {
            CurrentPlayer.Skills = CalculatorSkills.None;
            foreach (var skill in selected)
                CurrentPlayer.Skills |= skill;
            PlayerChanged();
        }

        private void PlayerChanged()
        {
            OnPlayerChanged.InvokeAsync(CurrentPlayer);
        }

        private void NewPlayer()
        {
            OnNewPlayer.InvokeAsync();
        }
        
        private void ClearSelected()
        {
            CurrentPlayer.Skills = CalculatorSkills.None;
            PlayerChanged();
        }

        private void LonerChanged(int i)
        {
            CurrentPlayer.LonerValue = i;
            PlayerChanged();
        }

        private void MightyBlowChanged(int i)
        {
            CurrentPlayer.MightyBlowValue = i;
            PlayerChanged();
        }

        private void DirtyPlayerChanged(int i)
        {
            CurrentPlayer.DirtyPlayerValue = i;
            PlayerChanged();
        }

        private void BreakTackleChanged(int i)
        {
            CurrentPlayer.BreakTackleValue = i;
            PlayerChanged();
        }

        protected override void OnParametersSet()
        {
            if (Season == "Season 2" && CurrentPlayer.BreakTackleValue > 2)
            {
                CurrentPlayer.BreakTackleValue = 2;
                PlayerChanged();
            }
        }
    }
}
