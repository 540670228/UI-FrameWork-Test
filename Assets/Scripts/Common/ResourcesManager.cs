using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    ///<summary>
    ///��Դ���ع�����
    ///<summary>
    public class ResourcesManager : MonoSingleton<ResourcesManager>
    {
        //���𴢴���Դ�����ֺ�·��ӳ��
        private static Dictionary<string, string> configMap;

        //�����Ѿ����ص���Դ
        private static Dictionary<string, object> cacheDic;

        protected override void Init()
        {
            base.Init();
            //�����ļ�
            string fileContent = ConfigReader.GetConfigFile("ConfigMap.txt");

            configMap = new Dictionary<string, string>();

            cacheDic = new Dictionary<string, object>();

            //�����ļ���string ----> prefabConfigMap)
            ConfigReader.Reader(fileContent, BuildMap);
        }

        /// <summary>
        /// ���������ÿ���ַ����Ĺ���
        /// </summary>
        /// <param name="line">ÿ���ַ���</param>
        private void BuildMap(string line)
        {
            string[] keyValue = line.Split('=');
            if(configMap.ContainsKey(keyValue[0]))
            {
                Debug.Log($"Resources have the same name {keyValue[0]}");
                return;
            }
            configMap.Add(keyValue[0], keyValue[1]);
        }


        /// <summary>
        /// ͬ��������Դ
        /// </summary>
        /// <typeparam name="T">������Դ����</typeparam>
        /// <param name="resourceName">��Դ����</param>
        /// <returns></returns>
        public T Load<T>(string resourceName)where T:Object
        {
            //���ֵ��л�ȡ·������Ԥ�Ƽ�
            if (configMap.ContainsKey(resourceName))
            {
                string resourceKey = $"{resourceName}_{typeof(T)}";
                if (!cacheDic.ContainsKey(resourceKey))
                {
                    T res = Resources.Load<T>(configMap[resourceName]);
                    cacheDic.Add(resourceKey, res);
                }    
                return cacheDic[resourceKey] as T;

            }
            else return default(T);
        }


        /// <summary>
        /// �첽������Դ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceName"></param>
        /// <param name="action"></param>
        public void LoadAsync<T>(string resourceName,UnityAction<T> action = null) where T : Object
        {
            StartCoroutine(LoadAsyncCore<T>(resourceName, action));
        }
        private IEnumerator LoadAsyncCore<T>(string resourceName, UnityAction<T> action) where T : Object
        {
            if (configMap.ContainsKey(resourceName))
            {
                string resourceKey = $"{resourceName}_{typeof(T)}";
                if (!cacheDic.ContainsKey(resourceKey))
                {
                    ResourceRequest request = Resources.LoadAsync<T>(configMap[resourceName]);
                    yield return request;
                    //���ڲ����첽Э�̣���Ҫ�����ж�
                    if (!cacheDic.ContainsKey(resourceKey))
                        cacheDic.Add(resourceKey, request.asset as T);
                }
                action?.Invoke(cacheDic[resourceKey] as T);

            }
            else action?.Invoke(default(T));
        }

    }

}
