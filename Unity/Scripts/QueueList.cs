using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName="Behavior/QueueList")]
public class QueueList : ScriptableObject
{

    [System.NonSerialized]
    List<StudentQueue> queues = new List<StudentQueue>();

    public StudentQueue Get(int i) {
        return queues[i];
    }

    public int Count() {
        return queues.Count;
    }

    public void Add(StudentQueue queue) 
    {
        queues.Add(queue);
    }
}
