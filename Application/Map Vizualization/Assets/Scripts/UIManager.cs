using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public TerrainData tData;
    public Terrain myTerrain;
    public int xBase = 0;
    public int yBase = 0;

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
        int simplifyConstant = 40;
        int height = Mathf.Abs(minMaxs[3] - minMaxs[2])/simplifyConstant;
        int width = Mathf.Abs(minMaxs[1] - minMaxs[0])/simplifyConstant;
        HashSet<Vector2> usedPoints;
        ArrayList reducedLine;
        usedPoints = new HashSet<Vector2>();

        for (int i = 0; i < appContours.Length; i++) {
            for (int j = 1; j < appContours[i].Length; j++) {
                line = builder.bresenhamLine(appContours[i][j - 1], appContours[i][j]);
                reducedLine = new ArrayList();

                for (int k = 0; k < line.Length; k++) {
                    line[k].x = Mathf.Abs(line[k].x / simplifyConstant + width/2-50);
                    line[k].y = Mathf.Abs(line[k].y / simplifyConstant + height/2-50);

                    if (!usedPoints.Contains(line[k])) {
                        usedPoints.Add(line[k]);
                        reducedLine.Add(line[k]);
                    }
                }
                rasterizedAL.Add((Vector2[])reducedLine.ToArray(typeof(Vector2)));
            }
        }
        Vector2[][] rasterized = (Vector2[][])rasterizedAL.ToArray(typeof(Vector2[]));

        //Debug.Log("Min Max x:" + minMaxs[0].ToString() + " " + minMaxs[1].ToString() + "  Min Max y:" + minMaxs[2].ToString() + " " + minMaxs[3].ToString());
        //Debug.Log(height.ToString() + " " + width.ToString());
        
        int[][][] drawnContours = builder.drawContours(rasterized, height, width);
        int[][] scanlined = builder.scanline(drawnContours);
        float[][] res = builder.sampleQuantization(scanlined, 513, 513);
        terrain.FillTerrain(res, tData, myTerrain, xBase, yBase);
    }
}
