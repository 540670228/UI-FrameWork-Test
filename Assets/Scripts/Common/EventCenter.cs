using System.Collections.Generic;
using UnityEngine.Events;

public enum EventType
{
    FinishLoadData,
    ChangeGold,
    ChangeDiamond,
    GoldChange,
    DiamondChange,
    AddItem,
    RemoveItem,
    ItemChanged,
    ItemCellBeginDrag,
    ItemCellDrag,
    ItemCellEndDrag,
    ItemCellEnter,
    ItemCellExit,
    AddEquip,
    RemoveEquip,
    AddTask,
    SubmitTask,
    TaskChanged,
    ClickTaskToggle,
}


namespace Common
{
    public interface IEventInfo
    {

    }

    public class EventInfo<T> : IEventInfo
    {
        private UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions = action;
        }

        public void AddAction(UnityAction<T> action)
        {
            actions += action;
        }

        public void RemoveAction(UnityAction<T> action)
        {
            actions -= action;
        }

        public void Invoke(T obj)
        {
            actions?.Invoke(obj);
        }
    }

    public class EventInfo : IEventInfo
    {
        private UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions = action;
        }

        public void AddAction(UnityAction action)
        {
            actions += action;
        }

        public void RemoveAction(UnityAction action)
        {
            actions -= action;
        }

        public void Invoke()
        {
            actions?.Invoke();
        }
    }

    /// <summary>
    /// 事件中心 -- 单例
    /// </summary>
    public class EventCenter : Singleton<EventCenter>
    {
        private Dictionary<EventType, IEventInfo> eventDic = new Dictionary<EventType, IEventInfo>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="action">处理函数</param>
        public void AddEventListener<T>(EventType name,UnityAction<T> action)
        {
            if(eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).AddAction(action);
            }
            else
            {
                eventDic.Add(name, new EventInfo<T>(action));
            }
        }

        public void AddEventListener(EventType name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).AddAction(action);
            }
            else
            {
                eventDic.Add(name, new EventInfo(action));
            }
        }

        /// <summary>
        /// 事件触发
        /// </summary>
        /// <param name="name">事件名</param>
        public void EventTrigger<T>(EventType name,T obj)
        {
            if(eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).Invoke(obj);
            }
        }


        public void EventTrigger(EventType name)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).Invoke();
            }
        }

        /// <summary>
        /// 注销事件监听
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="action">处理函数</param>
        public void RemoveEventListener<T>(EventType name,UnityAction<T> action)
        {
            if(eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).RemoveAction(action);
            }
        }

        public void RemoveEventListener(EventType name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).RemoveAction(action);
            }
        }

        /// <summary>
        /// 清空事件中心
        /// 主要用在场景切换时
        /// </summary>
        public void Clear()
        {
            eventDic.Clear();
        }
    }
}