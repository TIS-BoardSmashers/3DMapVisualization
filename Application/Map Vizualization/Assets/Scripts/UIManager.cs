using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private Parser parser;
    private BuilderScript builder;
    private TerrainFill terrain;

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
        builder = new BuilderScript();
        terrain = new TerrainFill();
        
        parser.loadStringFromFile(path);
        int[] minMaxs = parser.parseOmap();
        Vector3[][] appContours = parser.myTerr.getApproximatedContours(5);

        ArrayList rasterizedAL = new ArrayList();
        //Mathf.Abs(minMaxs[1] - minMaxs[0]), Mathf.Abs(minMaxs[3] - minMaxs[2])

        for (int i = 0; i < appContours.Length; i++) {
            for (int j = 1; j < appContours[i].Length; j++) {
                rasterizedAL.Add(builder.bresenhamLine(appContours[i][j - 1], appContours[i][j]));
            }
        }
        Vector2[][] rasterized = (Vector2[][])rasterizedAL.ToArray(typeof(Vector2[]));
        int[][][] drawnContours = builder.drawContours(rasterized, Mathf.Abs(minMaxs[3] - minMaxs[2]), Mathf.Abs(minMaxs[1] - minMaxs[0]));
        int[][] scanlined = builder.scanline(drawnContours);
        int[][] res = builder.sampleQuantization(scanlined, 257, 257);
        terrain.FillTerrain(res);
    }
}
