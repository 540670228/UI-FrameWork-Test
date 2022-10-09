using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class BagManager : Singleton<BagManager>
    {
        //记录开始拖动选中的格子
        private ItemCell startItemCell;
        //记录当前进入的格子
        private ItemCell enterItemCell;

        //当前拖拽的图片TF
        private Transform curDragTF;
        public new void Init()
        {
            EventCenter.Instance.AddEventListener<ItemCell>(EventType.ItemCellEnter, ItemCellEnterHandler);
            EventCenter.Instance.AddEventListener<ItemCell>(EventType.ItemCellExit, ItemCellExitHandler);
            EventCenter.Instance.AddEventListener<ItemCell>(EventType.ItemCellBeginDrag, ItemCellBeginDragHandler);
            EventCenter.Instance.AddEventListener<ItemCell>(EventType.ItemCellDrag, ItemCellDragHandler);
            EventCenter.Instance.AddEventListener<ItemCell>(EventType.ItemCellEndDrag, ItemCellEndDragHandler);
        }

        private void ItemCellEndDragHandler(ItemCell itemCell)
        {
            GameObjectPool.Instance.CollectObject(curDragTF.gameObject);
            ChangeEquip();
            startItemCell = null;
        }

        private void ItemCellDragHandler(ItemCell itemCell)
        {
            RectTransform rect = curDragTF as RectTransform;
            rect.position = Input.mousePosition;
        }

        private void ItemCellBeginDragHandler(ItemCell itemCell)
        {
            //创建临时图片
            GameObject tmpIcon = ResourcesManager.Instance.Load<GameObject>("imgDragIcon");
            tmpIcon = GameObjectPool.Instance.CreateObject(tmpIcon.name, tmpIcon,Vector3.zero,Quaternion.identity);
            tmpIcon.GetComponent<Image>().sprite = GameDataManager.Instance.GetItemsConfig().GetItemIcon(itemCell.ItemInfo.id);
            curDragTF = tmpIcon.transform;
            startItemCell = itemCell;
            UIManager.Instance.SetLayer(curDragTF, 3);

            UIManager.Instance.HidePanel("BagTipsPanel");
        }

        private void ItemCellExitHandler(ItemCell itemCell)
        {
            Debug.Log("Exit");
            enterItemCell = null;
        }

        private void ItemCellEnterHandler(ItemCell itemCell)
        {
            Debug.Log("Enter");
            enterItemCell = itemCell;
        }

        public void ChangeEquip()
        {
            if (startItemCell == null) return;
            //从背包拖进对应的装备栏
            if (enterItemCell != null && enterItemCell.cellType != ItemCellType.Bag && 
                GameDataManager.Instance.GetEquipConfig().GetEquipInfo(startItemCell.ItemInfo.id).cellType == enterItemCell.cellType)
            {
                //如果装备栏为空直接放入，否则交换
                if(enterItemCell.ItemInfo == null)
                {
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddEquip, startItemCell.ItemInfo);
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.RemoveItem, startItemCell.ItemInfo);
                }
                else
                {
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddItem, enterItemCell.ItemInfo);
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.RemoveEquip, enterItemCell.ItemInfo);
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddEquip, startItemCell.ItemInfo);
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.RemoveItem, startItemCell.ItemInfo);
                }
            }
        
            //从装备栏拖出, 且拖出了当前格子,则退还给背包
            if(startItemCell.cellType != ItemCellType.Bag)
            {
                if(enterItemCell == null)
                {
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddItem, startItemCell.ItemInfo);
                    EventCenter.Instance.EventTrigger<ItemInfo>(EventType.RemoveEquip, startItemCell.ItemInfo);
                }
                
            }
        }
    }
}