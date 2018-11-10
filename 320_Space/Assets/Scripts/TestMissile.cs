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
        destinationIndex = 4;
    }
	
	//Update is called once per frame
	void Update ()
    {

        //Update Interpolation data
        timer += Time.deltaTime;
        float fracComplete = timer / interval;
        Vector3 slerp = Vector3.Slerp(stations[originIndex].Direction, stations[destinationIndex].Direction, fracComplete);
        //slerp.z = 0;
        //slerp = new Vector3(slerp.x, slerp.y, 0);
        //Quaternion qSlerp = Quaternion.Slerp(Quaternion.Euler(stations[originIndex].Direction), Quaternion.Euler(stations[destinationIndex].Direction), fracComplete);
        //qSlerp = Quaternion.Euler(new Vector3(0f, qSlerp.eulerAngles.y, 0f));

        float arc = Mathf.Sin(fracComplete * Mathf.PI);
        transform.position = (asteroid.radius + (arc * maxHeight)) * slerp.normalized;

        //Update Rotation
        float angle = 90.0f;
        if(Vector3.SignedAngle(stations[originIndex].Direction, stations[destinationIndex].Direction, Vector3.forward) > 0)
                angle = -90.0f;
        transform.up = Quaternion.AngleAxis(angle, Vector3.forward) * slerp.normalized;


        //Arrive at destination of interpolation
        if (fracComplete >= 1.0f)
        {
            timer = 0;

            //Start next interpolation
            originIndex++;
            if (originIndex >= stations.Count) originIndex = 0;
            destinationIndex++;
            if (destinationIndex >= stations.Count) destinationIndex = 0;
        }

	}
}
