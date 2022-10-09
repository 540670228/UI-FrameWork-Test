using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    /// <summary>
    /// 音乐管理器
    /// </summary>
    public class MusicManager : MonoSingleton<MusicManager>
    {

        private AudioSource bkMusic = null;
        private float bkVolume = 1f;

        //音效 一个gameobject上多个audio source
        private GameObject soundObj = null;
        private List<AudioSource> soundList = new List<AudioSource>();
        private float soundVolume = 1f;


        private void Update()
        {
            //每帧检测，移除未使用的AudioSource
            for(int i = soundList.Count - 1; i >= 0; i--)
            {
                if (!soundList[i].isPlaying)
                    GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayBkMusic(string name)
        {
            if(bkMusic == null)
            {
                GameObject obj = new GameObject("BackgroundMusic");
                bkMusic = obj.AddComponent<AudioSource>();
            }
            //在资源中加载音乐文件
            ResourcesManager.Instance.LoadAsync<AudioClip>(name,(clip)=> {
                bkMusic.clip = clip;
                bkMusic.Play();
                bkMusic.loop = true;
                bkMusic.volume = bkVolume;
            });
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBkMusic()
        {
            if (bkMusic != null)
                return;
            bkMusic.Stop();
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBkMusic()
        {
            if (bkMusic != null)
                return;
            bkMusic.Pause();
        }

        /// <summary>
        /// 改变背景音量大小
        /// </summary>
        /// <param name="volume"></param>
        public void SetBkVolume(float volume)
        {
            bkVolume = volume;
            bkMusic.volume = volume;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name">音效名</param>
        public void PlaySound(string name,bool isLoop,UnityAction<AudioSource> playHandler = null)
        {
            if(soundObj == null)
            {
                GameObject obj = new GameObject("SoundMusic");
            }
            bkMusic.clip = ResourcesManager.Instance.Load<AudioClip>(name);

            AudioSource source = soundObj.AddComponent<AudioSource>();
            soundList.Add(source);
            bkMusic.Play();
            bkMusic.volume = soundVolume;
            bkMusic.loop = isLoop;
            playHandler?.Invoke(source);
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="source"></param>
        public void StopSound(AudioSource source)
        {
            if(soundList.Contains(source))
            {
                soundList.Remove(source);
                source.Stop();
                GameObject.Destroy(source);
            }
        }

        /// <summary>
        /// 改变音效大小
        /// </summary>
        /// <param name="value"></param>
        public void ChangeSoundVolume(float value)
        {
            soundVolume = value;
            foreach(AudioSource source in soundList)
            {
                source.volume = value;
            }
        }
    }
}