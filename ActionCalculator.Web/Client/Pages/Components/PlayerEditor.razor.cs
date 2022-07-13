using ActionCalculator.Models;
using ActionCalculator.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ActionCalculator.Web.Client.Pages.Components
{
    public partial class PlayerEditor
    {
        private int _lonerValue = 4;
        private int _mightyBlowValue = 1;
        private int _dirtyPlayerValue = 1;
        private int _breakTackleValue = 1;
        
        private MudChip[] _selected;

        [Parameter] 
        public Player CurrentPlayer { get; set; } = null!;

        [Parameter]
        public int PlayerNumber { get; set; }

        [Parameter] 
        public EventCallback<Player> OnPlayerChanged { get; set; }

        public PlayerEditor()
        {
            _selected = Array.Empty<MudChip>();
        }

        private IEnumerable<Skills> GetSkills() =>
            typeof(Skills).ToEnumerable<Skills>().Where(x => x is > 0 and < Skills.DivingTackle);

        private bool SkillIsSelected(string skill) => _selected.Any(x => x.Text == skill);

        private void PlayerChanged()
        {
            var skills = _selected.Aggregate(Skills.None, (current, chip) => 
                current | Enum.Parse<Skills>(chip.Text.Replace(" ", "")));

            CurrentPlayer = new Player(CurrentPlayer.Id, skills, _lonerValue, _breakTackleValue, _mightyBlowValue, _dirtyPlayerValue, 0);
            OnPlayerChanged.InvokeAsync(CurrentPlayer);
        }

        private void NewPlayer()
        {
            CurrentPlayer = new Player();
            ClearSelected();
        }
        
        private void ClearSelected()
        {
            _selected = Array.Empty<MudChip>();
            PlayerChanged();
        }

        private void LonerChanged(int i)
        {
            _lonerValue = i;
            PlayerChanged();
        }

        private void MightyBlowChanged(int i)
        {
            _mightyBlowValue = i;
            PlayerChanged();
        }

        private void DirtyPlayerChanged(int i)
        {
            _dirtyPlayerValue = i;
            PlayerChanged();
        }

        private void BreakTackleChanged(int i)
        {
            _breakTackleValue = i;
            PlayerChanged();
        }
    }
}
