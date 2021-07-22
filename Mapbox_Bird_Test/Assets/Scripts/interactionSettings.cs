using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Mapbox.Unity.Map;
using UnityEngine.UI;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map.TileProviders;//For map rangeAroundcenter
public class interactionSettings : MonoBehaviour
{
    /* public Camera camera;
     public GameObject vrPlayer;
     bool VRActived;*/
    public GameObject vrPlayer;
    public GameObject ID;
    public GameObject buttonInfo;
    public bool change;
    bool stop;
    public AbstractMap map;
    public float zoomingskip = 1f;
    public float cameraMove = 400f;
    public process processor;
    public line_gen lineGen;
    public Spawn_Bird spawn;
    public int count=0;
    public bool needUpdate=false;
    List<GameObject> Model_List;// list of bird models
    Vector3 PlayerPos;
    int playerHeight = 300;
    Vector3 curPlayerPos;
    float textDur = 4f;//duration of text in seconds
    float t = 0;
    private void Start()
    {
        count = 0;
        vrPlayer.transform.position = new Vector3(0, 0, 0);
        vrPlayer.transform.eulerAngles = new Vector3(0f, 0.0f, 0.0f);
        spawn = map.GetComponent<Spawn_Bird>();
        Model_List = spawn.spawned_individuals;
        //Debug.Log(Model_List.Count);
        string textInhalt = spawn._IDs[count];
        ID.GetComponent<Text>().text = textInhalt;
        change = false;
    }
    void Update()
    {
        checkChangeView();//from overview to vr scene
        zoomin();//zoom in
        zoomout();//zoom out
        checkMapUpdate();//center button for map update
        ExtendMap();//Move the camera in overview mode
        checkRestart();// restart with trigger
        changeSpeed();
        fadeOutText();
        //testing UpdateMap
        if (needUpdate) {
            MapBoxupdate();
            lineGen.needUpdate = true;
            needUpdate = false;
        }
        if (change !=false) {
            count++;

            if (count >= Model_List.Count)
            {
                count = 0;
            }
            change = false;
        }
    }
    void fadeOutText() {
        Color tmpcolor = Color.black;
        
        t += Time.deltaTime / textDur;
        tmpcolor.a = Mathf.Lerp(1f, 0f, t);
        buttonInfo.GetComponent<Text>().color = tmpcolor;
    }
    void updateText(string text) {
        buttonInfo.GetComponent<Text>().text = text;
        Color tmpcolor = Color.black;

        buttonInfo.GetComponent<Text>().color = tmpcolor;
        t = 0;
        
    }
    void checkRestart()
    {
        if(SteamVR_Actions._default.Restart.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            processor.restart = true;
            lineGen.restart = true;
            updateText("Restarted");
        }
    }
    void changeSpeed()
    {
        if (SteamVR_Actions._default.SpeedUp.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            processor.speed++;
            updateText("Speed up"+ " current speed="+processor.speed);
        }
        if (SteamVR_Actions._default.SpeedDown.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            processor.speed--;
            if (processor.speed == 0) {
                processor.speed = 1;
            }
            updateText("Speed down"+ " current speed=" + processor.speed);
        }
    }
    //Change the view of the player
    void checkChangeView()
    {
        if (SteamVR_Actions._default.ChangeView.GetStateUp(SteamVR_Input_Sources.RightHand))
        { //Right controller, the player can switch the individual to observe
            
            //change = !change;
            count++;
            
            if (count >= Model_List.Count)
            {
                count = 0;
            }
            string textInhalt = spawn._IDs[count];
            ID.GetComponent<Text>().text = textInhalt;
            updateText("Individual switched");
        }
        if (SteamVR_Actions._default.ChangeView.GetStateUp(SteamVR_Input_Sources.LeftHand))
        { //Left controller, the player will fly with the individual
           // Debug.Log("left menu pressed");
            processor.mode = !processor.mode;
            //get current individual's position
            curPlayerPos = new Vector3(Model_List[count].transform.position.x + 50, Model_List[count].transform.position.y + playerHeight, Model_List[count].transform.position.z + 50);
            if (processor.mode)
            {
                updateText("Fly with the individual");
            } else {
                updateText("Free Mode");
                vrPlayer.transform.position = curPlayerPos;
            }

        }
        if (processor.mode)
        {
            //get the model and let player look at
            vrPlayer.transform.position = new Vector3(Model_List[count].transform.position.x+30, Model_List[count].transform.position.y+50, Model_List[count].transform.position.z+30);
            vrPlayer.transform.LookAt(Model_List[count].transform);
            
        }
        
    }
    void zoomin()
    {
        if (SteamVR_Actions._default.ZoomingLevelUp.GetStateDown(SteamVR_Input_Sources.RightHand))
        { //Right controller
            //Debug.Log("right pad north pressed");
            map.SetZoom(map.Zoom + zoomingskip);
            map.UpdateMap(map.Zoom);
            lineGen.needUpdate = true;
            updateText("Zoom in");
        }
    }
    void zoomout()
    {
        if (SteamVR_Actions._default.ZoomingLevelDown.GetStateDown(SteamVR_Input_Sources.RightHand))
        { //Right controller
            //Debug.Log("right pad south pressed");
            map.SetZoom(map.Zoom - zoomingskip);
            map.UpdateMap(map.Zoom);
            lineGen.needUpdate = true;
            updateText("Zoom out");
        }
    }
    void checkMapUpdate()
    { //Only Update the View in VR scene
        if (SteamVR_Actions._default.Update.GetStateUp(SteamVR_Input_Sources.RightHand))
        { //Both controller
            //Debug.Log("touchpad center pressed");
            MapBoxupdate();
            lineGen.needUpdate = true;
            updateText("Map updated you are the center");
        }

    }
    void MapBoxupdate()
    {
        //var OldLatLon = Conversions.StringToLatLon(map._options.locationOptions.latitudeLongitude);

        //Debug.Log(PlayerPos);
        //PlayerPos = new Vector3(PlayerPos.x, 0, PlayerPos.z);
        PlayerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        var newMapPos = map.WorldToGeoPosition(PlayerPos);
        //Debug.Log(newMapPos);
        //PlayerPos = new Vector3(PlayerPos.x+20, 10, PlayerPos.z);

        //newMapPos = map.WorldToGeoPosition(PlayerPos);
        //Debug.Log(newMapPos);
        //Debug.Log(LatLon.GetType());
        map.UpdateMap(newMapPos, map.Zoom);
        
        
    }
    void ExtendMap()// mode =true : extend the map, mode = false: change user position
    {
       
        
        if (processor.mode)
        {
            if (SteamVR_Actions._default.OverviewRight.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad right pressed");
                map.TileProvider.GetComponent<RangeTileProvider>()._rangeTileProviderOptions.east++;
                MapBoxupdate();
                updateText("Map extended (East)");
            }
            if (SteamVR_Actions._default.OverviewUp.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad up pressed");
                map.TileProvider.GetComponent<RangeTileProvider>()._rangeTileProviderOptions.north++;
                MapBoxupdate();
                updateText("Map extended (North)");
            }
            if (SteamVR_Actions._default.OverviewLeft.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad left pressed");
                map.TileProvider.GetComponent<RangeTileProvider>()._rangeTileProviderOptions.west++;
                MapBoxupdate();
                updateText("Map extended (West)");
            }
            if (SteamVR_Actions._default.OverviewDown.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad down pressed");
                map.TileProvider.GetComponent<RangeTileProvider>()._rangeTileProviderOptions.south++;
                MapBoxupdate();
                updateText("Map extended (South)");
            }
        }
        else {
            PlayerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (SteamVR_Actions._default.OverviewRight.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad right pressed");
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(PlayerPos.x , PlayerPos.y, PlayerPos.z + cameraMove);
                updateText("Your z positon + " + cameraMove);
            }
            if (SteamVR_Actions._default.OverviewUp.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad up pressed");
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(PlayerPos.x - cameraMove, PlayerPos.y , PlayerPos.z );
                updateText("Your x positon - " + cameraMove);
            }
            if (SteamVR_Actions._default.OverviewDown.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad up pressed");
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(PlayerPos.x + cameraMove, PlayerPos.y, PlayerPos.z);
                updateText("Your x positon + " + cameraMove);
            }
            if (SteamVR_Actions._default.OverviewLeft.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                //Debug.Log("left touchpad left pressed");
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(PlayerPos.x, PlayerPos.y, PlayerPos.z - cameraMove);
                updateText("Your z positon - " + cameraMove);

            }
        }
        

    }

}
