using UnityEngine;
using UnityEngine.UI;
using System;
public class timelineController : MonoBehaviour
{
    public GameObject timeline;
    public GameObject textIndicator;

    public GameObject dataManager;

    public GameObject polylineManager;
    float speed;
    double overallTime;
    public  double timeDiffSec;
    //float timeDiffSecFloat;
    DateTime tMin;
    DateTime tMax;
    
    [SerializeField] private double currentAmount;
    
    void Start()
    {
        
        speed = dataManager.GetComponent<process>().speed;
        string Min = polylineManager.GetComponent<line_gen>().curMin;
        Min = Min.Replace('-', '/');
        DateTime.TryParse(Min, out tMin);
        string Max = polylineManager.GetComponent<line_gen>().curMax;
        Max = Max.Replace('-', '/');
        DateTime.TryParse(Max, out tMax);
        TimeSpan TimeDiff = tMax-tMin;
        timeDiffSec = TimeDiff.TotalSeconds;
        //polylineManager.GetComponent<line_gen>().timeDiffSec = timeDiffSec;
        //float timeDiffSecFloat = Convert.ToInt32(timeDiffSec);
        //check  
        //Debug.Log("Timeline"+timeDiffSecFloat);
    }
    // Update is called once per frame
    void Update()
    {
        speed = dataManager.GetComponent<process>().speed;
        string Min = polylineManager.GetComponent<line_gen>().curMin;
        Min = Min.Replace('-', '/');
        DateTime.TryParse(Min, out tMin);
        if (polylineManager.GetComponent<line_gen>().OverallTime == 0) {
            //it will be reset to 0 when restart(or initialization)
            currentAmount = 0;
            timeline.gameObject.SetActive(true);
        }
        if (currentAmount < 100)
        {
            //currentAmount += speed * Time.deltaTime/ timeDiffSec;
            overallTime = polylineManager.GetComponent<line_gen>().OverallTime;
            currentAmount = overallTime / timeDiffSec;

            string textInhalt = tMin.AddSeconds(overallTime).ToString("yyyy-MM-dd HH:mm:ss");
            textIndicator.GetComponent<Text>().text = textInhalt;
        }
        if (timeline.transform.GetComponent<Slider>().value == 1) {

            //Debug.Log("Timeline need reset");
            timeline.gameObject.SetActive(false);
            
            if (polylineManager.GetComponent<line_gen>().restart == true) {
                polylineManager.GetComponent<line_gen>().OverallTime = 0;
                currentAmount = 0;
                timeline.gameObject.SetActive(true);
            }
            
        }
        timeline.transform.GetComponent<Slider>().value = (float)currentAmount;
    }
}
