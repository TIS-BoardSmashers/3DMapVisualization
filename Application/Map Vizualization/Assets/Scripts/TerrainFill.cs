using UnityEngine;

public class TerrainFill : MonoBehaviour
{

    //public TerrainData tData;
    //public Terrain myTerrain;
    //public int xBase = 0;
    //public int yBase = 0;
    public float[,] heights;
    private int res;

    // Use this for initialization
    void Start() {
        /*int res = tData.heightmapResolution;
        heights = new float[res, res];

        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                heights[i, j] = Random.Range(0f, 0.005f);
            }
        }
        tData.SetHeightsDelayLOD(xBase, yBase, heights);*/
    }

    // Update is called once per frame
    void Update() {

    }

    public void FillTerrain(float[][] input, TerrainData tData, Terrain myTerrain, int xBase, int yBase) {
        //tData = (TerrainData)Object.Instantiate(Resources.Load("Terrains/TerrainHighRes"));
        res = tData.heightmapResolution;
        heights = new float[res, res];
        Debug.Log(res);

        string debstr = "[";
        for (int i = 0; i < input.Length; i++) {
            debstr += "[";
            for (int j = 0; j < input[0].Length; j++) {
                debstr += input[i][j].ToString() + ",";
            }
            debstr += "]";
        }
        debstr += "]";
        Debug.Log(debstr);

        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                heights[i, j] = input[i][j] / 70;
            }
        }
        Debug.Log(heights.ToString());
        tData.SetHeightsDelayLOD(xBase, yBase, heights);
    }
}
