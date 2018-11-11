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

    public List<PlayerStation> players { get; set; }

    //Private Fields
    RoundState roundState;
    short currPlayer;

    // Called before start
    void Awake () {
        State = GameState.Begin;
        roundState = RoundState.Begin;
        players = new List<PlayerStation>();
        currPlayer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        switch (State)
        {
            //Game is starting
            case GameState.Begin:
                //set up for number of players goes here i think
                Debug.Log("Player Count: " + players.Count);
                State = GameState.Playing;
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
                        Debug.Log("Begin Round");
                        currPlayer = 0;
                        
                        roundState = RoundState.TurnOrder;
                        break;

                    //Players are taking their turns
                    case RoundState.TurnOrder:
                        Debug.Log("Player Turn");
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

                        //Player chooses their action
                        if(players[currPlayer].IsAlive && !players[currPlayer].ActionChosen)
                        {
                            Debug.Log("Current Player: " + currPlayer);
                            players[currPlayer].ChooseAction();
                        }
                        currPlayer++;

                        break;

                    //Play all the choices out
                    case RoundState.PlayChoices:
                        foreach (PlayerStation player in players)
                        {
                            player.PerformAction();
                        }
                        roundState = RoundState.End;
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
}
