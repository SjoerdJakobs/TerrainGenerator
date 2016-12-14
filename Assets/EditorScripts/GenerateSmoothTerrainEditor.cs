using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreateSmoothTerrain))]
public class GenerateSmoothTerrainEditor : Editor
{
    /// <summary>
    /// Editor script for the CreateSmoothTerrain script, incase you want to generate a terrain outside runtime.
    /// </summary>
    public override void OnInspectorGUI()
    {
        CreateSmoothTerrain terrainGen = (CreateSmoothTerrain)target;
        if (DrawDefaultInspector())
        {

        }
        if (GUILayout.Button("generate"))
        {
            terrainGen.Generate();
        }
    }
}
