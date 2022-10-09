using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class ShopBuyPanel : BasePanel
    {
        ShopCellInfo shopCellInfo = null;
        int buyCount = 0;
        private void Start()
        {
            GetControl<Slider>("sliderCount").onValueChanged.AddListener(OnSliderValueChanged);
            GetControl<Button>("btnTotalBuy").onClick.AddListener(BuyItem);
            GetControl<Button>("btnClose").onClick.AddListener(() =>
            {
                Hide();
            });
        }

        private void OnSliderValueChanged(float arg0)
        {
            buyCount = (int)(arg0);
            GetControl<Text>("txtTotalPrice").text = (buyCount * shopCellInfo.price).ToString();
            GetControl<Text>("txtCount").text = buyCount.ToString();
        }

        public void InitInfo(ShopCellInfo info)
        {
            shopCellInfo = info;
            PlayerInfoMessage playerInfo = GameDataManager.Instance.GetPlayerInfo();
            GetComponentInChildren<ShopCell>().InitInfo(info);
            int maxValue = 0;
            switch((MoneyType)info.priceType)
            {
                case MoneyType.Gold:
                    maxValue = playerInfo.goldCount / info.price;
                    break;
                case MoneyType.Diamond:
                    maxValue = playerInfo.diamondCount / info.price;
                    break;
            }
            Slider countSlider = GetControl<Slider>("sliderCount");
            countSlider.minValue = 0;
            countSlider.maxValue = maxValue;
            countSlider.wholeNumbers = true;
            
            countSlider.value = 0;
            buyCount = 0;
            GetControl<Image>("imgTotalMoney").sprite = ResourcesManager.Instance.Load<Sprite>(info.priceIcon);
            GetControl<Text>("txtTotalPrice").text = 0.ToString();
        }

        /// <summary>
        /// 购买物体逻辑
        /// </summary>
        private void BuyItem()
        {
            if(buyCount == 0)
            {
                TipsManager.Instance.ShowOneBtnTips("数量不可以为零哦");
                return;
            }

            ItemInfo buyInfo = shopCellInfo.itemInfo.Clone();
            buyInfo.num *= buyCount;
            MoneyType mType = (MoneyType)shopCellInfo.priceType;
            if (mType == MoneyType.Gold)
            {
                //添加物品
                EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddItem, buyInfo);

                //扣除货币
                EventCenter.Instance.EventTrigger<int>(EventType.ChangeGold , -shopCellInfo.price * buyCount);
            }
            else if (mType == MoneyType.Diamond)
            {
                //添加物品
                EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddItem, buyInfo);

                //扣除货币
                EventCenter.Instance.EventTrigger<int>(EventType.ChangeDiamond , -shopCellInfo.price * buyCount);
            }

            TipsManager.Instance.ShowOneBtnTips("购买成功");

            Hide();
        }
    }
}