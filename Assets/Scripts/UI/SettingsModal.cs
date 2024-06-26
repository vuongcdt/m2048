﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class SettingsModal : Modal
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSfx;

        private SoundManager _soundManager;

        public override UniTask Initialize(Memory<object> args)
        {
            _soundManager = SoundManager.Instance;
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);

            SetVolumeUI();
            return UniTask.CompletedTask;
        }

        private void SetVolumeUI()
        {
            if (Mathf.Approximately(Prefs.VolumeMusic, -1) || Mathf.Approximately(Prefs.VolumeSfx, -1))
            {
                sliderMusic.value = 0.5f;
                sliderSfx.value = 0.5f;
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
            _soundManager.SetVolumeSoundSfx(sliderSfx.value);
        }

        private void OnCloseBtnClick()
        {
            _soundManager.SaveVolume(sliderMusic.value, sliderSfx.value);
            ModalContainer.Of(transform).Pop(true);
        }
    }
}