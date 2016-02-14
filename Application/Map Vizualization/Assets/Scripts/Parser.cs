using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Xml.Linq;

public class Parser : MonoBehaviour {
    public string ptext;
    private XmlReader reader;
    private Dictionary<int, int> idhash;
    public MyTerrain myTerr;

    public void load() {
        ptext = "<xml><start></start></xml>";
        reader = XmlReader.Create(new StringReader(ptext));
        idhash = new Dictionary<int, int>();
        myTerr = new MyTerrain();
}

    public void load(string text) {
        ptext = text;
        reader = XmlReader.Create(new StringReader(ptext));
        idhash = new Dictionary<int, int>();
        myTerr = new MyTerrain();

    }

    public void loadStringFromFile(string path) {
        try {
            ptext = File.ReadAllText(path);
            myTerr = new MyTerrain();
        }
        catch (Exception e) {
            Debug.LogError("Nenasiel sa file " + e.Message);
        }
    }

    public void print() {
        Debug.Log("Obsah ptext je : " + ptext);
    }

    public string IndentedNewWSDLString(string filePath) {
        var xml = File.ReadAllText(filePath);
        XDocument doc = XDocument.Parse(xml);
        return doc.ToString();
    }

    public int[] myParseOmap() {
        if (ptext == null) {
            return null;
        }
        int[] ret = new int[4];

        ret[0] = Int32.MaxValue; // min x
        ret[1] = Int32.MinValue; // max x
        ret[2] = Int32.MaxValue; // min y
        ret[3] = Int32.MinValue; // max y

        int ix = findFrom("default part",0); // Position, where in all omap files begin changes

        Debug.Log("ix po default part: " + ix.ToString());
        Debug.Log("znaky default part: " + ptext[ix] + ptext[ix-1] + ptext[ix-2] +
            ptext[ix-3] + ptext[ix-4] + ptext[ix-5] + ptext[ix-6]);

        List<Vector3[]> clist = new List<Vector3[]>(); // array of contours
        List<Vector3> contour = new List<Vector3>(); // temporary variable for contour
        int countObj = 0;
        int countCoords = 0;
        int symbol = -1;
        int x, y;

        while (ix < ptext.Length) {
            ix = findFrom("<objects ", ix); // index from "default part", gets index after "objects"
            if (ix == -1) break;

            ix = findFrom("count=\"", ix); // index from "<objects ", gets index after "count=""
            countObj = readInt(ix); // reads integer from actual index

            for (int i = 0; i < countObj; i++) { // from previous parent objects parameter count we know number of object elements
                ix = findFrom("<object ", ix);
                if (ix == -1) break;
                ix = findFrom("symbol=\"", ix);
                symbol = readInt(ix);

                ix = findFrom("<coords ", ix);
                ix = findFrom("count=\"", ix); // index from "<coords ", gets index after "count=""
                countCoords = readInt(ix);
                contour.Clear();

                for (int j = 0; j < countCoords; j++) { // from parent element parameter count we know number of coord elements
                    ix = findFrom("<coord ", ix);
                    if (ix == -1) break;

                    ix = findFrom("x=\"", ix); // x value
                    x = readInt(ix);

                    ix = findFrom("y=\"", ix); // y value
                    y = readInt(ix);

                    ret[0] = Mathf.Min(ret[0], x); // min x
                    ret[2] = Mathf.Min(ret[2], y); // min y
                    ret[1] = Mathf.Max(ret[1], x); // max x
                    ret[3] = Mathf.Max(ret[3], y); // max y

                    contour.Add(new Vector3((float)x,(float)y,0)); // add x and y values to contour
                }
                ix = findFrom("</coords>", ix); // close element coords
                ix = findFrom("</object>", ix); // close element object

                clist.Add(contour.ToArray()); // add contour from last object's coords to array of contours
            }
            ix = findFrom("</objects>", ix); // close element objects
        }

        myTerr.load(clist); // add array of contours to terrain object
        Debug.Log("min x: " + ret[0].ToString() +
            " max x: " + ret[1].ToString() +
            " min y: " + ret[2].ToString() +
            " max y: " + ret[3].ToString());
        return ret; // return mins a maxs of x and y
    }

    public int findFrom(string keyword, int fromIx) {
        // Finds keyword from index in string ptext and returns index after last char in keyword from ptext.
        bool test;
        for (int i = fromIx; i < ptext.Length - keyword.Length; i++) {
            test = true;
            for (int j = 0; j < keyword.Length; j++) { // compare actual position with keyword
                if (ptext[i + j] != keyword[j]) { // if chars are not equal, then test is false
                    test = false;
                    break;
                }
            }
            if (test) { // if test is not false, then return index after keyword
                return i + keyword.Length;
            }
        }
        return -1; // if keyword is not found, then return -1
    }

    public int readInt(int fromIx) {
        // Reads integer from index in ptext string and returns it as int.
        string ints = "-0123456789"; // Valid chars.
        string strInt = ""; 
        int i = 0;
        
        while (fromIx + i < ptext.Length && ints.IndexOf(ptext[fromIx + i]) != -1) { // while char on index i is from valid chars do concatenation
            strInt += ptext[fromIx + i];
            i++;
        }

        return Convert.ToInt32(strInt); // return int from concatenation of valid chars
    }
}
