using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreatePlane : MonoBehaviour
{

    /// <summary>
    /// this is a simple class to make a plane mesh.
    /// add this script to a object and click generate in the editor or call Generate() from a script.
    /// the centre of the plane wil be the same position as the object this script is attatched to.
    /// this script also doesnt add a material so that needs to be added to the object or the plane will be pink.
    /// </summary>

    public int xSize = 19;
    public int zSize = 19;

    private bool showDebugGizmos;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] debugVertices;

    public void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, z);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void DebugNodes()
    {
        StartCoroutine(DebugGrid());
    }

    //public void Debug()
    public IEnumerator DebugGrid()
    {
        debugVertices = new Vector3[vertices.Length];
        showDebugGizmos = true;
        for (int i = 0; i < vertices.Length; i++)
        {
            debugVertices[i] = vertices[i];
            yield return new WaitForSeconds(.1f);
        }
        showDebugGizmos = false;
        yield return null;
    }
    public void OnDrawGizmos()
    {
        if (showDebugGizmos && vertices.Length >= 0)
        {
            foreach (Vector3 v in debugVertices)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(v+transform.position, Vector3.one / 2);
            }
        }
    }
}