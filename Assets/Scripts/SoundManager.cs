using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceShootSfx;
    [SerializeField] private AudioSource audioSourceMergeSfx;
    [SerializeField] private AudioSource audioSourceSortSfx;
    [SerializeField] private AudioSource audioSourceComboSfx;
    [SerializeField] private AudioSource audioSourceGameOverSfx;
    [SerializeField] private AudioSource audioSourceMaxItemColumnSfx;
    [SerializeField] private AudioSource audioSourceClickSfx;

    private float _volumeSfx;

    private void Start()
    {
        if (Mathf.Approximately(Prefs.VolumeMusic, -1) || Mathf.Approximately(Prefs.VolumeSfx, -1))
        {
            audioSourceMusic.volume = Constants.Volume.VOLUME_DEFAULT;
            _volumeSfx = Constants.Volume.VOLUME_DEFAULT;
        }
        else
        {
            audioSourceMusic.volume = Prefs.VolumeMusic;
            _volumeSfx = Prefs.VolumeSfx;
        }

        audioSourceMusic.Play();
    }

    public void PlaySoundShootSfx()
    {
        audioSourceShootSfx.volume = _volumeSfx;
        audioSourceShootSfx.Play();
    }

    public void PlaySoundSortSfx()
    {
        audioSourceSortSfx.volume = _volumeSfx;
        audioSourceSortSfx.Play();
    }

    public void PlaySoundMergeSfx()
    {
        audioSourceMergeSfx.volume = _volumeSfx;
        audioSourceMergeSfx.Play();
    }

    public void PlaySoundComboSfx()
    {
        audioSourceComboSfx.volume = _volumeSfx;
        audioSourceComboSfx.Play();
    }

    public void PlaySoundGameOverSfx()
    {
        audioSourceGameOverSfx.volume = _volumeSfx;
        audioSourceGameOverSfx.Play();
    }

    public void PlaySoundMaxItemColumnSfx()
    {
        audioSourceMaxItemColumnSfx.volume = _volumeSfx;
        audioSourceMaxItemColumnSfx.Play();
    }

    public void PlaySoundClickSfx()
    {
        audioSourceClickSfx.volume = _volumeSfx;
        audioSourceClickSfx.Play();
    }

    public void SetVolumeMusic(float value)
    {
        audioSourceMusic.volume = value;
    }

    public void SetVolumeSoundShootSfx(float value)
    {
        _volumeSfx = value;
        PlaySoundShootSfx();
    }

    public void SaveVolume(float sliderMusicValue, float sliderSfxValue)
    {
        Prefs.VolumeMusic = sliderMusicValue;
        Prefs.VolumeSfx = sliderSfxValue;
    }
}