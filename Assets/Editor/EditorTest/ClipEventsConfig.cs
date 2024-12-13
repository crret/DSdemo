using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ClipEventsConfig")]
public class ClipEventsConfig : ScriptableObject
{
    
    public List<AnimationClip> animationClips = new List<AnimationClip>();
    
    public List<TracksList> tracksLists = new List<TracksList>();
}
