using UnityEngine;
using System.Collections;

public class CreateSmoothTerrain : MonoBehaviour
{
    /// <summary>
    /// This script creates a terrain by using a plane and moving its vertices on the y-axis and spawning given flora.
    /// This script wont create a plane itself and will not work without a plane.
    /// Add objects to spawn in the editor for the flora.
    /// </summary>
    //Ints
    [SerializeField]
    private int heightScale = 5;//hoogte scale in units
    [SerializeField]
    private float scaleModefier;
    [SerializeField]
    private float pulldownModefier;
    //Ints

    //Floats
    [SerializeField]
    private float detai = 5.0f;//hoe stijl zijn je heuvels?
    private float randomFarmer;
    private float randomHeight;
    private float randomSeed;
    [SerializeField]
    private float seed = 1000;
    private float randomDetai;
    //Floats


    //GameObjects
    [SerializeField]
    private GameObject bush1;
    [SerializeField]
    private GameObject bush2;
    [SerializeField]
    private GameObject grass1;
    [SerializeField]
    private GameObject grass2;
    [SerializeField]
    private GameObject rock1;
    [SerializeField]
    private GameObject rock2;
    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject treesObject;
    [SerializeField]
    private GameObject bushesObject;
    [SerializeField]
    private GameObject stonesObject;
    [SerializeField]
    private GameObject grassObject;
    //GameObjects

    public void Generate()
    {
        randomSeed = (float)Random.Range(150, 300) / 100;
        //print(randomSeed +" randomSeed");
        randomHeight = heightScale + heightScale * (randomSeed / 10);
        //print(randomHeight+" randomheight");
        randomDetai = detai * (randomSeed / 2);
        //print(randomDetai +" random detai");
        Mesh plane = this.GetComponent<MeshFilter>().mesh;
        Vector3[] newVertices = plane.vertices;
        for (int v = 0; v < newVertices.Length; v++)
        {
            //(Mathf.PerlinNoise(((x + (seed) - 1000) / randomDetai) * (randomSeed),((z + (seed) - 1000) / randomDetai) * (randomSeed)) * randomHeight) - (int)((Mathf.Pow((x - (width / 2)), 2)) / (width * 6.5f)) - (int)((Mathf.Pow((z - (depth / 2)), 2)) / (depth * 6.2f));
            newVertices[v].y = (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / randomDetai) * randomSeed, ((newVertices[v].z + seed - 1000) / randomDetai) * randomSeed) * randomHeight) - (Mathf.Pow(newVertices[v].x, 2)/ pulldownModefier) - (Mathf.Pow(newVertices[v].z, 2)/ pulldownModefier);
            newVertices[v].y += (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / 1.2f) * randomSeed, ((newVertices[v].z + seed - 1000) / 4f) * randomSeed) * (randomHeight / 20 ));
            newVertices[v].y += (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / 0.1f) * randomSeed, ((newVertices[v].z + seed - 1000) / 1f) * randomSeed) * (randomHeight / 40));

            if (newVertices[v].y > 2f)
            {
                //print(1);
                randomFarmer = (float)Random.Range(0, 100) / 10;


                if (randomFarmer <= 0.5f && newVertices[v].y > 3f)
                {
                    //print("tree1");
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(trees[Random.Range(0, trees.Length)], new Vector3(newVertices[v].x * scaleModefier + (randomFarmer / 2), newVertices[v].y - 0.4f, newVertices[v].z * scaleModefier + (randomFarmer / 2)), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = treesObject.transform;
                }
                else if (randomFarmer <= 1f && randomFarmer > 0.5f && newVertices[v].y > 3.5f)
                {
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(bush1, new Vector3(newVertices[v].x * scaleModefier - (randomFarmer / 2), newVertices[v].y + 0.2f, newVertices[v].z * scaleModefier - (randomFarmer / 2)), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = bushesObject.transform;
                    //print("bush");
                }
                else if (randomFarmer <= 1.5f && randomFarmer > 1f && newVertices[v].y > 3.5f)
                {
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(trees[Random.Range(0, trees.Length)], new Vector3(newVertices[v].x * scaleModefier - (randomFarmer / 2), newVertices[v].y - 0.4f, newVertices[v].z * scaleModefier - (randomFarmer / 2)), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = treesObject.transform;
                    //print("tree2");
                }
                else if (randomFarmer <= 2f && randomFarmer > 1.5f)
                {
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(rock1, new Vector3(newVertices[v].x * scaleModefier + (randomFarmer / 2), newVertices[v].y + 0.5f, newVertices[v].z * scaleModefier - (randomFarmer / 2)), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = stonesObject.transform;
                    //print("rock1");
                }
                else if (randomFarmer <= 2.5f && randomFarmer > 2f)
                {
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(rock2, new Vector3(newVertices[v].x * scaleModefier + (randomFarmer / 2), newVertices[v].y + 0.5f, newVertices[v].z * scaleModefier + (randomFarmer / 2)), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = stonesObject.transform;
                    //print("rock2");
                }
                else if (randomFarmer > 7.5f)
                {
                    GameObject spawnedObject;
                    spawnedObject = Instantiate(grass1, new Vector3(newVertices[v].x * scaleModefier, newVertices[v].y - 0.1f, newVertices[v].z * scaleModefier), Quaternion.identity) as GameObject;
                    spawnedObject.transform.parent = grassObject.transform;
                    //print("grass1");
                }
            }
        }
        plane.vertices = newVertices;

        MeshRenderer planeRenderer = GetComponent<MeshRenderer>();
        //planeRenderer.material = beachLands;
        this.gameObject.AddComponent<MeshCollider>();

        plane.RecalculateBounds();
        plane.RecalculateNormals();
        ;
    }
}
