using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoneyType
{
    Gold = 1,
    Diamond = 2,
}
namespace Common.UI.Inventory
{
    public class ShopCell : BasePanel
    {
        private ShopCellInfo shopCellInfo = null;
        public void InitInfo(ShopCellInfo info)
        {
            shopCellInfo = info;
            Item item = GameDataManager.Instance.GetItemsConfig().GetItem(info.itemInfo.id);

            GetControl<Image>("imgIcon").sprite = ResourcesManager.Instance.Load<Sprite>(item.icon);
            GetControl<Text>("txtItemCount").text = info.itemInfo.num.ToString();
            GetControl<Text>("txtItemName").text = item.name;
            GetControl<Text>("txtTips").text = shopCellInfo.tips;
            GetControl<Text>("txtPrice").text = info.price.ToString();
            GetControl<Image>("imgMoney").sprite = ResourcesManager.Instance.Load<Sprite>(info.priceIcon);
        }

        private void Start()
        {
            GetControl<Button>("btnBuy").onClick.AddListener(BuyItem);
        }

        private void BuyItem()
        {
            UIManager.Instance.ShowPanel<ShopBuyPanel>("ShopBuyPanel", 3, (panel) =>
              {
                  panel.InitInfo(shopCellInfo);
              });
        }
    }
}