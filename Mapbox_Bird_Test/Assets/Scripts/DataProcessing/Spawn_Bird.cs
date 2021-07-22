using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System;

public class Spawn_Bird : MonoBehaviour
{
    //Map base
    [SerializeField]
    AbstractMap _map;
    //Location information
    public string[][] _LocationStrings;
    public float[][] _HeightArray;
    public string[] _IDs;

    //Vector2d[] _locations;
    //have to be public for resetButton.cs to get the positions
   // public Vector3[] _3Dlocations;
    
    public float Bird_SpawnScale = 40f;
    //declear the model we use for the bird
    [SerializeField]
    GameObject birdModel;
    //a list of bird that have to be spawned
    public List<GameObject> spawned_individuals;
    // Start is called before the first frame update
    public double speed; // it takes 1/speed s to fly between datapoints,
    void Start()
    {
        createObjs();
        
    }
    
    //here include the model of bird
    void createObjs()
    {

        spawned_individuals = new List<GameObject>();
        for (int j = 0; j < _LocationStrings.Length; j++)
        {
            
            //string for each single location in trajectory
            string[] _locationStrings = _LocationStrings[j];
            float[] _heightArray = _HeightArray[j];
            //_locations = new Vector2d[_locationStrings.Length];
            //_3Dlocations = new Vector3[_locationStrings.Length];
            float height = _heightArray[0];
            string locationString = _locationStrings[0];
            //Debug.Log(j+" "+ _LocationStrings.Length);
            Vector2d location_zero = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(birdModel);
            var curpos = _map.GeoToWorldPosition(location_zero,  true);
            instance.transform.localPosition = new Vector3(curpos.x, curpos.y+height, curpos.z);
            instance.transform.localScale = new Vector3(Bird_SpawnScale, Bird_SpawnScale, Bird_SpawnScale);
            instance.transform.name = _IDs[j];
            spawned_individuals.Add(instance);
            //_3Dlocations[i] = instance.transform.localPosition;
            
        }
    }
        // Update is called once per frame
    void Update()
    {
        // here is used to control the bird model's scale
        int count = spawned_individuals.Count;
        //int count = _LocationStrings.Length;
        //Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            //string[] _locationStrings = _LocationStrings[i];
            //float[] _heightArray = _HeightArray[i];
            //float height = _heightArray[0];
            var spawnedObject = spawned_individuals[i];
            
            spawnedObject.transform.localScale = new Vector3(Bird_SpawnScale, Bird_SpawnScale, Bird_SpawnScale);
            
        }

    }
}
