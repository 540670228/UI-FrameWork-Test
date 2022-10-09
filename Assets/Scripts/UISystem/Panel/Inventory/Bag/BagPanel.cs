using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryType
{
    Bag = 1,
    Shop = 2,
    Player = 3,
}
public enum BagType
{
    Equip = 1,
    Item = 2,
    Gem = 3,
}
namespace Common.UI.Inventory
{
    public class BagPanel : BasePanel
    {
        private BagType currentBagType = BagType.Equip;
        public override void Show()
        {
            base.Show();
            RefreshBagView();
        }
        private void Start()
        {
            GetControl<Button>("btnClose").onClick.AddListener(()=> { Hide(); });
            GetControl<Toggle>("togEquip").onValueChanged.AddListener(ToggleValueChanged);
            GetControl<Toggle>("togItem").onValueChanged.AddListener(ToggleValueChanged);
            GetControl<Toggle>("togGems").onValueChanged.AddListener(ToggleValueChanged);

            EventCenter.Instance.AddEventListener(EventType.ItemChanged, RefreshBagView);
        }

        private void ToggleValueChanged(bool state)
        {
            if (GetControl<Toggle>("togEquip").isOn)
                currentBagType = BagType.Equip;
            else if (GetControl<Toggle>("togItem").isOn)
                currentBagType = BagType.Item;
            else if (GetControl<Toggle>("togGems").isOn)
                currentBagType = BagType.Gem;

            RefreshBagView();
        }

        public void RefreshBagView()
        {
            InitInfo(GameDataManager.Instance.GetPlayerInfo());
        }

        private void InitInfo(PlayerInfoMessage playerInfo)
        {
            List<ItemInfo> itemInfoList = null;
            switch (currentBagType)
            {
                case BagType.Equip:
                    itemInfoList = playerInfo.equips;
                    break;
                case BagType.Item:
                    itemInfoList = playerInfo.items;
                    break;
                case BagType.Gem:
                    itemInfoList = playerInfo.gems;
                    break;
            }
            //更新View
            Transform content = GetControl<ScrollRect>("svBag").content;
            //对象池回收所有格子，生成新格子
            GameObjectPool.Instance.Clear(InventoryType.Bag.ToString());
            foreach (ItemInfo itemInfo in itemInfoList)
            {
                GameObject cellObj = ResourcesManager.Instance.Load<GameObject>("ItemCell");
                cellObj = GameObjectPool.Instance.CreateObject(InventoryType.Bag.ToString(), cellObj, Vector3.zero, Quaternion.identity);
                cellObj.transform.SetParent(content, false);
                cellObj.GetComponent<ItemCell>().InitCellInfo(itemInfo);
            }
            Canvas.ForceUpdateCanvases();
        }

    }
}