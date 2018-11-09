using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    enum CameraState { Start, Base, Full , End };
    CameraState currState;

    public GameObject world;

    public float lastZoom;
    public float startZoom, baseZoom;//2f,5f

    public Vector3 currPos;
    public Vector3 lastPos;
    public Vector3 startPos, basePos, fullPos, endPos;

    public Quaternion currRot;
    public Quaternion lastRot;
    public Quaternion startRot, baseRot;
    public float playerCount;
    public Vector3 degrees;

    public float speed;//2.0f
    public float meteorRadious;//4 for now

    public float startTime;
    public float distance, distCovered;
    public float percent;

    public float spacesOver;
    //bool wantToRotate = false;


    // Use this for initialization
    void Start () {
        startTime = Time.time;
        startPos = transform.position;//new Vector3(0,0,-10);
        lastPos = startPos;
        basePos = new Vector3(0, meteorRadious, -10);
        currState = CameraState.Start;

        degrees= new Vector3(0, 0, 360 / playerCount);
        startRot = world.transform.rotation;
        lastRot = startRot;
        spacesOver = 1;
        baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
        
    }
	
	// Update is called once per frame
	void Update () {

        currPos = transform.position;

        currRot = world.transform.rotation;

        if (Input.GetKeyDown(KeyCode.X))
        {
            spacesOver++;
            if(spacesOver > playerCount)
            {
                spacesOver = 1;
            }
            baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
            //wantToRotate = !wantToRotate;
        }

        //  world.transform.Rotate(world.transform.forward * speed * Time.deltaTime , Space.World);
        //if (wantToRotate == true)
        //{
            //world.transform.rotation = Quaternion.Slerp(lastRot, baseRot, percent);
       // }

        if (currState == CameraState.Start)
        {
            if (currPos != startPos)
            {
                distCovered = (Time.time - startTime) * speed;
                distance = Vector3.Distance(lastPos, startPos);
                percent = distCovered / distance;

                transform.position = Vector3.Lerp(lastPos, startPos, percent);
                Camera.main.orthographicSize = Mathf.Lerp(lastZoom, startZoom, percent);

                world.transform.rotation = Quaternion.Slerp(lastRot, startRot, percent);
            }

            if (Input.GetKeyDown(KeyCode.C))
             {
                currState = CameraState.Base;
                lastPos = startPos;
                lastZoom = startZoom;
                startTime = Time.time;

                //startRot = currRot;
                lastRot = currRot;
            }
        }
        else if (currState == CameraState.Base)
        {
            if (currPos != basePos)
            {
                distCovered = (Time.time - startTime) * speed;
                distance = Vector3.Distance(lastPos, basePos);
                percent = distCovered / distance;

                transform.position = Vector3.Lerp(lastPos, basePos, percent);
                Camera.main.orthographicSize = Mathf.Lerp(lastZoom, baseZoom, percent);

                world.transform.rotation = Quaternion.Slerp(lastRot, baseRot, percent);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                currState = CameraState.Start;
                lastPos = basePos;
                lastZoom = baseZoom;
                startTime = Time.time;

                baseRot = currRot * Quaternion.Euler(degrees*spacesOver); 
                lastRot = currRot;
                startRot = lastRot;
            }
        }
        else if (currState == CameraState.Full)
        {

        }
        else if (currState == CameraState.End)
        {

        }

    }
}
