using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour {

    public Gameboard _gameboard;

    public int tx;
    public int ty;
    private static int[] takeCount = new int[8]; //keeps track of the number of pieces taken in each of the directions

    // Use this for initialization
    void Start()
    {
        _gameboard = GameManager.instance.gameObject.GetComponent<Gameboard>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool checkValid(int x, int y)
    {
        Debug.Log("checkValid Called");
        if (_gameboard.owner[x, y] != 0)
        {
            Debug.Log("owner is not empty");
            return false;
        }

        bool valid = false;
        checkTaken(x, y);

        for (int i = 0; i < takeCount.Length; i++)
        {
            valid |= takeCount[i] > 0;
        }

        return valid;
    }

    //This function checks each direction to see if there is any pieces taken in a certain direction
    //stores the number of pieces that can be taken in takeCount[]
    void checkTaken(int x, int y)
    {
        takeCount = new int[8];
        int count = 0;

        if (countTaken(x, y, -1, -1, ref count))//down left diag
        {
            takeCount[0] = count;
        }
        count = 0;
        if (countTaken(x, y, 0, -1, ref count))//left
        {
            takeCount[1] = count;
        }
        count = 0;
        if (countTaken(x, y, 1, -1, ref count))//up left diag
        {
            takeCount[2] = count;
        }
        count = 0;
        if (countTaken(x, y, 1, 0, ref count))//up
        {
            takeCount[3] = count;
        }
        count = 0;
        if (countTaken(x, y, 1, 1, ref count))//up right diag
        {
            takeCount[4] = count;
        }
        count = 0;
        if (countTaken(x, y, 0, 1, ref count))//right
        {
            takeCount[5] = count;
        }
        count = 0;
        if (countTaken(x, y, -1, 1, ref count))//down right diag
        {
            takeCount[6] = count;
        }
        count = 0;
        if (countTaken(x, y, -1, 0, ref count))//down
        {
            takeCount[7] = count;
        }
    }

    //This function returns the number of pieces you could take in a direction
    bool countTaken(int x, int y, int xOffset, int yOffset, ref int count)
    {
        int currX = x + xOffset;
        int currY = y + xOffset;
        if (currX > 7 || currX < 0 || currY > 7 || currY < 0) //check if the next space is in bounds
        {
            return false;
        }
        if (_gameboard.owner[currX, currY] != 0)
        {
            if (checkOwner(currX, currY))
                return count > 0;
            else
            {
                count++;
                return countTaken(currX, currY, xOffset, yOffset, ref count);
            }
        }
        else
        {
            return false;
        }
    }
    //checks if the current player is the owner of the current tile
    private bool checkOwner(int x, int y)
    {
        if (GameManager.instance.currentTurn == 1 && _gameboard.owner[x, y] == 1)
        {
            return true;
        }
        if (GameManager.instance.currentTurn == 2 && _gameboard.owner[x, y] == 2)
        {
            return true;
        }
        return false;
    }

    public void findTakenPieces(int x, int y)
    {
        if(takeCount[0] > 0)
        {
            takePiece(x,y,-1,-1);
        }
        if (takeCount[1] > 0)
        {
            takePiece(x, y, 0, -1);
        }
        if (takeCount[2] > 0)
        {
            takePiece(x, y, 1, -1);
        }
        if (takeCount[3] > 0)
        {
            takePiece(x, y, 1, 0);
        }
        if (takeCount[4] > 0)
        {
            takePiece(x, y, 1, 1);
        }
        if (takeCount[5] > 0)
        {
            takePiece(x, y, 0, 1);
        }
        if (takeCount[6] > 0)
        {
            takePiece(x, y, -1, 1);
        }
        if (takeCount[7] > 0)
        {
            takePiece(x, y, -1, 0);
        }
    }

    void takePiece(int x, int y, int xOffset, int yOffset)
    {
        int currX = x + xOffset;
        int currY = y + yOffset;

        if(checkOwner(currX,currY))
        {
            return;
        }
        else
        {
            if(GameManager.instance.currentTurn == 1)
            {
                _gameboard.owner[currX, currY] = 1;
            }
            if(GameManager.instance.currentTurn == 2)
            {
                _gameboard.owner[currX, currY] = 2;
            }
            takePiece(currX,currY,xOffset,yOffset);
        }
    }

}
