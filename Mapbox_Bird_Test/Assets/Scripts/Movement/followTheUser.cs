using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTheUser : MonoBehaviour { 
    public GameObject vrplayer;
    float player_y;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player_y = vrplayer.transform.position.y;
        this.transform.position = new Vector3(this.transform.position.x,player_y,this.transform.position.z);
        
    }
}
