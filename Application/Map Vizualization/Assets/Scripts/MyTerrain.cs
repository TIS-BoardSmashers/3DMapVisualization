using UnityEngine;
using System.Collections.Generic;

public class MyTerrain {
    public List<Vector3[]> Contours;

    
    public void load(List<Vector3[]> contours) {
        // Loads cubic bezier representation of all contours in the map
        this.Contours = contours;
    }
    
    public Vector3[][] getApproximatedContours(int detail) {
        /* Returns polygonal path representation of all contours in the map approximated with (detail-1) being number of approximated points on each curve
         * Uses Bernstein basis polynomial explicit definition as approximation strategy by setting t values
        */
        List<Vector3[]> approx = new List<Vector3[]>();
        List<Vector3> c = new List<Vector3>();
        Vector3 approxPoint = Vector3.zero;
        float dt = 1.0f / detail;

        foreach (Vector3[] contour in this.Contours) {
            for (int i = 0; i < contour.Length - 4; i += 3) {
                for (float t = 0f; (t < 1.0f) || ((t - 1.0f) < 0.00001f); t += dt) {
                    for (int j = 0; j < 4; j++) {
                        approxPoint += combinatorialNumber(3, j) * Mathf.Pow(1.0f - t, 3 - j) * Mathf.Pow(t, j) * contour[i + j];
                    }
                    c.Add(approxPoint);
                    approxPoint = Vector3.zero;
                }
            }

            approx.Add(c.ToArray());
            c.Clear();
        }

        return approx.ToArray();
    }
    
    private int combinatorialNumber(int n, int k) {
        // Returns value of combinatorial number n over k
        return factorial(n)/(factorial(n-k) * factorial(k));
    }
    
    private int factorial(int n) {
        // Returns value of factorial n
        int res = 1;
        while (n > 1) {
            res *= n;
            n--;
        }
        return res;
    }
}
