using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Utilities;
//using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Examples;

/**
*  we append this to a game object and we this will spawn lines automatically
*/
public class line_gen : MonoBehaviour
{
    public Spawn_Bird spawn_bird;
    //public SpawnOnMap MySpawnMap;
    //
    public AbstractMap _map;
    //read data from 
    public csvReader reader;
    public process process;
    //the number of individual
    private int indiviudal_num;
    // array of colors
    private Color[] colors;
    public double OverallTime;

    public string curMin;
    public string curMax;
    public bool needUpdate;//true: we need to update the line
    public bool canUpdateModel;//true: BirdMovement.cs can update the model(position)
    public double overallSpeed;
    public double timeDiffSec=1;
    public bool restart;
    bool canStartCounting = false;
    public bool isfinished = false;
    bool needCheck = false;
    public interactionSettings iS;

    float timer = 0f;
    void Start()
    {
        needUpdate = true;
        restart = process.restart;
        canUpdateModel = true;
        indiviudal_num = reader.gp.individualBehaviors.Count;
        colors = new Color[indiviudal_num];
        drawline();
        OverallTime=0;
        GetMinMaxTime();
        canStartCounting = true;


    }
    void Update() {
        restart = process.restart;
        if (needUpdate) {
            /*//befor we update the line we have to remeber the position of bird
            int count = transform.childCount;
            //Debug.Log("Updated Line");
            for (int i = 0; i < count; i++)
            {
                GameObject bird = spawn_bird.spawned_individuals[i];
                Vector3 birdpos = new Vector3(bird.transform.position.x, bird.transform.position.y, bird.transform.position.z);
                //calculate lat lng
                var geoBirdPos = _map.WorldToGeoPosition(birdpos);
            }*/
            //We check whether is needed to update the line again
            updateLine();
            
            canUpdateModel = true;
            needUpdate = false;
            needCheck = true;

        }
        overallSpeed = spawn_bird.speed;
        if (canStartCounting) {
            // To make sure the time counting synchornized as movement
            OverallTime += Time.deltaTime * overallSpeed;
        }
        //check restart when one of the trajectories is already hidden
        if (isfinished && restart) {
            //SetActive all the gameobjects
            foreach (Transform child in transform) {
                child.gameObject.SetActive(true);
            }
            process.restart = false;//reset the value
        }
        process.restart = false;//reset the value
        if (needCheck) {
            int count = 0;
            foreach (Transform child in transform) {
                Debug.Log(child.gameObject.name);
                if (child.GetComponent<birdMovement>().canContinute == true) {
                   
                    count++;
                }
               
            }
            if (count == indiviudal_num)
            {
                canUpdateModel = false;
                needCheck = false;
                Debug.Log("Dui");
                foreach (Transform child in transform)
                {
                    child.GetComponent<birdMovement>().canContinute = false;
                    
                }
            }
            else {
                count = 0;
            }
            
        }
        //canUpdateModel = false;
        //needCheck = false;
    }
   
