using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.game.connectfour
{
    public class Engine
    {
        public GameBoard Board { get; private set; }

        public PlayerColor Play(Player redPlayer, Player yellowPlayer) 
        {
            //set up a new game board for every play
            this.Board = new GameBoard();

            //set the propper colors
            redPlayer.PlayerColor = PlayerColor.RedPLayer;
            yellowPlayer.PlayerColor = PlayerColor.YellowPlayer;

            int totalMoves = GameBoard.GameBoardHeight * GameBoard.GameBoardWidth;
            for(int i = 1; i <= totalMoves; i++)
            {
                Player currentPlayer = (i % 2 == 0) ? yellowPlayer : redPlayer;
                if (ExecuteTurn(this.Board, currentPlayer) == false)
                    return PlayerColor.NoPlayer; //shouldn't happen

                PlayerColor potentialWinner = this.Board.CheckForWin();
                if (potentialWinner != PlayerColor.NoPlayer)
                    return potentialWinner;
            }

            //no more moves left and no one wins
            return PlayerColor.NoPlayer;
        }

        public bool ExecuteTurn(GameBoard gameBoard, Player player) 
        {
            int x = player.GetNextMove(gameBoard);
            if (x == -1)
                return false;

            player.Moves++;
            gameBoard.AddGamePiece(player.PlayerColor, x);

            return true;
        }
    }
}
