using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class RolePanel : BasePanel
    {
        public ItemCell itemHead;
        public ItemCell itemNeck;
        public ItemCell itemWeapon;
        public ItemCell itemCloth;
        public ItemCell itemTrousers;
        public ItemCell itemShoes;
        protected override void Init()
        {
            base.Init();
            itemHead = transform.FindChildByName("ItemHead").GetComponent<ItemCell>();
            itemNeck = transform.FindChildByName("ItemNeck").GetComponent<ItemCell>();
            itemWeapon = transform.FindChildByName("ItemWeapon").GetComponent<ItemCell>();
            itemCloth = transform.FindChildByName("ItemCloth").GetComponent<ItemCell>();
            itemTrousers = transform.FindChildByName("ItemTrousers").GetComponent<ItemCell>();
            itemShoes = transform.FindChildByName("ItemShoes").GetComponent<ItemCell>();
            GetControl<Button>("btnClose").onClick.AddListener(() =>
            {
                Hide();
            });

            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.AddEquip, (itemInfo) =>
            {
                UpdateRoleInfo();
            });
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.RemoveEquip, (itemInfo) =>
            {
                UpdateRoleInfo();
            });
        }

        public override void Show()
        {
            base.Show();
            UpdateRoleInfo();
        }

        private void UpdateRoleInfo()
        {
            PlayerInfoMessage msg = GameDataManager.Instance.GetPlayerInfo();
            //更新前应置空
            itemHead.InitCellInfo(null);
            itemNeck.InitCellInfo(null);
            itemWeapon.InitCellInfo(null);
            itemCloth.InitCellInfo(null);
            itemTrousers.InitCellInfo(null);
            itemShoes.InitCellInfo(null);
            foreach (Equip equip in msg.nowEquips)
            {
                ItemInfo itemInfo = new ItemInfo()
                {
                    id = equip.id,
                    num = 1,
                };
                switch(equip.cellType)
                {
                    case ItemCellType.Head:
                        itemHead.InitCellInfo(itemInfo);
                        break;
                    case ItemCellType.Neck:
                        itemNeck.InitCellInfo(itemInfo);
                        break;
                    case ItemCellType.Weapon:
                        itemWeapon.InitCellInfo(itemInfo);
                        break;
                    case ItemCellType.Cloth:
                        itemCloth.InitCellInfo(itemInfo);
                        break;
                    case ItemCellType.Trousers:
                        itemTrousers.InitCellInfo(itemInfo);
                        break;
                    case ItemCellType.Shoes:
                        itemShoes.InitCellInfo(itemInfo);
                        break;
                }
            }
        }


    }
}