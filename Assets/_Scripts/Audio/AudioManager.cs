using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Singletons/AudioManager")]
    public class AudioManager : ScriptableSingleton<AudioManager>
    {
        public enum MixerGroupVolumeNames { MusicVolume, GeneralSoundsVolume}
        
        private static float logarithmicAttenuationMult = 20f;

        private static float minVolume = 0.0001f;

        private static int maxVolume = 0;

        #region Serializable Fields

        [SerializeField] private AudioMixerGroup audioMixer, musicMixer, generalSoundsMixer;

        #endregion

        #region Fields

        private AudioSource _musicAudioSource;
        private List<KeyValuePair<AudioClip, GameObject>> _activeAudio;
        private AudioMixer _masterMixer;
        private AudioManagerCallbackHandler _callbackHandler;

        private float _defaultMusicVolume = maxVolume;
        private float _defaultGeneralSoundsVolume = maxVolume;

        //group fade routines (fades must cancel each other out)
        private Coroutine musicFadeRoutine;
        private Coroutine generalSoundFadeRoutine;

        private float musicBeforeMute;
        private float generalSoundsBeforeMute;
    
        #endregion

        #region Properties

        public float MusicVolume { get; set; }
        public float OtherVolume { get; set; }
        public bool IsMuted { get; set; } = false;
        public float MinVolume { get => minVolume; }

        #endregion

        #region Enums

        public enum PlayMode
        {
            Single,
            Loop
        }

        #endregion

        #region Unity Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            Instance._activeAudio = new List<KeyValuePair<AudioClip, GameObject>>();
            Instance.InstantiateCallbackHandler();

            //fetch fixer and groups
            Instance._masterMixer = Instance.audioMixer.audioMixer;
        }

        private void InstantiateCallbackHandler()
        {
            _callbackHandler = new GameObject("Audio Manager Callback Handler").AddComponent<AudioManagerCallbackHandler>();
            DontDestroyOnLoad(_callbackHandler);
            GameObject musicGo = new GameObject("Music");
            musicGo.transform.parent = _callbackHandler.transform;
            _musicAudioSource = musicGo.AddComponent<AudioSource>();
            _musicAudioSource.outputAudioMixerGroup = musicMixer;
            _musicAudioSource.playOnAwake = false;
            
            LoadVolumeLevels();
        }
        
        #endregion

        #region Private Methods

        public void LoadVolumeLevels()
        {
            ChangeSoundVolume(AudioSaveData.Data.OtherVolume);
            ChangeMusicVolume(AudioSaveData.Data.MusicVolume);
        }

        public void MuteAudios()
        {
            _masterMixer.GetFloat(MixerGroupVolumeNames.MusicVolume.ToString(), out musicBeforeMute);
            _masterMixer.GetFloat(MixerGroupVolumeNames.GeneralSoundsVolume.ToString(), out generalSoundsBeforeMute);

            ChangeSoundVolume(minVolume);
            ChangeMusicVolume(minVolume);

            IsMuted = true;
        }

        public void UnMuteAudios()
        {
            IsMuted = false;

            _masterMixer.SetFloat(MixerGroupVolumeNames.MusicVolume.ToString(), musicBeforeMute);
            _masterMixer.SetFloat(MixerGroupVolumeNames.GeneralSoundsVolume.ToString(), generalSoundsBeforeMute);
        }

        private void SetGroupVolumes()
        {
            if (!_masterMixer || IsMuted) return;

            _defaultGeneralSoundsVolume = Mathf.Log10(OtherVolume) * logarithmicAttenuationMult;
            _defaultMusicVolume = Mathf.Log10(MusicVolume) * logarithmicAttenuationMult;

            _masterMixer.SetFloat(MixerGroupVolumeNames.MusicVolume.ToString(), _defaultMusicVolume);
            _masterMixer.SetFloat(MixerGroupVolumeNames.GeneralSoundsVolume.ToString(), _defaultGeneralSoundsVolume);
        }

        private void CancelGroupsFade(MixerGroupVolumeNames name)
        {
            switch (name)
            {
                case MixerGroupVolumeNames.MusicVolume:
                    if(musicFadeRoutine != null) { _callbackHandler.StopCoroutine(musicFadeRoutine); }
                    break;
                case MixerGroupVolumeNames.GeneralSoundsVolume:
                    if (generalSoundFadeRoutine != null) { _callbackHandler.StopCoroutine(generalSoundFadeRoutine); }
                    break;
                default:
                    break;
            }
        }

        private void SetGroupFade(MixerGroupVolumeNames name, Coroutine currentRoutine)
        {
            switch (name)
            {
                case MixerGroupVolumeNames.MusicVolume:
                    musicFadeRoutine = currentRoutine;
                    break;
                case MixerGroupVolumeNames.GeneralSoundsVolume:
                    generalSoundFadeRoutine = currentRoutine;
                    break;
                default:
                    break;
            }
        }

        private IEnumerator FadeOutMixerGroupVolumeRoutine(MixerGroupVolumeNames name, float fadeOffDuration)
        {
            CancelGroupsFade(name);
            string groupName = name.ToString();
            _masterMixer.GetFloat(groupName, out float curVolume);
            float stepLength = fadeOffDuration / (curVolume + 80);
            while (curVolume>-80)
            {
                if (IsMuted)
                {
                    _masterMixer.SetFloat(groupName, minVolume);
                    break;
                }
                curVolume--;
                if (curVolume < -80) curVolume = -80;
                _masterMixer.SetFloat(groupName, curVolume);
                yield return new WaitForSecondsRealtime(stepLength);
            }
        }

        private IEnumerator FadeInMixerGroupVolumeRoutine(MixerGroupVolumeNames name, float defaultGoupVolume ,float fadeInDuration)
        {
            CancelGroupsFade(name);
            string groupName = name.ToString();
            _masterMixer.GetFloat(groupName, out float curVolume);
            float stepLength = fadeInDuration / (defaultGoupVolume-curVolume);
            while (curVolume < defaultGoupVolume)
            {
                if (IsMuted)
                {
                    _masterMixer.SetFloat(groupName, minVolume);
                    break;
                }
                curVolume++;
                if (curVolume > defaultGoupVolume) curVolume = defaultGoupVolume;
                _masterMixer.SetFloat(groupName, curVolume);
                yield return new WaitForSecondsRealtime(stepLength);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays an audio clip directly
        /// </summary>
        /// <param name="audioClip">Clip to play</param>
        /// <param name="mode">Play mode; loop or single</param>
        public AudioSource PlayAudio(AudioClip audioClip, PlayMode mode, float volume , MixerGroupVolumeNames groupName = MixerGroupVolumeNames.GeneralSoundsVolume, float pitch = 1f)
        {
            if (audioClip == null) return null;
            AudioMixerGroup group = audioMixer;
            switch (groupName)
            {
                case MixerGroupVolumeNames.MusicVolume:
                    group = musicMixer;
                    break;
                case MixerGroupVolumeNames.GeneralSoundsVolume:
                    group = generalSoundsMixer;
                    break;
                default:
                    break;
            }
            float cliptime = audioClip.length+0.5f;
            volume = Mathf.Clamp(volume, 0, 1);
            GameObject audioGo = new GameObject("Audio");
            audioGo.transform.parent = _callbackHandler.transform;
            AudioSource audioSource = audioGo.AddComponent<AudioSource>();
            audioSource.pitch = pitch;
            audioSource.outputAudioMixerGroup = group;
            _activeAudio.Add(new KeyValuePair<AudioClip, GameObject>(audioClip, audioGo));

            if (mode == PlayMode.Single)
            {
                audioSource.clip = audioClip;
                audioSource.loop = false;
                audioSource.volume = volume;
                audioSource.Play();
                Destroy(audioGo, cliptime); //clean the object up after playing
            }
            if (mode == PlayMode.Loop)
            {
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.loop = true;
                audioSource.Play();
            }
            return audioSource;
        }

        /// <summary>
        /// Plays background music
        /// </summary>
        /// <param name="music">Music AudioClip to play</param>
        public void PlayMusic(AudioClip music, bool loop=true, float volume=1f)
        {
            _musicAudioSource.loop = false;
            _musicAudioSource.Stop();
            _musicAudioSource.clip = music;
            _musicAudioSource.loop = loop;
            _musicAudioSource.volume = volume;
            _musicAudioSource.Play();
        }

        public void ChangeMusicVolume(float volume)
        {
            MusicVolume = volume;
            SetGroupVolumes();
        }

        public void ChangeSoundVolume(float percentage)
        {
            OtherVolume = percentage;
            SetGroupVolumes();
        }

        /// <summary>
        /// Turns mixer group volume down slowly
        /// </summary>
        /// <param name="duration"></param>
        public void FadeOutVolume(MixerGroupVolumeNames name, float duration)
        {
            SetGroupFade(name, _callbackHandler.StartCoroutine(FadeOutMixerGroupVolumeRoutine(name, duration)));
        }

        /// <summary>
        /// Turns mixer group volume up slowly
        /// </summary>
        /// <param name="duration"></param>
        public void FadeInVolume(MixerGroupVolumeNames name, float duration)
        {
            SetGroupFade(name, _callbackHandler.StartCoroutine(FadeInMixerGroupVolumeRoutine(name, _defaultGeneralSoundsVolume, duration)));
        }

        /// <summary>
        /// Stops the active music
        /// </summary>
        public void StopMusic()
        {
            _musicAudioSource.Stop();
        }

        /// <summary>
        /// Stops all active sounds which are currently playing
        /// </summary>
        public void StopAllActiveSounds()
        {
            foreach (KeyValuePair<AudioClip, GameObject> audio in _activeAudio)
            {
                Destroy(audio.Value);
            }
        }

        /// <summary>
        /// Stops a single playing sound
        /// </summary>
        /// <param name="clipName">AudioClip's name</param>
        public void StopAudio(string clipName)
        {
            var found = _activeAudio.Where(clip => clip.Key.name == clipName);
            foreach (var val in found)
            {
                Destroy(val.Value);
            }
        }

        /// <summary>
        /// Stops a single playing sound
        /// </summary>
        /// <param name="audioClip">AudioClip's reference</param>
        public void StopAudio(AudioClip audioClip)
        {
            var found = _activeAudio.Where(clip => clip.Key == audioClip);
            foreach (var val in found)
            {
                Destroy(val.Value);
            }
        }

        #endregion

        public class AudioManagerCallbackHandler : MonoBehaviour { }

        public class AudioSaveData: Saveable<AudioSaveData>
        {
            public float OtherVolume = .7f;
            public float MusicVolume =.45f;
        }
    }
}