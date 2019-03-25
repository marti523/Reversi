using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameLogicV2 : MonoBehaviour {

    // Use this for initialization
    public Gameboard _gameboard;

    int[] takenCount = new int[8];     //stores the number of chips taken for each direction

    public GameObject chipPrefab;      //prefab for the chip 
    public bool playerTurn = true;     //if true, then it's the player's turn
    public int remainingMoves = 60;    //the number of remaining moves before board fills up
    //public int newdepth = 5;                  //curr depth
    private int difficulty = 4;        //the max depth of the search
    public bool gameOver = false;

    public Text playerScore;
    public Text AIScore;


    void Start()
    {
        _gameboard = GameManager.instance.gameObject.GetComponent<Gameboard>();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    //This function will return true if the tile clicked, is a valid location to put a chip
    public bool isMove(int x, int y, int[,] tempBoard)
    {
        if (tempBoard[x, y] != 0)
        {
            return false;
        }
        checkTaken(x, y, tempBoard);
        return checkValid();
    }

    //This function will return true if the selected play has pieces that it can take.
    bool checkValid()
    {
        bool valid = false;

        for (int i = 0; i < takenCount.Length; i++)
        {
            valid |= takenCount[i] > 0;
        }
        return valid;
    }
    //check if there is any move available for a player
    public bool movesAvailable()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(isMove(i,j,_gameboard.owner))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AITurn()
    {
        int[] AIMove = new int[2] { -1, -1 };//placeholder for AI's move choice
        miniMax(_gameboard.owner, difficulty, ref AIMove);
        playerTurn = false;
        if(AIMove[0] >= 0 && AIMove[1] >= 0)//make sure the move is not the placeholder
        {
            int x = AIMove[0];
            int y = AIMove[1];
            checkTaken(x,y,_gameboard.owner);
            GameObject newPiece = Instantiate(chipPrefab, new Vector3(x, .52F, y), transform.rotation);
            _gameboard.chips[x, y] = newPiece;
            _gameboard.owner[x, y] = 2;
            remainingMoves--;
            findTaken(x, y, _gameboard.owner, true);
        }
        playerTurn = true;
    }

    private int miniMax(int[,] tempBoard, int depth, ref int[] bestMove)
    {
        double bestScore = double.NegativeInfinity;

        if (depth == 0)
        {
            int[] scores = countScore(tempBoard, true);
            int adv = scores[0] - scores[1];
            return adv * (playerTurn ? -1 : 1);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (isMove(i, j, tempBoard))
                {
                    int[,] testBoard = (int[,])tempBoard.Clone(); //Do a move on a testBoard in order to find best move 
                    testBoard[i, j] = playerTurn ? 1 : 2;
                    findTaken(i, j, testBoard, false);

                    int[] currBestMove = new int[2];              //hold the current best move

                    playerTurn = !playerTurn;
                    int score = -miniMax(testBoard, depth - 1, ref currBestMove);

                    if (score > bestScore)
                    {
                        bestScore = score;                       //if the found score is better than previous this is the new best move.
                        bestMove = new int[2] { i, j };
                    }
                }
            }
        }
        return (int)bestScore;
    }

    public void updateScore()
    {
        int[] currentScores = countScore(_gameboard.owner, false);
        playerScore.text = "Player Score: " + currentScores[0];
        AIScore.text = "AI Score: " + currentScores[1];
    }

    //This function checks if the current spot is a corner tile
    private bool checkCorner(int x, int y)
    {
        if ((x == 0 && y == 0) || (x == 0 && y == 7) ||
           (x == 7 && y == 0) || (x == 7 && y == 7))
            return true;
        
        else
            return false; 
    }

    //This function checks if the current spot is a side tile
    private bool checkSide(int x, int y)
    {
        if (x == 7 || x == 0 || y == 0 || y == 7)
            return true;
        else
            return false;
    }

    //give weight in the form of score to corner pieces and side pieces if the AI is searching for a move.
    //otherwise the bias is just 1 to count the actual score
    public int[] countScore(int[,] tempBoard, bool AItest)
    {
        int currPlayerScore = 0;
        int currAIScore = 0;

        for (int i = 0; i<8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (tempBoard[i, j] == 1)
                {
                    int bias = AItest ? (checkCorner(i,j) ? 7 : checkSide(i,j) ? 3 : 1) : 1;
                    currPlayerScore += bias;
                }
                else if(tempBoard[i,j] == 2)
                {
                    int bias = AItest ? (checkCorner(i, j) ? 7 : checkSide(i, j) ? 3 : 1) : 1;
                    currAIScore += bias;
                }

            }
        }
        return new int[2] { currPlayerScore,currAIScore};
    }

    //This function checks if a direction around the chip has enemy chips that can be taken.
    void checkTaken(int x, int y, int[,] tempBoard)
    {
        takenCount = new int[8];
        int count = 0;

        if(countTaken(x,y,1,-1,ref count, tempBoard))//check diagonal down-left, offset (+1,-1)
        {
            takenCount[0] = count;
        }
        count = 0;
        if (countTaken(x, y, 0, -1, ref count, tempBoard))//check left, offset(0,-1)
        {
            takenCount[1] = count;
        }
        count = 0;
        if (countTaken(x, y, -1, -1, ref count, tempBoard))//check diagonal up-left, offset(-1,-1)
        {
            takenCount[2] = count;
        }
        count = 0;
        if (countTaken(x, y, -1, 0, ref count, tempBoard))//check up, offset(-1,0)
        {
            takenCount[3] = count;
        }
        count = 0;
        if (countTaken(x, y, -1, 1, ref count, tempBoard))//check diagonal up-right, offset(-1,+1)
        {
            takenCount[4] = count;
        }
        count = 0;
        if (countTaken(x, y, 0, 1, ref count, tempBoard))//check right, offset(0,+1)
        {
            takenCount[5] = count;
        }
        count = 0;
        if (countTaken(x, y, 1, 1, ref count, tempBoard))//check diagonal down-right, offset(+1,+1)
        {
            takenCount[6] = count;
        }
        count = 0;
        if (countTaken(x, y, 1, 0, ref count, tempBoard))//check diagonal down, offset(+1,0)
        {
            takenCount[7] = count;
        }
        count = 0;
    }
    //This function counts the number of chips that can be taken in a certain direction
    bool countTaken(int x,int y, int xOffset, int yOffset, ref int count, int[,] tempBoard)
    {
        int currX = x + xOffset;
        int currY = y + yOffset;

        if(currX > 7 || currX < 0 ||
           currY > 7 || currY < 0)
        {
            return false;
        }

        if (tempBoard[currX, currY] != 0)
        {
            if (checkOwner(currX,currY,tempBoard))
            {
                return count > 0;
            }
            else
            {
                count++;
                return countTaken(currX, currY, xOffset, yOffset, ref count, tempBoard);
            }
        }
        else return false;
    }

    //This function checks the takenCount array to see if any chips were taken
    //for each direction.
    public void findTaken(int x, int y, int[,] tempBoard, bool changeBoard)
    {
        if(takenCount[0] > 0) //diagonal down-left
        {
            takePiece(x, y, 1, -1, tempBoard, changeBoard);
        }
        if (takenCount[1] > 0)//left
        {
            takePiece(x, y, 0, -1, tempBoard, changeBoard);
        }
        if (takenCount[2] > 0)//diagonal up-left
        {
            takePiece(x, y, -1, -1, tempBoard, changeBoard);
        }
        if (takenCount[3] > 0)//up
        {
            takePiece(x, y, -1, 0, tempBoard, changeBoard);
        }
        if (takenCount[4] > 0)//diagonal up-right
        {
            takePiece(x, y, -1, 1, tempBoard, changeBoard);
        }
        if (takenCount[5] > 0)//right
        {
            takePiece(x, y, 0, 1, tempBoard, changeBoard);
        }
        if (takenCount[6] > 0)//diagonal down-right
        {
            takePiece(x, y, 1, 1, tempBoard, changeBoard);
        }
        if (takenCount[7] > 0)//down
        {
            takePiece(x, y, 1, 0, tempBoard, changeBoard);
        }
    }

    //This function changes the chips that were taken for a certain direction.
    void takePiece(int x, int y, int xOffset, int yOffset, int[,] tempBoard, bool changeBoard)
    {
        int currX = x + xOffset;
        int currY = y + yOffset;

        if (checkOwner(currX, currY, tempBoard))
        {
            return;
        }
        else
        {
            if(changeBoard)
            {
                //int rotate = 0;
                if (playerTurn)
                {
                    _gameboard.chips[currX, currY].transform.Rotate(180, 0, 0);
                }
                else if (!playerTurn)
                {
                    //Debug.Log("AI FLIP");
                    //Debug.Log("Current chip at : (" + currX + "," + currY + ")");
                    _gameboard.chips[currX, currY].transform.Rotate(-180, 0, 0);
                }
                //Debug.Log("Current Rotate: "+rotate);
              //  _gameboard.chips[currX, currY].transform.Rotate(rotate,0,0);
            }

            tempBoard[currX, currY] = playerTurn ? 1 : 2;
            takePiece(currX,currY,xOffset,yOffset,tempBoard,changeBoard);
        }

    }

    //checks the owner of the current space
    private bool checkOwner(int x, int y, int[,] tempBoard)
    {
        return playerTurn ? tempBoard[x, y] == 1 : tempBoard[x, y] == 2;
    }
}
