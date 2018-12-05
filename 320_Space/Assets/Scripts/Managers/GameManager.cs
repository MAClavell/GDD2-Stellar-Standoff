using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Begin, Paused, Playing, End }
enum RoundState { Begin, TurnOrder, PlayChoices, End }
enum TurnState { Begin, Choosing, End }

public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// The current GameState
    /// </summary>
    public GameState State { get; set; }

    //menu stuff
    public Canvas mainMenu;
    public Canvas playerMenu;
    public Canvas enemyMenu;
    public Canvas turnCanvas;
    public Canvas animationCanvas;
    public Text playerTotal;
    public Text currentPlayer;

    //Camera
    public CameraScript cam;

    public GameObject stationPre;
    public GameObject resourcePre;
    public GameObject healthPre;
    public List<PlayerStation> players { get; set; }
    public List<MissileData> missiles { get; set; }
    public List<GameObject> healthList { get; set; }
    public List<GameObject> resourceList { get; set; }
    public int numMissiles;
    //public int numShields

    public float radius;

    //Private Fields
    RoundState roundState;
    short currPlayer;
    bool readyToPlay;
    public int numPlayers;
    bool missilesLaunched;
    public GameObject missilePrefab;
    bool zoomed;
    int camBase;
    Color baseColor;
    bool playerReady;
    bool actionsReady;

    // Called before start
    void Awake() {
        State = GameState.Begin;
        roundState = RoundState.Begin;
        players = new List<PlayerStation>();
        missiles = new List<MissileData>();
        healthList = new List<GameObject>();
        resourceList = new List<GameObject>();
        currPlayer = 0;
        readyToPlay = false;
        numPlayers = 2;
        mainMenu.enabled = true;
        playerMenu.enabled = false;
        enemyMenu.enabled = false;
        missilesLaunched = false;
        playerTotal.enabled = true;
        zoomed = false;
        camBase = 0;
        baseColor = stationPre.GetComponentInChildren<SpriteRenderer>().color;
        playerReady = false;
        actionsReady = false;
        animationCanvas.enabled = false;
        turnCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        switch (State)
        {
            //Game is starting
            case GameState.Begin:
                //set up for number of players
                playerTotal.text = numPlayers.ToString();
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

                        //Orient the stations
                        if (direction == new Vector3(0, -1, 0))
                            newPlayer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        else
                            newPlayer.transform.up = direction;

                        //Set station color and number
                        newPlayer.GetComponent<PlayerStation>().Hue = PickColor(i);
                        newPlayer.GetComponent<PlayerStation>().PlayerNumber = i + 1;

                        //Add prefab's PlayerStation component to list
                        players.Add(newPlayer.GetComponent<PlayerStation>());
                    }
                    mainMenu.enabled = false;
                    turnCanvas.enabled = true;
                    State = GameState.Playing;
                    cam.UseManagerStart();
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
                        foreach (PlayerStation player in players)
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
                        if (currPlayer == players.Count)
                        {
                            roundState = RoundState.PlayChoices;
                            currPlayer = 0;
                            return;
                        }

                        if (playerReady)
                        {
                            //displaying menu and resources
                            turnCanvas.enabled = false;
                            if (zoomed)
                            {
                                if (camBase == currPlayer)
                                {
                                    playerMenu.enabled = true;
                                }
                                else
                                {
                                    enemyMenu.enabled = true;
                                }
                            }
                            else
                            {
                                playerMenu.enabled = false;
                                enemyMenu.enabled = false;
                            }

                            //setting home base color
                            for (int i = 0; i < players.Count; i++)
                            {
                                if (i == currPlayer)
                                {
                                    //players[i].GetComponentInChildren<SpriteRenderer>().color = new Color(0.0f, 1.0f, 1.0f);
                                }
                                else
                                {
                                    //players[i].GetComponentInChildren<SpriteRenderer>().color = baseColor;
                                }
                            }
                            
                            DisplayHealth(players[currPlayer]);
                            DisplayResources(players[currPlayer]);

                            CheckForStationTouch();

                            //Player chooses their action
                            if (players[currPlayer].IsAlive && !players[currPlayer].ActionChosen)
                            {
                                players[currPlayer].ChooseAction();
                            }
                            else if ((players[currPlayer].Action == "Shoot" || players[currPlayer].Action == "Reflect") && players[currPlayer].Target == null)
                            {
                                players[currPlayer].ChooseTarget();
                            }
                            else
                            {
                                cam.currState = CameraScript.CameraState.Start;
                                zoomed = false;
                                currPlayer++;
                                playerReady = false;
                            }
                        }
                        else
                        {
                            enemyMenu.enabled = false;
                            playerMenu.enabled = false;
                            turnCanvas.enabled = true;
                            currentPlayer.text = "Player " + (currPlayer + 1);
                            currentPlayer.color = players[currPlayer].Hue;
                        }

                        break;

                    //Play all the choices out
                    case RoundState.PlayChoices:
                        playerMenu.enabled = false;
                        enemyMenu.enabled = false;
                        if (actionsReady)
                        {
                            animationCanvas.enabled = false;
                            if (!cam.moving)
                            {
                                if (!missilesLaunched)
                                {
                                    foreach (PlayerStation player in players)
                                    {
                                        player.PerformAction();
                                    }
                                    missilesLaunched = true;
                                }

                                foreach (MissileData missile in missiles)
                                {
                                    if (!missile.InFlight)
                                    {
                                        GameObject m = (GameObject)Instantiate(missilePrefab, cam.world.transform);
                                        m.GetComponent<Missile>().Launch(missile.Origin, missile.Destination);
                                        missile.InFlight = true;
                                    }
                                }

                                if (missilesLaunched && GameManager.Instance.numMissiles <= 0)
                                {
                                    GameManager.Instance.missiles.Clear();
                                    actionsReady = false;
                                    roundState = RoundState.End;
                                }
                            }
                        }
                        else
                        {
                            animationCanvas.enabled = true;
                        }
                        break;

                    //End the round
                    case RoundState.End:
                        int alivePlayers = 0;
                        foreach (PlayerStation player in players)
                        {
                            if (player.IsAlive)
                            {
                                alivePlayers++;
                            }

                            if(player.ShieldOn)
                            {
								player.TurnOffShield();
                            }
                        }

                        if (alivePlayers <= 1)
                        {
                            State = GameState.End;
                            break;
                        }

                        roundState = RoundState.Begin;
                        break;
                }
                break;

            //Ending the game
            case GameState.End:
                break;
        }
    }

    public Color PickColor(int index)
    {
        //Red, Blue, Green, Yellow, Purple, Aqua, Orange, Pink
        switch (index)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return Color.magenta;
            case 5:
                return Color.cyan;
            case 6:
                return Color.gray;
            case 7:
                return Color.black;
            default:
                return Color.white;
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

    /// <summary>
    /// Checking for touch input on stations
    /// </summary>
    void CheckForStationTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "Station")
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (hit.collider.gameObject == players[i])
                    {
                        cam.SetNextPlayer(i);
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checking if station was clicked
    /// </summary>
    public void CheckForStationClicked(PlayerStation station)
    {
        if (!cam.moving)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == station)
                {
                    if (cam.currState == CameraScript.CameraState.Start)
                    {
                        cam.SetNextPlayer(i);
                        cam.currState = CameraScript.CameraState.Base;
                        zoomed = true;
                        camBase = i;
                    }
                    else
                    {
                        cam.currState = CameraScript.CameraState.Start;
                        zoomed = false;
                    }
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Shoot for menu functionality
    /// </summary>
    public void Shoot()
    {
        if (players[currPlayer].Resources > 0)
        {
            players[currPlayer].ActionChosen = true;
            players[currPlayer].Action = "Shoot";
            players[currPlayer].Target = players[camBase];
        }
    }

    /// <summary>
    /// Reflect for menu functionality
    /// </summary>
    public void Reflect()
    {
        if (players[currPlayer].Resources > 0)
        {
            players[currPlayer].ActionChosen = true;
            players[currPlayer].Action = "Reflect";
            players[currPlayer].Target = players[camBase];
        }
    }

    /// <summary>
    /// Load for menu functionality
    /// </summary>
    public void Load()
    {
        players[currPlayer].ActionChosen = true;
        players[currPlayer].Action = "Load";
    }

    /// <summary>
    /// Shield for menu functionality
    /// </summary>
    public void Shield()
    {
        players[currPlayer].ActionChosen = true;
        players[currPlayer].Action = "Shield";
    }


    void DisplayHealth(PlayerStation player)
    {
        if (healthList.Count > 0)
        {
            for (int i = 0; i < healthList.Count; i++)
            {
                GameObject cont = healthList[i];
                healthList.Remove(healthList[i]);
                Destroy(cont);
            }
        }
        for (int i = 0; i < player.Health; i++)
        {
            GameObject newHealth = Instantiate(healthPre);
            Vector3 pos = newHealth.transform.position;
            pos.x += i;
            newHealth.transform.position = pos;
            healthList.Add(newHealth);
        }
    }


    void DisplayResources(PlayerStation player)
    {
        if (resourceList.Count > 0)
        {
            for (int i = 0; i < resourceList.Count; i++)
            {
                GameObject cont = resourceList[i];
                resourceList.Remove(resourceList[i]);
                Destroy(cont);
            }
        }
        for (int i = 0; i < player.Resources; i++)
        {
            GameObject newResource = Instantiate(resourcePre);
            Vector3 pos = newResource.transform.position;
            pos.x += i;
            newResource.transform.position = pos;
            healthList.Add(newResource);
        }
    }


    public void TogglePlayerReady()
    {
        playerReady = true;
    }

    public void ToggleAnimationReady()
    {
        actionsReady = true;
    }
}
