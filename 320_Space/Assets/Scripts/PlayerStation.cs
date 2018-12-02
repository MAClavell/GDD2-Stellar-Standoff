using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStation : MonoBehaviour {

    /// <summary>
    /// Whether the station is alive
    /// </summary>
    public bool IsAlive { get { return Health > 0; } }

    /// <summary>
    /// Whether the station has chosen their action or not
    /// </summary>
    public bool ActionChosen { get; set; }

    /// <summary>
    /// What station is being targeted
    /// </summary>
    public PlayerStation Target { get; set; }

    /// <summary>
    /// What action the station prepared
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Coordinates of station (local to asteroid)
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Normal vector pointing from center of asteroid to station
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// Player Number
    /// </summary>
    public int PlayerNumber { get; set; }

    /// <summary>
    /// Color to tint this station and it's related assets
    /// </summary>
    public Color Hue { get; set; }

    /// <summary>
    /// True if shield is active
    /// </summary>
    public bool ShieldOn { get; set; }

    public GameObject missile;

    public SpriteRenderer shieldRender;

    public GameObject label;

    //Private fields
    public short Health { get; set; }
    public short Resources { get; set; }

    //Force fields
    public float shieldTimer; //Controls exact opacity during fade
    private float shieldTime; //Max amount of second to fade in/out
    private bool shieldVisible; // True if shield is at max (permitted) opacity

	// Called before start
	void Awake () {
        Health = 3;
        Resources = 1;
        ActionChosen = false;
        Action = "";
        Target = null;
	}

    //Called on startup
    void Start()
    {
        //GameManager.Instance.players.Add(this);
        transform.SetParent(GameManager.Instance.cam.world.transform);
        transform.position = Position;
        GetComponentInChildren<SpriteRenderer>().color = Hue;
        shieldVisible = false;
        shieldTimer = 0;
        shieldTime = 1.5f;
        label.GetComponent<TextMeshPro>().text = "Player " + PlayerNumber;
    }

    // Update is called once per frame
    void Update ()
    {
        //Runs shield fade-in/fade-out
        if (ShieldOn && !shieldVisible)
        {
            ActivateShield();
        }
        else if (!ShieldOn && shieldVisible)
        {
            DeactivateShield();
        }

        label.transform.position = transform.position + new Vector3(0, 1, 0);
        label.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Logic for the player choosing their action
    /// </summary>
    public void ChooseAction()
    {
        if (!ActionChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)&&Resources>0)
            {
                Action = "Shoot";
                ActionChosen = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)&&Resources>0)
            {
                Action = "Reflect";
                ActionChosen = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Action = "Shield";
                ActionChosen = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Action = "Load";
                ActionChosen = true;
            }
        }
    }

    /// <summary>
    /// Logic for the player choosing their target
    /// </summary>
    public void ChooseTarget()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Target = GameManager.Instance.players[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Target = GameManager.Instance.players[1];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (GameManager.Instance.players[2] != null)
            {
                Target = GameManager.Instance.players[2];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (GameManager.Instance.players[3] != null)
            {
                Target = GameManager.Instance.players[3];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (GameManager.Instance.players[4] != null)
            {
                Target = GameManager.Instance.players[4];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (GameManager.Instance.players[5] != null)
            {
                Target = GameManager.Instance.players[5];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (GameManager.Instance.players[6] != null)
            {
                Target = GameManager.Instance.players[6];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (GameManager.Instance.players[7] != null)
            {
                Target = GameManager.Instance.players[7];
            }
        }
    }

    /// <summary>
    /// Logic for the player performing their action
    /// </summary>
    public void PerformAction()
    {
        Direction = (new Vector3(transform.position.x, transform.position.y, 0) - Vector3.zero).normalized;

        switch (Action)
        {
            case "Shoot":
                if(Target == null)  //target shouldn't be null if shooting
                {
                    break;
                }
                //GameObject newMissile = Instantiate(missile, transform);
                MissileData newMissileData = new MissileData(this, Target);
                GameManager.Instance.missiles.Add(newMissileData);
                GameManager.Instance.numMissiles++;
                //newMissile.GetComponent<Missile>().Launch(this, Target);
                //newMissile.GetComponent<Missile>().origin = this;
                //newMissile.GetComponent<Missile>().destination = Target;

                Resources -= 1;
                break;
            case "Reflect":
                Resources -= 1;
                break;
            case "Shield":
                ShieldOn = true;
                break;
            case "Load":
                Resources++;
                break;
        }
    }

    /// <summary>
    /// Finds the target of attack, accounting for target reflections
    /// Preventing Reflection loops
    /// </summary>
    /// <returns> Whether a target was found before missle dies off </returns>
    public bool FindTarget()
    {
        int reflections = 0;
        while(Target.Action == "Reflect" && reflections < 8)
        {
            reflections++;
            PlayerStation newTarget = Target.Target;
            Target = newTarget;
        }

        if (reflections < 8)
            return true;
        else
            Debug.Log("Missle ran out of fuel!");
            return false;
    }

    /// <summary>
    /// When the player is hit by missle, take damage
    /// </summary>
    public void TakeDamage()
    {
        Health--;
    }

    //Fades shield in to partial opacity
    public void ActivateShield()
    {
        shieldTimer += Time.deltaTime;
        shieldRender.color = new Color(Hue.r, Hue.g, Hue.b, (shieldTimer / shieldTime) / 1);
        if (shieldTimer >= shieldTime - 0.5f)
        {
            shieldTimer = 0;
            shieldVisible = true;
        }
    }

    //Fades shield out until full transparency
    public void DeactivateShield()
    {
        shieldTimer += Time.deltaTime;
        shieldRender.color = new Color(Hue.r, Hue.g, Hue.b, (1 - (shieldTimer / shieldTime) ) / 1);
        if (shieldTimer <= 0)
        {
            shieldTimer = 0;
            shieldVisible = false;
        }
    }

    public void DebugInfo()
    {
        Debug.Log("Resource count: " + Resources);
        Debug.Log("Health: " + Health);
    }

    private void OnMouseDown()
    {
        GameManager.Instance.CheckForStationClicked(this);
    }
}
