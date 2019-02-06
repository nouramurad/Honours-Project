using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileGenerator : MonoBehaviour {

    public GameObject HexagonObject;
    public Material material;

    public int mapWidth = 20;
    public int mapHight = 18;

    //used to ditect hits and collision with mouse clicks
    public RaycastHit hit;
    public Ray ray;
    public Camera cam;
    public Vector3 screenpoint;
    public Vector3 offset;

    // time calculation
    float startTime, timeTaken, timeCounter;

    //for shifting the tiles 
    float tileIOffset = 35.3f;
    float tileJOffset = 30.3f;
	// Use this for initialization
	void Start () {
        startTime = Time.realtimeSinceStartup;
        GenerateHexMap();
        Update();

    }
	
	// Update is called once per frame
	void GenerateHexMap() {

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHight; j++)
            {
                //instantiate the hex object
                GameObject HexTile = Instantiate(HexagonObject);
                //name the tile
                HexTile.name = "tile (" + i.ToString() + ", " + j.ToString() + " )";
                //apply texture
                HexTile.transform.GetComponentInChildren<Renderer>().material.mainTexture = material.GetTexture("_MainTex");
                //position the hex using the offsets
                if (j % 2 == 0)
                {
                    HexTile.transform.position = new Vector3(i * tileIOffset, 0, j * tileJOffset);
                }
                else
                {
                    HexTile.transform.position = new Vector3(i * tileIOffset + tileIOffset / 2, 0, j * tileJOffset);
                }
                //add texture to the hex depending on the position on the map
                if (i > mapWidth / 9 && i <= (mapWidth / 4) || i >= (mapWidth / 4) + (mapWidth / 2) && i < (mapWidth - mapWidth / 9))
                {
                    if (j > mapHight / 9 && j <= (mapHight / 4) || j >= (mapHight / 4) + (mapHight / 2) && j < (mapHight - mapHight / 9))
                    {
                        HexTile.transform.GetComponentInChildren<Renderer>().material.mainTexture = material.GetTexture("_ThirdTex");
                    }
                }
                else if (i > mapWidth / 4 && i < (mapWidth / 4) + (mapWidth / 2))
                {
                    if (j > mapHight / 4 && j < (mapHight / 4) + (mapHight / 2))
                    {
                        HexTile.transform.GetComponentInChildren<Renderer>().material.mainTexture = material.GetTexture("_SecondTex");
                    }
                }
                
            }
        }
        //calculate the time of generation
        timeTaken = Time.time - startTime;
        Debug.Log("Time to generate map: " + timeTaken);
 
    }
    //calculating the cameras screenpoint and offset
    private void OnMouseDown()
    {
        screenpoint = cam.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenpoint.z));

    }
    //drag the mesh with the mouse
    private void OnMouseDrag()
    {
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenpoint.z);
        Vector3 cursorPosition = cam.ScreenToWorldPoint(cursorScreenPoint) + offset;
        transform.position = cursorPosition;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        ray.origin = cam.transform.position;

        //check if there is a collision/hit
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, 100000))
            {
                Debug.Log("hit");
            }
            else
            {
                Debug.Log("not");
            }
        }
        //debugging for time and updates
        timeTaken = Time.time - startTime;
        timeCounter++;
        if(timeCounter == 100)
        {
            Debug.Log("Time after 100 updates: " + timeTaken);
            timeCounter = 0;
            timeTaken = 0;
        }
      

    }
}