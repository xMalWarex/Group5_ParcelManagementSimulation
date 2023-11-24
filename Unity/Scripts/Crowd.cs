using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public Student studentPrefab;
    List<Student> crowd = new List<Student>();
 
    public int startingCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = groundpos.y + grounddim.y/2;

        while (crowd.Count < startingCount) {
            var x = Random.Range(groundpos.x-grounddim.x/2, groundpos.x+grounddim.x/2);
            var z = Random.Range(groundpos.z-grounddim.z/2, groundpos.z+grounddim.z/2);
            Vector3 spawnPos = new Vector3(x, y, z);

            Student student = Instantiate(studentPrefab, 
                                    spawnPos,
                                     Quaternion.identity);
            student.name = "Student-"+crowd.Count;            
            crowd.Add(student);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            for (int i=0; i<3; i++) { // try 3 times
                Student student = crowd[Random.Range(0, crowd.Count)];
                if (student.IsInQueue() == false && student.MovingToQueue() == false) {
                    string q = student.MoveToQueue();
                    Debug.Log("Moving student "+student.name+" to queue "+q);
                    break;
                }
            }
        }
    }

}