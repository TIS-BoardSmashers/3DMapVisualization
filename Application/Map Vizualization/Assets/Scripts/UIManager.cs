using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadMapButton() {
        Application.LoadLevel("fileBrowser");
    }

    public void LoadTrackButton() {
        Application.LoadLevel("fileBrowser");
    }
}
