using UnityEngine;
using System.Collections.Generic;
using System;

public class BuilderScript : MonoBehaviour {
    /* Takes two points returns (x,y) and returns rasterized line between them(array of points).
     * Uses Bresenham line rasterization algorithm.
    */
    public Vector2[] bresenhamLine(Vector3 a, Vector3 b) {
        List<Vector2> coords = new List<Vector2>();

        int x0, y0, x1, y1, dx, dy;
        x0 = Convert.ToInt32(a.x); y0 = Convert.ToInt32(a.y);
        x1 = Convert.ToInt32(b.x); y1 = Convert.ToInt32(b.y);
        dx = Mathf.Abs(x1 - x0);
        dy = Mathf.Abs(y1 - y0);

        bool steep = false; // iterate x, calculate y
        if (dy > dx)  // iterate y, calculate x
            steep = true;

        int g, l, gt; float e = 0f, d;
        if (steep) {
          g = y0; gt = y1; l = x0;
          d = (dy == 0 ? 0f : Mathf.Abs(Convert.ToSingle(dx) / Convert.ToSingle(dy)));
        } else {
          g = x0; gt = x1; l = y0;
          d = (dx == 0 ? 0f : Mathf.Abs(Convert.ToSingle(dy) / Convert.ToSingle(dx)));
        }

        int j = l;
        for (int i = g; i <= gt; i++) {
            if (steep)
              coords.Add(new Vector2(Convert.ToSingle(Convert.ToInt32(e)+j), Convert.ToSingle(i)));
            else
              coords.Add(new Vector2(Convert.ToSingle(i), Convert.ToSingle(Convert.ToInt32(e)+j)));
            e += d;
        }

        return coords.ToArray();
    }
    /* Takes two integers and returns an array with them swapped in order.
    */
    private int[] swapInts(int a, int b) {
        return new int[] {b, a};
    }
    /* Takes and array of rasterized contours, total width and height of contour map and marks them
     * into a 3 dimensional array. First two dimensions represent y and x coordinates of points and
     * the third serves for storing multiple overlapping contours. Returns this array.
    */
    public int[,,] drawContours(Vector2[][] contours, int height, int width) {
        int[,,] ret = new int[height, width, 4];  // 3 contours can overlap
        for (int i = 0; i < contours.Length; i++) {
            Vector2[] c = contours[i];
            int y, x, saveTo;
            foreach (Vector2 point in c) {
                y = Convert.ToInt32(point.y); x = Convert.ToInt32(point.x);
                if (ret[y, x, 0] == 3)
                  Debug.LogError("drawContours overflow @" + y + " " + x);
                ret[y, x, 0]++;
                saveTo = ret[y, x, 0];
                ret[y, x, saveTo] = i;
            }
        }
        return ret;
    }
    /* Scans through drawn contour array horizontally and vertically, averaging the values, and
     * marks relative height level for every field according to number of contours crossed on
     * the way. 0 marks lowest level.
    */
    public int[,] scanline(int[,,] contours) {   // TODO test
        int[,] ret = new int[contours.GetLength(0), contours.GetLength(1)];
        Dictionary<int,bool> seen = new Dictionary<int,bool>();
        List<int> recentContours = new List<int>(), contourBuffer = new List<int>();

        // Horizontal pass
        for (uint y = 0; y < contours.GetLength(0); y++) {
            int level = 0;
            for (uint x = 0; x < contours.GetLength(1); x++) {
                int contourID;
                for (uint ci = 1; ci <= contours[y, x, 0]; ci++) {
                    contourID = contours[y, x, ci];
                    contourBuffer.Add(contourID);
                    if (recentContours.Contains(contourID)) {
                        continue;
                    } else {
                        if (seen.ContainsKey(contourID)) {
                            level += seen[contourID] ? -1 : 1;
                            seen[contourID] = !seen[contourID];
                        } else {
                            seen.Add(contourID, true);
                            level++;
                        }
                    }
                }
                recentContours.Clear();
                recentContours.AddRange(contourBuffer);
                contourBuffer.Clear();

                ret[y, x] = level;
            }
            seen.Clear();
        }

        // Horizontal pass
        for (uint x = 0; x < contours.GetLength(1); x++) {
            int level = 0;
            for (uint y = 0; y < contours.GetLength(0); y++) {
                int contourID;
                for (uint ci = 1; ci <= contours[y, x, 0]; ci++) {
                    contourID = contours[y, x, ci];
                    contourBuffer.Add(contourID);
                    if (recentContours.Contains(contourID)) {
                        continue;
                    } else {
                        if (seen.ContainsKey(contourID)) {
                            level += seen[contourID] ? -1 : 1;
                            seen[contourID] = !seen[contourID];
                        } else {
                            seen.Add(contourID, true);
                            level++;
                        }
                    }
                }
                recentContours.Clear();
                recentContours.AddRange(contourBuffer);
                contourBuffer.Clear();

                ret[y, x] += level;
                ret[y, x] = Convert.ToInt32(
                    Math.Ceiling(Convert.ToSingle(ret[y, x]) / 2.0f));
            }
            seen.Clear();
        }
        return ret;
    }
}
