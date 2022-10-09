using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum ItemCellType
{
    Bag = 0,
    Head = 1,
    Neck = 2,
    Weapon = 3,
    Cloth = 4,
    Trousers = 5,
    Shoes = 6,
    Task = 7,
}
namespace Common.UI.Inventory
{
    public class ItemCell : BasePanel,IPointerEnterHandler,IPointerExitHandler
    {
        public ItemCellType cellType = ItemCellType.Bag;
        private bool isDrag = false;

        public ItemInfo ItemInfo { get; set; }

        private void OnEnable()
        {
            EventTrigger trigger = GetControl<Image>("imgIcon").gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry beginDown = new EventTrigger.Entry();
            beginDown.eventID = EventTriggerType.PointerDown;
            beginDown.callback.AddListener(OnPointerDown);
            EventTrigger.Entry beginUp = new EventTrigger.Entry();
            beginUp.eventID = EventTriggerType.PointerUp;
            beginUp.callback.AddListener(OnPointerUp);
            trigger.triggers.Add(beginDown);
            trigger.triggers.Add(beginUp);
            if(cellType != ItemCellType.Task)
                GetControl<Image>("imgIcon").gameObject.SetActive(false);
            ItemInfo = null;
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            Debug.Log("UP");
            if (ItemInfo == null || GameDataManager.Instance.GetEquipConfig().GetEquipInfo(ItemInfo.id) == null) return;
            isDrag = false;
            EventCenter.Instance.EventTrigger<ItemCell>(EventType.ItemCellEndDrag, this);
        }

        private void OnPointerDown(BaseEventData arg0)
        {
            Debug.Log("Down");
            if (ItemInfo == null || GameDataManager.Instance.GetEquipConfig().GetEquipInfo(ItemInfo.id) == null) return;
            isDrag = true;
            EventCenter.Instance.EventTrigger<ItemCell>(EventType.ItemCellBeginDrag, this);
        }

        public void InitCellInfo(ItemInfo info)
        {
            ItemInfo = info;
            if(info == null)
            {
                GetControl<Image>("imgIcon").gameObject.SetActive(false);
                return;
            }
            GetControl<Image>("imgIcon").gameObject.SetActive(true);
            Item item = GameDataManager.Instance.GetItemsConfig().GetItem(info.id);
            //更新图标
            GetControl<Image>("imgIcon").sprite = ResourcesManager.Instance.Load<Sprite>(item.icon);
            if(cellType == ItemCellType.Bag)
                GetControl<Text>("txtCount").text = info.num.ToString();
        }

        /// <summary>
        /// 指针移入回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventCenter.Instance.EventTrigger<ItemCell>(EventType.ItemCellEnter, this);
            if (ItemInfo == null || isDrag == true) return;
            //显示提示面板
            UIManager.Instance.ShowPanel<BagTipsPanel>("BagTipsPanel",3,(bagTipsPanel)=> {
                //更新信息
                bagTipsPanel.InitInfo(ItemInfo);
                //更新位置
                bagTipsPanel.transform.position = GetControl<Image>("imgBk").transform.position;
            });
        }

        /// <summary>
        /// 指针移出回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            EventCenter.Instance.EventTrigger<ItemCell>(EventType.ItemCellExit, this);
            if (ItemInfo == null || isDrag == true) return;
            UIManager.Instance.HidePanel("BagTipsPanel");
        }

        private void Update()
        {
            if(isDrag)
            {
                EventCenter.Instance.EventTrigger<ItemCell>(EventType.ItemCellDrag, this);
            }
        }
    }
}