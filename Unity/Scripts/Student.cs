using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Student : MonoBehaviour
{
    static string[] possibleTags = {"Group-1", "Group-2"};

    NavMeshAgent navstudent;
    public QueueList queueList;

    System.Random rnd = new System.Random();

    DateTime motionT;
    Vector3 lastPosition;

    DateTime mouseT;
    const float mouseTime = 5;
    StudentState currState;
    int destinationQueueCounter;

    const float neighRadius = 10;
    GameObject[] queuecounters; // TODO: make this static/scriptable

    Collider studentCollider;
    public Collider StudentCollider {get { return studentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        studentCollider = GetComponent<CapsuleCollider>();
        navstudent = GetComponent<NavMeshAgent>();

        int tagIndex = UnityEngine.Random.Range(0, possibleTags.Length);
        this.tag = possibleTags[tagIndex];
        Debug.Log("Created a "+this.tag+" student");

        // student chooses to go to a random queuecounter
        queuecounters = GameObject.FindGameObjectsWithTag("QueueCounter");
        int queuecounter = GetRandomQueueCounter();
        GotoQueueCounter(queuecounter);
        currState = StudentState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        if (currState == StudentState.InQueue)
            return;

        if (currState == StudentState.ToQueue) {
            if (ReachedDestination()) {
                currState = StudentState.InQueue;
            }

            return;
        }

        if (currState == StudentState.FollowingMouse) {
            if (ReachedDestination()) {
                currState = StudentState.Wandering;
            }

            DateTime mouseTNow = DateTime.Now;
            var diffTime = mouseTNow - mouseT;
            if (diffTime.Seconds > mouseTime)
                currState = StudentState.Wandering;
            return;
        }

        if (Input.GetMouseButtonDown(1)) {
                if (rnd.Next(2) == 0)
                    return;
                
                Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(movePosition, out var hitInfo)) {
                    SetDestination(hitInfo.point);
                    currState = StudentState.FollowingMouse; 
                    mouseT = DateTime.Now;       
                }
            } 
        else {
            if (ReachedDestination(10)) {
                if (this.name == "Student-19")
                    Debug.Log("Student "+this.name +" is moving elsewhere");


                bool joinQ = UnityEngine.Random.value < 0.5;
                if (joinQ) {
                    joinQ = MoveToQueue(destinationQueueCounter);
                }

                if (!joinQ ) {
                    destinationQueueCounter = GetRandomQueueCounter();
                    GotoQueueCounter(destinationQueueCounter);
                    }
                }
            }
    }

    int GetQueueCounter() {
        return destinationQueueCounter;
    }

    int GetRandomQueueCounter() {
        
        int index = UnityEngine.Random.Range(0, queuecounters.Length);        
        return index;
    }

    void GotoQueueCounter(int index) {
        SetDestination(RandomQueueCounterPos(index));
    }

    int CheckGangDestination() {
        List<Student> students = GetNearbyStudents(true);
        Vector3 center = Vector3.zero;
        if (students.Count == 0) {
            return GetQueueCounter();
        }        
        
        int numQueueCounters = queuecounters.Length;
        int[] vote = new int[numQueueCounters];

        foreach (Student student in students) {
            if (student.IsInQueue() || student.MovingToQueue() || student.MovingToMouse())
                continue;
            vote[student.GetQueueCounter()] ++;
         }

        int max = 0;
        int index = 0;
        for (int i=0; i<numQueueCounters; i++) {
            if (vote[i] > max) {
                max = vote[i];
                index = i;
            }
        }

        return index;
    }


    List<Student> GetNearbyStudents(bool aliketype) {
        Collider[] hitColliders = new Collider[20];
        float radius = neighRadius;
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

        List<Student> context = new List<Student>();
        for (int i=0; i<numColliders; i++)  {
            Collider c = hitColliders[i];
            Student student = c.GetComponent<Student>();
            if (student == null)
                continue;

            if (student.CompareTag(this.tag) != aliketype)
                    continue;
                    
            if (student.name == this.name) 
                continue;
            
            context.Add(student);
        }

        return context;
    }

    public bool MovingToQueue() {
        return currState == StudentState.ToQueue;
    }

    public bool IsInQueue() {
        return currState == StudentState.InQueue;
    }

    public bool MovingToMouse() {
        return currState == StudentState.FollowingMouse;
    }

    public bool MoveToQueue(int qindex)
    {
        StudentQueue chosenQ = queueList.Get(qindex);
        bool success = chosenQ.Add(this);
        if (!success)
            return false;        
        currState = StudentState.ToQueue;
        return true;
    }

    public string MoveToQueue()
    {
        StudentQueue chosenQ = queueList.Get(rnd.Next(queueList.Count()));
        bool success = chosenQ.Add(this);
        if (!success)
            return "None";
        currState = StudentState.ToQueue;
        return chosenQ.name;
    }

    public void MoveFromQueue()
    {
        TurnOnNavMeshAgent();
    }

    public void SetDirection(Vector3 dir) {
        navstudent.Move(dir);
    }

    public void SetDestination(Vector3 pos) {
        navstudent.SetDestination(pos);
        motionT = DateTime.Now;
    }

    public float GetTimeInMotion() {
        return (DateTime.Now - motionT).Seconds;
    }

    public bool ReachedDestination() {
      return (navstudent.remainingDistance <= navstudent.stoppingDistance);
    }

    public bool ReachedDestination(float maxsec) {
      if (navstudent.remainingDistance <= navstudent.stoppingDistance) {
        return true;
      }

      if (GetTimeInMotion() > maxsec) {
        return true;
      }

      return false;
    }


    public Vector3 RandomQueueCounterPos(int index) {
        GameObject queuecounter = queuecounters[index];
        Vector3 queuecounterdim = queuecounter.transform.localScale;
        Vector3 queuecounterpos = queuecounter.transform.position;
        Vector3 randPos = new Vector3(-4, 1, 0);

        return randPos;
    }

    public Vector3 RandomNearPosition() {

        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;

        Vector3 randPos = new Vector3(-2, 1, 0);
        Vector3 rdir = randPos - transform.position;

        return rdir; //*5;
  
    }


    public void SetRandomDestination() {
        SetDestination(transform.position + RandomNearPosition());
        currState = StudentState.Wandering;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
                }
            }
        result = Vector3.zero;
        return false;
        }

    public Vector3 GetPos() {
        return transform.position;
    }

    public float GetRadius() {
        return ((CapsuleCollider)studentCollider).radius; 
    }

    public void TurnOffNavMeshAgent(Vector3 pos) {
        navstudent.updatePosition = false;        
        transform.position = pos;
    }

    public void TurnOnNavMeshAgent() {
        navstudent.updatePosition = true;   
    }
}
