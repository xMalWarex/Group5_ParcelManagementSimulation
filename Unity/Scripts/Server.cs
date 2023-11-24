
// this doesnt work in Unity
//using MathNet.Numerics.Distributions;  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class Server : MonoBehaviour {

    public float mean;
    public float std;

    public StudentQueue customers;

    Student currCustomer = null!;
    bool serverFree = true;
    int serviceTime = 0;
    int cumulativeQSize = 0;
    int maxQSize = 0;
    Normal serviceGaussian;

    DateTime prev;
    const int updateRate = 1;
    long time;
    float lastUpdate;

    void Start() {
        serviceGaussian = new Normal(mean, std);
        prev = DateTime.Now;
    }

    void Update() {
        time++;
        lastUpdate += Time.deltaTime;
        if (lastUpdate > updateRate) {
            update(customers, time);
            lastUpdate = 0;
        }
    }

    public Student getCurrCustomer() {
        return currCustomer;
    }

    public int getCumulativeQSize() {
        return cumulativeQSize;
    }

    public int getMaxQSize() {
        return maxQSize;
    }

    public bool update(StudentQueue studentQueue, long Time) {

        int currQSize = studentQueue.Size();
        cumulativeQSize += currQSize;
        if (currQSize > maxQSize)
            maxQSize = currQSize;

        if (serverFree == false) {
            serviceTime --;
            Debug.Log(serviceTime+" second remaining for server "+name);


            if (serviceTime <= 0) {
                Debug.Log(name + " server done at time="+Time);
                currCustomer = studentQueue.Pop();

                serverFree = true;
                return true;    // done
            }
        }

        if (serverFree == true && studentQueue.Size() > 0) {
            //serviceTime = (int) serviceGaussian.Sample();
            serviceTime = 10;
            Debug.Log(name+" server starts. To finish in "+serviceTime);
            serverFree = false;
        }

        return false;
    }


}