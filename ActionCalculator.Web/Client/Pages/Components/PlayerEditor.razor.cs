using ActionCalculator.Models;
using ActionCalculator.Utilities;
using Microsoft.AspNetCore.Components;

namespace ActionCalculator.Web.Client.Pages.Components
{
    public partial class PlayerEditor
    {
        [Parameter] 
        public Player CurrentPlayer { get; set; } = null!;

        [Parameter]
        public int PlayerNumber { get; set; }

        [Parameter] 
        public EventCallback<Player> OnPlayerChanged { get; set; }

        [Parameter]
        public EventCallback OnNewPlayer { get; set; }
        
        private IEnumerable<Skills> GetSkills() =>
            typeof(Skills).ToEnumerable<Skills>().Where(x => x is > 0 and < Skills.DivingTackle);
        
        private void ToggleSkill(Skills skill, bool isSelected)
        {
            if (isSelected)
            {
                CurrentPlayer.Skills |= skill;
            }
            else
            {
                CurrentPlayer.Skills &= ~skill;
            }

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
            CurrentPlayer.Skills = Skills.None;
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
    }
}
