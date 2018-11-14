using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameManager manager;
    //List<PlayerStation> players;
    PlayerStation currPlayer, nextPlayer;
    int playerNum, nextNum;

    enum CameraState { Start, Base, Far , End };
    CameraState currState;

    public GameObject world;

    public float lastZoom;
    public float startZoom, baseZoom, farZoom;//5f, 2f, 10f

    public Vector3 currPos;
    public Vector3 lastPos;
    public Vector3 startPos, basePos, farPos, endPos;

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

    bool stayOnMe = false;

    void Awake()
    {
        manager = GameManager.Instance;
    }

    // Use this for initialization
    void Start()
    {

        startTime = Time.time;
        startPos = transform.position;//new Vector3(0,0,-10);
        farPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10);
        lastPos = startPos;
        basePos = new Vector3(0, meteorRadious, -10);
        currState = CameraState.Start;

        degrees = new Vector3(0, 0, 360 / playerCount);
        startRot = world.transform.rotation;
        lastRot = startRot;
        spacesOver = 1;
        baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);

        //players = manager.players;
        playerCount = manager.players.Count;
        playerNum = 0;
        currPlayer = manager.players[playerNum];
        nextPlayer = manager.players[playerNum+1];


    }
	
	// Update is called once per frame
	void Update () {

        currPos = transform.position;

        currRot = world.transform.rotation;

        /*if (!nextPlayer.IsAlive)
        {
            nextNum = Mathf.Abs((playerNum + 1) % (int)playerCount);
            nextPlayer = manager.players[nextNum];
            while (!nextPlayer.IsAlive)
            {
                spacesOver = Mathf.Abs(playerNum - nextNum);
            }
            baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
        }*/

        //increments the number of spaces the camera will go over
        if (Input.GetKeyDown(KeyCode.X))
        {
            spacesOver++;
            if(spacesOver > playerCount)
            {
                spacesOver = 1;
            }
            baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
        }
        //keeps the camera focused on the same base
        if (Input.GetKeyDown(KeyCode.Z))
        {
            stayOnMe = !stayOnMe;
        }

        if (currState == CameraState.Start)
        {
            if (currPos != startPos)
            {
                distCovered = (Time.time - startTime) * speed;
                distance = Vector3.Distance(lastPos, startPos);
                percent = distCovered / distance;

                transform.position = Vector3.Lerp(lastPos, startPos, percent);
                Camera.main.orthographicSize = Mathf.Lerp(lastZoom, startZoom, percent);

                if (!stayOnMe)
                    world.transform.rotation = Quaternion.Slerp(lastRot, startRot, percent);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                nextNum = Mathf.Abs((playerNum + 1) % (int)playerCount);
                nextPlayer = manager.players[nextNum];
                while (!nextPlayer.IsAlive)
                {
                    spacesOver = Mathf.Abs(playerNum - nextNum);
                }
                baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);


                transform.position = startPos;
                Camera.main.orthographicSize = startZoom;

                if (!stayOnMe)
                    world.transform.rotation = startRot;



                currState = CameraState.Base;
                lastPos = startPos;
                lastZoom = startZoom;
                startTime = Time.time;

                //startRot = currRot;
                if (!stayOnMe)
                    lastRot = startRot;//currRot;
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

                if (!stayOnMe)
                    world.transform.rotation = Quaternion.Slerp(lastRot, baseRot, percent);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                transform.position = basePos;
                Camera.main.orthographicSize = baseZoom;

                if (!stayOnMe)
                    world.transform.rotation = baseRot;



                currState = CameraState.Start;
                lastPos = basePos;
                lastZoom = baseZoom;
                startTime = Time.time;

                if (!stayOnMe)
                {
                    lastRot = baseRot;
                    baseRot = baseRot * Quaternion.Euler(degrees * spacesOver);

                    startRot = lastRot;
                }
            }

        }
        else if (currState == CameraState.Far)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (currPos != farPos)
                {
                    distCovered = (Time.time - startTime) * speed;
                    distance = Vector3.Distance(lastPos, farPos);
                    percent = distCovered / distance;

                    transform.position = Vector3.Lerp(lastPos, farPos, percent);
                    Camera.main.orthographicSize = Mathf.Lerp(lastZoom, farZoom, percent);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    transform.position = farPos;
                    Camera.main.orthographicSize = farZoom;


                    currState = CameraState.Start;
                    lastPos = farPos;
                    lastZoom = farZoom;
                    startTime = Time.time;
                }
            }
        }
        else if (currState == CameraState.End)
        {

        }

    }
}
