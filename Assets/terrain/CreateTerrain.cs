using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CreateTerrain : MonoBehaviour {

    [SerializeField]
    private int xSize;
    [SerializeField]
    private int zSize;
    [SerializeField]
    private int heightScale = 5;//hoogte scale in units
    [SerializeField]
    private float scaleModefier;
    [SerializeField]
    private float pulldownModefier;
    [SerializeField]
    private float detai = 5.0f;//hoe stijl zijn je heuvels?
    private float randomHeight;
    private float randomSeed;
    [SerializeField]
    private float seed = 1000;
    private float randomDetai;

    [SerializeField]
    private bool showVirtualGrid;

    Vector3[] virtualVertices;
    Object[,] planes;
    Thread buildThread;

    readonly object locker = new object();



    public void Generate()
    {
        randomSeed = (float)Random.Range(150, 300) / 100;
        buildThread = new Thread(Generation);
        planes = new Object[xSize, zSize];
        for (int i = 0, z = 0 - zSize / 2; z <= zSize / 2; z++)
        {
            for (int x = 0 - xSize / 2; x <= xSize / 2; x++, i++)
            {
                Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                virtualVertices[i] = new Vector3(x, 0, z);
            }
        }

        buildThread.Start();
    }

    void Generation()
    {
        lock (locker)
        {
            virtualVertices = new Vector3[((xSize * 20) + 1) * ((zSize * 20) + 1)];
            for (int i = 0, z = 0 - (zSize * 20) / 2; z <= (zSize * 20) / 2; z++)
            {
                for (int x = 0 - (xSize * 20) / 2; x <= (xSize * 20) / 2; x++, i++)
                {
                    virtualVertices[i] = new Vector3(x, 0, z);
                }
            }
            //randomSeed = (float)Random.Range(150, 300) / 100;
            randomHeight = heightScale + heightScale * (randomSeed / 10);
            randomDetai = detai * (randomSeed / 2);
            for (int v = 0; v < virtualVertices.Length; v++)
            {
                virtualVertices[v].y = (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / randomDetai) * randomSeed, ((virtualVertices[v].z + seed - 1000) / randomDetai) * randomSeed) * randomHeight) - (Mathf.Pow(virtualVertices[v].x, 2) / pulldownModefier) - (Mathf.Pow(virtualVertices[v].z, 2) / pulldownModefier);
                virtualVertices[v].y += (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / 1.2f) * randomSeed, ((virtualVertices[v].z + seed - 1000) / 4f) * randomSeed) * (randomHeight / 20));
                virtualVertices[v].y += (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / 0.1f) * randomSeed, ((virtualVertices[v].z + seed - 1000) / 1f) * randomSeed) * (randomHeight / 40));
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (showVirtualGrid)
        {
            foreach(Vector3 v in virtualVertices)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(v, Vector3.one/2);
            }
        }
    }
}
