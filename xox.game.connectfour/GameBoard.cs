using System;
using System.Collections.Generic;
using System.Text;

namespace xox.game.connectfour
{
    public class GameBoard
    {
        public const int GameBoardHeight = 6;
        public const int GameBoardWidth = 7;

        public int Moves { get; set; } = 0;

        private PlayerColor[,] gameBoard = new PlayerColor[GameBoardWidth, GameBoardHeight];

        private void Clear()
        {
            Moves = 0;
            for (int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    gameBoard[x, y] = PlayerColor.NoPlayer;
                }
            }
        }

        public byte[] GetBoardPayload()
        {
            StringBuilder bits = new StringBuilder();

            for (int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    switch (gameBoard[x, y])
                    {
                        default:
                        case PlayerColor.NoPlayer:
                            bits.Append("00");
                            break;
                        case PlayerColor.RedPLayer:
                            bits.Append("01");
                            break;
                        case PlayerColor.YellowPlayer:
                            bits.Append("10");
                            break;
                    }
                }
            }

            string bitString = bits.ToString();
            byte[] payload = new byte[bitString.Length / 8];

            for (int i = 0; i < payload.Length; i++)
            {
                for (int b = 0; b < 8; b++)
                {
                    payload[i] |= (byte)((bitString[i * 8 + b] == '1' ? 1 : 0) << (7 - b));
                }
            }

