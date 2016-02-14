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
            //reader = XmlReader.Create(new StringReader(ptext));
            //idhash = new Dictionary<int, int>();
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

/*    public int[] parseOmap() {
        if (reader == null) {
            return null;
        }
        int[] ret = new int[4];


        reader.ReadToFollowing("colors"); //ignoring the part with colors
        reader.Skip();


        reader.ReadToFollowing("symbols"); //finding relevant symbol codes and ids
        reader.ReadToFollowing("symbol");
        string spracuj = reader.GetAttribute("code");
        reader.ReadToNextSibling("symbol");
        string spracuj2 = reader.GetAttribute("code");


        while (true) { //need to find elements used as dictionary
            reader.ReadToNextSibling("symbol");
            if (reader.AttributeCount >= 4) {
                if (reader.GetAttribute(3) == "OpenOrienteering Logo") {
                    reader.ReadToNextSibling("symbol");
                    try {
                        var nieco = reader.GetAttribute("code").Split('.');
                        if (Convert.ToInt32(nieco[0]) == Convert.ToInt32(spracuj)) {
                            idhash.Add(Convert.ToInt32(spracuj), Convert.ToInt32(reader.GetAttribute("id")));
                        }
                    } catch (NullReferenceException e) {
                        Debug.Log(e);
                    }

                    break;
                }
            }

        }
        reader.ReadToNextSibling("symbol");
        try {
            var r = reader.GetAttribute("code").Split('.');
            if (Convert.ToInt32(r[0]) == Convert.ToInt32(spracuj2)) {
                idhash.Add(Convert.ToInt32(spracuj2), Convert.ToInt32(reader.GetAttribute("id")));
            }
        } catch (NullReferenceException e) {
            Debug.Log(e);
        }
        


        reader.ReadToFollowing("parts"); //parsing Contours
        reader.ReadToDescendant("object");
        ret[0] = 999999999;
        ret[1] = -999999999;
        ret[2] = 999999999;
        ret[3] = -999999999;

        List<Vector3[]> clist = new List<Vector3[]>();
        List<Vector3> contour = new List<Vector3>();

        while (reader.ReadToFollowing("object")) {
            if (idhash.ContainsValue(Convert.ToInt32(reader.GetAttribute("symbol")))) { 
                //Fill the contour field with a contour object
                

                contour.Clear();
                reader.ReadToFollowing("coords");
                reader.ReadToDescendant("coord");
                ret[0] = Mathf.Min(ret[0], Convert.ToInt32(reader.GetAttribute("x")));
                ret[2] = Mathf.Min(ret[2], Convert.ToInt32(reader.GetAttribute("y")));
                ret[1] = Mathf.Max(ret[1], Convert.ToInt32(reader.GetAttribute("x")));
                ret[3] = Mathf.Max(ret[3], Convert.ToInt32(reader.GetAttribute("y")));

                contour.Add(new Vector3(float.Parse(reader.GetAttribute("x"), CultureInfo.InvariantCulture.NumberFormat),
                                        float.Parse(reader.GetAttribute("y"),CultureInfo.InvariantCulture.NumberFormat),
                                                                                                                     0));

                while (reader.ReadToNextSibling("coord"))
                {
                    contour.Add(new Vector3(float.Parse(reader.GetAttribute("x"), CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(reader.GetAttribute("y"), CultureInfo.InvariantCulture.NumberFormat),
                                                                                                                         0));
                    ret[0] = Mathf.Min(ret[0], Convert.ToInt32(reader.GetAttribute("x")));
                    ret[2] = Mathf.Min(ret[2], Convert.ToInt32(reader.GetAttribute("y")));
                    ret[1] = Mathf.Max(ret[1], Convert.ToInt32(reader.GetAttribute("x")));
                    ret[3] = Mathf.Max(ret[3], Convert.ToInt32(reader.GetAttribute("y")));
                }
                reader.ReadToNextSibling("object");
                clist.Add(contour.ToArray());
            }
        }

        myTerr.load(clist);
        return ret;
    }*/

    public int[] myParseOmap() {
        if (ptext == null) {
            return null;
        }
        int[] ret = new int[4];

        ret[0] = Int32.MaxValue;
        ret[1] = Int32.MinValue;
        ret[2] = Int32.MaxValue;
        ret[3] = Int32.MinValue;

        int ix = findFrom("default part",0); // Position, where in all omap files begin changes

        Debug.Log("ix po default part: " + ix.ToString());
        Debug.Log("znaky default part: " + ptext[ix] + ptext[ix-1] + ptext[ix-2] +
            ptext[ix-3] + ptext[ix-4] + ptext[ix-5] + ptext[ix-6]);

        List<Vector3[]> clist = new List<Vector3[]>();
        List<Vector3> contour = new List<Vector3>();
        int countObj = 0;
        int countCoords = 0;
        int symbol = -1;
        int x, y;
        bool containsObject = true;

        while (ix < ptext.Length && containsObject) {
            ix = findFrom("<objects ", ix); // index from "default part", gets index after "objects"
            if (ix == -1) break;

            ix = findFrom("count=\"", ix); // index from "objects", gets index after "count=""
            countObj = readInt(ix); // reads integer from actual index

            for (int i = 0; i < countObj; i++) {
                ix = findFrom("<object ", ix);
                if (ix == -1) break;
                ix = findFrom("symbol=\"", ix);
                symbol = readInt(ix);

                ix = findFrom("<coords ", ix);
                ix = findFrom("count=\"", ix); // index from "objects", gets index after "count=""
                countCoords = readInt(ix);
                contour.Clear();

                for (int j = 0; j < countCoords; j++) {
                    ix = findFrom("<coord ", ix);
                    if (ix == -1) break;

                    ix = findFrom("x=\"", ix);
                    x = readInt(ix);

                    ix = findFrom("y=\"", ix);
                    y = readInt(ix);

                    ret[0] = Mathf.Min(ret[0], x);
                    ret[2] = Mathf.Min(ret[2], y);
                    ret[1] = Mathf.Max(ret[1], x);
                    ret[3] = Mathf.Max(ret[3], y);

                    contour.Add(new Vector3((float)x,(float)y,0));
                }
                ix = findFrom("</coords>", ix);
                ix = findFrom("</object>", ix);

                clist.Add(contour.ToArray());
            }
            ix = findFrom("</objects>", ix);
        }

        myTerr.load(clist);
        Debug.Log("min x: " + ret[0].ToString() +
            " max x: " + ret[1].ToString() +
            " min y: " + ret[2].ToString() +
            " max y: " + ret[3].ToString());
        return ret;
    }

    public int findFrom(string keyword, int fromIx) {
        string test = "";
        for (int i = fromIx; i < ptext.Length - keyword.Length; i++) {
            test = "";
            for (int j = 0; j < keyword.Length; j++) {
                test += ptext[i + j];
            }
            if (keyword == test) {
                return i + keyword.Length;
            }
        }
        return -1;
    }

    public int readInt(int fromIx) {
        string ints = "-0123456789";
        string strInt = "";
        int i = 0;
        
        while (fromIx + i < ptext.Length && ints.IndexOf(ptext[fromIx + i]) != -1) {
            strInt += ptext[fromIx + i];
            i++;
        }

        return Convert.ToInt32(strInt);
    }
}
