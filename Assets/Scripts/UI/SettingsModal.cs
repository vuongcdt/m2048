using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class SettingsModal : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSfx;

        private SoundManager _soundManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _soundManager = SoundManager.Instance;
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);

            SetVolumeUI();
            gameObject.SetActive(false);
        }

        private void SetVolumeUI()
        {
            if (Mathf.Approximately(Prefs.VolumeMusic, -1) || Mathf.Approximately(Prefs.VolumeSfx, -1))
            {
                sliderMusic.value = Constants.Volume.VOLUME_DEFAULT;
                sliderSfx.value = Constants.Volume.VOLUME_DEFAULT;
            }
            else
            {
                sliderMusic.value = Prefs.VolumeMusic;
                sliderSfx.value = Prefs.VolumeSfx;
            }
        }

        public void OnChangeVolumeMusic()
        {
            _soundManager.SetVolumeMusic(sliderMusic.value);
        }

        public void OnChangeVolumeSFX()
        {
            _soundManager.SetVolumeSoundShootSfx(sliderSfx.value);
        }

        private void OnCloseBtnClick()
        {
            _soundManager.SaveVolume(sliderMusic.value, sliderSfx.value);
            ModalContainer.Of(transform).Pop(true);
        }
    }
}