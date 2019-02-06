using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControls : MonoBehaviour {

    
    public RaycastHit hit;
    public Ray ray;
    public Camera cam;
    public Vector3 screenpoint;
    public Vector3 offset;

    float startTime, timeTaken, timeCounter;
    // Use this for initialization
    void Start () {
        startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
        //calculate the cameras screenpoint and the offset
        screenpoint = cam.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenpoint.z));

    }
    private void OnMouseDrag()
    {
        //move the mesh witht he mouse using the mouse position
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenpoint.z);
        Vector3 cursorPosition = cam.ScreenToWorldPoint(cursorScreenPoint) + offset;
        transform.position = cursorPosition;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        ray.origin = cam.transform.position;


       //check if there is a collision/ hit
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, 100000))
            {
                Vector3 v = hit.point;
                Debug.Log(v.x + ", " + v.y + ", " + v.z);
                Debug.Log(hit.point);
            }
            else
            {
                Debug.Log("not");
            }
        }
        // debugging for time calculations 
        timeTaken = Time.time - startTime;
        timeCounter++;
        if (timeCounter == 100)
        {
            Debug.Log("Time after 100 updates: " + timeTaken);
            timeCounter = 0;
            timeTaken = 0;
        }

    }
}
