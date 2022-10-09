using Common.UI.Inventory;
using Common.UI.Task;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class MainPanel : BasePanel
    {
        private void Start()
        {
            GetControl<Button>("btnRole").onClick.AddListener(ShowBagRolePanel);
            GetControl<Button>("btnShop").onClick.AddListener(ShowShopPanel);
            GetControl<Button>("btnTask").onClick.AddListener(ShowTaskPanel);
            GetControl<Button>("btnGoldInvest").onClick.AddListener(AddGold);
            GetControl<Button>("btnDiamondInvest").onClick.AddListener(AddDiamond);


            GetControl<Button>("btnTest01").onClick.AddListener(() =>
            {
                EventCenter.Instance.EventTrigger<int>(EventType.AddTask, 1);
            });

            GetControl<Button>("btnTest02").onClick.AddListener(() =>
            {
                EventCenter.Instance.EventTrigger<int>(EventType.AddTask, 2);
            });

            //×¢²á¼àÌý
            EventCenter.Instance.AddEventListener<int>(EventType.DiamondChange, SetDiamondCount);

            EventCenter.Instance.AddEventListener<int>(EventType.GoldChange, SetGoldCount);
        }

        private void ShowTaskPanel()
        {
            UIManager.Instance.ShowPanel<TaskPanel>("TaskPanel",2);
        }

        private void AddGold()
        {
            EventCenter.Instance.EventTrigger<int>(EventType.ChangeGold, 1000);
        }

        private void AddDiamond()
        {
            EventCenter.Instance.EventTrigger<int>(EventType.ChangeDiamond, 1000);
        }

        private void ShowShopPanel()
        {
            UIManager.Instance.ShowPanel<ShopPanel>("ShopPanel", 2);
        }

        private void ShowBagRolePanel()
        {
            UIManager.Instance.ShowPanel<BagPanel>("BagPanel",2);
            UIManager.Instance.ShowPanel<RolePanel>("RolePanel", 2);
        }

        public override void Show()
        {
            base.Show();
            InitInfo(GameDataManager.Instance.GetPlayerInfo());
        }

        private void InitInfo(PlayerInfoMessage info)
        {
            SetRoleLevel(info.level);
            SetRoleName(info.playerName);
            SetGoldCount(info.goldCount);
            SetDiamondCount(info.diamondCount);
            SetActionCount(info.actionCount);
        }

        private void SetRoleLevel(int level)
        {
            GetControl<Text>("txtRoleLevel").text = level.ToString();
        }
        private void SetRoleName(string playerName)
        {
            GetControl<Text>("txtRoleName").text = playerName;
        }
        private void SetGoldCount(int goldCount)
        {
            GetControl<Text>("txtGoldCount").text = goldCount.ToString();
        }
        private void SetDiamondCount(int diamondCount)
        {
            GetControl<Text>("txtDiamondCount").text = diamondCount.ToString();
        }
        private void SetActionCount(int actionCount)
        {
            GetControl<Text>("txtActionCount").text = actionCount.ToString();
        }
        
    }
}