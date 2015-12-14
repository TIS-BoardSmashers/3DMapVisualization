using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunTestsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //testMyTerrain();
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
}