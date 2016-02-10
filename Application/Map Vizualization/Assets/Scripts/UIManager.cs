using UnityEngine;

public class UIManager : MonoBehaviour {

    private Parser parser;

	// Use this for initialization
	void Start () {
        PlayerPrefs.SetString("type", "nic");
        
	}

	// Update is called once per frame
	void Update () {
        
	}

    void OnLevelWasLoaded() {
        if (PlayerPrefs.GetString("path") == "") {
            Debug.Log("nic");
        } else {
            Debug.Log(PlayerPrefs.GetString("path"));
            if (PlayerPrefs.GetString("type") == "omap") {
                LoadMap(PlayerPrefs.GetString("path"));
            }
        }
    }

    public void LoadMapButton() {
        PlayerPrefs.SetString("type", "omap");
        Application.LoadLevel("fileBrowser");
    }

    public void LoadTrackButton() {
        PlayerPrefs.SetString("type", ".gpx");
        Application.LoadLevel("fileBrowser");
    }

    public void LoadMap(string path) {
        parser = new Parser();
        parser.loadStringFromFile(path);
        int[] minMaxs = parser.parseOmap();
        //Vector3[][] appContours = parser.myTerr.getApproximatedContours(5);

        /*Vector2[][] rasterized = 
        for (int i = 0; i < appContours.Length; i++) {
            for (int j = 0; j < appContours[i].Length; j++) {

            }
        }*/
    }
}
