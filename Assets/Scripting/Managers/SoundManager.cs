using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    //Ajustar volumen del sonido y de la música.
    #region AJUSTAR_VOLUMEN
    private float myMusicVolume;

    public float MusicVolume
    {
        get
        {
            return myMusicVolume;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.MUSIC_VOLUME, value);
            myMusicVolume = value;
        }
    }

    public float MusicVolumeSave
    {
        get
        {
            return myMusicVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            //Esta línea lo diferencia del MusicVolume
            musicAudioSource.volume = myMusicVolume;
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.MUSIC_VOLUME, value);
            myMusicVolume = value;
        }
    }

    private float mySfxVolume;

    public float SfxVolume
    {
        get
        {
            return mySfxVolume;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.SFX_VOLUME, value);
            mySfxVolume = value;
        }
    }

    public float SfxVolumeSave
    {
        get
        {
            return mySfxVolume;
        }

        set
        {
            value = Mathf.Clamp(value, 0, 1);
            //Esta línea lo diferencia del SfxVolume
            sFXAudioSource.volume = myMusicVolume;
            PlayerPrefs.SetFloat(AppPlayerPrefKeys.SFX_VOLUME, value);
            mySfxVolume = value;
        }
    }

    #endregion

    //Reproducir, pausar y parar.
    #region AUDIO
    public void PlayMusic(string audioName)
    {
        if (soundMusicDictionary.ContainsKey(audioName))
        {
            musicAudioSource.clip = soundMusicDictionary[audioName];
            musicAudioSource.volume = myMusicVolume;
            musicAudioSource.Play();
        }
    }

    public void PlaySound(string audioName)
    {
        if (soundSFXDictionary.ContainsKey(audioName))
        {
            sFXAudioSource.volume = mySfxVolume;
            sFXAudioSource.PlayOneShot(soundSFXDictionary[audioName]);

        }
    }

    public void StopMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Pause();
        }
    }

    #endregion

    //Diccionarios y AudioSource
    #region INIT
    private Dictionary<string, AudioClip> soundSFXDictionary = null;
    private Dictionary<string, AudioClip> soundMusicDictionary = null;
    private AudioSource musicAudioSource;
    private AudioSource sFXAudioSource;

    #endregion

    //Crear AudioSource
    #region AUDIOSOURCE
    public AudioSource CreateAudioSource(string name, bool isLoop)
    {
        GameObject temporaryAudioHost = new GameObject(name);
        AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
        audioSource.playOnAwake = false;
        audioSource.loop = isLoop;
        audioSource.spatialBlend = 0.0f;

        temporaryAudioHost.transform.SetParent(transform);
        return audioSource;
    }

    #endregion

    //Awake
    public override void Awake()
    {
        base.Awake();

        musicAudioSource = CreateAudioSource("Music", true);
        sFXAudioSource = CreateAudioSource("Sfx", false);

        soundSFXDictionary = new Dictionary<string, AudioClip>();
        soundMusicDictionary = new Dictionary<string, AudioClip>();

        MusicVolume = PlayerPrefs.GetFloat(AppPlayerPrefKeys.MUSIC_VOLUME, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(AppPlayerPrefKeys.SFX_VOLUME, 0.5f);

        AudioClip[] audioSfxVector = Resources.LoadAll<AudioClip>(AppPaths.PATH_RESOURCE_SFX);

        for (int i = 0; i < audioSfxVector.Length; i++)
        {
            soundSFXDictionary.Add(audioSfxVector[i].name, audioSfxVector[i]);
        }

        audioSfxVector = Resources.LoadAll<AudioClip>(AppPaths.PATH_RESOURCE_MUSIC);
        for (int i = 0; i < audioSfxVector.Length; i++)
        {
            soundMusicDictionary.Add(audioSfxVector[i].name, audioSfxVector[i]);
        }
    }

}
