using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DoorManager : MonoBehaviour
{
    public Material lockedDoorMaterial, unlockedDoorMaterial;

    private float openRate = 0.2f;
    private float openAngle = -90;
    private float closedAngle = 0;
    private Color doorColorBackup;
    private int doorUnlockCoolDownTimer;



    private static List<DoorInfo> doorInfoList;    
    private GameObject[] allTheDoors;

    public static List<DoorInfo> DoorInfoList
    {
        get { return doorInfoList; }
        set { doorInfoList = value; }
    }

    // Use this for initialization
    void Start()
    {

        doorInfoList = new List<DoorInfo>();

        allTheDoors = GameObject.FindGameObjectsWithTag("door");

        foreach (var item in allTheDoors)
        {
            DoorInfo dI = new DoorInfo();

            dI.Door = item;
            dI.ClosedRotation = item.transform.rotation;
            dI.Timer = 0;
            dI.IsLocked = true;
            dI.IsOpen = false;            
            doorInfoList.Add(dI);

            
        }

        Debug.Log(doorInfoList.Count);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in doorInfoList)
        {
            if (item.Open & !item.Close )
                OpenDoor(item);                     
            
            if (item.Close & !item.Open )
                CloseDoor(item);

            if (item.Timer > 0)
            {
                item.Timer--;
            }

            if (item.Timer <= 0 && !item.IsLocked)
            {
                //Debug.Log("Unlocking");
                item.IsLocked = true;
                Material[] mats = item.Door.renderer.materials;
                mats[1] = lockedDoorMaterial;
                item.Door.renderer.materials = mats;
            }
        }

        if (doorUnlockCoolDownTimer <= 0)
            UnlockDoor();
        else
            doorUnlockCoolDownTimer--;
               
                
    }

    private void UnlockDoor()
    {
        doorUnlockCoolDownTimer = 20;

        System.Random randomDoorNumber = new System.Random();
        int magicDoorToBeUnlocked = randomDoorNumber.Next(doorInfoList.Count);

        if (doorInfoList[magicDoorToBeUnlocked].IsLocked)
        {
            doorInfoList[magicDoorToBeUnlocked].IsLocked = false;
            doorInfoList[magicDoorToBeUnlocked].Timer = 200;
            Material[] mats = doorInfoList[magicDoorToBeUnlocked].Door.renderer.materials;
            mats[1] = unlockedDoorMaterial;
            doorInfoList[magicDoorToBeUnlocked].Door.renderer.materials = mats;


            //Debug.Log("Door" + magicDoorToBeUnlocked);
        }
    }

    void CloseDoor(DoorInfo door)
    {
        //Debug.Log("Closing");

        door.Door.transform.localRotation = Quaternion.Slerp(door.Door.transform.localRotation, door.ClosedRotation, Time.deltaTime * 2);
                
    }

    private void OpenDoor(DoorInfo door)
    {

        //Debug.Log("Opening");
                        
        var targ = Quaternion.Euler(0, openAngle, 0);

        door.Door.transform.localRotation = Quaternion.Slerp(door.Door.transform.localRotation, targ, Time.deltaTime * 2);

        

    }
}
