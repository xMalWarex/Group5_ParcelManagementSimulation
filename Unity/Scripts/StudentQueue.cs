using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentQueue : MonoBehaviour
{
    public QueueList queueList;

    List<Student> studentqueue = new List<Student>();

    //string counterId;
    Vector3 qdir;  // queue direction
    Vector3 qPos;
    Student incomingStudent;
    float queueSpacing = 5;
  

    // Start is called before the first frame update
    void Start()
    {
        qdir = -transform.right; // the red vector
        qPos = transform.position + qdir*(studentqueue.Count+1)*queueSpacing;
        queueList.Add(this);

        Debug.Log("Queue created for "+gameObject.name+". With pos: "+qPos);

    }

    public bool Add(Student student) {
        if (incomingStudent != null) {
            return false;
        }
        incomingStudent = student;
        qPos.y = student.GetPos().y;
        student.SetDestination(qPos);
        queueSpacing = student.GetRadius()*2f;
        qPos += qdir*queueSpacing;
        return true;        
    }

    public void Shift() {
        foreach (Student student in studentqueue) {
            student.transform.position -= qdir*queueSpacing;
        }
        qPos -= qdir*queueSpacing; 
    }

    public Student Pop() {
        Student student = studentqueue[0];
        studentqueue.RemoveAt(0); 
        student.MoveFromQueue();

        student.SetRandomDestination();
        Shift();
        return student;  
    }

    public int Size() {
        return studentqueue.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingStudent == null){
            return;
        }

        if (incomingStudent.IsInQueue()) {
            incomingStudent.TurnOffNavMeshAgent(qPos);
            studentqueue.Add(incomingStudent);
            incomingStudent = null;
            Debug.Log("Queue for "+gameObject.name+" now has size "+studentqueue.Count+". Next pos: "+qPos);
        }    
    }
}