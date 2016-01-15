using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RunTestsScript : MonoBehaviour {
    public Parser p;
    public BuilderScript b;
	// Use this for initialization
	void Start () {
        //testMyTerrain();
        //testParser();
        testBresenham();
	}
    private void testMyTerrain() {
        Debug.Log("Testing MyTerrain class NOW!");
        MyTerrain t = new MyTerrain();
        List<Vector3[]> cs = new List<Vector3[]>();
        cs.Add(new Vector3[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(100f, 0f, 0f),
            new Vector3(100f, 100f, 0f),
            new Vector3(0f, 100f, 0f)
        });
        cs.Add(new Vector3[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(100f, 0f, 0f),
            new Vector3(0f, 100f, 0f),
            new Vector3(0f, 0f, 0f)
        });
        t.load(cs);
        for (int i = 0; i < cs.Count; i++) {
            Debug.Log("- " + "Contour number " + i);
            foreach (Vector3 v in t.getApproximatedContours(3)[i])
                Debug.Log("- " + v.ToString());
        }
        Debug.Log("Testing MyTerrain class finished.");
    }

    private void testParser()
    {
        Debug.Log("Testing Parser...");
        p.loadStringFromFile(Directory.GetCurrentDirectory() +
          String.Format("{0}Assets{0}Test files{0}TextFile1.txt", Path.DirectorySeparatorChar));
        int[] minMaxs = p.parseOmap();
        Debug.Log("Munching on Parsed data...");
        Debug.Log("Mins & Maxs of data: " + minMaxs[0] + ", " + minMaxs[1] + ", " + minMaxs[2] + ", " + minMaxs[3]);
        var k = p.myTerr.getApproximatedContours(2)[0];
        foreach(Vector3 v in k)
        {
            Debug.Log(v);
        }
        Debug.Log("Testing Parser finished.");
    }
    private void testBresenham()
    {
      Debug.Log("Testing Bresenham...");

      Debug.Log("test1:");
      Vector3 x = new Vector3(0f,0f,0f), y = new Vector3(10f,0f,0f);
      foreach (Vector2 v in b.bresenham_line(x, y))
          Debug.Log(v);

      Debug.Log("test2:");
      y = new Vector3(10f, 10f, 0f);
      foreach (Vector2 v in b.bresenham_line(x, y))
          Debug.Log(v);

      Debug.Log("test3:");
      x = new Vector3(0f, 5f, 0f);
      foreach (Vector2 v in b.bresenham_line(x, y))
          Debug.Log(v);

      Debug.Log("Testing Bresenham finished.");
    }
}
