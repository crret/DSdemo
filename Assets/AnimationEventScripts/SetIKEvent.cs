using System;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.Events;

namespace SG
{
    

    [CreateAssetMenu(menuName = "SetRightIKOff")]
    public class SetIKEvent : AnimationEventsData
    {
        public Func<SetIKEvent, IEnumerator> SetIKFunc;
        public SetIKType setIKType;
      

    }

}