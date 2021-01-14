﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VissimPlayback : MonoBehaviour
{
    public Dictionary<int, VissimCar> vehicles = new Dictionary<int, VissimCar>();
    [SerializeField] private ConvertFZP fileConverter = default;

    public float timeCounter;
    public int loopCounter = 0;
    public int loopCounterFuture = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeCounter = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(fileConverter.finishedLoadingData) 
        {
            SendCommand(fileConverter.allVissimData);
        }
        else
        {
            loopCounter = 0;
            loopCounterFuture = 0;
        }
    }
    /// <summary>
    /// Checks the current simulation time and sends according commands to all vehicles.
    /// </summary>
    /// <param name="dataList"></param>
    public void SendCommand(List<VissimData> dataList)
    {
        
        if (Time.time > timeCounter)
        {
            timeCounter = Time.time + fileConverter.timeBetweenFrames; // runs the simulation at the imported simspeed
            fileConverter.frameCounter++;
           
            for (int i = loopCounter; i < dataList.Count; i++)
            {
                if(fileConverter.frameCounter != dataList[i].simsec)
                {
                    loopCounter = i;
                    loopCounterFuture = i;
                    break;
                }
                else
                {
                    if (vehicles.ContainsKey(dataList[i].id))
                    {
                        // send vehicle command
                        vehicles[dataList[i].id].vehicleCommandData = dataList[i];
                    }
                    else
                    {
                        // change 0 with i when you have more models
                        GameObject tempObject = Instantiate(fileConverter.vehicleTypes[dataList[i].vehicleType][Random.Range(0, fileConverter.vehicleTypes[dataList[i].vehicleType].Length )], transform.position, new Quaternion(0f,0f,0f,0f));
                        tempObject.transform.SetParent(this.transform);

                        VissimCar carInstance = tempObject.GetComponent<VissimCar>();
                        vehicles.Add(dataList[i].id, carInstance);
                        vehicles[dataList[i].id].vehicleCommandData = dataList[i];
                    }
                }
            }
            // checks the point where the car is heading to
            for (int i = loopCounterFuture; i < dataList.Count; i++)
            {
                if (fileConverter.frameCounter + fileConverter.timeBetweenFrames != dataList[i].simsec)
                {
                    break;
                }
                else
                {
                    if (vehicles.ContainsKey(dataList[i].id))
                    {
                        // send vehicle command
                        vehicles[dataList[i].id].futurePosition = dataList[i].coordRear;
                        vehicles[dataList[i].id].MoveAnimation(vehicles[dataList[i].id].vehicleCommandData.coordRear, vehicles[dataList[i].id].futurePosition, fileConverter.timeBetweenFrames);
                    }
                }
            }

        }
        

    }
}