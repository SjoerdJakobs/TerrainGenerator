using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreatePlane))]
public class CreatePlaneEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CreatePlane terrainGen = (CreatePlane)target;
        if (DrawDefaultInspector())
        {

        }
        if (GUILayout.Button("generate"))
        {
            terrainGen.Generate();
        }
    }
}
