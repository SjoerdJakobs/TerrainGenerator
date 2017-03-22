using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public enum TerrainType
{
    pangea = 0,
    straitZ = 1,
    straitX = 2,
    islands = 3,
    roundIsland = 4,
    inlandSea = 5,
    flat = 6
}

public class CreateTerrain : MonoBehaviour {

    [SerializeField]
    private int xSize;
    [SerializeField]
    private int zSize;

    [SerializeField]
    private float scaleModefier;
    [SerializeField]
    private float pulldownModefier;
    private float randomHeight;
    private float randomSeed;
    private float randomDetai;

    [SerializeField]
    private TerrainType TerrainTypes;    

    [SerializeField]
    private List<PerlinMod> perlinModifiers;

    [SerializeField]
    private Material terrainMat;


    Vector3[] virtualVertices;
    VirtualSubPlane[,] virtualPlanes;
    GameObject[,] planes;
    GameObject chunkParrent;
    Thread buildThread;


    //readonly object locker = new object();



    public void Generate()
    {
        if (chunkParrent != null)
        {
            Destroy(chunkParrent);
            foreach (GameObject G in planes)
            {
                Destroy(G);
            }
            Array.Clear(virtualPlanes, 0, virtualPlanes.Length);
            Array.Clear(planes, 0, planes.Length);
            Array.Clear(virtualVertices, 0, virtualVertices.Length);

            virtualPlanes = new VirtualSubPlane[0,0];
            planes = new GameObject[0, 0];
            virtualVertices = new Vector3[0];
        }
        chunkParrent = new GameObject("Terrain");
        randomSeed = (float)UnityEngine.Random.Range(150, 300) / 100;
        //buildThread = new Thread(GenerateVirtualVertices);
        virtualPlanes = new VirtualSubPlane[xSize, zSize];
        planes = new GameObject[xSize, zSize];
        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++, i++)
            {
                GameObject chunk = new GameObject("chunk");
                VirtualSubPlane virtualPlane = new VirtualSubPlane();
                chunk.transform.localScale = new Vector3(scaleModefier, 1, scaleModefier);
                chunk.transform.position = new Vector3(((x*20) * scaleModefier)-x*(1*scaleModefier), 0, ((z*20) * scaleModefier)-z*(1*scaleModefier));
                chunk.AddComponent<CreatePlane>().Generate();
                chunk.GetComponent<Renderer>().material = terrainMat;
                planes[x, z] = chunk;
                virtualPlane.vertices = chunk.GetComponent<MeshFilter>().sharedMesh.vertices;
                virtualPlanes[x, z] = virtualPlane;
                chunk.transform.parent = chunkParrent.transform;
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
        for (int v = 0; v < virtualVertices.Length; v++)
        {
            switch (TerrainTypes)
            {
                case TerrainType.flat:
                    
                    break;
                case TerrainType.inlandSea:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                case TerrainType.islands:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                case TerrainType.pangea:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                case TerrainType.roundIsland:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                case TerrainType.straitX:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                case TerrainType.straitZ:
                    virtualVertices[v].y -= ((Mathf.Pow(virtualVertices[v].x - gridDimx * scaleModefier / 2, 2) / pulldownModefier) - xSize) + ((Mathf.Pow(virtualVertices[v].z - gridDimz * scaleModefier / 2, 2) / pulldownModefier) - zSize);
                    break;
                default:
                    Debug.LogError("wrong switch input");
                    break;
            }
            for (int i = 0; i < perlinModifiers.Count; i++)
            {
                randomHeight = perlinModifiers[i].heightScale + perlinModifiers[i].heightScale * (randomSeed / 10);
                randomDetai = perlinModifiers[i].detai * (randomSeed / 2);
                virtualVertices[v].y += perlinModifiers[i].slopeRatio.Evaluate(Mathf.PerlinNoise(((virtualVertices[v].x + perlinModifiers[i].seed - 1000) / randomDetai) * randomSeed, ((virtualVertices[v].z + perlinModifiers[i].seed - 1000) / randomDetai) * randomSeed)) * randomHeight;
            }
            //virtualVertices[v].y = (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / randomDetai) * randomSeed, ((virtualVertices[v].z + seed - 1000) / randomDetai) * randomSeed) * randomHeight) - ((Mathf.Pow(virtualVertices[v].x - gridDimx*scaleModefier/2, 2) / pulldownModefier)-xSize) - ((Mathf.Pow(virtualVertices[v].z-gridDimz*scaleModefier/2, 2) / pulldownModefier)-zSize);
            //virtualVertices[v].y += (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 500) / randomDetai/1000) * randomSeed, ((virtualVertices[v].z + seed - 1000) / randomDetai/500) * randomSeed) * (randomHeight/10));
            //virtualVertices[v].y += (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / 1.2f) * randomSeed, ((virtualVertices[v].z + seed - 1000) / 4f) * randomSeed) * (randomHeight / 20));
            //virtualVertices[v].y += (Mathf.PerlinNoise(((virtualVertices[v].x + seed - 1000) / 0.1f) * randomSeed, ((virtualVertices[v].z + seed - 1000) / 1f) * randomSeed) * (randomHeight / 40));
        }
    }

    void SetPlaneVertices()
    {
        int subPlanex = 0;
        int subPlanez = 0;
        int row = 0;
        for (int i = 0, j = 0, t = 0; t < (xSize * 20) * (zSize * 20); i++, j++, t++)
        {
            if (j > 19)
            {
                subPlanex++;
                i--;
                if (subPlanex == xSize)
                {
                    subPlanex = 0;
                    i++;
                    row++;
                }
                j = 0;
                if (row > 19)
                {
                    row = 0;
                    i -= 1+(xSize * 19);
                    subPlanez++;
                }
            }
            virtualPlanes[subPlanex, subPlanez].vertices[(row * 20) + j].y = virtualVertices[i].y;
        } 
        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++, i++)
            {
                Mesh planeMesh = planes[x, z].GetComponent<MeshFilter>().sharedMesh;
                planeMesh.vertices = virtualPlanes[x, z].vertices;
                planeMesh.RecalculateBounds();
                planeMesh.RecalculateNormals();
            }
        }

        Array.Clear(virtualPlanes, 0, virtualPlanes.Length);
        Array.Clear(planes, 0, planes.Length);
        Array.Clear(virtualVertices, 0, virtualVertices.Length);
    }

    #region Debuging
    /*
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
        if (showVirtualGrid && virtualVertices.Length >= 0)
        {
            foreach(Vector3 v in virtualVertices)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(v, Vector3.one/2);
            }
        }
    }*/
    #endregion
}

public class VirtualSubPlane
{
    public Vector3[] vertices;
}

[System.Serializable]
public class PerlinMod
{
    public string ModifierName;
    public int heightScale;
    public float detai;
    public float seed;
    public AnimationCurve slopeRatio;
}






/*foreach (GameObject m in planes)
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