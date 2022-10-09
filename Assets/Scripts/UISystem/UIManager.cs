using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
        private List<Transform> layers = new List<Transform>();
        private System.Object locker = new System.Object();
        protected override void Init()
        {
            base.Init();
            GameObject canvasObj = ResourcesManager.Instance.Load<GameObject>("Canvas");
            GameObject eventObj = ResourcesManager.Instance.Load<GameObject>("EventSystem");
            canvasObj = Instantiate(canvasObj);
            eventObj = Instantiate(eventObj);

            Transform canvasTF = canvasObj.transform;
            for (int i = 0 ; i < canvasTF.childCount; i++)
            {
                layers.Add(canvasTF.GetChild(i));
            }

            DontDestroyOnLoad(canvasObj);
            DontDestroyOnLoad(eventObj);
            DontDestroyOnLoad(this.gameObject);
        }
        /// <summary>
        /// 创建/显示面板
        /// </summary>
        /// <param name="panelName">面板名称</param>
        /// <param name="layer">层级 0-3</param>
        /// <param name="action">面板显示后的处理委托函数</param>
        public void ShowPanel<T>(string panelName,int layer = 0,UnityAction<T> action = null)where T : BasePanel
        {
            if (layer > layers.Count) return;
            ResourcesManager.Instance.LoadAsync<GameObject>(panelName, (obj) =>
            {
                lock(locker)
                {
                    if (!panelDic.ContainsKey(panelName))
                    {
                        //实例化面板对象
                        obj = Instantiate(obj, layers[layer]);
                        BasePanel panel = obj.GetComponent<BasePanel>();
                        panelDic.Add(panelName, panel);
                    }
                    panelDic[panelName].Show(); 
                    action?.Invoke(panelDic[panelName] as T);
                }
            });

        }

        /// <summary>
        /// 销毁面板
        /// </summary>
        /// <param name="panelName">面板名称</param>
        public void ClosePanel(string panelName)
        {
            if(panelDic.ContainsKey(panelName))
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="panelName">面板名称</param>
        public void HidePanel(string panelName)
        {
            if(panelDic.ContainsKey(panelName))
            {
                panelDic[panelName].Hide();
            }
        }

        public void SetLayer(Transform transform,int layer)
        {
            transform.SetParent(layers[layer],false);
        }
    }
}