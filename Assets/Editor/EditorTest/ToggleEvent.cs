using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;


[CreateAssetMenu(menuName = "ClipEventOnTrack")]
public class ToggleEvent: ScriptableObject
{
    public AnimationEventsData clipEvent;
    public bool isSelected=false;
    public Rect box;

    public ToggleEvent(Rect rect, bool sel)
    {
        box = rect;
        isSelected = sel;
    }
}