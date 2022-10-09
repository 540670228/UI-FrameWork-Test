using Common.UI.Inventory;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI.Task
{
    public class TaskPanel : BasePanel
    {
        ToggleGroup toggleGroup = null;
        
        protected override void Init()
        {
            base.Init();
            GetControl<Button>("btnClose").onClick.AddListener(() =>
            {
                Hide();
            });

            toggleGroup = GetControl<ToggleGroup>("groupTask");
            //初始化任务列表
            RefreshTaskPanel();
            EventCenter.Instance.AddEventListener<TaskCell>(EventType.TaskChanged, (obj) => {
                RefreshTaskPanel();
                });
            EventCenter.Instance.AddEventListener<TaskCell>(EventType.ClickTaskToggle, CilckTaskToggleHandler);
        }

        public override void Show()
        {
            base.Show();
            RefreshLayout();
            Canvas.ForceUpdateCanvases();
        }

        private void CilckTaskToggleHandler(TaskCell cell)
        {
            GetControl<Text>("txtTaskTips").text = cell.taskTips;
            GameObjectPool.Instance.Clear("TaskTarget");
            GameObjectPool.Instance.Clear("TaskItem");
            //更新目标
            for(int i = 0; i < cell.targetList.Count; i++)
            {
                GameObject targetObj = ResourcesManager.Instance.Load<GameObject>("TaskTargetPanel");
                targetObj = GameObjectPool.Instance.CreateObject("TaskTarget", targetObj, Vector3.zero, Quaternion.identity);
                targetObj.transform.SetParent(GetControl<VerticalLayoutGroup>("layoutTargets").transform,false);
                targetObj.transform.GetComponent<TaskTargetPanel>().InitInfo(cell.targetList[i], cell.currentList[i]);
            }
            //更新战利品
            for (int i = 0; i < cell.rewardList.Count;i++)
            {
                GameObject itemObj = ResourcesManager.Instance.Load<GameObject>("ItemCell");
                itemObj = GameObjectPool.Instance.CreateObject("TaskItem", itemObj, Vector3.zero, Quaternion.identity);
                itemObj.transform.SetParent(GetControl<HorizontalLayoutGroup>("layoutItems").transform,false);
                itemObj.GetComponent<ItemCell>().InitCellInfo(cell.rewardList[i]);
                itemObj.GetComponent<ItemCell>().cellType = ItemCellType.Task;
            }
            RefreshLayout();
            Canvas.ForceUpdateCanvases();

        }

        private void RefreshLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetControl<VerticalLayoutGroup>("layoutTargets").transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetControl<HorizontalLayoutGroup>("layoutItems").transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetControl<VerticalLayoutGroup>("groupTask").transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetControl<VerticalLayoutGroup>("layoutRight").transform as RectTransform);
        }

        private void RefreshTaskPanel()
        {
            PlayerInfoMessage msg = GameDataManager.Instance.GetPlayerInfo();

            for(int i = 0; i < msg.taskList.Count; i++)
            {
                GameObject taskObj = null;
                if (i < toggleGroup.transform.childCount)
                {
                    taskObj = toggleGroup.transform.GetChild(i).gameObject;
                }
                else
                {
                    taskObj = ResourcesManager.Instance.Load<GameObject>("TaskCell");
                    taskObj = Instantiate<GameObject>(taskObj, Vector3.zero, Quaternion.identity);
                    taskObj.transform.SetParent(toggleGroup.transform, false);
                }
                
                taskObj.GetComponent<TaskCellPanel>().InitInfo(msg.taskList[i], toggleGroup);
            }
            Canvas.ForceUpdateCanvases();
            RefreshLayout();
        }
    }
}