﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Begin, Paused, Playing, End }
enum RoundState { Begin, TurnOrder, PlayChoices, End }
enum TurnState { Begin, Choosing, End }

public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// The current GameState
    /// </summary>
    public GameState State { get; set; }
    public Canvas mainMenu;
    public Canvas playerMenu;
    public Canvas enemyMenu;

    public GameObject stationPre;
    public List<PlayerStation> players { get; set; }
    public List<Missile> missiles { get; set; }

    public float radius;

    //Private Fields
    RoundState roundState;
    short currPlayer;
    bool readyToPlay;
    int numPlayers;
    bool missilesLaunched;

    // Called before start
    void Awake () {
        radius = 2.0f;
        State = GameState.Begin;
        roundState = RoundState.Begin;
        players = new List<PlayerStation>();
        missiles = new List<Missile>();
        currPlayer = 0;
        readyToPlay = false;
        numPlayers = 2;
        mainMenu.enabled = true;
        playerMenu.enabled = false;
        enemyMenu.enabled = false;
        missilesLaunched = false;
	}
	
	// Update is called once per frame
	void Update () {
        switch (State)
        {
            //Game is starting
            case GameState.Begin:
                //set up for number of players
                if (readyToPlay)
                {

                    float xPos;
                    float yPos;
                    float angle = 90;
                    float interval = 360 / numPlayers;
                    Vector3 position;
                    Vector3 direction;

                    for (int i = 0; i < numPlayers; i++)
                    {
                        //Instantiate station prefab
                        GameObject newPlayer = Instantiate(stationPre);

                        //Calculate direction and position of station
                        xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                        yPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                        position = new Vector3(xPos, yPos, 0);
                        direction = (new Vector3(position.x, position.y, 0) - Vector3.zero).normalized;
                        angle -= interval;

                        //Assign direction and position to new prefab
                        newPlayer.GetComponent<PlayerStation>().Direction = direction;
                        newPlayer.GetComponent<PlayerStation>().Position = position;

                        //Add prefab's PlayerStation component to list
                        players.Add(newPlayer.GetComponent<PlayerStation>());
                    }
                    mainMenu.enabled = false;
                    State = GameState.Playing;
                }
                break;

            //Game is paused
            case GameState.Paused:

                break;

            //Playing the game
            case GameState.Playing:
                switch (roundState)
                {
                    //Clear previous player actions
                    case RoundState.Begin:
                        currPlayer = 0;
                        missilesLaunched = false;
                        foreach(PlayerStation player in players)
                        {
                            player.ActionChosen = false;
                            player.Action = "";
                            player.Target = null;
                        }
                        roundState = RoundState.TurnOrder;
                        break;

                    //Players are taking their turns
                    case RoundState.TurnOrder:
                        //Should never be less than 2 players
                        if (players.Count < 2)
                        {
                            Debug.LogError("Should never be less than 2 players!");
                            return;
                        }

                        //End the turn order if we are past the max players
                        if(currPlayer == players.Count)
                        {
                            roundState = RoundState.PlayChoices;
                            currPlayer = 0;
                            return;
                        }

                        playerMenu.enabled = true;
                        //Player chooses their action
                        if (players[currPlayer].IsAlive && !players[currPlayer].ActionChosen)
                        {
                            players[currPlayer].ChooseAction();
                        }
                        else if((players[currPlayer].Action == "Shoot"||players[currPlayer].Action == "Reflect")&&players[currPlayer].Target == null)
                        {
                            players[currPlayer].ChooseTarget();
                        }
                        else
                            currPlayer++;

                        break;

                    //Play all the choices out
                    case RoundState.PlayChoices:
                        if (!missilesLaunched)
                        {
                            foreach (PlayerStation player in players)
                            {
                                player.PerformAction();
                            }
                            missilesLaunched = true;
                        }


                        if (missilesLaunched && missiles.Count == 0)
                        {
                            roundState = RoundState.End;
                        }
                        break;

                    //End the round
                    case RoundState.End:
                        roundState = RoundState.Begin;

                        for (int i = 0; i < players.Count; i++)
                        {
                            Debug.Log("Player " + i);
                            players[i].DebugInfo();
                        }
                        break;
                }
                break;

            //Ending the game
            case GameState.End:

                break;
        }
    }

    /// <summary>
    /// Adding players from menu
    /// </summary>
    public void AddPlayer()
    {
        if (numPlayers < 8)
        {
            numPlayers++;
        }
        Debug.Log(numPlayers);
    }

    /// <summary>
    /// Removing player from menu
    /// </summary>
    public void RemovePlayer()
    {
        if (numPlayers > 2)
        {
            numPlayers--;
        }
        Debug.Log(numPlayers);
    }

    /// <summary>
    /// Hitting the play button on main menu
    /// </summary>
    public void ToggleReady()
    {
        readyToPlay = !readyToPlay;
        Debug.Log("Play");
    }
}
