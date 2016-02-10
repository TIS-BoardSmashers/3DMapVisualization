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
    public int[][][] drawContours(Vector2[][] contours, int height, int width) {
        int[][][] ret = new int[height][][];  // 3 contours can overlap

        for (int i = 0; i < height; i++) {
            ret[i] = new int[width][];
            for (int j = 0; j < width; j++) {
                ret[i][j] = new int[4];
            }
        }

        for (int i = 0; i < contours.Length; i++) {
            Vector2[] c = contours[i];
            int y, x, saveTo;
            foreach (Vector2 point in c) {
                y = Convert.ToInt32(point.y); x = Convert.ToInt32(point.x);
                if (ret[y][x][0] == 3)
                  Debug.LogError("drawContours overflow @" + y + " " + x);
                ret[y][x][0]++;
                saveTo = ret[y][x][0];
                ret[y][x][saveTo] = i;
            }
        }
        return ret;
    }
    /* Scans through drawn contour array horizontally and vertically, averaging the values, and
     * marks relative height level for every field according to number of contours crossed on
     * the way. 0 marks lowest level.
    */
    public int[][] scanline(int[][][] contours) {
        int[][] ret = new int[contours.GetLength(0)][];

        for (int i = 0; i < contours.GetLength(0); i++) {
            ret[i] = new int[contours.GetLength(1)];
        }

        Dictionary<int,bool> seen = new Dictionary<int,bool>();
        List<int> recentContours = new List<int>(), contourBuffer = new List<int>();

        // Horizontal pass
        for (uint y = 0; y < contours.GetLength(0); y++) {
            int level = 0;
            for (uint x = 0; x < contours.GetLength(1); x++) {
                int contourID;
                for (uint ci = 1; ci <= contours[y][x][0]; ci++) {
                    contourID = contours[y][x][ci];
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

                ret[y][x] = level;
            }
            seen.Clear();
        }

        // Horizontal pass
        for (uint x = 0; x < contours.GetLength(1); x++) {
            int level = 0;
            for (uint y = 0; y < contours.GetLength(0); y++) {
                int contourID;
                for (uint ci = 1; ci <= contours[y][x][0]; ci++) {
                    contourID = contours[y][x][ci];
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

                ret[y][x] += level;
                ret[y][x] = Convert.ToInt32(
                    Math.Ceiling(Convert.ToSingle(ret[y][x]) / 2.0f));
            }
            seen.Clear();
        }
        return ret;
    }

    public int[][] sampleQuantization(int[][] vstup, int x, int y) {
        //inicializacia
        int[][] vystup = new int[y][];
        for (int i = 0; i < y; i++) {
            vystup[i] = new int[x];
        }

        //pre pripad , ze je mensie pole nez vystup
        if (vstup.GetLength(0) < y && vstup.GetLength(1) < x) {
            for (int a = 0; a < vystup.Length; a++) {
                for (int b = 0; b < x; b++) {
                    if (vstup.Length > a) {
                        if (vstup.GetLength(1) > b) {
                            vystup[a][b] = vstup[a][b];
                        } else {
                            vystup[a][b] = 0;
                        }
                    } else {
                        vystup[a][b] = 0;
                    }
                }
            }
        }

        //ak mame velke pole a potrebujeme zmensit
        int maxsize = Math.Max(vstup.GetLength(1), vstup.GetLength(0));
        int interval = maxsize / Math.Max(x, y);
        int amount = interval * interval;
        //naplnim vsetky policka
        for (int a = 0; a < y; a++) {
            for (int b = 0; b < x; b++) {
                int sum = 0;
                for (int i = 0; i < interval; i++) {
                    for (int j = 0; j < interval; j++) {
                        //ak mi nestacia policka povodnych, ratam ich ako nuly
                        if (a * interval + i < vstup.GetLength(0) && b * interval + j < vstup.GetLength(1)) {
                            sum += vstup[a * interval + i][b * interval + j];
                        }
                    }
                }
                //ulozim priemer s nacitanych hodnot
                float k = sum;
                float am = amount;
                k = k / am;
                vystup[a][b] = (int)(Math.Round(k));
            }
        }
        //vratim vyplnenu tabulku
        return vystup;
    }
}
