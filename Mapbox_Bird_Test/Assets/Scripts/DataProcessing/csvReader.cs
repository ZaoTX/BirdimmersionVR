using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class csvReader : MonoBehaviour
{
    //public List<Individual> IndividualBehaviors { get; set; } = new List<Individual>();
    public Group gp { get; set; } = new Group();
    //public int[] highresolutionList;
    //public string path = "C:\\Users\\ZiyaoHe\\Documents\\BirdImmersive\\white storks\\Blacky\\Blacky.csv";
    // Start is called before the first frame update
    string last_id;
    string id_name = "TDTR_sample_5.0meters";//ThangKhaar4
    void Start()
    {
        last_id = null;

        ReadCSVFile();
        
    }
    
    void ReadCSVFile()
    {
        //from Resources folder load data
        TextAsset Individual_data = Resources.Load<TextAsset>(id_name);
        string[] data = Individual_data.text.Split(new char[] {'\n'});//split for each line

        //initialize a list of tracks
        IndividualBehavior individualBehaviors = new IndividualBehavior();
        //for the whole dateset
        for (int i = 1; i <= data.Length-1; i++)//when there is no header i=0
        {

            string[] row = data[i].Split(new char[] { ',' });

            //Debug.Log(row.Length);
            //int a = row.Length;
            string cur_id = row[0];
            // check if last id is equal this id
            
            if (last_id!=null&&cur_id != last_id)
            {
                if (row.Length == 5)
                {
                 //store the last individualBehavior
                    gp.individualBehaviors.Add(individualBehaviors);
                    //create a new InidividualBehavior to store the data
                    individualBehaviors = new IndividualBehavior();
                    //store to individual typ
                    Individual individual = new Individual();
                    individual.id = cur_id;
                    individual.timestamp = row[1];
                    //Debug.Log(individual.timestamp);
                    individual.lng = row[2];
                    individual.lat = row[3];
                    float.TryParse(row[4], out individual.height);
                    //Debug.Log(individual.height);
                    //individual.height = individual.height - 1000;
                    if (individual.height < 0)
                    {
                        individual.height = 0;
                        Debug.LogError("there are height lower equal 0s");
                    }

                    individualBehaviors.Individualtracks.Add(individual);
                }
                
                
            }
            else
            {
                if (row.Length == 5) {
                    //that include 2 possiblities: last id= null(initial) or the id doesn't change
                    //We just store the id into individualBehaviors
                    //store to individual typ
                    Individual individual = new Individual();
                    individual.id = cur_id;
                    individual.timestamp = row[1];
                    //Debug.Log(individual.timestamp);
                    individual.lng = row[2];
                    individual.lat = row[3];
                    float.TryParse(row[4], out individual.height);
                    //individual.height = individual.height - 1000;
                    if (individual.height < 0)
                    {

                        individual.height = 0;
                        Debug.LogError("there are height lower equal 0s");//warn the user
                    }
                    //get the last individualBehaviors to store data

                    individualBehaviors.Individualtracks.Add(individual);
                    //Debug.Log(individualBehaviors.Individualtracks.Count + " < " + individual.height);
                }


            }
            //update id
            last_id = cur_id;
            if (i== data.Length - 1)
            {
                gp.individualBehaviors.Add(individualBehaviors);
            }


        }
        /*if (gp.individualBehaviors[2].Individualtracks != null)
        {
            for (int i = 0; i < gp.individualBehaviors[2].Individualtracks.Count; i++)
            {
                Debug.Log(gp.individualBehaviors[2].Individualtracks[i].height);
            }
        }
        if (gp.individualBehaviors[2].Individualtracks == null)
        {
            Debug.Log("null");
        }*/
    }
}
