using UnityEngine;
using System.Collections;

public class PlayerStuff : MonoBehaviour {

    int radius;
    SphereCollider sc;

    float walkSpeed = 15, turnSpeed = 0.05f;


    void OnGUI()
    { 
       
    }

    private void Decrease()
    {
        
        sc.radius -= 10;
    }

    private void Increase()
    {
        
        sc.radius += 10;
    }

	// Use this for initialization
	void Start () {

        //gameObject.AddComponent<SphereCollider>();

        //sc = gameObject.GetComponent<SphereCollider>();

        //sc.radius = 10;

        //sc.isTrigger = true;
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
            Forward();
        if (Input.GetKey(KeyCode.A))
            RotateLeft();
        if (Input.GetKey(KeyCode.D))
            RotateRight();
        if (Input.GetKey(KeyCode.S))
            Reverse();        	
	}

    private void Reverse()
    {
        
    }

    private void RotateRight()
    {
        transform.RotateAround(transform.up, (turnSpeed));
    }

    private void RotateLeft()
    {
        transform.RotateAround(transform.up, (turnSpeed * -1));
    }

    private void Forward()
    {
        transform.position += transform.forward * walkSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

}
