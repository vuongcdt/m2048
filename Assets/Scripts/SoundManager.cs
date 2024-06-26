using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource audioSourceSfx;

    private AudioSource audioSourceMusic;

    private void Start()
    {
        audioSourceMusic = GetComponent<AudioSource>();
        if (Mathf.Approximately(Prefs.VolumeMusic, -1) || Mathf.Approximately(Prefs.VolumeSfx, -1))
        {
            audioSourceMusic.volume = 0.5f;
            audioSourceSfx.volume = 0.5f;
        }
        else
        {
            audioSourceMusic.volume = Prefs.VolumeMusic;
            audioSourceSfx.volume = Prefs.VolumeSfx;
        }

        audioSourceMusic.Play();
    }

    public void PlaySoundShoot()
    {
        audioSourceSfx.Play();
    }

    public void SetVolumeMusic(float value)
    {
        audioSourceMusic.volume = value;
    }

    public void SetVolumeSoundSfx(float value)
    {
        audioSourceSfx.volume = value;
        PlaySoundShoot();
    }

    public void SaveVolume(float sliderMusicValue, float sliderSfxValue)
    {
        Prefs.VolumeMusic = sliderMusicValue;
        Prefs.VolumeSfx = sliderSfxValue;
    }
}