using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;
using Mapbox.Unity.Map;

public class process : MonoBehaviour
{
    //public SpawnOnMap MySpawnMap;
    public Spawn_Bird spawn_Bird;
    public AbstractMap map;
    public line_gen line_Gen;

    public float birdScale = 40f;
    public float lineScale = 4f;
    //read data from 
    public csvReader reader;
    
    //initialize 2 list contain the lat lng information
    
    private List<string> locations = new List<string>();//in lat lng
    public string[][] Locations;
    private List<float> heights = new List<float>();
    public float[][] Heights;
    public List<string> IDs;

    public bool restart=false;//true: restart the animation
    // initial pos for map
    private string initial_pos;
    public bool mode;//define a overview of the dataset or a immersive environment
    
    public float speed; // it takes 1/speed s to fly between datapoints
    //check if the linescale is changed
    double lastLineScale;

    void Start() {
        //reader = gameObject.GetComponent(typeof(csvReader)) as csvReader;
        loadData();                                     
        spawn_Bird.speed = speed;

        lastLineScale = lineScale;
    }
    // Update is called once per frame
    void Update()
    {
        spawn_Bird.speed = speed;
        
        rescale();
    }
    //Load data and store them in spawn_Bird.cs
    void loadData() {
       
        //the number of individuals
        int gpLength = reader.gp.individualBehaviors.Count;
        //Debug.Log(gpLength);
        Heights = new float[gpLength][];
        Locations = new string[gpLength][];
        
        int count = 0;
        for (int i = 0; i < gpLength; i++) {
            //IndividualBehavior individualBehaviors = reader.gp.individualBehaviors[i];
            if (reader.gp.individualBehaviors[i].Individualtracks != null)
            {

                //Debug.Log(reader.gp.individualBehaviors[i].Individualtracks.Count);
            }
            else {

                Debug.Log("is null"); 
            }
            foreach (Individual individual in reader.gp.individualBehaviors[i].Individualtracks)
            {
                
                string id_lat = individual.lat;
                string id_lng = individual.lng;
                float id_height = individual.height;
                if (count == 0)
                {
                    count++;
                    initial_pos = id_lat + ',' + id_lng;
                }

                heights.Add(id_height); 
                locations.Add(id_lat + ',' + id_lng);
                //Debug.Log(id_height);
            }
            //Debug.Log("lat "+id_lat);
            string[] loc_array = locations.ToArray();
            float[] height_array = heights.ToArray();
            //store height_array into Heights[][]
            Heights[i] = height_array;
            
            //store loc_array into Locations[][]
            Locations[i] = loc_array;
            locations = new List<string>();//initialize locations
            heights = new List<float>();//initialize heights
            string name = reader.gp.individualBehaviors[i].Individualtracks[0].id;
            IDs.Add(name);
        }
        spawn_Bird._LocationStrings = Locations;
        spawn_Bird._HeightArray = Heights;
        spawn_Bird._IDs = IDs.ToArray();
        map.default_pos = initial_pos;

    }
    void rescale() {
        //user can rescale the bird and the line
        if (spawn_Bird != null)
        {
            spawn_Bird.Bird_SpawnScale = birdScale;
            
        }
        
        if (lineScale!=lastLineScale) {
            line_Gen.needUpdate = true;
        }

        lastLineScale = lineScale;
    }
    
}