    /*
     * After reading the data we want to understand:
     *  1.when does the first individual come,
     *  2.when does the last individual finish
     * so that we can calculate the timeline amount in percentage
     */
    void GetMinMaxTime()
    {
        IndividualBehavior firstId = reader.gp.individualBehaviors[0];
        int firstlen = firstId.Individualtracks.Count;
        string fFirst = firstId.Individualtracks[0].timestamp;
        string fLast = firstId.Individualtracks[firstlen - 1].timestamp;
        curMin=fFirst;
        curMax=fLast;
        /*minIndex=0;
        maxIndex=0;*/
        for (int i = 1; i < indiviudal_num; i++)
        {
            IndividualBehavior id = reader.gp.individualBehaviors[i];
            int len = id.Individualtracks.Count;
            string localFirst = id.Individualtracks[0].timestamp;
            string localLast = id.Individualtracks[len-1].timestamp;
            if (!compareTime(localFirst, curMin)) 
            { //local is ealier
                curMin = localFirst;
                //minIndex = i;
            }
            if (!compareTime(curMax, localLast))
            { //local is later
                curMax = localLast;
               //maxIndex = i;
            }

        }
    }
    /*
     * the form of timstamp is like 2014-08-31 18:00:06.000
     * we use this function to help getting the min max timestamp
     */
    bool compareTime(string time1, string time2)
    {
        time1 = time1.Replace('-', '/');
        time2 = time2.Replace('-', '/');
        System.DateTime t1;
        System.DateTime.TryParse(time1, out t1);
        System.DateTime t2;
        System.DateTime.TryParse(time2, out t2);
        int result = System.DateTime.Compare(t1, t2);
        if (result < 0) { //t1 is ealier
            return false;
        }//else
        return true;
    }
    /*
     *Here we need to update the scale and positions of important vertices in our line
     */
    void updateLine() {
        int count = transform.childCount;
        //Debug.Log("Updated Line");
        for (int i = 0; i < count; i++)
        {
            //GameObject bird = spawn_bird.spawned_individuals[i];
            
            GameObject child = transform.GetChild(i).transform.gameObject;
            IndividualBehavior id = reader.gp.individualBehaviors[i];
            int trackCount = id.Individualtracks.Count;//number of datapoints for each individual
            string[] locations = spawn_bird._LocationStrings[i];
            float[] heights = spawn_bird._HeightArray[i];
            Vector3[] vP = new Vector3[trackCount];
            child.GetComponent<LineRenderer>().positionCount = trackCount;
            child.GetComponent<LineRenderer>().widthMultiplier = process.lineScale;
            for (int j = 0; j < trackCount; j++)
            {
                string loc = locations[j];
                float height = heights[j];
                var location = Conversions.StringToLatLon(loc);
                var localPosition = _map.GeoToWorldPosition(location,  true);
                Vector3 point = new Vector3(localPosition.x, localPosition.y+height, localPosition.z);
                vP[j] = point;
            }
            
            //bird.transform.position = new Vector3(vP[0].x, vP[0].y, vP[0].z);
            child.GetComponent<LineRenderer>().SetPositions(vP);
        }
        

    }
    /*
     * Initialize, we want to draw different lines for different individuals
     */
    void drawline() {
        for (int i = 0; i < indiviudal_num; i++)
        {
            //random bright color
            Color color = new Color(
            Random.Range(0.3f, 3f),
            Random.Range(0.3f, 3f),
            Random.Range(0.3f, 3f)
            );
           
            string childID =  i.ToString();
            GameObject child = new GameObject(childID);
            child.AddComponent<birdMovement>();
            child.transform.parent = this.transform;
            LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
            //Color color = ;
            colors[i] = color;
            lineRenderer.widthMultiplier = 30f;
            lineRenderer.material = new Material(Shader.Find("Standard"));
            lineRenderer.material.SetColor("_Color",color);

            IndividualBehavior id = reader.gp.individualBehaviors[i];
            int trackCount = id.Individualtracks.Count;//number of datapoints for each individual
            string[] locations = spawn_bird._LocationStrings[i];
            float[] heights = spawn_bird._HeightArray[i];
            Vector3[] vP = new Vector3[trackCount];
            lineRenderer.positionCount=trackCount;
            child.GetComponent<birdMovement>().firstTimestamp = id.Individualtracks[0].timestamp;
            for (int j = 0; j < trackCount; j++) {
                string loc = locations[j];
                float height = heights[j];
                var location = Conversions.StringToLatLon(loc);
                var localPosition = _map.GeoToWorldPosition(location, true);
                Vector3 point = new Vector3(localPosition.x, localPosition.y+height, localPosition.z);
                vP[j] = point;
            }
            //Debug.Log(vP.Length);
            lineRenderer.SetPositions(vP);
        }
    }
}
