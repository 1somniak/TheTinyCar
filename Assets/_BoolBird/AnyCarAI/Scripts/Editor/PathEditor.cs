using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointsPath))]

public class PathEditor : Editor
{
    private SerializedObject soTarget;
   
    private SerializedProperty smoothRoute;
    private SerializedProperty editorVisualisationSubsteps;

    public Texture pathLabel;
    
    private void OnEnable()
    {
        soTarget = new SerializedObject(target);

        pathLabel = Resources.Load<Texture>("waypointsPathLabel");

        smoothRoute = soTarget.FindProperty("smoothRoute");
        editorVisualisationSubsteps = soTarget.FindProperty("editorVisualisationSubsteps");
    }


    public override void OnInspectorGUI()
    {
        GUILayout.Box(pathLabel, GUILayout.ExpandWidth(true), GUILayout.Height(55));
        soTarget.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();

        if (smoothRoute.boolValue == true)
        {
            GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
        }
        else
        {
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Smooth Path"))
        {
            if (smoothRoute.boolValue == true)
            {
                smoothRoute.boolValue = false;
            }
            else
            {
                smoothRoute.boolValue = true;
            }
        }

        if (smoothRoute.boolValue)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("", "box");

            EditorGUILayout.PropertyField(editorVisualisationSubsteps, GUIContent.none, true);


            GUILayout.EndVertical();
        }


        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }
    }



}
