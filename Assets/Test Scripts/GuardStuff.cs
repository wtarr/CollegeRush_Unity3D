using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardStuff : MonoBehaviour
{
    public GameObject target;
    List<Vector3> targetsPositionList;
    private int coolDownTimer;
    bool chase, backtrack = false;
    int count = 1;
    float walkSpeed = 10, turnSpeed = 0.05f;
       
    Vector3 next = Vector3.zero;
    private bool addStart;

    private RaycastHit hitInfo;

    // Use this for initialization
    void Start()
    {

        targetsPositionList = new List<Vector3>();

    }

    // Update is called once per frame
    void Update()
    {

        if (targetsPositionList.Count > 1)
        {
            Vector3 previ = Vector3.zero;

            foreach (var item in targetsPositionList)
            {

                if (previ != Vector3.zero)
                {
                    Debug.DrawLine(previ, item, Color.blue, 2, false);
                }

                previ = item;
            }
        }

        if (chase)
            Chase();

        if (backtrack)
            BackTrack();
        
        if ((chase || backtrack) && next != Vector3.zero)
        {
            transform.LookAt(next);
            transform.position += walkSpeed * transform.forward * Time.deltaTime;
        }

        targetsPositionList.RemoveAll(o => o == Vector3.zero);
        
    }

    void FixedUpdate()
    { 
        if (Physics.Raycast(transform.position, (target.transform.position - transform.position), out hitInfo))
        {

            //Debug.Log(hitInfo.collider.name);
            if (hitInfo.collider.name.Equals("Player"))
            {

                TrackTarget(target.transform.position);

                chase = true;

            }
            else
            {
                if (chase)
                {
                    chase = false;

                    backtrack = true;                   

                }
            }
            
        }

        
    }

    //void OnTriggerEnter(Collider other)
    //{ 
        
    //}

    //void OnTriggerStay(Collider other)
    //{       
    //    chase = true;
    //    addStart = true;
    //    TrackTarget(other.transform.position);


    //}
   
    //void OnTriggerExit(Collider other)
    //{
    //    chase = false;

    //    backtrack = true;


    //}

    private void TrackTarget(Vector3 position)
    {
        coolDownTimer--;

        if (coolDownTimer <= 0)
        {

            targetsPositionList.Add(position);

            coolDownTimer = 30;
        }


    }

    private void Chase()
    {
        if (addStart && targetsPositionList.Count == 0)
        {
            targetsPositionList.Add(transform.position);
            addStart = false;
        }
        
        if (count < targetsPositionList.Count)
        {                       
            if (next == Vector3.zero && targetsPositionList.Count > 0)
                next = targetsPositionList[count];

            if ((int)transform.position.x == (int)next.x && (int)transform.position.y == (int)next.y && (int)transform.position.z == (int)next.z)
                
            {
                // Its reached its destination
                Debug.Log("Next");
                               
                count++;

                if (count < targetsPositionList.Count)
                    next = targetsPositionList[count];

            }


        }
        else
        {
            chase = false;
            backtrack = true;
        }
    }

    private void BackTrack()
    {
        if (count >= 0)
        {

            if ((int)transform.position.x == (int)next.x && (int)transform.position.y == (int)next.y && (int)transform.position.z == (int)next.z)
               
            {
                // Its reached its destination
                Debug.Log("Prev");                                        
                
                count--;

                if (count >= 0)
                {                   
                    next = targetsPositionList[count];
                }
            }


        }
        else
        {
            backtrack = false;
            
            Debug.Log("back to false " + targetsPositionList.Count);

            targetsPositionList.Clear();
            addStart = true;


        }
    }




}
