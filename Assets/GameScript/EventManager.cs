using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SG
{


    public class EventManager 
    {
        private static EventManager instance;
        private Dictionary<string, IEventInfo> _eventDic = new Dictionary<string, IEventInfo>();
        
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance=new EventManager();
                }
                return instance;
            }
        }

        public void AddEventListener(string name, UnityAction action)
        {
            if (_eventDic.ContainsKey(name))
            {
                (_eventDic[name] as EventInfo).actions += action;
            }
            else
            {
                _eventDic.Add(name, new EventInfo(action));
            }
        }

        public void EventTrigger(string eventName)
        {
            if (_eventDic.ContainsKey(eventName))
            {
                if ((_eventDic[eventName] as EventInfo).actions != null)
                {
                    (_eventDic[eventName] as EventInfo).actions.Invoke();
                }
            }
            
        }
        public interface IEventInfo
        {
            
        }

        public class EventInfo : IEventInfo
        {
            public UnityAction actions;

            public EventInfo(UnityAction action)
            {
                actions += action;
            }
        }
    }
}
