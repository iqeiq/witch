using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;


public class TitleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire"))
        {
            SceneManager.LoadScene("Main");
        }
	}

    void OnGUI()
    {
        // TODO:
        if (GUI.Button(new Rect(10, 10, 50, 20), "START"))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
