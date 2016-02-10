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
        Vector2[] line;
        int simplifyConstant = 100;

        for (int i = 0; i < appContours.Length; i++) {
            for (int j = 1; j < appContours[i].Length; j++) {
                line = builder.bresenhamLine(appContours[i][j - 1], appContours[i][j]);
                for (int k = 0; k < line.Length; k++) {
                    line[k] = line[k]/simplifyConstant;
                }
                rasterizedAL.Add(line);
            }
        }
        Vector2[][] rasterized = (Vector2[][])rasterizedAL.ToArray(typeof(Vector2[]));
        int height = Mathf.Abs(minMaxs[3] - minMaxs[2])/simplifyConstant;
        int width = Mathf.Abs(minMaxs[1] - minMaxs[0])/simplifyConstant;

        Debug.Log("Min Max x:" + minMaxs[0].ToString() + " " + minMaxs[1].ToString() + "  Min Max y:" + minMaxs[2].ToString() + " " + minMaxs[3].ToString());
        Debug.Log(height.ToString() + " " + width.ToString());
        
        int[][][] drawnContours = builder.drawContours(rasterized, height, width);
        int[][] scanlined = builder.scanline(drawnContours);
        int[][] res = builder.sampleQuantization(scanlined, 257, 257);
        terrain.FillTerrain(res);
    }
}
