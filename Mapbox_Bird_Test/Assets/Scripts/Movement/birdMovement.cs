using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System;
/*
  This script is designed to automatically attached to 
  each trajectories under the polyline manager
     */
public class birdMovement : MonoBehaviour
{
    public Spawn_Bird spawn;
    LineRenderer lineRenderer;
    GameObject bird;
    //AbstractMap map;
    Vector3[] positions;
    //flying overallSpeed 
    public double overallSpeed; // it takes 1/overallSpeed s to fly between datapoints,
    //the local time
    public double t=0;
    //whether the bird is ready to fly
    public  bool isok = false;
    //count where is the bird
    int count=0;
    //the whole number of datapoints
    int num;
    int index;
    string birdName;
    //Time difference
    //public float TimeDiff = 2f;
    Vector3 birdpos;
    //if the time is ok for individual
    //public bool canStart;
    public string firstTimestamp;
    public GameObject DataManager;
    public IndividualBehavior idb;
    double multipler;
    public bool canContinute = false;
    Vector3 nextpos;
    List<GameObject> spawned_individuals;
    csvReader reader;
    Vector3 lastBirdPos;// store the geopos if we need
    // Start is called before the first frame update
    void Start()
    {
        //find the abstract map
        //map = GameObject.Find("mapbase").GetComponent<AbstractMap>();
        // find the spawn bird script
        spawn = GameObject.Find("mapbase").GetComponent<Spawn_Bird>();
        
        //get the list of birds
        spawned_individuals = spawn.spawned_individuals;
        DataManager = GameObject.Find("DataManager");
        reader= DataManager.GetComponent<csvReader>();
        initialize();
        //canStart = false;

    }
    void initialize() {
        
        //get the name of the object(correspond to the index of spawned_individuals)
        string objectName = transform.name;
        index = int.Parse(objectName);
        //this bird should be animated
        bird = spawned_individuals[index];
        //get the linerenderer from this obj
        UpdateLineRenderer();
        birdpos = positions[0];
        nextpos = positions[1];
        bird.transform.position = new Vector3(birdpos.x, birdpos.y, birdpos.z);
        t = 0;
        /* check
         * if (num == lineRenderer.positionCount) {
         *   Debug.Log(positions[0]);
         * }
         */
        //code below is to initialize a list information for each individual
        
        idb = reader.gp.individualBehaviors[index];
        Individual first = idb.Individualtracks[0];
        Individual second = idb.Individualtracks[1];
        overallSpeed = spawn.speed;
        //Debug.Log("OverallSpeed " + overallSpeed);
        string time1 = first.timestamp;
        string time2 = second.timestamp;
        DateTime t1;
        DateTime.TryParse(time1, out t1);
        DateTime t2;
        DateTime.TryParse(time2, out t2);
        TimeSpan TimeDiff = t2 - t1;
        double timeDiffSec = TimeDiff.TotalSeconds;
        //timeDiffSecDou = Convert.ToInt64(timeDiffSec);
        multipler = overallSpeed / timeDiffSec;
    }

    // Update is called once per frame
    void UpdateLineRenderer()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        positions = new Vector3[lineRenderer.positionCount];
        num = lineRenderer.GetPositions(positions);
    }
    void FixedUpdate()
    {
        
        overallSpeed = spawn.speed;
       
        //check for update the scene
        if (transform.parent.GetComponent<line_gen>().canUpdateModel)
        {
            //remember the double t to calculate the birdposition
            float tStore =(float) t;
            canContinute = false;
            UpdateLineRenderer();
            //current bird position

            birdpos = Vector3.Lerp(positions[count], positions[count + 1],tStore);
            //reset Bird position
            bird.transform.position = new Vector3(birdpos.x,birdpos.y,birdpos.z);
            
            nextpos = positions[count + 1];
            canContinute = true;
        }
        //check restart
        if (transform.parent.GetComponent<line_gen>().restart)
        {
            isok = false;
            count = 0;
            initialize();
            //Debug.Log("Still working");
            transform.parent.GetComponent<line_gen>().OverallTime = 0;
            //this below is to ensure the restart when the animation is finished
            bird.SetActive(true);
            //enable the line
            gameObject.SetActive(true);
            isok = true;
            
        }
        /* if (this.transform.parent.GetComponent<line_gen>().needUpdate) {
             positions = new Vector3[lineRenderer.positionCount];
             //birdName=
             num = lineRenderer.GetPositions(positions);//get an array of positions

         }*/
        
        if (bird.transform.position == positions[0] && count < num - 1)
        {
            isok = true;// it's ready to do animation
        }
        if (isok)
        {
            if (t >= Math.Abs(1-0.0012*overallSpeed))
            {
                //we switch to next position
                count++;
                if (count == num - 1)
                {//the next position is out of range
                    //Debug.Log("The " + index+" shoud be finished");
                    bird.SetActive(false);
                    //disable the line
                    gameObject.SetActive(false);
                    isok = false;
                    transform.parent.GetComponent<line_gen>().isfinished = true;
                    //return;
                }
                //Here we want to calculate the time difference between this position and next position
                Individual id = idb.Individualtracks[count];
                //check out of range
                if (count == num - 1)
                {//this position is out of range
                 //Debug.Log("The " + index + " in first if shoud be finished");

                    bird.transform.position = new Vector3(positions[count].x, positions[count].y, positions[count].z);
                    isok = false;
                    //t = 0f;
                    //count = 0;
                    return;
                }
                Individual nextId = idb.Individualtracks[count + 1];

                string time1 = id.timestamp;
                string time2 = nextId.timestamp;
                time1 = time1.Replace('-', '/');
                time2 = time2.Replace('-', '/');

                DateTime t1;
                DateTime.TryParse(time1, out t1);
                DateTime t2;
                DateTime.TryParse(time2, out t2);
                TimeSpan TimeDiff = t2 - t1;
                double timeDiffSec = TimeDiff.TotalSeconds;
                //timeDiffSecDou =double) Convert.ToInt64(timeDiffSec);

                birdpos = nextpos;
                nextpos = positions[count + 1];
                multipler = overallSpeed / timeDiffSec;
                //initialize the time t
                t = 0f;

                //Debug.Log("The time we need is: " + (1 / multipler));

            }
            if(t<1) {
                if (count == num - 1) {
                    return;
                }
                    
                bird.transform.LookAt(nextpos);
                t += Time.deltaTime * multipler;
                    
            }
            bird.transform.position = Vector3.Lerp(birdpos, nextpos, (float)t);
            //Debug.Log("However, the real t is: " + (float)t);
            lastBirdPos = bird.transform.position;

        }
        

    }
}
