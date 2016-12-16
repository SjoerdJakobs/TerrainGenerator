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
    private Material terrainMat;

    [SerializeField]
    private bool showVirtualGrid;

    Vector3[] virtualVertices;
    GameObject[,] planes;
    Thread buildThread;

    readonly object locker = new object();



    public void Generate()
    {
        randomSeed = (float)Random.Range(150, 300) / 100;
        //buildThread = new Thread(GenerateVirtualVertices);
        planes = new GameObject[xSize, zSize];
        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++, i++)
            {
                GameObject chunk = new GameObject("chunk");
                chunk.transform.localScale = new Vector3(scaleModefier, 1, scaleModefier);
                chunk.transform.position = new Vector3(((x*20) * scaleModefier)-xSize*20/2+10, 0, ((z*20) * scaleModefier)-zSize*20/2+10);
                chunk.AddComponent<CreatePlane>().Generate();
                chunk.GetComponent<Renderer>().material = terrainMat;
                planes[x, z] = chunk;
            }
        }
        //buildThread.Start();
        GenerateVirtualVertices();
        SetPlaneVertices();
    }

    void GenerateVirtualVertices()
    {
        int gridDimx = (xSize * 20) + 1;
        int gridDimz = (zSize * 20) + 1;

        virtualVertices = new Vector3[(gridDimx * gridDimz)];
        for (int i = 0, z = 0 - (zSize * 20) / 2; z <= (zSize * 20) / 2; z++)
        {
            for (int x = 0 - (xSize * 20) / 2; x <= (xSize * 20) / 2; x++, i++)
            {
                virtualVertices[i] = new Vector3(x * scaleModefier, 0, z * scaleModefier);
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

    void SetPlaneVertices()
    {
        for (int i = 0; i < virtualVertices.Length; i++)
        {
            //print("test");
            //print(planes.Length);
        }
        for (int i = 0; i < virtualVertices.Length; i++)
        {


            int hoofdPlanex = (i % (21 * 21)) / 21;
            int hoofdPlanez = i / (21 * 21);

            int subPlanex = i % 21;
            int subPlanez = (i % (21 * 21)) / (21 * 21);

            //print(hoofdPlanex + " hoofdplanex ");
            //print(hoofdPlanez + " hoofdplanez ");

            //print(subPlanex + " subplanex " + i);
            //print(subPlanez + " subplanez " + i);
            Mesh plane = planes[0, 0].GetComponent<MeshFilter>().mesh;
            plane.vertices[subPlanex * subPlanez].y = virtualVertices[i].y;
            print(planes[0, 0].gameObject.name);
            print(plane.vertices[i]);
            if (subPlanex == 0 && hoofdPlanex > 0)
            {
                //Mesh prevSubPlane = array[hoofdPlanez][hoofdPlanex - 1];
                //prevSubPlane[subPlanez][20].y = y;
            }
            if (subPlanez == 0 && hoofdPlanez > 0)
            {
                //Plane prevSubPlane = array[hoofdPlanez - 1][hoofdPlanex];
                //prevSubPlane[20][subPlanex].y = y;
            }
            plane.RecalculateBounds();
            plane.RecalculateNormals();
            plane.Optimize();

        }
        /*foreach(GameObject G in planes)
        {
            Mesh plane = planes[0, 0].GetComponent<MeshFilter>().mesh;
            plane.RecalculateBounds();
            plane.RecalculateNormals();
            plane.Optimize();
        }*/
    }

    public void OnDrawGizmos()
    {
        if (showVirtualGrid && virtualVertices.Length >= 0)
        {
            foreach(Vector3 v in virtualVertices)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(v, Vector3.one/2);
            }
        }
    }
}
