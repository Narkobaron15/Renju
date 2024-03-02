namespace RenjuLib.Player;

public class HumanPlayer(
    bool isBlack,
    string name,
    ISession session = null!
) : Player(isBlack, name, session)
{
    /**
     * Await a move from the player.
     */
    public override async Task<Move> MakeMove()
    {
        throw new NotImplementedException();
    }
}
