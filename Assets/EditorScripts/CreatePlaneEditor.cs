using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreatePlane))]
public class CreatePlaneEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CreatePlane planeGen = (CreatePlane)target;
        if (DrawDefaultInspector())
        {

        }
        if (GUILayout.Button("generate"))
        {
            planeGen.Generate();
        }
        if (GUILayout.Button("debug"))
        {
            planeGen.DebugNodes();
        }
    }
}
