using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    //public static Logic _logic;

    public int currentTurn = 0; // Player1(White) = 1, Player2(Black) = 2, Nobody = 0
    public int totalSpacesLeft = 64;
    public int player1Score = 0;
    public int player2Score = 0;

    public GameObject playerPrefab;   //White Piece
    public GameObject opponentPrefab; //Black Piece
    private Vector3 _selectedTile;    //The vector where the piece should be placed
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        currentTurn = 1;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void placePiece(Vector3 _position)
    { 
        if(currentTurn == 1)
        {
            Instantiate(playerPrefab, _position, Quaternion.identity);
            player1Score++;///TEMPORARY
            currentTurn = 2;
        }
        else if(currentTurn == 2)
        {
            Instantiate(opponentPrefab, _position, Quaternion.identity);
            currentTurn = 1;
        }
    }
}
