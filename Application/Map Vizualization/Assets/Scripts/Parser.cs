using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

public class Parser : MonoBehaviour {
    private string ptext;
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
            reader = XmlReader.Create(new StringReader(ptext));
            idhash = new Dictionary<int, int>();
            myTerr = new MyTerrain();
        }
        catch (Exception e) {
            Debug.LogError("Nenasiel sa file " + e.Message);
        }
    }

    public void print() {
        Debug.Log("Obsah ptext je : " + ptext);
    }

    public bool parseOmap() {
        if (reader == null) {
            return false;
        }

        //ignoring the part with colors
        reader.ReadToFollowing("colors");
        reader.Skip();

        //finding relevant symbol codes and ids
        reader.ReadToFollowing("symbols");
        reader.ReadToFollowing("symbol");
        string spracuj = reader.GetAttribute("code");
        // Console.WriteLine(spracuj + " pracujem na tomto");
        reader.ReadToNextSibling("symbol");
        string spracuj2 = reader.GetAttribute("code");
        //Console.WriteLine(spracuj2 + " pracujem na tomto");

        //need to find elements used as dictionary
        while (true) {
            reader.ReadToNextSibling("symbol");
            if (reader.AttributeCount >= 4) {
                if (reader.GetAttribute(3) == "OpenOrienteering Logo") {
                    reader.ReadToNextSibling("symbol");
                    var nieco = reader.GetAttribute("code").Split('.');
                    if (Convert.ToInt32(nieco[0]) == Convert.ToInt32(spracuj)) {
                        idhash.Add(Convert.ToInt32(spracuj), Convert.ToInt32(reader.GetAttribute("id")));
                        // Console.WriteLine(spracuj + " **** " + reader.GetAttribute("id"));
                    }

                    break;
                }
            }

        }
        reader.ReadToNextSibling("symbol");
        var r = reader.GetAttribute("code").Split('.');
        if (Convert.ToInt32(r[0]) == Convert.ToInt32(spracuj2)) {
            idhash.Add(Convert.ToInt32(spracuj2), Convert.ToInt32(reader.GetAttribute("id")));
            // Console.WriteLine(spracuj2+" **** " + reader.GetAttribute("id"));
        }

        //parsing Contours
        reader.ReadToFollowing("parts");
        reader.ReadToDescendant("object");
        int minX = 999999999;
        int maxX = -999999999;
        int minY = 999999999;
        int maxY = -999999999;

        List<Vector3[]> clist = new List<Vector3[]>();
        List<Vector3> contour = new List<Vector3>();

        while (reader.ReadToFollowing("object")) {
            if (idhash.ContainsValue(Convert.ToInt32(reader.GetAttribute("symbol")))) {
                //Fill the contour field with a contour object

                contour.Clear();
                reader.ReadToFollowing("coords");
                reader.ReadToDescendant("coord");
                minX = Mathf.Min(minX, Convert.ToInt32(reader.GetAttribute("x")));
                minY = Mathf.Min(minY, Convert.ToInt32(reader.GetAttribute("y")));
                maxX = Mathf.Max(maxX, Convert.ToInt32(reader.GetAttribute("x")));
                maxY = Mathf.Max(maxY, Convert.ToInt32(reader.GetAttribute("y")));

               // Debug.Log("X:"+reader.GetAttribute("x")+" Y:" + reader.GetAttribute("y"));
                contour.Add(new Vector3(float.Parse(reader.GetAttribute("x"), CultureInfo.InvariantCulture.NumberFormat), 
                                        float.Parse(reader.GetAttribute("y"),CultureInfo.InvariantCulture.NumberFormat),
                                                                                                                     0));

                while (reader.ReadToNextSibling("coord"))
                {
                   // Debug.Log("X:" + reader.GetAttribute("x")+ " Y:"+reader.GetAttribute("y"));
                    contour.Add(new Vector3(float.Parse(reader.GetAttribute("x"), CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(reader.GetAttribute("y"), CultureInfo.InvariantCulture.NumberFormat),
                                                                                                                         0));
                    minX = Mathf.Min(minX, Convert.ToInt32(reader.GetAttribute("x")));
                    minY = Mathf.Min(minY, Convert.ToInt32(reader.GetAttribute("y")));
                    maxX = Mathf.Max(maxX, Convert.ToInt32(reader.GetAttribute("x")));
                    maxY = Mathf.Max(maxY, Convert.ToInt32(reader.GetAttribute("y")));
                }
                reader.ReadToNextSibling("object");
                clist.Add(contour.ToArray());
            }
        }

        myTerr.load(clist);

        return true;
    }
}
