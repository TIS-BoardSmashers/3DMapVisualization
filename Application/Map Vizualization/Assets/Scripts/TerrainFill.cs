using UnityEngine;

public class TerrainFill : MonoBehaviour
{

    public TerrainData tData;
    public Terrain myTerrain;
    public int xBase = 0;
    public int yBase = 0;
    public float[,] heights;

    // Use this for initialization
    void Start() {
        int res = tData.heightmapResolution;
        heights = new float[res, res];

        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                heights[i, j] = Random.Range(0f, 0.001f);
            }
        }
        tData.SetHeightsDelayLOD(xBase, yBase, heights);
    }

    // Update is called once per frame
    void Update() {

    }
}
