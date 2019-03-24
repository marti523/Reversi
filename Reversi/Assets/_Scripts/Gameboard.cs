using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour {
       
    private Camera GameCamera;          //Reference to the MainCamera
    public Logic _logic;                //References Logic script
    public GameObject chipPrefab;       //Game chip prefab
    public Vector3 selectedTile;        //Tile that was clicked

    public GameObject[,] chips = new GameObject[8,8]; // array that holds references to the actual chips
    public int[,] owner = new int[8,8]; // array of ownership of space. 1 = player1, 2 = player2, 0 = empty 

    [HideInInspector]
    //Used only for naming the cubes in the inspector when they are created
    public static string[] alphabet = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" }; 

    // Use this for initialization
    void Start()
    {
        GameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _logic = GameManager.instance.gameObject.GetComponent<Logic>();
        DrawBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentTurn == 1)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GetMouseInputs();
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
        GameObject white1 = Instantiate(chipPrefab,new Vector3(4,1,4),transform.rotation);
        white1.transform.Rotate(new Vector3(180, 0, 0));
        chips[4, 4] = white1;
        owner[4, 4] = 1;
        
        GameObject white2 = Instantiate(chipPrefab, new Vector3(3, 1, 3), transform.rotation);
        white2.transform.Rotate(new Vector3(180, 0, 0));
        chips[3, 3] = white2;
        owner[3, 3] = 1;
        
        //black
        GameObject black1 = Instantiate(chipPrefab, new Vector3(4, 1, 3), transform.rotation);
        chips[4, 3] = black1;
        owner[4, 3] = 2;
        
        GameObject black2 = Instantiate(chipPrefab, new Vector3(3, 1, 4), transform.rotation);
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
                Debug.Log("HIT");
                    if (owner[px, py] == 0)
                    {
                    Debug.Log("NO OWNER");
                        if(_logic.checkValid(px,py))
                        {
                        Debug.Log("Valid space");
                            //instantiate the piece at the currect position
                            selectedTile = hit.collider.transform.position;
                            selectedTile.y = 1;
                            GameManager.instance.placePiece(selectedTile);

                            //update the board
                            owner[px, py] = 1;
                            GameManager.instance.totalSpacesLeft--;

                            _logic.findTakenPieces(px,py);
                        }
                    }

                }
            
            }

    }


}
