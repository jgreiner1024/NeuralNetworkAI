using System;
using System.Collections.Generic;
using System.Text;

namespace xox.game.connectfour
{
    public abstract class Player
    {
        public PlayerColor PlayerColor { get; set; }
        public int Moves { get; set; } = 0;

        public Player() { }
        public Player(PlayerColor color)
        {
            PlayerColor = color;
        }

        public abstract int GetNextMove(GameBoard gameBoard);

    }
}
