using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core{
    public class SoundSystemManager : Singleton<SoundSystemManager>
    {
        protected SoundSystemManager()
        {

        }

        [SerializeField]
        AudioSource _musicAudio;
        [SerializeField]
        AudioSource _soundAudio;

        // Use this for initialization
        void Start()
        {

        }

        public void PlayMusic(string path){
            _musicAudio.clip = Resources.Load<AudioClip>(path);
            _musicAudio.loop = true;
            _musicAudio.Play();
        }

        public void StopMusic(){
            _musicAudio.Stop();
        }

        public void PlaySoundEffect(string path){
            _soundAudio.clip = Resources.Load<AudioClip>(path);
            _soundAudio.loop = false;
            //_soundAudio.Play();
            _soundAudio.PlayOneShot(_soundAudio.clip);
        }

        public void UpdateMusicVolumn(bool isOff){
            _musicAudio.volume = isOff ? 0 : 1;
        }

        public void UpdateSoundVolumn(bool isOff)
        {
            _soundAudio.volume = isOff ? 0 : 1;
        }
    }
}

