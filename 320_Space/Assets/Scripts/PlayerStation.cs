using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStation : MonoBehaviour {

    /// <summary>
    /// Whether the station is alive
    /// </summary>
    public bool IsAlive { get { return health > 0; } }

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

    //Private fields
    short health;
    short resources;

	// Called before start
	void Awake () {
        health = 3;
        resources = 0;
        ActionChosen = false;
        Action = "";
        Target = null;
	}

    //Called on startup
    void Start()
    {
        GameManager.Instance.players.Add(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// Logic for the player choosing their action
    /// </summary>
    public void ChooseAction()
    {
        while (!ActionChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Action = "Shoot";
                ActionChosen = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
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
        if(Action == "Shoot"||Action == "Reflect")
        {
            while (Target == null)
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
                    Target = GameManager.Instance.players[2];
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Target = GameManager.Instance.players[3];
                }
            }
        }
    }

    /// <summary>
    /// Logic for the player performing their action
    /// </summary>
    public void PerformAction()
    {
        switch (Action)
        {
            case "Shoot":
                if(Target == null)  //target shouldn't be null if shooting
                {
                    break;
                }
                if (FindTarget())
                {
                    if(Target.Action != "Shield")
                        Target.TakeDamage();
                }

                resources -= 1;
                break;
            case "Reflect":
                resources -= 1;
                break;
            case "Shield":
                Debug.Log("All incoming missles are destroyed!");
                break;
            case "Load":
                resources++;
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
        health--;
    }

    public void DebugInfo()
    {
        Debug.Log("Resource count: " + resources);
        Debug.Log("Health: " + health);
    }
}
