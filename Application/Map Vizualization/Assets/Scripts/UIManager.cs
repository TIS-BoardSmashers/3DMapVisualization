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
            if (PlayerPrefs.GetString("type") == "omap") {
                Debug.LogError("moj error: " + PlayerPrefs.GetString("path") + "  Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory());
                LoadMap(PlayerPrefs.GetString("path"));
            }
        }
    }

    public void LoadMapButton() {
        PlayerPrefs.SetString("type", "omap");
        Application.LoadLevel("example2");
    }

    public void LoadTrackButton() {
        PlayerPrefs.SetString("type", ".gpx");
        //Application.LoadLevel("example2");
    }

    public void LoadMap(string path) {
        parser = new Parser();
        builder = new BuilderScript();
        terrain = new TerrainFill();
        
        parser.loadStringFromFile(path);
        int[] minMaxs = parser.parseOmap();
        Vector3[][] appContours = parser.myTerr.getApproximatedContours(5);

        ArrayList rasterizedAL = new ArrayList();
        Vector2[] line;
        int simplifyConstant = 40;
        int height = Mathf.Abs(minMaxs[3] - minMaxs[2])/simplifyConstant;
        int width = Mathf.Abs(minMaxs[1] - minMaxs[0])/simplifyConstant;
        ArrayList reducedLine;

        for (int i = 0; i < appContours.Length; i++) {
            Dictionary<string, int> usedPoints = new Dictionary<string, int>();
            for (int j = 1; j < appContours[i].Length; j++) {
                line = builder.bresenhamLine(appContours[i][j - 1], appContours[i][j]);
                reducedLine = new ArrayList();

                for (int k = 0; k < line.Length; k++) {
                    line[k].x = Mathf.Abs(line[k].x / simplifyConstant + width/2-50);
                    line[k].y = Mathf.Abs(line[k].y / simplifyConstant + height/2-50);
                    int x = (int)Math.Ceiling(line[k].x);
                    int y = (int)Math.Ceiling(line[k].y);
                    string point = x.ToString() + "," + y.ToString();

                    if (!usedPoints.ContainsKey(point)) {
                        usedPoints.Add(point, 0);
                        reducedLine.Add(line[k]);
                    } 
                }
                rasterizedAL.Add((Vector2[])reducedLine.ToArray(typeof(Vector2)));
            }
        }
        Vector2[][] rasterized = (Vector2[][])rasterizedAL.ToArray(typeof(Vector2[]));
        
        int[][][] drawnContours = builder.drawContours(rasterized, height, width);
        int[][] scanlined = builder.scanline(drawnContours);
        float[][] res = builder.sampleQuantization(scanlined, 65, 65);
        terrain.FillTerrain(res, tData, myTerrain, xBase, yBase);
    }
}
