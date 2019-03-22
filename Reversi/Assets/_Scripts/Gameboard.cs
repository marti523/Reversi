using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour {

    public enum tileOwner
    {
        PLAYER_ONE,
        PLAYER_TWO,
        NO_OWNER
    }

    public GameObject boardPrefab;
    public GameObject playerPrefab;
    public GameObject opponentPrefab;

    public GameObject[,] cubes = new GameObject[8,8]; // array of cubes
    private tileOwner[,] owner = new tileOwner[8,8]; // array of ownership of cubes


    // Use this for initialization
    void Start()
    {
        DrawBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [HideInInspector]
    public static string[] alphabet = new string[] {"a","b","c","d","e","f","g","h"};


    public void DrawBoard()
    {
        //Draw board at beginning of game and set all tiles to NO_OWNER
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cubes[i,j] = Instantiate (boardPrefab, new Vector3 (i,0,j), Quaternion.identity);
                cubes[i,j].transform.SetParent(gameObject.transform);
                cubes[i, j].name = alphabet[i] + (j + 1);
                owner[i, j] = tileOwner.NO_OWNER;
            }
        }

        //Instantiate the 4 middle tiles
        //white
        Instantiate(playerPrefab,new Vector3(4,1,4),Quaternion.identity);
        owner[4, 4] = tileOwner.PLAYER_ONE;
        Instantiate(playerPrefab, new Vector3(3, 1, 3), Quaternion.identity);
        owner[3, 3] = tileOwner.PLAYER_ONE;
        //black
        Instantiate(opponentPrefab, new Vector3(4, 1, 3), Quaternion.identity);
        owner[4, 3] = tileOwner.PLAYER_TWO;
        Instantiate(opponentPrefab, new Vector3(3, 1, 4), Quaternion.identity);
        owner[3, 4] = tileOwner.PLAYER_TWO;
    }

}
