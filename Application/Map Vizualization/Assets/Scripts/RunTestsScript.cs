using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RunTestsScript : MonoBehaviour {
    public Parser p;
	// Use this for initialization
	void Start () {
        //testMyTerrain();
        testParser();
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
        p.loadStringFromFile(Directory.GetCurrentDirectory() + "\\Assets\\Test files\\TextFile1.txt");
        p.parseOmap();
        Debug.Log("Munching on Parsed data...");
        var k = p.myTerr.getApproximatedContours(2)[0];
        foreach(Vector3 v in k)
        {
            Debug.Log(v);
        }
    }
}