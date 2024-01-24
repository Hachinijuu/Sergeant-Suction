//#-----------------------------------------------------
//# Script:     LockedObjectInspector.cs
//#
//# Author:     Mark Proveau (Professor)
//# Course:     GAME1204 - Niagara College - Game Dev Program
//#-----------------------------------------------------

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LockedObject))]
public class LockedObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Reference to the script being inspected
        LockedObject lockedObjectScript = (LockedObject)target;
        EditorGUILayout.BeginVertical("HelpBox");

        // Style for the title bar
        GUIStyle titleStyle = new GUIStyle("HelpBox");
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 14;
        titleStyle.normal.textColor = Color.white;
        titleStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/tab active.png") as Texture2D;

        GUIStyle headerStyle = new GUIStyle(EditorStyles.label);
        headerStyle.fontStyle = FontStyle.Bold;

        // Draw a title bar
        GUILayout.Box(" Lock and Key Settings ", titleStyle);

        // Draw an outlined section
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("HelpBox");
        // Display the lockedObject variable
        lockedObjectScript.lockedObject = (Lock)EditorGUILayout.ObjectField(new GUIContent("Locked Object", "Reference to the chest object."), lockedObjectScript.lockedObject, typeof(Lock), true);
        // Display the lockedObject variable
        lockedObjectScript.keyPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Key Prefab", "Prefab for respawning keys."), lockedObjectScript.keyPrefab, typeof(GameObject), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("HelpBox");
        // Display the keyUseSound variable
        lockedObjectScript.keyUseSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Key Use Sound", "Audio clip to play when using a key."), lockedObjectScript.keyUseSound, typeof(AudioClip), false);
        // Display the chestOpenSound variable
        lockedObjectScript.chestOpenSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Object Open Sound", "Audio clip to play when the chest is opened."), lockedObjectScript.chestOpenSound, typeof(AudioClip), false);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("HelpBox");
        // Display the spawnPrefabs list
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15); // Add an indentation
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPrefabs"), new GUIContent("Spawn Prefabs", "Prefab to spawn when chest is opened (optional)."), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.HelpBox
        (
            "- You must use at least one Key object!\n" +
            "- Can have multiple keys\n" +
            "- Doesn't need to spawn anything, just remove items from the list",
            MessageType.Info
        );

        EditorGUILayout.EndVertical();

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
