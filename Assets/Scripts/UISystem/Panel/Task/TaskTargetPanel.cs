using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Task
{
    public class TaskTargetPanel : BasePanel
    {
        public void InitInfo(ItemInfo targetInfo,ItemInfo curInfo)
        {
            string itemName = GameDataManager.Instance.GetItemsConfig().GetItem(targetInfo.id).name;
            GetControl<Text>("txtItemName").text = itemName;
            GetControl<Text>("txtItemCount").text = $"{curInfo.num}/{targetInfo.num}";
        }
    }
}