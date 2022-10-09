using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Inventory
{
    public class TipsPanel : BasePanel
    {
        private void Start()
        {
            GetControl<Button>("btnClose").onClick.AddListener(() =>
            {
                Hide();
            });
        }
        public void InitInfo(string tips)
        {
            GetControl<Text>("txtTips").text = tips;
        }
    }
}