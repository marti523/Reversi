using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gameboard : MonoBehaviour {
       
    private Camera GameCamera;          //Reference to the MainCamera
    public GameLogicV2 _logic;          //References Logic script
    public GameObject chipPrefab;       //Game chip prefab
    public Vector3 selectedTile;        //Tile that was clicked

    public Text notification;

    public GameObject[,] chips = new GameObject[8,8]; // array that holds references to the actual chips
    public int[,] owner = new int[8,8]; // array of ownership of space. 1 = player1, 2 = player2, 0 = empty 

 
    // Use this for initialization
    void Start()
    {
        GameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _logic = GameManager.instance.gameObject.GetComponent<GameLogicV2>();
        DrawBoard();
    }

    void Awake()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (_logic.gameOver)
        {
            int[] scores = _logic.countScore(owner, false);
            if(scores[0] > scores[1])
            {
                notification.text = "Player 1 Wins!";
            }
            else if(scores[0]<scores[1])
            {
                notification.text = "AI Wins!";
            }
            else if(scores[0] == scores[1])
            {
                notification.text = "DRAW!";
            }
            return;
        }
        else
        { 
            if(_logic.remainingMoves == 0)
            {
                _logic.gameOver = true;
                return;
            }
            if (_logic.movesAvailable() == false)
            {
                int[] scores = _logic.countScore(owner, false);
                if (scores[0] * scores[1] == 0)
                    _logic.gameOver = true;
                else
                {
                    string currPlayer = _logic.playerTurn ? "PLAYER 1" : "AI";
                    notification.text = currPlayer + " had no moves available!";
                    _logic.playerTurn = !_logic.playerTurn;
                }
                return;
            }
            if (_logic.playerTurn)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    GetMouseInputs();
                    _logic.updateScore();
                }
            }
            else
            {
                _logic.AITurn();
                _logic.updateScore();
            }
        }
    }

    /* DrawBoard()
    The purpose of this function is to draw the beginning format of the board.
    4 pieces in the middle, 2 white and 2 black in alternating pattern.
    This function also sets all spaces to have No owner.
    */
    public void DrawBoard()
    {
        //Instantiate the 4 middle tiles
        //white
        GameObject white1 = Instantiate(chipPrefab,new Vector3(4,(float).52,4),transform.rotation);
        white1.transform.Rotate(new Vector3(180, 0, 0));
        chips[4, 4] = white1;
        owner[4, 4] = 1;
        
        GameObject white2 = Instantiate(chipPrefab, new Vector3(3, (float).52, 3), transform.rotation);
        white2.transform.Rotate(new Vector3(180, 0, 0));
        chips[3, 3] = white2;
        owner[3, 3] = 1;
        
        //black
        GameObject black1 = Instantiate(chipPrefab, new Vector3(4, (float).52, 3), transform.rotation);
        chips[4, 3] = black1;
        owner[4, 3] = 2;
        
        GameObject black2 = Instantiate(chipPrefab, new Vector3(3, (float).52, 4), transform.rotation);
        chips[3, 4] = black2;
        owner[3, 4] = 2;
    }

    /* GetMouseInputs()
     *The purpose of this function is to check if a space on the board is empty when it is clicked.
     *If the tile is empty, the ownership is changed, and the placePiece function in the GameManager 
     *is called to place a piece. 
     */

    void GetMouseInputs()
    {
        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int px; //holds the x value for owner[x,y]
        int py; //holds the y value for owner[x,y]

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag == ("Tile"))
                {
                    px = (int)hit.collider.transform.position.x;
                    py = (int)hit.collider.transform.position.z;
                    Debug.Log("HIT at (" +px+","+py+")");
                   
                    if (_logic.isMove(px,py, owner))
                    {
                        Debug.Log("Valid space");
                        //instantiate the piece at the currect position
                        selectedTile = hit.collider.transform.position;
                        selectedTile.y = .52F;
                        GameObject newPiece = Instantiate(chipPrefab, selectedTile, transform.rotation);
                        newPiece.transform.Rotate(180,0,0);
                    
                        //update the board
                        chips[px, py] = newPiece;
                        owner[px, py] = 1;
                        _logic.remainingMoves--;

                        //Take enemy pieces
                        _logic.findTaken(px, py, owner, true);
                        _logic.playerTurn = false;
                    }                    
                }
            
            }

    }
}
