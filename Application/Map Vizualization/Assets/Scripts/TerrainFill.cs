using UnityEngine;

public class TerrainFill : MonoBehaviour
{
    public float[,] heights;
    private int res;
    /*public TerrainData mytData;
    public Terrain myTerrain;
    public int xBase0 = 0;
    public int yBase0 = 0;*/

    
    void Start() {
        /*res = mytData.heightmapResolution;
        heights = new float[res, res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                heights[i, j] = 0;
            }
        }
        mytData.SetHeightsDelayLOD(xBase0, yBase0, heights);*/
    }

    
    void Update() {
    }

    
    public void FillTerrain(float[][] input, TerrainData tData, Terrain myTerrain, int xBase, int yBase) {
        /* Fills terrain with values from scanline and quantization, but it must be divided by large number.
         * Terrain needs float values between 0 and 1.
         */
        res = tData.heightmapResolution;
        heights = new float[res, res];

        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                heights[i, j] = input[i][j] / 70;
            }
        }
        tData.SetHeightsDelayLOD(xBase, yBase, heights);
    }
}
