using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuilderScript : MonoBehaviour {
    public Vector2[] bresenham_line(Vector3 a, Vector3 b) {
        List<Vector2> coords = new List<Vector2>();
        bool steep = false;

        int x0, y0, x1, y1, dx, dy, sx, sy, d;

        x0 = Convert.ToInt32(a.x); y0 = Convert.ToInt32(a.y);
        x1 = Convert.ToInt32(b.x); y1 = Convert.ToInt32(b.y);

        dx = Mathf.Abs(x1 - x0);
        if (x1 - x0 > 0)
            sx = 1;
        else
            sx = -1;

        dy = Mathf.Abs(y1 - y0);
        if (y1 - y0 > 0)
            sy = 1;
        else
            sy = -1;

        if (dy > dx) {
            steep = true;
            int[] s = swapInts(x0, y0);
            y0 = s[0]; x0 = s[1];

            s = swapInts(dx, dy);
            dy = s[0]; dx = s[1];

            s = swapInts(sx, sy);
            sy = s[0]; sx = s[1];
        }

        d = (2 * dy) - dx;

        for (uint i = 0; i < dx; i++) {
            if (steep)
                coords.Add(new Vector2(y0, x0));
            else
                coords.Add(new Vector2(x0, y0));

            while (d >= 0) {
                y0 = y0 + sy;
                d = d - (2 * dx);
            }

            x0 = x0 + sx;
            d = d + (2 * dy);
        }

        coords.Add(new Vector2(x1, y1));
        return coords.ToArray();
    }
    private int[] swapInts(int a, int b) {
        return new int[] {b, a};
    }
}
