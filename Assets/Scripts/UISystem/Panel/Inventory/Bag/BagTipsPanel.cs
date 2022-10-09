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
            //更新图标
            GetControl<Image>("imgIcon").sprite = ResourcesManager.Instance.Load<Sprite>(item.icon);
            //更新名称
            GetControl<Text>("txtName").text = item.name;
            //更新数量
            GetControl<Text>("txtCount").text = $"数量：{info.num}";
            //更新Tips
            GetControl<Text>("txtTips").text = item.tips;
        }
    }
}