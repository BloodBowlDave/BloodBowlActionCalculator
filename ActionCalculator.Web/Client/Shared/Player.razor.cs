using MudBlazor;

namespace ActionCalculator.Web.Client.Shared
{
    public partial class Player
    {
        public Player()
        {
            _selected = Array.Empty<MudChip>();
            _lonerValue = 4;
            _breakTackleValue = 1;
            _dirtyPlayerValue = 1;
            _mightyBlowValue = 1;
            _playerId = Guid.NewGuid();
        }
    }
}
