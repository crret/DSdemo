using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "TracksList")]
public class TracksList : ScriptableObject
{   
    public AnimationClip animationClip;
    public List<Vector3> deltaPositions;
    public bool deltaPositionsLoaded=false;
    public List<ToggleTrack> tracksList = new List<ToggleTrack>();
}
