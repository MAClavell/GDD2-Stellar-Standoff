using System.Collections;
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
                State = GameState.Playing;
                break;

            //Game is paused
            case GameState.Paused:

                break;

            //Playing the game
            case GameState.Playing:
                switch (roundState)
                {
                    //
                    case RoundState.Begin:
                        currPlayer = 0;
                        foreach(PlayerStation player in players)
                        {
                            player.ActionChosen = false;
                        }
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
                            return;
                        }

                        //Player chooses their action
                        if(players[currPlayer].IsAlive && !players[currPlayer].ActionChosen)
                        {
                            players[currPlayer].ChooseAction();
                        }
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
                        break;
                }
                break;

            //Ending the game
            case GameState.End:

                break;
        }
    }
}
