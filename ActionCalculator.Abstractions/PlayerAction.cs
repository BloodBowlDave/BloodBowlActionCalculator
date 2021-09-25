namespace ActionCalculator.Abstractions
{
    public class PlayerAction
    {
	    public PlayerAction(Player player, Action action)
	    {
		    Player = player;
		    Action = action;
	    }

	    public Player Player { get; }
	    public Action Action { get; }
    }
}