            //TODO: Add checks to confirm 11byte length?
            return payload;
        }

        public void SetBoardFromPayload(byte[] payload)
        {
            //TODO: Confirm length?
            StringBuilder bits = new StringBuilder();
            for(int i = 0; i < payload.Length; i++)
            {
                bits.Append(Convert.ToString(payload[i], 2).PadLeft(8, '0'));
            }

            int bitIndex = 0;
            string bitString = bits.ToString();
            for (int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    string subBits = bitString.Substring(bitIndex, 2);
                    bitIndex += 2;

                    switch(subBits)
                    {
                        default:
                        case "00":
                            gameBoard[x, y] = PlayerColor.NoPlayer;
                            break;
                        case "01":
                            gameBoard[x, y] = PlayerColor.RedPLayer;
                            break;
                        case "10":
                            gameBoard[x, y] = PlayerColor.YellowPlayer;
                            break;
                    }

                }
            }

                    

        }

        public GameBoard Clone()
        {
            GameBoard board = new GameBoard();
            
            for(int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    board.gameBoard[x, y] = gameBoard[x, y];
                }
            }

            return board;
        }

        public void DisplayBoard()
        {
            for (int y = 0; y < GameBoard.GameBoardHeight; y++)
            {
                for (int x = 0; x < GameBoard.GameBoardWidth; x++)
                {
                    var player = gameBoard[x, y];

                    string playerIcon = "X";
                    switch (player)
                    {
                        case PlayerColor.RedPLayer:
                            playerIcon = "R";
                            break;

                        case PlayerColor.YellowPlayer:
                            playerIcon = "Y";
                            break;

                        default:
                        case PlayerColor.NoPlayer:
                            playerIcon = " ";
                            break;
                    }
                    Console.Write($"{playerIcon} ");
                }
                Console.WriteLine();
            }



        }

        public void AddGamePiece(PlayerColor player, int xPosition)
        {
            if (CheckAvailableColumn(xPosition) == false)
                throw new ArgumentException("xPosition", $"Column {xPosition} already populated with a peice from {gameBoard[xPosition, 0]}");

            //set to the bottom position
            int yPosition = GameBoardHeight - 1;

            //we already know the 0 position is blank, check the below piece to see if we find any pieces to place on top of
            for (int y = 1; y < GameBoardHeight; y++)
            {
                if(gameBoard[xPosition, y] != PlayerColor.NoPlayer)
                {
                    //we want the previous position to place our current piece
                    yPosition = y - 1;
                    break;
                }
            }

            Moves++;

            //assign the piece's final position
            gameBoard[xPosition, yPosition] = player;
        }

        public PlayerColor GetPlayerColorAtPosition(int xPosition, int yPosition)
        {
            if (xPosition < 0 || xPosition >= GameBoardWidth)
                throw new ArgumentOutOfRangeException("xPosition", $"Column must be between 0 and {GameBoardWidth}");

            if (yPosition < 0 || yPosition >= GameBoardHeight)
                throw new ArgumentOutOfRangeException("yPosition", $"Column must be between 0 and {GameBoardHeight}");

            return gameBoard[xPosition, yPosition];
        }

        public int[] GetAvailableMoves()
        {
            List<int> availablePositions = new List<int>();
            for (int x = 0; x < GameBoard.GameBoardWidth; x++)
            {
                if (CheckAvailableColumn(x) == true)
                    availablePositions.Add(x);
            }

            return availablePositions.ToArray();
        }

        public bool CheckAvailableColumn(int xPosition)
        {
            //make sure this is a valid position
            if (xPosition < 0 || xPosition >= GameBoardWidth)
                throw new ArgumentOutOfRangeException("xPosition", $"Column must be between 0 and {GameBoardWidth}");

            return gameBoard[xPosition, 0] == PlayerColor.NoPlayer;
        }

        public PlayerColor CheckForWin()
        {
            PlayerColor winner = PlayerColor.NoPlayer;

            for (int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    winner = CheckPositionForWin(x, y);
                    if (winner != PlayerColor.NoPlayer)
                        return winner;
                }
            }
            

            return winner;
        }

        public int GetPlayerScore(PlayerColor color)
        {
            if (color == PlayerColor.NoPlayer)
                return 0;

            //do we care what the enemy color is?
            PlayerColor enemyColor = (color == PlayerColor.RedPLayer) ? PlayerColor.YellowPlayer : PlayerColor.RedPLayer;

            int score = 0;
            for (int x = 0; x < GameBoardWidth; x++)
            {
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    if (gameBoard[x, y] == color)
                    {
                        score++;

                        //could consolidate to a loop, but seems easier to read/debug 
                        //and ensure all directions are covered this way

                        //horizontal right
                        score += CheckForLine(x, 1, y, 0, enemyColor);

                        //horizontal left
                        score += CheckForLine(x, -1, y, 0, enemyColor);

                        //vertical up
                        score += CheckForLine(x, 0, y, -1, enemyColor);

                        //vertical down
                        score += CheckForLine(x, 0, y, 1, enemyColor);

                        //diagonal down right
                        score += CheckForLine(x, 1, y, 1, enemyColor);

                        //diagonal down left
                        score += CheckForLine(x, -1, y, 1, enemyColor);

                        //diagonal up right
                        score += CheckForLine(x, 1, y, -1, enemyColor);

                        //diagonal up left
                        score += CheckForLine(x, -1, y, -1, enemyColor);
                    }
                }
            }

            return score;

        }

        private int CheckForLine(int x, int xdir, int y, int ydir, PlayerColor enemyColor)
        {
            //could maybe consolidate to a loop, but this seems easier to read

            int score = 0;
            int xPosition = x + xdir;
            int yPosition = y + ydir;

            if((xPosition < 0 || xPosition >= GameBoardWidth) || (yPosition < 0 || yPosition >= GameBoardHeight) || 
                    (gameBoard[xPosition, yPosition] != enemyColor))
                return score;

            //we have a piece next to our main piece
            score++;
            PlayerColor originalColor = gameBoard[xPosition, yPosition];

            //move to next position
            xPosition += xdir;
            yPosition += ydir;

            //are we out of bounds or don't match the original color?
            if ((xPosition < 0 || xPosition >= GameBoardWidth) || (yPosition < 0 || yPosition >= GameBoardHeight) ||
                   (gameBoard[xPosition, yPosition] != originalColor))
                return score;

            //second piece in a line
            score += 2;

            //move to next position
            xPosition += xdir;
            yPosition += ydir;

            //are we out of bounds or don't match the original color?
            if ((xPosition < 0 || xPosition >= GameBoardWidth) || (yPosition < 0 || yPosition >= GameBoardHeight) ||
                   (gameBoard[xPosition, yPosition] != originalColor))
                return score;

            //third piece in a line
            score += 4;

            return score;

        }

        private PlayerColor CheckPositionForWin(int xPosition, int yPosition)
        {
            if (xPosition < 0 || xPosition >= GameBoardWidth)
                throw new ArgumentOutOfRangeException("xPosition", $"Column must be between 0 and {GameBoardWidth}");

            if (yPosition < 0 || yPosition >= GameBoardHeight)
                throw new ArgumentOutOfRangeException("yPosition", $"Column must be between 0 and {GameBoardHeight}");

            if (gameBoard[xPosition, yPosition] == PlayerColor.NoPlayer)
                return PlayerColor.NoPlayer;

            PlayerColor potentialWinner = gameBoard[xPosition, yPosition];

            //check right
            if (xPosition < GameBoardWidth - 3 &&
                gameBoard[xPosition + 1, yPosition] == potentialWinner &&
                gameBoard[xPosition + 2, yPosition] == potentialWinner &&
                gameBoard[xPosition + 3, yPosition] == potentialWinner)
            {
                return potentialWinner;
            }

            //check left
            if (xPosition >= 3 &&
                gameBoard[xPosition - 1, yPosition] == potentialWinner &&
                gameBoard[xPosition - 2, yPosition] == potentialWinner &&
                gameBoard[xPosition - 3, yPosition] == potentialWinner)
            {
                return potentialWinner;
            }

            //check down
            if (yPosition < GameBoardHeight - 3 &&
                gameBoard[xPosition, yPosition + 1] == potentialWinner &&
                gameBoard[xPosition, yPosition + 2] == potentialWinner &&
                gameBoard[xPosition, yPosition + 3] == potentialWinner)
            {
                return potentialWinner;
            }

            //check up
            if (yPosition >= 3 &&
                gameBoard[xPosition, yPosition - 1] == potentialWinner &&
                gameBoard[xPosition, yPosition - 2] == potentialWinner &&
                gameBoard[xPosition, yPosition - 3] == potentialWinner)
            {
                return potentialWinner;
            }

            //check lower right
            if (xPosition < GameBoardWidth - 3 && yPosition < GameBoardHeight - 3 &&
                gameBoard[xPosition + 1, yPosition + 1] == potentialWinner &&
                gameBoard[xPosition + 2, yPosition + 2] == potentialWinner &&
                gameBoard[xPosition + 3, yPosition + 3] == potentialWinner)
            {
                return potentialWinner;
            }

            //check lower left
            if (xPosition >= 3 && yPosition < GameBoardHeight - 3 &&
                gameBoard[xPosition - 1, yPosition + 1] == potentialWinner &&
                gameBoard[xPosition - 2, yPosition + 2] == potentialWinner &&
                gameBoard[xPosition - 3, yPosition + 3] == potentialWinner)
            {
                return potentialWinner;
            }

            //check upper right
            if (xPosition < GameBoardWidth - 3 && yPosition >= 3 &&
                gameBoard[xPosition + 1, yPosition - 1] == potentialWinner &&
                gameBoard[xPosition + 2, yPosition - 2] == potentialWinner &&
                gameBoard[xPosition + 3, yPosition - 3] == potentialWinner)
            {
                return potentialWinner;
            }

            //check upper left
            if (xPosition >= 3 && yPosition >= 3 &&
                gameBoard[xPosition - 1, yPosition - 1] == potentialWinner &&
                gameBoard[xPosition - 2, yPosition - 2] == potentialWinner &&
                gameBoard[xPosition - 3, yPosition - 3] == potentialWinner)
            {
                return potentialWinner;
            }
            return PlayerColor.NoPlayer;
        }

    }
}
