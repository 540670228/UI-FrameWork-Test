using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class BagTipsPanel : BasePanel
    {
        private ItemInfo itemInfo = null;
        public void InitInfo(ItemInfo info)
        {
            itemInfo = info;
            Item item = GameDataManager.Instance.GetItemsConfig().GetItem(info.id);
            //����ͼ��
            GetControl<Image>("imgIcon").sprite = ResourcesManager.Instance.Load<Sprite>(item.icon);
            //��������
            GetControl<Text>("txtName").text = item.name;
            //��������
            GetControl<Text>("txtCount").text = $"������{info.num}";
            //����Tips
            GetControl<Text>("txtTips").text = item.tips;
        }
    }
}