using UnityEngine;
using System.Collections;

public class DoorInfo {

    GameObject door;
    Quaternion closedRotation;
    int timer;
    bool isLocked;
    bool isOpen;
    bool open, close;
    bool isObjective;

    public bool IsObjective
    {
        get { return isObjective; }
        set { isObjective = value; }
    }
                
    public GameObject Door
    {
        get { return door; }
        set { door = value; }
    }

    public Quaternion ClosedRotation
    {
        get { return closedRotation; }
        set { closedRotation = value; }
    }

    public int Timer
    {
        get { return timer; }
        set { timer = value; }
    }

    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }

    public bool Open
    {
        get { return open; }
        set { open = value; }
    }

    public bool Close
    {
        get { return close; }
        set { close = value; }
    }

    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }
        
    
}
