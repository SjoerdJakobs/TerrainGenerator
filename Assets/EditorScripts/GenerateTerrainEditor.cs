using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreateTerrain))]
public class GenerateTerrainEditor : Editor
{
    /// <summary>
    /// Editor script for the CreateSmoothTerrain script, incase you want to generate a terrain outside runtime.
    /// </summary>
    public override void OnInspectorGUI()
    {
        CreateTerrain terrainGen = (CreateTerrain)target;
        if (DrawDefaultInspector())
        {

        }
        if (GUILayout.Button("generate"))
        {
            terrainGen.Generate();
        }
        if (GUILayout.Button("debug"))
        {
            terrainGen.DebugNodes();
        }
    }
}
