using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    enum CameraState { Start, Base, Full , End };
    CameraState currState;

    public float lastZoom;
    public float startZoom, baseZoom;//2f,5f

    public Vector3 currPos;
    public Vector3 lastPos;
    public Vector3 startPos, basePos, fullPos, endPos;

    public float speed;//2.0f
    public float meteorRadious;//4 for now

    public float startTime;
    public float distance, distCovered;
    public float percent;


    // Use this for initialization
    void Start () {
        startTime = Time.time;
        startPos = transform.position;//new Vector3(0,0,-10);
        lastPos = startPos;
        basePos = new Vector3(0, meteorRadious, -10);
        currState = CameraState.Start;
	}
	
	// Update is called once per frame
	void Update () {

        currPos = transform.position;
        

        if (currState == CameraState.Start)
        {
            if (currPos != startPos)
            {
                distCovered = (Time.time - startTime) * speed;
                distance = Vector3.Distance(lastPos, startPos);
                percent = distCovered / distance;

                transform.position = Vector3.Lerp(lastPos, startPos, percent);
                Camera.main.orthographicSize = Mathf.Lerp(lastZoom, startZoom, percent);
            }

            if (Input.GetKeyDown(KeyCode.C))
             {
                currState = CameraState.Base;
                lastPos = startPos;
                lastZoom = startZoom;
                startTime = Time.time;
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
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                currState = CameraState.Start;
                lastPos = basePos;
                lastZoom = baseZoom;
                startTime = Time.time;
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
