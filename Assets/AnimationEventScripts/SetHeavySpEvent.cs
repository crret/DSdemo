using System;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

public class SetHeavySpEvent :AnimationEventsData
{ 
    public SetHeavySpType setHeavySpType;
    public Func<SetHeavySpEvent, IEnumerator> SetHeavySpFunc;
    
}
