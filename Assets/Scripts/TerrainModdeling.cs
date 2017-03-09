using UnityEngine;
using System.Collections;

public class TerrainModdeling : MonoBehaviour {

    public Terrain myTerrain;
    int xResolution;
    int zResolution;
    float[,] heights;

    // Use this for initialization
    void Start () {
        xResolution = myTerrain.terrainData.heightmapWidth;
        zResolution = myTerrain.terrainData.heightmapHeight;
        heights = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
    }
	
	// Update is called once per frame
	void Update () {
	    //Noget kode der skal være på controllere der checker om man bruger det her og man klikker
	}


    private void raiseTerrain(Vector3 point) //https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/
    {
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        float[,] height = myTerrain.terrainData.GetHeights(terX - 4, terZ - 4, 9, 9);  //new float[1,1];

        for (int tempY = 0; tempY < 9; tempY++)
            for (int tempX = 0; tempX < 9; tempX++)
            {
                float dist_to_target = Mathf.Abs((float)tempY - 4f) + Mathf.Abs((float)tempX - 4f);
                float maxDist = 8f;
                float proportion = dist_to_target / maxDist;

                height[tempX, tempY] += 0.01f * (1f - proportion);
                heights[terX - 4 + tempX, terZ - 4 + tempY] += 0.01f * (1f - proportion); // skal måske commentes out
            }

        myTerrain.terrainData.SetHeights(terX - 4, terZ - 4, height);
    }

    private void lowerTerrain(Vector3 point)
    {
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        float[,] height = myTerrain.terrainData.GetHeights(terX - 4, terZ - 4, 9, 9);  //new float[1,1];

        for (int tempY = 0; tempY < 9; tempY++)
            for (int tempX = 0; tempX < 9; tempX++)
            {
                float dist_to_target = Mathf.Abs((float)tempY - 4f) + Mathf.Abs((float)tempX - 4f);
                float maxDist = 8f;
                float proportion = dist_to_target / maxDist;

                height[tempX, tempY] -= 0.01f * (1f - proportion);
                heights[terX - 4 + tempX, terZ - 4 + tempY] += 0.01f * (1f - proportion); // skal måske commentes out
            }

        myTerrain.terrainData.SetHeights(terX - 4, terZ - 4, height);
    }
}
