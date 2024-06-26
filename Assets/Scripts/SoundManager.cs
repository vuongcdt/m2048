using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource audioSourceSfx;

    private AudioSource audioSourceMusic;

    private void Start()
    {
        audioSourceMusic = GetComponent<AudioSource>();
        audioSourceMusic.volume = Prefs.VolumeMusic;
        audioSourceSfx.volume = Prefs.VolumeSfx;

        audioSourceMusic.Play();
    }

    public void SetVolumeMusic(float value)
    {
        audioSourceMusic.volume = value;
    }

    public void SetVolumeSoundSfx(float value)
    {
        audioSourceSfx.volume = value;
        audioSourceSfx.Play();
    }

    public void SaveVolume(float sliderMusicValue, float sliderSfxValue)
    {
        Prefs.VolumeMusic = sliderMusicValue;
        Prefs.VolumeSfx = sliderSfxValue;
    }
}