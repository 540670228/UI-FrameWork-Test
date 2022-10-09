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
        //��¼��ʼ�϶�ѡ�еĸ���
        private ItemCell startItemCell;
        //��¼��ǰ����ĸ���
        private ItemCell enterItemCell;

        //��ǰ��ק��ͼƬTF
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
            //������ʱͼƬ
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
            //�ӱ����Ͻ���Ӧ��װ����
            if (enterItemCell != null && enterItemCell.cellType != ItemCellType.Bag && 
                GameDataManager.Instance.GetEquipConfig().GetEquipInfo(startItemCell.ItemInfo.id).cellType == enterItemCell.cellType)
            {
                //���װ����Ϊ��ֱ�ӷ��룬���򽻻�
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
        
            //��װ�����ϳ�, ���ϳ��˵�ǰ����,���˻�������
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