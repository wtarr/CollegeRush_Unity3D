using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    public string levelToLoad;
    public Texture2D normalTexture;
    public Texture2D rollOverTexture;    

    public bool quitButton = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        guiTexture.texture = rollOverTexture;
    }

    void OnMouseExit()
    {
        guiTexture.texture = normalTexture;     
    }

    void OnMouseUp()
    {
        if (quitButton)
        {
            Application.Quit();
        }
        else
        {
            Application.LoadLevel(levelToLoad);
        }
    }
}
