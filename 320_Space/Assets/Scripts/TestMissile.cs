using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMissile : MonoBehaviour {

    //Fields

    //Asteroid data
    public PlaceStations asteroid;
    public List<StationData> stations;

    //Interpolation
    public float timer; //counts from zero to interval
    public float interval; //seconds it takes to complete each arc
    public float maxHeight; //distance away from surface
    public int originIndex; 
    public int destinationIndex;

    //Use this for initialization
    void Start ()
    {
        stations = asteroid.stations;
        timer = 0;
        interval = 2.5f;
        maxHeight = 1.0f;
        originIndex = 0;
        destinationIndex = 1;
    }
	
	//Update is called once per frame
	void Update ()
    {


        timer += Time.deltaTime;
        float fracComplete = timer / interval;
        Vector3 slerp = Vector3.Slerp(stations[originIndex].Direction, stations[destinationIndex].Direction, fracComplete);
        
        float arc = Mathf.Sin(fracComplete * Mathf.PI);
        transform.position = (asteroid.radius + (arc * maxHeight)) * slerp.normalized;

        if(fracComplete >= 1.0f)
        {
            timer = 0;

            originIndex++;
            if (originIndex >= stations.Count) originIndex = 0;
            destinationIndex++;
            if (destinationIndex >= stations.Count) destinationIndex = 0;
        }

	}
}
