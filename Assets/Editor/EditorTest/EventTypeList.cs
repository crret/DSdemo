using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "EventType List")]
public class EventTypeList : ScriptableObject
{   
    public List<ClipEventType> clipEventTypes = new List<ClipEventType>();
    
    public List<SetIKType> SetIKList = new List<SetIKType>();
    public List<SetHeavySpType> SetHeavySpList = new List<SetHeavySpType>();
}
