using UnityEngine;
using System.Collections;

public class Opendoor : MonoBehaviour {
    
    private RaycastHit hit;
    private Ray ray;

    private bool openTheDoor;

    private DoorInfo door;

    private int coolDownTimer;

	// Use this for initialization
	void Start () {

        openTheDoor = false;
                                
	}
	
	// Update is called once per frame
	void Update () {

        
        if (Input.GetMouseButton(0))
        {  
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.transform.tag.Equals("door"))
                {
                    //Debug.Log(hit.transform.name);
                    door = DoorManager.DoorInfoList.Find(d => d.Door.transform.name.Equals(hit.transform.name));
                    //Debug.Log(door.Door.name);
                    if (!door.IsLocked && !door.IsOpen) // all doors unlocked for now
                    {
                        if (coolDownTimer <= 0)
                        {
                            door.IsOpen = true;
                            door.Open = true;
                            door.Close = false;
                            coolDownTimer = 100;
                            
                        }
                    }
                    else if (door.IsOpen)
                    {
                        if (coolDownTimer <= 0)
                        {
                            door.IsOpen = false;
                            door.Open = false;
                            door.Close = true;
                            coolDownTimer = 100;                            
                        }
                    }
                    
                }
            }
        }

        if (coolDownTimer > 0)
            coolDownTimer--;
       

	}
       
    
}
