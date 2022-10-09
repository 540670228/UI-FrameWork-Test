using Common.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI
{
    public class TipsManager : MonoSingleton<TipsManager>
    {
        public void ShowOneBtnTips(string info)
        {
            UIManager.Instance.ShowPanel<TipsPanel>("TipsPanel", 3, (tipsPanel) =>
            {
                tipsPanel.InitInfo(info);
            });
        }
    }
}