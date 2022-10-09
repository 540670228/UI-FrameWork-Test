using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.UI
{
    public class BasePanel : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> controlsDic = new Dictionary<string, List<UIBehaviour>>();

        private void Awake()
        {
            FindChildrenControls<Button>();
            FindChildrenControls<Image>();
            FindChildrenControls<Text>();
            FindChildrenControls<Toggle>();
            FindChildrenControls<Slider>();
            FindChildrenControls<ScrollRect>();
            FindChildrenControls<Scrollbar>();
            FindChildrenControls<VerticalLayoutGroup>();
            FindChildrenControls<HorizontalLayoutGroup>();
            FindChildrenControls<GridLayoutGroup>();
            FindChildrenControls<ToggleGroup>();
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        protected T GetControl<T>(string objName) where T : UIBehaviour
        {
            if (!controlsDic.ContainsKey(objName))
                return default(T);

            foreach(UIBehaviour control in controlsDic[objName])
            {
                if (control is T)
                    return control as T;
            }
            return default(T);
        }

        private void FindChildrenControls<T>() where T : UIBehaviour
        {
            T[] controls = transform.GetComponentsInChildren<T>();
            foreach(T control in controls)
            {
                string objName = control.gameObject.name;
                if(!controlsDic.ContainsKey(objName))
                {
                    controlsDic.Add(objName, new List<UIBehaviour>());
                }
                controlsDic[objName].Add(control);
            }
        }
    }
}