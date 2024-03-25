namespace RenjuLib.Round;

/**
 * <summary>
 * <b>A game round.</b> <br/>
 *
 * A game round is a game between two players. <br/>
 * It contains board data and the result of the game.
 * </summary>
 */
public class GameRound(
    IPlayer blackPlayer,
    IPlayer whitePlayer
)
{
    /**
     * <summary>
     * <b>The board of the game.</b> <br/>
     *
     * It contains the moves of the players.
     * </summary>
     */
    public RenjuBoard RenjuBoard { get; } = new();

    // TODO: Add event for error handling
    // public event EventHandler<Exception> OnError;

    // TODO: Add event for end of the game
    // public event EventHandler<GameResult> OnEnd;

    // TODO: Add collection of moves for replay, analysis and logging
    // public IList<Move> Moves { get; } = [];

    /**
     * <summary>
     * <b>The result of the game.</b> <br/>
     *
     * It can be OnGoing, BlackWon, WhiteWon, or Draw.
     * </summary>
     */
    public GameResult Result { get; private set; }

    /**
     * <summary>
     * <b>Checks for a win of player of certain color,
     * considering the last move.</b><br/>
     * Checks in all directions (rows, columns, diagonals).
     * </summary>
     * <param name="stoneColor">The color of the player</param>
     * <param name="move">The last move</param>
     */
    private bool IsWin(CellStone stoneColor, Move move)
        => Direction.AllDirections.Any(
            direction => HasFiveInARow(move.X, move.Y, direction, stoneColor)
        );

    /**
     * <summary>
     * Checks for a win of player in a certain direction
     * </summary>
     * <param name="x">The x coordinate of the move</param>
     * <param name="y">The y coordinate of the move</param>
     * <param name="direction">The direction to check</param>
     * <param name="stoneToCheck">The stone to check</param>
     */
    private bool HasFiveInARow(
        int x, int y,
        Direction direction,
        CellStone stoneToCheck
    )
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (RenjuBoard.IsWithinBounds(x, y) &&
                RenjuBoard[x, y].Stone == stoneToCheck)
                count++;
            else break;

            (x, y) = direction.MoveNext(x, y);
        }

        return count >= 5;
    }

    /**
     * <summary>
     * <b>Checks if the game is a draw.</b> <br/>
     * (Checks if the board is full)
     * </summary>
     */
    private bool IsDraw()
    {
        // Check if the board is full
        for (int i = 0; i < RenjuBoard.BoardSize; i++)
        for (int j = 0; j < RenjuBoard.BoardSize; j++)
            if (RenjuBoard.IsCellEmpty(i, j))
                // Board is not full, so it's not a draw
                return false;

        // Board is full, so it's a draw
        return true;
    }

    /**
     * <summary>
     * <b>Makes a turn for a player.</b> <br/>
     * It adds the move to the board and checks for win or draw.
     * </summary>
     * <param name="player">The player to make the turn</param>
     */
    private async Task<GameResult> MakeTurn(IPlayer player)
    {
        try
        {
            var move = await player.MakeMove();
            RenjuBoard.AddMove(move);

            if (IsWin(player.Color, move))
                return player.IsBlack ? GameResult.BlackWon : GameResult.WhiteWon;
            // else
            return IsDraw() ? GameResult.Draw : GameResult.OnGoing;
        }
        catch (ArgumentException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            throw new InvalidOperationException("Invalid move");
        }
        catch
        {
            // On error, show the error and ask for another move
            // OnError?.Invoke(player, e);
            return await MakeTurn(player);
        }
    }

    /**
     * <summary>
     * <b>Plays the game.</b> <br/>
     * Provides logic of turns of the players until the game ends.
     * </summary>
     */
    public async Task<GameResult> Play()
    {
        bool isBlack = true;
        while (true)
        {
            var result = await MakeTurn(isBlack ? blackPlayer : whitePlayer);
            if (result != GameResult.OnGoing)
            {
                Result = result;
                return result;
            }

            isBlack = !isBlack;
        }
    }
}
