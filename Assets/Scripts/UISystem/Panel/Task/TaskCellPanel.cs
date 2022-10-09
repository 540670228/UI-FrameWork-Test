using Game;
using System;
using UnityEngine.UI;

namespace Common.UI.Task
{
    public class TaskCellPanel : BasePanel
    {
        TaskCell cell = null;
        public void InitInfo(TaskCell taskCell,ToggleGroup group)
        {
            cell = taskCell;
            string suffix = "";
            switch(taskCell.taskState)
            {
                case TaskState.WaitToSubmit:
                    suffix = "(待提交)";
                    break;
                case TaskState.Finish:
                    suffix = "(完成)";
                    break;
                default:
                    suffix = "";
                    break;
                    
            }
            GetControl<Text>("txtTaskName").text = taskCell.taskName + suffix;
            GetComponent<Toggle>().group = group;
            GetComponent<Toggle>().onValueChanged.AddListener(InitTaskInfo);
        }

        private void InitTaskInfo(bool isOn)
        {
            if (!isOn) return;
            EventCenter.Instance.EventTrigger<TaskCell>(EventType.ClickTaskToggle, cell);
        }
    }
}