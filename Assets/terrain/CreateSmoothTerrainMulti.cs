using UnityEngine;
using System.Collections;

public class CreateSmoothTerrainMulti : MonoBehaviour {

	[SerializeField]
    private int heightScale = 5;//hoogte scale in units
    [SerializeField]
    private float detai = 5.0f;//hoe stijl zijn je heuvels?
   
    void Start()
    {
        Mesh plane = this.GetComponent<MeshFilter>().mesh;
        Vector3[] newVertices = plane.vertices;
        for (int v = 0; v < newVertices.Length; v++)
        {
            newVertices[v].y = Mathf.PerlinNoise((newVertices[v].x + this.transform.position.x) / detai, (newVertices[v].z + this.transform.position.z) / detai)*heightScale;
        }
        plane.vertices = newVertices;
        plane.RecalculateBounds();
        plane.RecalculateNormals();
        this.gameObject.AddComponent<MeshCollider>();
        print("done in " + Time.realtimeSinceStartup);
	}
}
