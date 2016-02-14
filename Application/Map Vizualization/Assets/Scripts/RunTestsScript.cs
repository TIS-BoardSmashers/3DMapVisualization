using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class RunTestsScript : MonoBehaviour
{
    public Parser p;
    public BuilderScript b;
    
    void Start() {
        /* Use this for initialization
         * some tests may need to have readjusted protection levels for methods in order to run
         */
        //testApproximation();
        //testParser();
        //testBresenham();
        //testDrawContours();
        //testScanline();
        //testParserFunctions();
    }
    private void testApproximation() {
        Debug.Log("Testing Approximation class!");
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
        Debug.Log("Testing Approximation finished.");
    }

    private void testParser()
    {
        Debug.Log("Testing Parser");
        p.loadStringFromFile(Directory.GetCurrentDirectory() +
          String.Format("{0,0,0,0}Assets{0,0,0,0}Test files{0,0,0,0}TextFile1.txt", Path.DirectorySeparatorChar));      
        int[] minMaxs = p.myParseOmap();
        Debug.Log("Munching on Parsed data...");
        Debug.Log("Mins & Maxs of data: " + minMaxs[0] + ", " + minMaxs[1] + ", " + minMaxs[2] + ", " + minMaxs[3]);
        var k = p.myTerr.getApproximatedContours(2)[0];
        foreach(Vector3 v in k)
        {
            Debug.Log(v);
        }
        Debug.Log("Testing Parser finished.");
    }

    private void testParserFunctions() {
        Debug.Log("Testing Parser methods");
        p.ptext = "0123456";
        int ix = p.findFrom("345", 0);
        Debug.Log("0123456 p.findFrom(345, 0) :" + ix.ToString());

        ix = p.findFrom("35", 0);
        Debug.Log("0123456 p.findFrom(35, 0) :" + ix.ToString());

        ix = p.findFrom("01", 0);
        Debug.Log("0123456 p.findFrom(01, 0) :" + ix.ToString());

        p.ptext = "default part\">\n\t<objects count=\"37\">";
        ix = p.findFrom("<objects ", 12);
        Debug.Log("default part\">\n\t<objects count=\"37\">  p.findFrom(\"<objects \", 12); :" + ix.ToString());

        p.ptext = "=\"-10";
        int res = p.readInt(2);
        Debug.Log("=\"-10 p.readInt(2) :" + res.ToString());

        Debug.Log("Testing Parser methods finished");
    }

    private void testBresenham() {
        Debug.Log("Testing Bresenham");

        Debug.Log("test1:");
        Vector3 x = new Vector3(0f, 0f, 0f), y = new Vector3(9f, 0f, 0f);
        foreach (Vector2 v in b.bresenhamLine(x, y))
            Debug.Log(v);

        Debug.Log("test2:");
        y = new Vector3(9f, 9f, 0f);
        foreach (Vector2 v in b.bresenhamLine(x, y))
            Debug.Log(v);

        Debug.Log("test3:");
        x = new Vector3(0f, 4f, 0f);
        foreach (Vector2 v in b.bresenhamLine(x, y))
            Debug.Log(v);

        Debug.Log("Testing Bresenham finished.");
    }
    private void testDrawContours() {
        Debug.Log("Testing drawContours() [this test depends on Builder.bresenhamLine()]");

        Debug.Log("test1:");
        List<Vector2[]> cs = new List<Vector2[]>();
        Vector2[] rasterized = b.bresenhamLine(new Vector3(0f, 0f, 0f), new Vector3(2f, 2f, 0f));
        cs.Add(rasterized);

        int[, ,] expected = new int[3, 3, 4] {
            {{1,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {1,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,0,0,0}}
        };
        int[][][] result = b.drawContours(cs.ToArray(), 3, 3);
        var equal =
            expected.Rank == result.Rank &&
            Enumerable.Range(0, expected.Rank).All(dimension => expected.GetLength(dimension) == result.GetLength(dimension)) &&
            expected.Cast<int>().SequenceEqual(result.Cast<int>());
        Debug.Log(equal ? "passed" : "failed");

        Debug.Log("test2:");
        rasterized = b.bresenhamLine(new Vector3(1f, 0f, 0f), new Vector3(1f, 2f, 0f));
        cs.Add(rasterized);

        expected = new int[3, 3, 4] {
            {{1,0,0,0}, {1,1,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {2,0,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {1,1,0,0}, {1,0,0,0}}
        };
        result = b.drawContours(cs.ToArray(), 3, 3);
        equal =
            expected.Rank == result.Rank &&
            Enumerable.Range(0, expected.Rank).All(dimension => expected.GetLength(dimension) == result.GetLength(dimension)) &&
            expected.Cast<int>().SequenceEqual(result.Cast<int>());
        Debug.Log(equal ? "passed" : "failed");

        Debug.Log("Testing drawContours() finished.");
    }
    private void testScanline() {
        Debug.Log("Testing scanline()");

        Debug.Log("test1:"); // test s 1 vrstevnicou
        int[][] expected = new int[][] {
            new int[] {0, 1, 1},
            new int[] {0, 1, 1},
            new int[] {0, 1, 1}
        };

        int[][][] drawn = new int[][][] {
            new int[][] {new int[] {0,0,0,0}, new int[] {1,0,0,0}, new int[] {0,0,0,0}},
            new int[][] {new int[] {0,0,0,0}, new int[] {1,0,0,0}, new int[] {0,0,0,0}},
            new int[][] {new int[] {0,0,0,0}, new int[] {1,0,0,0}, new int[] {0,0,0,0}}
        };
        int[][] result = b.scanline(drawn);
        var equal =
            expected.Rank == result.Rank &&
            Enumerable.Range(0, expected.Rank).All(dimension => expected.GetLength(dimension) == result.GetLength(dimension)) &&
            expected.Cast<int>().SequenceEqual(result.Cast<int>());
        Debug.Log(equal ? "passed" : "failed");

        Debug.Log("test2:");// test s 2 prekrizenymi vrstevnicami
        expected = new int[][] {
            new int[] {0, 1, 1},
            new int[] {0, 1, 1},
            new int[] {1, 2, 2}
        };

        drawn = new int[][][] {
            new int[][] {new int[] {0,0,0,0}, new int[] {1,1,0,0}, new int[] {0,0,0,0}},
            new int[][] {new int[] {0,0,0,0}, new int[] {1,1,0,0}, new int[] {0,0,0,0}},
            new int[][] {new int[] {1,0,0,0}, new int[] {2,0,1,0}, new int[] {1,0,0,0}}
        };
        result = b.scanline(drawn);
        equal =
            expected.Rank == result.Rank &&
            Enumerable.Range(0, expected.Rank).All(dimension => expected.GetLength(dimension) == result.GetLength(dimension)) &&
            expected.Cast<int>().SequenceEqual(result.Cast<int>());
        Debug.Log(equal ? "passed" : "failed");

        Debug.Log("Testing scanline() finished.");
    }
}
