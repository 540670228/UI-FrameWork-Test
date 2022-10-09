using Common;
using Common.UI;
using Common.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            UIManager.Instance.ShowPanel<MainPanel>("MainPanel");
            BagManager.Instance.Init();
        }
    }
}