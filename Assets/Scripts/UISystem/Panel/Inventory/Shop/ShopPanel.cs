using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class ShopPanel : BasePanel
    {
        private void Start()
        {
            GetControl<Button>("btnClose").onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();
            GameObjectPool.Instance.Clear(InventoryType.Shop.ToString());
            //初始化商店信息
            foreach(ShopCellInfo info in GameDataManager.Instance.GetShopList())
            {
                GameObject shopCellObj = ResourcesManager.Instance.Load<GameObject>("ShopCell");
                shopCellObj = GameObjectPool.Instance.CreateObject(InventoryType.Shop.ToString(), shopCellObj, Vector3.zero, Quaternion.identity);
                shopCellObj.transform.SetParent(GetControl<ScrollRect>("svShop").content, false);
                ShopCell shopCell = shopCellObj.GetComponent<ShopCell>();
                shopCell.InitInfo(info);
            }
            Canvas.ForceUpdateCanvases();
        }
    }
}