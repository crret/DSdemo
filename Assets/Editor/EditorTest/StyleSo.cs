using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "StyleSo")]
public class StyleSo : ScriptableObject
{   
    public GUIStyle style;
    public StyleSo child;
    public void add(StyleSo Child)
    {
        UnityEditor.AssetDatabase.AddObjectToAsset(child, this);
    }
   
}
