﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameManager manager;
    public PlayerStation currPlayer, nextPlayer;
    public int playerNum, nextNum;

    public enum CameraState { Start, Base, Far, End };
    public CameraState currState, prevState;

    public GameObject world;

    //zoom
    public float lastZoom;
    public float startZoom, baseZoom, farZoom;//5f, 2f, 10f
    //position
    public Vector3 currPos;
    public Vector3 lastPos;
    public Vector3 startPos, basePos, farPos, endPos;

    //rotation
    public Quaternion currRot;
    public Quaternion lastRot;
    public Quaternion startRot, baseRot;

    public float playerCount; // default to 2
    public Vector3 degrees;

    public float speed;//2.0f
    public float meteorRadius;//4 for now

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
        basePos = new Vector3(0, meteorRadius, -10);
        currState = CameraState.Start;
        prevState = CameraState.Start;

        degrees = new Vector3(0.0f, 0.0f, (360.0f / playerCount));
        startRot = world.transform.rotation;
        lastRot = startRot;
        spacesOver = 1;
        baseRot = (lastRot * Quaternion.Euler((degrees * spacesOver)));

    }
    /// <summary>
    /// sets values if using manager
    /// </summary>
    void UseManagerStart()
    {
        playerCount = manager.players.Count;
        playerNum = 0;
        currPlayer = manager.players[playerNum];
        nextPlayer = manager.players[playerNum + 1];

        degrees = new Vector3(0, 0, 360 / playerCount);
    }
    /// <summary>
    /// sets player count to passed in int
    /// </summary>
    /// <param name="totalPlayers"></param>
    void SetPlayerCount(int totalPlayers)
    {
        playerCount = totalPlayers;
    }
    /// <summary>
    /// sets base rotation to next non dead player
    /// </summary>
    void SetNextNotDead()
    {
        if (!nextPlayer.IsAlive)
        {
            nextNum = Mathf.Abs((playerNum + 1) % (int)playerCount);
            nextPlayer = manager.players[nextNum];
            while (!nextPlayer.IsAlive)
            {
                spacesOver = nextNum - playerNum;
            }
            baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
        }
    }
    /// <summary>
    /// sets base rotation to player number passed in
    /// </summary>
    /// <param name="targetPlayerNum"></param>
    public void SetNextPlayer(int targetPlayerNum)
    {
        if (targetPlayerNum < playerCount)
        {
            nextNum = targetPlayerNum;
            //nextPlayer = manager.players[nextNum];
            spacesOver = nextNum - playerNum;

            baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);
        }
    }
    void StartCam()
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
    }
    void BaseCam()
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
    }
    void ClickPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetNextPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetNextPlayer(2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetNextPlayer(3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetNextPlayer(4);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetNextPlayer(5);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SetNextPlayer(6);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SetNextPlayer(7);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetNextPlayer(0);
        }
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    SetNextPlayer(0);
        //}
    }
    // Update is called once per frame
    void Update()
    {
        currPos = transform.position;
        currRot = world.transform.rotation;

        ClickPlayer();
       
            //swaps states between start and base
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currState == CameraState.Start)
                currState = CameraState.Base;
            else if (currState == CameraState.Base)
                currState = CameraState.Start;
        }
        //increments the number of spaces the camera will go over
        if (Input.GetKeyDown(KeyCode.X))
        {
            spacesOver++;
            if (spacesOver > playerCount-1)
            {
                spacesOver = 0;
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
            if (prevState == CameraState.Base)
            {
                prevState = currState;

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
                    playerNum = nextNum;//set player num = next num

                    startRot = lastRot;
                }

            }
            StartCam();
        }
        else if (currState == CameraState.Base)
        {
            if (prevState == CameraState.Start)
            {

                spacesOver = nextNum - playerNum;
                baseRot = lastRot * Quaternion.Euler(degrees * spacesOver);

                prevState = currState;

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
            BaseCam();
        }
        else if (currState == CameraState.Far)
        {

        }
        else if (currState == CameraState.End)
        {

        }

    }
    void FarCam()
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
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public GameManager manager;
    //List<PlayerStation> players;
    PlayerStation currPlayer, nextPlayer;
    int playerNum, nextNum;

    enum CameraState { Start, Base, Far, End };
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
        nextPlayer = manager.players[playerNum + 1];


    }

    // Update is called once per frame
    void Update()
    {

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
/*      if (Input.GetKeyDown(KeyCode.X))
      {
          spacesOver++;
          if (spacesOver > playerCount)
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
          ZoomStart();
      }
      else if (currState == CameraState.Base)
      {
          ZoomBase();
      }
      else if (currState == CameraState.Far)
      {
          ZoomFar();
      }
      else if (currState == CameraState.End)
      {

      }

  }
  void ZoomBase()
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
  void ZoomStart()
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
  void ZoomFar()
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

}*/
