using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicV2 : MonoBehaviour {

    // Use this for initialization
    public Gameboard _gameboard;

    int[] takenCount = new int[8];

    public GameObject chip;            //prefab for the chip 
    public bool playerTurn = true;     //if true, then it's the player's turn
    public int remainingMoves = 60;           //the number of remaining moves before board fills up



    void Start ()
    {
        _gameboard = GameManager.instance.gameObject.GetComponent<Gameboard>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public bool isMove(int x, int y, int[,] tempBoard)
    {
        if(tempBoard[x,y] != 0)
        {
            return false;
        }
        checkTaken(x,y,tempBoard);
        return checkValid();
    }

    bool checkValid()
    {
        bool valid = false;

        for(int i = 0; i < takenCount.Length;i++)
        {
            valid |= takenCount[i] > 0;
        }
        return valid;
    }
    //This function checks if a direction around the piece has enemy pieces that can be taken.
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
    //This function counts the number of pieces that can be taken in a certain direction
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
                int rotate = playerTurn ? 180 : 90;
                _gameboard.chips[currX, currY].transform.Rotate(180,0,0);
            }

            tempBoard[currX, currY] = playerTurn ? 1 : 2;
            takePiece(currX,currY,xOffset,yOffset,tempBoard,changeBoard);
        }

        return;
    }

    //checks the owner of the current space
    private bool checkOwner(int x, int y, int[,] tempBoard)
    {
        return playerTurn ? tempBoard[x, y] == 1 : tempBoard[x, y] == 2;
    }
}
