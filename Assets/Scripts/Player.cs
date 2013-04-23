using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Texture2D timerTexture;
    public GameObject spawn;
    public Texture crosshairs;
    public GUIStyle styleNotification;
    public GUIStyle styleLabel;
    public GUIStyle styleInfo;    
    private int textureHeightWidth = 100;
    string collisionMessage;
    int level;    
    float y;

    private RaycastHit hit;
    private Ray ray;
    private bool openTheDoor;
    private DoorInfo door;
    private int coolDownTimer;

    int missedClassStrikes = 10;


    int maxTimeToGetToClass = 2000;
    int getToClassTimer;

    int classesMade;

    string objective;

    System.Random rand = new System.Random();

    string whatAmIlookingat;

    string whatAmIHitingOff;
        
    int displayNotificationTimer = 0;
    string msg;

    int timerBoxWidth = 200;

    public int Level
    {
        get { return level; }        
    }

    // Use this for initialization
	void Start () {
        // fill the timer texture
        timerTexture = new Texture2D(100, 10);
              

        var fillColorArray = timerTexture.GetPixels();


        for (int i = 0; i < fillColorArray.Length; i++)
        {
            fillColorArray[i] = new Color(255, 0, 0);
        }

        timerTexture.SetPixels(fillColorArray);
        timerTexture.Apply();
	
        openTheDoor = false;

        DoorInfo objectiveDoor = DoorManager.DoorInfoList[rand.Next(0, DoorManager.DoorInfoList.Count)];
        
        objectiveDoor.IsObjective = true;

        objective = objectiveDoor.Door.name.Split('_').GetValue(2).ToString();
        
        getToClassTimer = maxTimeToGetToClass;

        

	}
    

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 170), "");

        GUI.Label(new Rect(10, 20, 130, 30), "Strikes Left: ", styleLabel);
        GUI.Label(new Rect(150, 20, 130, 30), missedClassStrikes.ToString(), styleInfo);

        GUI.Label(new Rect(10, 50, 130, 30), "Objective: ", styleLabel);
        GUI.Label(new Rect(150, 50, 130, 30), objective, styleInfo);


        GUI.Label(new Rect(10, 80, 130, 30), "Timer: ", styleLabel);
        //GUI.Label(new Rect(150, 80, 130, 30), getToClassTimer.ToString(), styleInfo);

        GUI.DrawTexture(new Rect(150, 80, (getToClassTimer / (float)maxTimeToGetToClass) * timerTexture.width, timerTexture.height), timerTexture);

        

        GUI.Label(new Rect(10, 110, 130, 30), "Classes made: ", styleLabel);
        GUI.Label(new Rect(150, 110, 130, 30), classesMade.ToString(), styleInfo);

        
        if (displayNotificationTimer > 0)
            GUI.Label(new Rect((Screen.width/2)-400, Screen.height/2, 800, 60), msg, styleNotification);

        GUI.DrawTexture(new Rect((Screen.width / 2) - (textureHeightWidth / 2), (Screen.height / 2) - (textureHeightWidth / 2), textureHeightWidth, textureHeightWidth), crosshairs);
    }
	
	// Update is called once per frame
	void Update () {
	    
        getToClassTimer--;
        
        y = transform.position.y;


        if (y >= 0 && y < 10)
            level = 1;
        else if (y >= 10 && y < 21)
            level = 2;
        else
            level = 3;

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


        if (getToClassTimer <= 0)
        {
            // missed the class

            missedClassStrikes--;

            DoorInfo di = DoorManager.DoorInfoList.Find(o => o.IsObjective == true); // reset
            
            di.IsObjective = false;

            msg = "You missed your class, try harder";

            displayNotificationTimer = 200;

            // Pick a new class to get to
            DoorInfo objectiveDoor = DoorManager.DoorInfoList[rand.Next(0, DoorManager.DoorInfoList.Count)];

            objectiveDoor.IsObjective = true;

            objective = objectiveDoor.Door.name.Split('_').GetValue(2).ToString();

            getToClassTimer = maxTimeToGetToClass;


        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
        {
            whatAmIlookingat = hit.collider.name;

            if (whatAmIlookingat.Contains("Door"))
            {
                DoorInfo temp = DoorManager.DoorInfoList.Find(o => o.Door.name.Equals(whatAmIlookingat));

                if (temp.IsObjective)
                {
                    classesMade++;

                    temp.IsObjective = false;

                    DoorInfo objectiveDoor = DoorManager.DoorInfoList[rand.Next(0, DoorManager.DoorInfoList.Count)];

                    objectiveDoor.IsObjective = true;

                    objective = objectiveDoor.Door.name.Split('_').GetValue(2).ToString();

                    getToClassTimer = maxTimeToGetToClass;
                
                }
            
            
            }
        }

        if (missedClassStrikes <= 0)
            Application.LoadLevel(2);

        if (Input.GetKeyDown(KeyCode.R))
        {
            getToClassTimer = maxTimeToGetToClass;

            transform.position = spawn.transform.position;
        }

        if (displayNotificationTimer > 0)
            displayNotificationTimer--;

	} // Update


    void OnTriggerEnter(Collider other)
    {
        whatAmIHitingOff = other.gameObject.name;

        if (whatAmIHitingOff.Equals("Guard"))
        {
            // Caught!!! so automatically miss a class

            missedClassStrikes--;

            DoorInfo di = DoorManager.DoorInfoList.Find(o => o.IsObjective == true); // reset

            di.IsObjective = false;


            // Pick a new class to get to
            DoorInfo objectiveDoor = DoorManager.DoorInfoList[rand.Next(0, DoorManager.DoorInfoList.Count)];

            objectiveDoor.IsObjective = true;

            objective = objectiveDoor.Door.name.Split('_').GetValue(2).ToString();

            getToClassTimer = maxTimeToGetToClass;

            other.gameObject.GetComponent<GuardPatrol>().Reset();

            transform.position = spawn.transform.position;

            displayNotificationTimer = 200;

            msg = "You were caught by security, sucks for you";
            
        }
    }
               
    
}
