using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugButtonEventSystem))]
public class DebugButtonEventDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugButtonEventSystem dbes = (DebugButtonEventSystem)target;

        if(GUILayout.Button("Invoke Events"))
        {
            dbes.onButton();
        }
    }
}