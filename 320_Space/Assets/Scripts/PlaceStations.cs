using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceStations : MonoBehaviour {

    //Fields
    public int numStations;
    public float radius;
    public float lastRadius;
    public List<Vector2> stationCoords;
    public float angle;
    private float interval;
    private float lastInterval;

    private float xPos;
    private float yPos;
    public GameObject station;

    // Use this for initialization
    void Start ()
    {
        angle = 90; //sets first station position as top of asteroid

        interval = 360 / numStations;

        //Iterates over asteroid and sets coordinates for each station
		for(int i = 0; i < numStations; i++)
        {
            xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            yPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            stationCoords.Add(new Vector2(xPos, yPos));

            angle -= interval;
        }
	}

    private void OnDrawGizmos()
    {
        //Draws asteroid
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(0, 0, 0), radius);

        //Draws stations
        for (int i = 0; i < numStations; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(stationCoords[i], radius: radius / 5);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //Updates layout if radius or number of stations is changed
        interval = 360 / numStations;

        if(interval != lastInterval || lastRadius != radius)
        {
            angle = 90;
            stationCoords.Clear();

            for (int i = 0; i < numStations; i++)
            {
                xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                yPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

                stationCoords.Add(new Vector2(xPos, yPos));

                angle -= interval;
            }
        }

        lastInterval = interval;
        lastRadius = radius;
    }
}
