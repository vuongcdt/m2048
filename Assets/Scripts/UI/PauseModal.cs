
using System;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;

namespace UI
{
    public class PauseModal : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSfx;

        private SoundManager _soundManager;
        private BoardManager _boardManager;
        private UIManager _uiManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.PAUSE_POPUP, e => ShowPausePopup());
            _soundManager = SoundManager.Instance;
            _boardManager = BoardManager.Instance;
            _uiManager = UIManager.Instance;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueBtnClick);

            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameBtnClick);

            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeBtnClick);

            SetVolumeUI();
            gameObject.SetActive(false);
        }

        private void ShowPausePopup()
        {
            gameObject.SetActive(true);
        }

        private void OnNewGameBtnClick()
        {
            Prefs.HighScore = _boardManager.highScore;
            _boardManager.isPlaying = true;
            _uiManager.ResetGame();
            _soundManager.SaveVolume(sliderMusic.value, sliderSfx.value);
            // ModalContainer.Find(ContainerKey.Modals).Pop(true);
            gameObject.SetActive(false);
        }

        private void OnHomeBtnClick()
        {
            _boardManager.isPlaying = false;
            _soundManager.SaveVolume(sliderMusic.value, sliderSfx.value);
            // ModalContainer.Find(ContainerKey.Modals).Pop(true);
            // ScreenContainer.Find(ContainerKey.Screens).Pop(true);
            gameObject.SetActive(false);
            Observer.Emit(Constants.EventKey.HOME_SCREEN);
        }

        private void OnContinueBtnClick()
        {
            _boardManager.isPlaying = true;
            _soundManager.SaveVolume(sliderMusic.value, sliderSfx.value);
            // ModalContainer.Find(ContainerKey.Modals).Pop(true);
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
    }
}