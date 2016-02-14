using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public TerrainData tData;
    public Terrain myTerrain;
    public int xBase = 0;
    public int yBase = 0;

    private Parser parser;
    private BuilderScript builder;
    private TerrainFill terrain;

	
	void Start () {
        // Initialization of type.
        PlayerPrefs.SetString("type", "nic");
        
	}

	void Update () {
        
	}

    
    void OnLevelWasLoaded() {
        // When main scene is loaded this will get path value.
        if (PlayerPrefs.GetString("path") == "") {
            Debug.Log("nic");
        } else {
            if (PlayerPrefs.GetString("type") == "omap") {
                //Debug.LogError("moj error: " + PlayerPrefs.GetString("path") + "  Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory());
                LoadMap(PlayerPrefs.GetString("path"));
            }
        }
    }

    
    public void LoadMapButton() {
        // Function for load map button. Changes scene to filemanager scene.
        PlayerPrefs.SetString("type", "omap");
        Application.LoadLevel("example2");
    }

    
    public void LoadTrackButton() {
        // Function for load track button. Changes scene to filemanager scene.
        PlayerPrefs.SetString("type", ".gpx");
    }

    
    public void LoadMap(string path) {
        /* Integration function. When file was selected, this function is called and it
         * runs all methods from project.
         */
        parser = new Parser();
        builder = new BuilderScript();
        terrain = new TerrainFill();


        parser.loadStringFromFile(path); // Calling parser with path.
        int[] minMaxs = parser.myParseOmap();
        Vector3[][] appContours = parser.myTerr.getApproximatedContours(5);


        ArrayList rasterizedAL = new ArrayList(); 
        Vector2[] line;
        int simplifyConstant = 40;
        int height = Mathf.Abs(minMaxs[3] - minMaxs[2])/simplifyConstant;
        int width = Mathf.Abs(minMaxs[1] - minMaxs[0])/simplifyConstant;
        ArrayList reducedLine = new ArrayList();
        Dictionary<string, int> usedPoints = new Dictionary<string, int>();

        int[] ret = new int[4];
        ret[0] = Int32.MaxValue;
        ret[1] = Int32.MinValue;
        ret[2] = Int32.MaxValue;
        ret[3] = Int32.MinValue;

        for (int i = 0; i < appContours.Length; i++) {
            usedPoints.Clear();
            for (int j = 1; j < appContours[i].Length; j++) {
                line = builder.bresenhamLine(appContours[i][j - 1], appContours[i][j]); // Fill gaps between aproximated points and simplify to smaller coords.
                reducedLine.Clear();

                for (int k = 0; k < line.Length; k++) {
                    line[k].x = Mathf.Abs((line[k].x - minMaxs[0]) / simplifyConstant);
                    line[k].y = Mathf.Abs((line[k].y - minMaxs[2]) / simplifyConstant);
                    int x = (int)Math.Ceiling(line[k].x);
                    int y = (int)Math.Ceiling(line[k].y);
                    string point = x.ToString() + "," + y.ToString();

                    ret[0] = Mathf.Min(ret[0], x);
                    ret[2] = Mathf.Min(ret[2], y);
                    ret[1] = Mathf.Max(ret[1], x);
                    ret[3] = Mathf.Max(ret[3], y);

                    if (!usedPoints.ContainsKey(point)) {
                        usedPoints.Add(point, 0);
                        reducedLine.Add(line[k]);
                    } 
                }
                rasterizedAL.Add((Vector2[])reducedLine.ToArray(typeof(Vector2)));
            }
        }
        Vector2[][] rasterized = (Vector2[][])rasterizedAL.ToArray(typeof(Vector2[]));
        Debug.Log("min x: " + ret[0].ToString() +
            " max x: " + ret[1].ToString() +
            " min y: " + ret[2].ToString() +
            " max y: " + ret[3].ToString() +
            " height: " + height.ToString() +
            " width: " + width.ToString());

        int[][][] drawnContours = builder.drawContours(rasterized, height, width); // Draw rasterized contours to two-dimensional array.

        int[][] scanlined = builder.scanline(drawnContours); // Run scanline and check height between contours.

        float[][] res = builder.sampleQuantization(scanlined, 65, 65); // Quantize huge two-dimensional array into smaller 65x65 for terrain input

        terrain.FillTerrain(res, tData, myTerrain, xBase, yBase); // Fill terrain with input.
    }

}
