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
    private int stopDebug;
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
    private bool showDebugGizmos;

    Vector3[] virtualVertices;
    Vector3[] debugVertices;
    VirtualSubPlane[] virtualPlanes;
    
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
                chunk.transform.position = new Vector3(((x*20) * scaleModefier)-x*1, 0, ((z*20) * scaleModefier)-z*1);
                chunk.AddComponent<CreatePlane>().Generate();
                chunk.GetComponent<Renderer>().material = terrainMat;
                planes[x, z] = chunk;
                print("b");
            }
        }
        //buildThread.Start();
        GenerateVirtualVertices();
        SetPlaneVertices();
    }

    void GenerateVirtualVertices()
    {
        int gridDimx = (xSize * 20);
        int gridDimz = (zSize * 20);

        virtualVertices = new Vector3[(gridDimx * gridDimz)];
        for (int i = 0, z = 0; z <= (zSize * 19); z++)
        {
            for (int x = 0; x <= (xSize * 19); x++, i++)
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
        //Vector3[] newVertices = new Vector3[20 * 20];
        int subPlanex = 0;
        int subPlanez = 0;
        int row = 0;
        for (int i = 0, j = 0, t = 0; t < (xSize*20)*(zSize*20); i++, j++, t++)
        {
            if (j > 19)
            {
                subPlanex++;
                i--;
                if (subPlanex == xSize)
                {
                    subPlanex = 0;
                    i ++;
                    row++;
                }
                j = 0;
                if (row > 19)
                {
                    row = 0;
                    i -= (xSize*20);
                    subPlanez++;
                }
            }
            print(t +" truecount  "+ i +" i count");
            Mesh plane = planes[subPlanex,subPlanez].GetComponent<MeshFilter>().mesh;
            //print(plane);
            Vector3 vertex = plane.vertices[(row * 20) + j];
            vertex.y = virtualVertices[i].y;
            plane.vertices[(row * 20) + j] = vertex;

        }
        foreach (GameObject m in planes)
        {
            Mesh planeMesh = m.GetComponent<MeshFilter>().mesh;
            planeMesh.RecalculateBounds();
            planeMesh.RecalculateNormals();
            print("a");
        }
        //Mesh plane = planes[0, 0].GetComponent<MeshFilter>().mesh;
        //plane.vertices = newVertices;
        //plane.RecalculateBounds();
        //plane.RecalculateNormals();

        //print("a");

        /*for (int i = 0; i < virtualVertices.Length; i++)
        {        
            //int hoofdPlanex = (i % (20 * 20)) / 20;
            //int hoofdPlanez = i / (20 * 20);

            //int subPlanex = i % 20;
            //int subPlanez = (i % (20 * 20)) / (20 * 20);

            //print(hoofdPlanex + " hoofdplanex ");
            //print(hoofdPlanez + " hoofdplanez ");

            //print(subPlanex + " subplanex " + i);
            //print(subPlanez + " subplanez " + i);
            Mesh plane = planes[0, 0].GetComponent<MeshFilter>().mesh;
            plane.vertices[subPlanex * subPlanez].y = virtualVertices[i].y;
            //print(planes[0, 0].gameObject.name);
            //print(plane.vertices[i]);
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
            

        }*/
    }

    public void DebugNodes()
    {
        StartCoroutine(DebugGrid());
    }

    //public void Debug()
    public IEnumerator DebugGrid()
    {
        debugVertices = new Vector3[virtualVertices.Length];
        showDebugGizmos = true;
        for (int i = 0; i < virtualVertices.Length; i++)
        {
            debugVertices[i] = virtualVertices[i];
            yield return new WaitForSeconds(.1f);
        }
        showDebugGizmos = false;
        yield return null;
    }

    public void OnDrawGizmos()
    {
        if (showDebugGizmos && virtualVertices.Length >= 0)
        {
            foreach (Vector3 v in debugVertices)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(v, Vector3.one / 2);
            }
        }
        if (stopDebug > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(virtualVertices[stopDebug], Vector3.one);
        }

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

public class VirtualSubPlane
{
    public Vector3[] vertices;
}
