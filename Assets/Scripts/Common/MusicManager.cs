using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    /// <summary>
    /// ���ֹ�����
    /// </summary>
    public class MusicManager : MonoSingleton<MusicManager>
    {

        private AudioSource bkMusic = null;
        private float bkVolume = 1f;

        //��Ч һ��gameobject�϶��audio source
        private GameObject soundObj = null;
        private List<AudioSource> soundList = new List<AudioSource>();
        private float soundVolume = 1f;


        private void Update()
        {
            //ÿ֡��⣬�Ƴ�δʹ�õ�AudioSource
            for(int i = soundList.Count - 1; i >= 0; i--)
            {
                if (!soundList[i].isPlaying)
                    GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        /// <param name="name"></param>
        public void PlayBkMusic(string name)
        {
            if(bkMusic == null)
            {
                GameObject obj = new GameObject("BackgroundMusic");
                bkMusic = obj.AddComponent<AudioSource>();
            }
            //����Դ�м��������ļ�
            ResourcesManager.Instance.LoadAsync<AudioClip>(name,(clip)=> {
                bkMusic.clip = clip;
                bkMusic.Play();
                bkMusic.loop = true;
                bkMusic.volume = bkVolume;
            });
        }

        /// <summary>
        /// ֹͣ��������
        /// </summary>
        public void StopBkMusic()
        {
            if (bkMusic != null)
                return;
            bkMusic.Stop();
        }

        /// <summary>
        /// ��ͣ��������
        /// </summary>
        public void PauseBkMusic()
        {
            if (bkMusic != null)
                return;
            bkMusic.Pause();
        }

        /// <summary>
        /// �ı䱳��������С
        /// </summary>
        /// <param name="volume"></param>
        public void SetBkVolume(float volume)
        {
            bkVolume = volume;
            bkMusic.volume = volume;
        }

        /// <summary>
        /// ������Ч
        /// </summary>
        /// <param name="name">��Ч��</param>
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
        /// ֹͣ��Ч
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
        /// �ı���Ч��С
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