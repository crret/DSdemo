using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ClipTrack")]
public class ToggleTrack : ScriptableObject
{   
    public Rect rect;
    [FormerlySerializedAs("animEventType")] public ClipEventType clipEventType;
    public AnimationClip animationClip;
    public bool toggleState = false; // Toggle 的状态
    public List<ToggleEvent> toggleEvents = new List<ToggleEvent>();

}


