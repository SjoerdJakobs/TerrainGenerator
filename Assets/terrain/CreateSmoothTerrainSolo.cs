using UnityEngine;
using System.Collections;

public class CreateSmoothTerrainSolo : MonoBehaviour {

    //Materials
    [SerializeField]
    private Material beachLands;
    [SerializeField]
    private Material lowLands;
    [SerializeField]
    private Material middleLands;
    [SerializeField]
    private Material highLands;
    //Materials

    //Ints
    [SerializeField]
    private int heightScale = 5;//hoogte scale in units
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
    private GameObject tree1;
    [SerializeField]
    private GameObject tree2;
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
    //GameObjects

    void Start()
    {
        print("ehBos");
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
            //(Mathf.PerlinNoise(((x + (seed) - 1000) / randomDetai) * (randomSeed),                            ((z + (seed) - 1000) / randomDetai) * (randomSeed)) * randomHeight) - (int)((Mathf.Pow((x - (width / 2)), 2)) / (width * 6.5f)) - (int)((Mathf.Pow((z - (depth / 2)), 2)) / (depth * 6.2f));
            newVertices[v].y = (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / randomDetai) * randomSeed, ((newVertices[v].z + seed - 1000) / randomDetai) * randomSeed) * randomHeight) - (Mathf.Pow(newVertices[v].x,2))/12 - (Mathf.Pow(newVertices[v].z, 2)) / 12;
            newVertices[v].y += (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / 1.2f) * randomSeed, ((newVertices[v].z + seed - 1000) / 1.8f) * randomSeed) * (randomHeight/8));
            newVertices[v].y += (Mathf.PerlinNoise(((newVertices[v].x + seed - 1000) / 0.1f) * randomSeed, ((newVertices[v].z + seed - 1000) / 0.6f) * randomSeed) * (randomHeight /13));
            
            if(newVertices[v].y > 2f)
            {
                //print(1);
                randomFarmer = (float)Random.Range(0, 100)/10;

                if (randomFarmer <= 0.5f && newVertices[v].y > 3f)
                {
                    //print("tree1");
                    Instantiate(tree1, new Vector3(newVertices[v].x * 10 + (randomFarmer/2), newVertices[v].y - 0.4f, newVertices[v].z * 10 + (randomFarmer / 2)), Quaternion.identity);
                }
                else if (randomFarmer <= 1f && randomFarmer > 0.5f && newVertices[v].y > 3.5f)
                {
                    Instantiate(bush1, new Vector3(newVertices[v].x * 10 - (randomFarmer / 2), newVertices[v].y + 0.2f, newVertices[v].z * 10 - (randomFarmer / 2)), Quaternion.identity);
                    //print("bush");
                }
                else if (randomFarmer <= 1.5f && randomFarmer > 1f && newVertices[v].y > 3.5f)
                {
                    Instantiate(tree2, new Vector3(newVertices[v].x * 10 - (randomFarmer / 2), newVertices[v].y - 0.4f, newVertices[v].z * 10 - (randomFarmer / 2)), Quaternion.identity);
                    //print("tree2");
                }
                else if (randomFarmer <= 2f && randomFarmer > 1.5f)
                {
                    Instantiate(rock1, new Vector3(newVertices[v].x * 10 + (randomFarmer / 2), newVertices[v].y + 0.5f, newVertices[v].z * 10 - (randomFarmer / 2)), Quaternion.identity);
                    //print("rock1");
                }
                else if (randomFarmer <= 2.5f && randomFarmer > 2f)
                {
                    Instantiate(rock2, new Vector3(newVertices[v].x * 10 + (randomFarmer / 2), newVertices[v].y+ 0.5f, newVertices[v].z * 10 + (randomFarmer / 2)), Quaternion.identity);
                    //print("rock2");
                }
                else if(randomFarmer > 7.5f)
                {
                    Instantiate(grass1, new Vector3(newVertices[v].x * 10, newVertices[v].y- 0.1f, newVertices[v].z * 10), Quaternion.identity);
                    //print("grass1");
                }
            }
        }
        plane.vertices = newVertices;

        //MeshRenderer planeRenderer = GetComponent<MeshRenderer>();
        //planeRenderer.material = beachLands;
        this.gameObject.AddComponent<MeshCollider>();

        plane.RecalculateBounds();
        plane.RecalculateNormals();
        ;
    }
}
