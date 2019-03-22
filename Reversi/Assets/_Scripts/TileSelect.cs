using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelect : MonoBehaviour {

    private Camera GameCamera;
    private GameManager _GameManager;
    public Vector3 selectedTile;

    // Use this for initialization
    void Start ()
    {
        GameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GetMouseInputs();
        }
    }

    void GetMouseInputs()
    {
        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100))
        {
            if(hit.collider.gameObject.tag == ("Tile"))
            { 
                Debug.Log(hit.collider.transform.position);
                selectedTile = hit.collider.transform.position;
            }
        }

    }
}
