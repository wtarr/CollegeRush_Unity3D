using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GuardPatrol : MonoBehaviour {

    #region Class State
    public GameObject target;

    List<GameObject> allWayPointsList;

    List<GameObject> waypoints_Level_3;
    List<GameObject> waypoints_Level_2;
    List<GameObject> waypoints_Level_1;

    List<GameObject> currentWaypointList;

    List<GameObject> stairs1WayPointList;
    List<GameObject> stairs2WayPointList;

    List<GameObject> currentStairsWayPointList;

    GameObject currentWaypoint;

    bool forward;

    bool onStairs;

    public float walkSpeed = 10;

    int indexOfCurrent, indexOfLast;

    int currentLevel;

    CharacterController cc;

    float distanceToTarget;

    // Chase related stuff
    List<Vector3> targetsPositionList;
    private int coolDownTimer;
    bool chase = false;
    

    Vector3 next = Vector3.zero;
    private bool addStart;

    RaycastHit hitInfo;
    private float targetSightingDistance = 10;
    public float acceptableDistance = 2f; 
    #endregion

    #region Start Method
    // Use this for initialization
    void Start()
    {        

        targetsPositionList = new List<Vector3>();

        waypoints_Level_3 = new List<GameObject>();
        waypoints_Level_2 = new List<GameObject>();
        waypoints_Level_1 = new List<GameObject>();

        allWayPointsList = new List<GameObject>(GameObject.FindGameObjectsWithTag("waypoint"));

        stairs1WayPointList = new List<GameObject>(GameObject.FindGameObjectsWithTag("stairs_1"));
        stairs2WayPointList = new List<GameObject>(GameObject.FindGameObjectsWithTag("stairs_2"));
        // Sort the list
        stairs1WayPointList.Sort((item1, item2) => item1.name.CompareTo(item2.name));
        stairs2WayPointList.Sort((item1, item2) => item1.name.CompareTo(item2.name));


        foreach (var item in allWayPointsList)
        {
            if (item.name.Substring(0, 1).Equals("1"))
                waypoints_Level_1.Add(item);
            if (item.name.Substring(0, 1).Equals("2"))
                waypoints_Level_2.Add(item);
            if (item.name.Substring(0, 1).Equals("3"))
                waypoints_Level_3.Add(item);
        }
        // Sort the lists
        waypoints_Level_1.Sort((item1, item2) => item1.name.CompareTo(item2.name));
        waypoints_Level_2.Sort((item1, item2) => item1.name.CompareTo(item2.name));
        waypoints_Level_3.Sort((item1, item2) => item1.name.CompareTo(item2.name));

        forward = true;
        
        currentWaypoint = FindClosestWaypoint(allWayPointsList);
        
        
        SelectAppropriateWaypointList();
        
        transform.LookAt(currentWaypoint.transform);

    } 
    #endregion

    #region Find Closest Waypoint
    GameObject FindClosestWaypoint(List<GameObject> wpList)
    {
        GameObject closest = wpList[0];

        float disttoclosest = Vector3.Distance(transform.position, closest.transform.position);


        foreach (var item in wpList)
        {
            float dist = Vector3.Distance(transform.position, item.transform.position);

            if (dist < disttoclosest)
            {
                closest = item;
                disttoclosest = dist;
            }
        }

        currentLevel = int.Parse(closest.name.Split('_').First());

        return closest;
    } 
    #endregion

    #region Select Appropriate Way Point List
    private void SelectAppropriateWaypointList()
    {
        switch (currentLevel)
        {
            case 1:
                currentWaypointList = waypoints_Level_1;
                break;
            case 2:
                currentWaypointList = waypoints_Level_2;
                break;
            case 3:
                currentWaypointList = waypoints_Level_3;
                break;
            default:
                break;


        }

        indexOfCurrent = currentWaypointList.IndexOf(currentWaypoint);
        indexOfLast = currentWaypointList.Count - 1;

    } 
    #endregion

    #region GUI
    void OnGUI()
    {
        //GUI.Label(new Rect(10, 10, 100, 30), string.Format("Gaurds Level: {0}", currentLevel));
    } 
    #endregion

    #region Go To Next
    void GoToNext(List<GameObject> wpList)
    {
        if (forward)
            indexOfCurrent++;
        else
            indexOfCurrent--;

        if (indexOfCurrent > indexOfLast)
        {
            indexOfCurrent -= 2;
            forward = false;
        }

        if (indexOfCurrent < 0)
        {
            indexOfCurrent += 2;
            forward = true;
        }

        currentWaypoint = wpList[indexOfCurrent];

        transform.LookAt(currentWaypoint.transform);

        //Debug.Log(currentWaypoint.name);


    } 
    #endregion

    #region Update Method
    // Update is called once per frame
    void Update()
    {       
        if(!chase)
            transform.position += transform.forward * walkSpeed * Time.deltaTime;

        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (chase && currentLevel != target.GetComponent<Player>().Level) // player is now on a different level, stop the chase 
        {
            chase = false;

            Destroy(gameObject.GetComponent<AIPathModified>());
            Destroy(gameObject.GetComponent<Seeker>());
           
            currentWaypoint = FindClosestWaypoint(allWayPointsList);
            SelectAppropriateWaypointList();

            

        }

        if (!chase && currentWaypoint == null)
        {

            currentWaypoint = FindClosestWaypoint(allWayPointsList);
            SelectAppropriateWaypointList();           
        
        }

        if (currentWaypoint != null)
            transform.forward = Vector3.Normalize(currentWaypoint.transform.position - transform.position);

        if (currentWaypoint != null)
            Debug.DrawLine(transform.position, currentWaypoint.transform.position, Color.red, 0.5f, false);
   


    } 
    #endregion
    
    #region Fixed Update Method
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, (target.transform.position - transform.position), out hitInfo))
        {
            Debug.Log(hitInfo.collider.name);

            if (!chase && hitInfo.collider.name.Equals("Player") && distanceToTarget < targetSightingDistance && !onStairs && target.GetComponent<Player>().Level == currentLevel)
            {

                currentWaypoint = null;
                
                Debug.DrawLine(transform.position, target.transform.position, Color.green, 2, false);

                chase = true;

                gameObject.AddComponent<AIPathModified>();
                AIPathModified ai = gameObject.GetComponent<AIPathModified>();
                ai.target = target.transform;
                ai.speed = walkSpeed;

            }           

        }
    } 
    #endregion

    #region On Trigger Enter Method
    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Triggered outer " + collider.gameObject.name);       
        if (!chase)
        {
            string colliderName = collider.gameObject.name;

            string[] splitName = colliderName.Split('_');

            if (colliderName.Equals(currentWaypoint.name)) // its found and hit its tasked waypoint
            {

                if (!onStairs)
                {
                    currentLevel = int.Parse(splitName.First());

                    if (splitName.GetValue(3).Equals("DECIDE")) // Make the decision to get on the stairs or not
                    {
                        //Debug.Log("DECIDE");
                        if (UseStairs())
                        {
                            //Debug.Log("Using stairs");
                            onStairs = true;
                            GetOnStairs(int.Parse(splitName.GetValue(4).ToString()), int.Parse(splitName.GetValue(0).ToString()));
                            GoToNext(currentStairsWayPointList); // for now
                        }
                        else
                        {

                            GoToNext(currentWaypointList); // continue on

                        }
                    }
                    else
                    {
                        GoToNext(currentWaypointList);
                    }

                }
                else
                {

                    GoToNext(currentStairsWayPointList); // as you were, keep patrolling the stairs

                    if (splitName.GetValue(1).Equals("DECIDE"))
                    {
                        if (!UseStairs())  // make the decision to get off the stairs
                        {
                            // revert back to landing patroll
                            onStairs = false;
                            currentWaypoint = FindClosestWaypoint(allWayPointsList);
                            SelectAppropriateWaypointList();

                            transform.LookAt(currentWaypoint.transform);
                        }

                    }

                }

            }
        }

    } // On trigger enter 
    #endregion

    #region  Use Stairs
    private bool UseStairs()
    {
        System.Random rand = new System.Random();
        int decision = rand.Next(2);
        //Debug.Log(decision);

        if (decision == 0)
            return false;
        else
            return true;


    } 
    #endregion

    #region Get On Stairs Method
    private void GetOnStairs(int stairs, int level)
    {

        switch (stairs)
        {
            case 1:
                currentStairsWayPointList = stairs1WayPointList;
                break;
            case 2:
                currentStairsWayPointList = stairs2WayPointList;
                break;
        }


        currentWaypoint = FindClosestWaypoint(currentStairsWayPointList);


        indexOfCurrent = currentStairsWayPointList.IndexOf(currentWaypoint);
        indexOfLast = currentStairsWayPointList.Count - 1;





        // need to know the stairs in question to select the appropriate list
        // need to know what level so to decide can go up(L2, L1)? can only go up(L1)? can go down(L3, L2)? can only go down(L3)? 
        // decide if to get off stairs and hand control back to landing waypoints


    } 
    #endregion

    public void Reset()
    {
        System.Random rand = new System.Random();

        GameObject randomWaypoint = allWayPointsList[rand.Next(0, allWayPointsList.Count)];

        transform.position = randomWaypoint.transform.position;

        chase = false;

        Destroy(gameObject.GetComponent<AIPathModified>());
        Destroy(gameObject.GetComponent<Seeker>());

        currentWaypoint = FindClosestWaypoint(allWayPointsList);
        SelectAppropriateWaypointList();
    
    }
}
