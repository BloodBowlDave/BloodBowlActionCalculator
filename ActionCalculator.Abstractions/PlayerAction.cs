namespace ActionCalculator.Abstractions
{
    public class PlayerAction
    {
	    public PlayerAction(Player player, Action action, int index)
	    {
		    Player = player;
		    Action = action;
            Index = index;
        }

	    public Player Player { get; }
	    public Action Action { get; }
	    public int Index { get; }
    }
}
