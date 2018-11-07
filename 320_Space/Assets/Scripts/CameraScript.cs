using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    enum CameraState { Start, Base, Full ,End };
    CameraState currState;

	// Use this for initialization
	void Start () {
        currState = CameraState.Start;
	}
	
	// Update is called once per frame
	void Update () {

        if(currState == CameraState.Start)
        {

        }
        else if (currState == CameraState.Base)
        {
            //Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize,);
        }
        else if (currState == CameraState.Full)
        {

        }
        else if (currState == CameraState.End)
        {

        }

    }
}
