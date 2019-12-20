using UnityEngine;
using System;
using UnityEditor;

[CustomEditor( typeof( GameObjectSearcher ) )]
[System.Serializable]
public class GameObjectSearcherCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var target_cs = ( GameObjectSearcher ) target;
        DrawDefaultInspector();

        GameObjectSearcher gameObjectSearcherScript = target_cs;

        if ( !Application.isPlaying )
        {
            string tagStr = gameObjectSearcherScript.searchTag;

            tagStr = EditorGUILayout.TagField( "Search Tag", tagStr );
            if ( tagStr != gameObjectSearcherScript.searchTag )
            {
                gameObjectSearcherScript.searchTag = tagStr;
            }
        }
    }
}