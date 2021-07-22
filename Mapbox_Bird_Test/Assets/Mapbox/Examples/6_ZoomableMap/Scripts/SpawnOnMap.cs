namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
    using System;
    public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		public string[][] _LocationStrings;
        public float[][] _HeightArray;
		Vector2d[] _locations;
        //have to be public for resetButton.cs to get the positions
        public Vector3[] _3Dlocations;
        //add bool to check whether can this start
        //public bool Can_start = false;
        //check whether the real update code can be run
        //public bool Can_update = false;

		public float Bird_SpawnScale = 10f;
        public float Point_SpawnScale = 5f;
        [SerializeField]
        GameObject _markerPrefab;
        // add a point Prefab
        [SerializeField]
        GameObject _pointPrefab;

        public List<List<GameObject>> _SpawnedObjects;

		void Start()
		{
            createObjs();
        }
        void createObjs() {
            _SpawnedObjects = new List<List<GameObject>>(_LocationStrings.Length);
            //Debug.Log(_LocationStrings.Length+"should be 30");
            //for loop to go through every trajectorys in locationsStrings
            for (int j = 0; j < _LocationStrings.Length; j++)
            {
                //string for each single location in trajectory
                string[] _locationStrings = _LocationStrings[j];
                float[] _heightArray = _HeightArray[j];

                _locations = new Vector2d[_locationStrings.Length];
                _3Dlocations = new Vector3[_locationStrings.Length];
                List<GameObject> _spawnedObjects = new List<GameObject>();
                Debug.Log("First For Loop works for " + j + " Times");

                for (int i = 0; i < _locationStrings.Length; i++)
                {
                    Debug.Log("Second For Loop works for " + i + " Times");
                    float height = _heightArray[i];

                    var locationString = _locationStrings[i];
                    _locations[i] = Conversions.StringToLatLon(locationString);

                    if (i == 0)//the first point of dataset we display a bird 
                    {
                        var instance = Instantiate(_markerPrefab);
                        var curpos = _map.GeoToWorldPosition(_locations[i], true);
                        instance.transform.localPosition = new Vector3(curpos.x, height, curpos.z);
                        instance.transform.localScale = new Vector3(Bird_SpawnScale, Bird_SpawnScale, Bird_SpawnScale);
                        instance.gameObject.tag = "Bird";//Here People should define a Tag in unity(can be improved)

                        _spawnedObjects.Add(instance);
                        _3Dlocations[i] = instance.transform.localPosition;
                    }
                    /*else
                    {//we display a sphere
                        var instance = Instantiate(_pointPrefab);
                        var curpos = _map.GeoToWorldPosition(_locations[i], true);
                        instance.transform.localPosition = new Vector3(curpos.x, height, curpos.z);
                        instance.transform.localScale = new Vector3(Point_SpawnScale, Point_SpawnScale, Point_SpawnScale);
                        //instance.AddComponent<MeshRenderer>();
                        _spawnedObjects.Add(instance);
                        _3Dlocations[i] = instance.transform.localPosition;
                    }*/
                }
                _SpawnedObjects.Add(_spawnedObjects);
            }
        }

		private void Update()
		{
            for (int j = 0; j < _LocationStrings.Length; j++)
            {
                List<GameObject> _spawnedObjects = _SpawnedObjects[j];
                string[] _locationStrings = _LocationStrings[j];
                float[] _heightArray = _HeightArray[j];
                int count = _spawnedObjects.Count;
                for (int i = 0; i < count; i++)
                {
                    float height = _heightArray[i];
                    var spawnedObject = _spawnedObjects[i];
                    if (spawnedObject.tag == "Bird")
                    {
                        //We don't want to update the position
                        var location = Conversions.StringToLatLon(_locationStrings[i]);
                        spawnedObject.transform.localPosition = _map.GeoToWorldPositionXYZ(location, height, true);
                        spawnedObject.transform.localScale = new Vector3(Bird_SpawnScale, Bird_SpawnScale, Bird_SpawnScale);
                    }
                    /*else
                    {
                        //We don't want to update the position
                        var location = Conversions.StringToLatLon(_locationStrings[i]);
                        spawnedObject.transform.localPosition = _map.GeoToWorldPositionXYZ(location, height, true);
                        spawnedObject.transform.localScale = new Vector3(Point_SpawnScale, Point_SpawnScale, Point_SpawnScale);
                    }*/
                }
            }
        }

	}
}