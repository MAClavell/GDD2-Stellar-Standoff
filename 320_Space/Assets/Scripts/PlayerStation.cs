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

    //Private fields
    short health;
    short resources;

	// Called before start
	void Awake () {
        health = 3;
        resources = 0;
        ActionChosen = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Logic for the player choosing their action
    /// </summary>
    public void ChooseAction()
    {

    }

    /// <summary>
    /// Logic for the player performing their action
    /// </summary>
    public void PerformAction()
    {
        
    }
}
