using Common;
using Common.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameDataManager : MonoSingleton<GameDataManager>
    {
        private ItemsConfig itemsConfig = null;
        private EquipConfig equipConfig = null;
        private static string PLAYER_INFO_URL = null;
        private PlayerInfo playerInfo = null;
        private ShopList shopList = null;
        private TaskConfig taskConfig = null;

        protected override void Init()
        {
            base.Init();
            PLAYER_INFO_URL = Application.persistentDataPath + "/PlayerInfo.txt";

            //====初始化道具信息=====
            string jsonFile = ConfigReader.GetConfigFile("ItemInfo.json");
            Items items = JsonUtility.FromJson<Items>(jsonFile);
            itemsConfig = new ItemsConfig(items.info);

            //====初始化装备信息=====
            string equipJsonFile = ConfigReader.GetConfigFile("EquipInfo.json");
            Equips equips = JsonUtility.FromJson<Equips>(equipJsonFile);
            equipConfig = new EquipConfig(equips.info);

            //====初始化角色信息=====
            if(File.Exists(PLAYER_INFO_URL))
            {
                byte[] bytes = File.ReadAllBytes(PLAYER_INFO_URL);
                string playerJson = Encoding.UTF8.GetString(bytes);
                playerInfo = JsonUtility.FromJson<PlayerInfo>(playerJson);
                Debug.Log(playerInfo.playerName);
                
                //注册任务事件
                foreach(TaskCell cell in playerInfo.taskList)
                {
                    cell.AddTaskLisenter();
                }
            }
            else
            {
                //初始化默认Player 并存储为json
                playerInfo = new PlayerInfo("Mr Liu");
                SavePlayerInfo();
            }

            //====初始化商店信息=====
            string shopJsonFile = ConfigReader.GetConfigFile("ShopInfo.json");
            shopList = JsonUtility.FromJson<ShopList>(shopJsonFile);

            //====初始化任务信息=====
            string taskJsonFile = ConfigReader.GetConfigFile("TaskInfo.json");
            taskConfig = JsonUtility.FromJson<TaskConfig>(taskJsonFile);            
            

            //===注册角色信息变化保存回调====
            EventCenter.Instance.AddEventListener<int>(EventType.GoldChange, (count) =>
            {
                SavePlayerInfo();
            });
            EventCenter.Instance.AddEventListener<int>(EventType.DiamondChange, (count) =>
            {
                SavePlayerInfo();
            });
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.AddItem, (itemInfo) =>
            {
                SavePlayerInfo();
            });
            EventCenter.Instance.AddEventListener<TaskCell>(EventType.TaskChanged, (obj) =>
            {
                SavePlayerInfo();
            });

            //触发一次物体变化 为了刷新任务列表
            EventCenter.Instance.EventTrigger(EventType.ItemChanged);

        }

        /// <summary>
        /// 保存角色信息
        /// </summary>
        public void SavePlayerInfo()
        {
            string playerJson = JsonUtility.ToJson(playerInfo);
            File.WriteAllBytes(PLAYER_INFO_URL, Encoding.UTF8.GetBytes(playerJson));
            Debug.Log($"PlayerInfo write to {PLAYER_INFO_URL}");
        }

        public PlayerInfoMessage GetPlayerInfo()
        {
            PlayerInfoMessage msg = new PlayerInfoMessage();
            msg.FromPlayer(playerInfo);
            return msg;
        }
        public ItemsConfig GetItemsConfig()
        {
            return itemsConfig;
        }
        public EquipConfig GetEquipConfig()
        {
            return equipConfig;
        }
        public List<ShopCellInfo> GetShopList()
        {
            return shopList.info;
        }

        public TaskConfig GetTaskConfig()
        {
            return taskConfig;
        }
    }


    #region Player Config 玩家配置表

    [System.Serializable]
    public class PlayerInfo
    {
        public string playerName;
        public int level;
        public int goldCount;
        public int diamondCount;
        public int actionCount;
        public List<ItemInfo> equips;
        public List<ItemInfo> items;
        public List<ItemInfo> gems;

        public List<Equip> nowEquips;

        public List<TaskCell> taskList;
        
        public PlayerInfo(string playerName)
        {
            this.playerName = playerName;
            level = 0;
            goldCount = 0;
            diamondCount = 0;
            actionCount = 200;
            equips = new List<ItemInfo>();
            items = new List<ItemInfo>();
            gems = new List<ItemInfo>();
            nowEquips = new List<Equip>();
            taskList = new List<TaskCell>();
            AddPlayerListener();
        }
        public PlayerInfo()
        {
            AddPlayerListener();
        }

        private void AddPlayerListener()
        {
            //为角色注册回调函数
            EventCenter.Instance.AddEventListener<int>(EventType.ChangeGold, ChangeGold);
            EventCenter.Instance.AddEventListener<int>(EventType.ChangeDiamond, ChangeDiamond);
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.AddItem, AddItem);
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.RemoveItem, RemoveItem);
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.AddEquip, AddEquip);
            EventCenter.Instance.AddEventListener<ItemInfo>(EventType.RemoveEquip, RemoveEquip);
            EventCenter.Instance.AddEventListener<int>(EventType.AddTask, AddTask);
            EventCenter.Instance.AddEventListener<int>(EventType.SubmitTask, SubmitTask);
        }

        private void SubmitTask(int taskId)
        {
            TaskCell taskCell = taskList.Find((item) => item.id == taskId);
            taskCell.SubmitTask();
            
        }

        private void AddTask(int taskId)
        {
            if (taskList.Find(obj => obj.id == taskId) != null) return;
            TaskCell taskCell = GameDataManager.Instance.GetTaskConfig().GetTask(taskId);
            if (taskCell == null) return;
            //加入任务列表
            taskList.Add(taskCell);
            taskCell.AddTaskLisenter();
            //触发事件
            EventCenter.Instance.EventTrigger<TaskCell>(EventType.TaskChanged,taskCell);
        }

        public void RemoveItem(ItemInfo info)
        {
            equips.Remove(info);
            items.Remove(info);
            gems.Remove(info);
            EventCenter.Instance.EventTrigger(EventType.ItemChanged);
        }

        public void AddItem(ItemInfo info)
        {
            info = info.Clone();
            BagType bagType = (BagType)GameDataManager.Instance.GetItemsConfig().GetItem(info.id).type;
            switch(bagType)
            {
                case BagType.Equip:
                    AddItemCore(info, equips);
                    break;
                case BagType.Item:
                    AddItemCore(info, items);
                    break;
                case BagType.Gem:
                    AddItemCore(info, gems);
                    break;
            }
            EventCenter.Instance.EventTrigger(EventType.ItemChanged);
        }
        private void AddItemCore(ItemInfo info,List<ItemInfo> infoList)
        {
            int stackCount = GameDataManager.Instance.GetItemsConfig().GetItem(info.id).stackCount;
            //判断堆叠
            ItemInfo target = infoList.Find((item) => { return item.id == info.id && item.num < stackCount; });
            int allCount = target == null ? info.num : target.num + info.num;
            infoList.Remove(target);
            while(allCount > 0)
            {
                int num = allCount - stackCount >= 0 ? stackCount : allCount;
                infoList.Add(new ItemInfo(info.id, num));
                allCount -= num;
            }
            
        }

        public void ChangeGold(int delta)
        {
            if (goldCount + delta < 0) return;
            goldCount += delta;
            EventCenter.Instance.EventTrigger<int>(EventType.GoldChange, goldCount);
        }

        public void ChangeDiamond(int delta)
        {
            if (diamondCount + delta < 0) return;
            diamondCount += delta;
            EventCenter.Instance.EventTrigger<int>(EventType.DiamondChange, diamondCount);
        }

        public void AddEquip(ItemInfo itemInfo)
        {
            Equip equip = GameDataManager.Instance.GetEquipConfig().GetEquipInfo(itemInfo.id);
            nowEquips.Add(equip);
        }

        private void RemoveEquip(ItemInfo itemInfo)
        {
            Equip equip = GameDataManager.Instance.GetEquipConfig().GetEquipInfo(itemInfo.id);
            nowEquips.Remove(equip);
        }

    }
    public class PlayerInfoMessage
    {
        public string playerName;
        public int level;
        public int goldCount;
        public int diamondCount;
        public int actionCount;
        public List<ItemInfo> equips;
        public List<ItemInfo> items;
        public List<ItemInfo> gems;

        public List<Equip> nowEquips;

        public List<TaskCell> taskList;

        public void FromPlayer(PlayerInfo info)
        {
            playerName = info.playerName;
            level = info.level;
            goldCount = info.goldCount;
            diamondCount = info.diamondCount;
            actionCount = info.actionCount;
            equips = info.equips;
            items = info.items;
            gems = info.gems;
            nowEquips = info.nowEquips;
            taskList = info.taskList;
        }

        public int GetItemTotalCount(int itemId)
        {
            int count = 0;
            List<ItemInfo> searchList = null;
            searchList = equips.FindAll((itemInfo) =>
            {
                return itemInfo.id == itemId;
            });
            if(searchList != null && searchList.Count > 0)
            {
                foreach (ItemInfo itemInfo in searchList)
                    count += itemInfo.num;
            }

            searchList = items.FindAll((itemInfo) =>
            {
                return itemInfo.id == itemId;
            });
            if (searchList != null && searchList.Count > 0)
            {
                foreach (ItemInfo itemInfo in searchList)
                    count += itemInfo.num;
            }
            searchList = gems.FindAll((itemInfo) =>
            {
                return itemInfo.id == itemId;
            });
            if (searchList != null && searchList.Count > 0)
            {
                foreach (ItemInfo itemInfo in searchList)
                    count += itemInfo.num;
            }

            return count;
        }

    }

    [System.Serializable]
    public class ItemInfo
    {
        public int id;
        public int num;

        public ItemInfo(int id,int num)
        {
            this.id = id;
            this.num = num;
        }

        public ItemInfo()
        {

        }

        public ItemInfo Clone()
        {
            ItemInfo clone = new ItemInfo();
            clone.id = id;
            clone.num = num;
            return clone;
        }
    }
    #endregion

    #region Items Config 道具配置表
    public class ItemsConfig
    {
        private Dictionary<int, Item> itemDic;

        public ItemsConfig(List<Item> items)
        {
            itemDic = new Dictionary<int, Item>();
            foreach(Item item in items)
            {
                itemDic.Add(item.id, item);
            }
        }

        public Item GetItem(int id)
        {
            if (itemDic.ContainsKey(id))
                return itemDic[id];
            return null;
        }

        public Sprite GetItemIcon(int id)
        {
            if (!itemDic.ContainsKey(id)) return null;
            Item item = itemDic[id];
            return ResourcesManager.Instance.Load<Sprite>(item.icon);
        }
    }

    [System.Serializable]
    public class Items
    {
        public List<Item> info;
    }

    [System.Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string icon;
        public int type;
        public int stackCount;
        public string tips;
    }
    #endregion

    #region Equip Config 装备配置表
    public class EquipConfig
    {
        private Dictionary<int, Equip> equipDic;

        public EquipConfig(List<Equip> equips)
        {
            equipDic = new Dictionary<int, Equip>();
            foreach(Equip equip in equips)
            {
                equip.cellType = (ItemCellType)equip.equipType;
                equipDic.Add(equip.id, equip);
            }
        }

        public Equip GetEquipInfo(int id)
        {
            if(equipDic.ContainsKey(id))
                return equipDic[id];
            return null;
        }
    }

    public class Equips
    {
        public List<Equip> info;
    }
    [System.Serializable]
    public class Equip
    {
        public int id;
        public int equipType;
        public ItemCellType cellType;

        public override bool Equals(object obj)
        {
            return this.id == (obj as Equip).id;
        }
    }

    #endregion

    #region Shop Config 商店配置表
    [System.Serializable]
    public class ShopCellInfo
    {
        public int id;
        public ItemInfo itemInfo;
        public int priceType;
        public string priceIcon;
        public int price;
        public string tips;
    }

    [System.Serializable]
    public class ShopList
    {
        public List<ShopCellInfo> info;
    }

    #endregion

    #region Task Config 任务配置表
    public enum TaskType
    {
        Collect = 1,//收集任务
        Monster = 2,//狩猎任务
        MoveTo = 3, //寻址任务
    }
    public enum TaskState
    {
        NoFinish,//未完成
        WaitToSubmit,//待提交
        Finish,//已完成
    }

    [System.Serializable]
    public class TaskCell
    {
        public int id;
        public string taskName;
        public int taskType;
        public string taskTips;
        public TaskType TaskType
        {
            get
            {
                return (TaskType)taskType;
            }
        }

        public TaskState taskState;

        public List<ItemInfo> targetList;
        public List<ItemInfo> rewardList;
        public List<ItemInfo> currentList;


        public void AddTaskLisenter()
        {
            if (taskState == TaskState.Finish) return;
            currentList = new List<ItemInfo>();
            foreach (ItemInfo cell in targetList)
            {
                currentList.Add(new ItemInfo(cell.id, 0));
            }
            switch (TaskType)
            {
                case TaskType.Collect:
                    EventCenter.Instance.AddEventListener(EventType.ItemChanged, CollectItemHandler);
                    break;
            }
        }
        public void RemoveTaskListener()
        {
            switch (TaskType)
            {
                case TaskType.Collect:
                    EventCenter.Instance.RemoveEventListener(EventType.ItemChanged, CollectItemHandler);
                    break;
            }
        }

        private void CollectItemHandler()
        {
            //从背包进行查询并给currentList
            PlayerInfoMessage msg = GameDataManager.Instance.GetPlayerInfo();
            bool isFinish = true;
            for(int i = 0; i < currentList.Count; i++)
            {
                currentList[i].num = msg.GetItemTotalCount(currentList[i].id);
                isFinish = currentList[i].num >= targetList[i].num;
            }

            taskState = isFinish ? TaskState.WaitToSubmit : TaskState.NoFinish;

            EventCenter.Instance.EventTrigger<TaskCell>(EventType.TaskChanged, this);
        }

        public void SubmitTask()
        {
            if (taskState == TaskState.WaitToSubmit)
            {
                //切换状态，获取奖励，根据类型移除物品
                taskState = TaskState.Finish;
                switch(TaskType)
                {
                    case TaskType.Collect:
                        foreach(ItemInfo info in targetList)
                        {
                            EventCenter.Instance.EventTrigger<ItemInfo>(EventType.RemoveItem, info);
                        }
                        foreach(ItemInfo info in rewardList)
                        {
                            EventCenter.Instance.EventTrigger<ItemInfo>(EventType.AddItem, info);
                        }
                        break;
                }
                EventCenter.Instance.EventTrigger<TaskCell>(EventType.TaskChanged, this);
            }
        }
        
    }
    [System.Serializable]
    public class TaskConfig
    {
        public List<TaskCell> info;

        public TaskCell GetTask(int id)
        {
            return info.Find(obj => obj.id == id);
        }
    }
    #endregion
}