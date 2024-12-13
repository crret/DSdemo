using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackPopMenu:EditorWindow
{   
    public static void ShowExample(float positionX, float positionY)
    {
        TrackPopMenu wnd = GetWindow<TrackPopMenu>();
        wnd.titleContent = new GUIContent();
        wnd.position = new Rect(positionX, positionY, 100, 50); 
        wnd.maxSize = new Vector2(100, 50);
        wnd.minSize = new Vector2(100, 50);
        
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        
        EditorGUILayout.EndVertical();
    }
}
